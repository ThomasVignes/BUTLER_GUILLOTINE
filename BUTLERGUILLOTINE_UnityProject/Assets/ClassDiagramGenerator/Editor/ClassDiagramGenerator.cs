// Assets/ClassDiagramGenerator/Editor/ClassDiagramGenerator.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ClassDiagramGenerator
{
    public class ClassDiagramGenerator : EditorWindow
    {
        private enum ExportFormat
        {
            PlantUMLFile,
            PlantUML_URL
        }

        // -------- State ----------
        private ExportFormat _exportFormat = ExportFormat.PlantUMLFile;
        private string _lastDiagramURL = "";
        private string _outputPath = "Assets/Scripts/ClassDiagram.puml";
        private string _status = "";
        private Vector2 _outerScroll;
        private bool _includeAssociations = true;

        // Sélection scripts
        private ScriptSelectionManager _scriptManager = new ScriptSelectionManager();
        private Vector2 _scriptScroll;
        private string _scriptSearch = "";
        private bool _scriptsScanned = false;

        // Cible à scanner (asset simple : dossier ou .cs)
        private DefaultAsset _scanTarget;

        private Texture2D _bgTex;
        private const float BG_OVERLAY_ALPHA = 0.26f;

        private static readonly Color COL_BG = new(0.12f, 0.12f, 0.14f, 1f); // fallback
        private static readonly Color COL_PANEL = new(0.16f, 0.17f, 0.20f, 1f);
        private static readonly Color COL_PANEL_2 = new(0.20f, 0.21f, 0.24f, 1f);
        private static readonly Color COL_BORDER = new(0.30f, 0.32f, 0.36f, 1f);
        private static readonly Color COL_TEXT = new(0.94f, 0.94f, 0.97f, 1f);
        private static readonly Color COL_TEXT_SUB = new(0.78f, 0.80f, 0.84f, 1f);
        private static readonly Color COL_ROW_EVEN = new(1f, 1f, 1f, 0.03f);

        private static readonly Color ACCENT_VIOLET = new(0.43f, 0.34f, 0.68f, 1f);
        private static readonly Color ACCENT_BLUE = new(0.36f, 0.62f, 0.97f, 1f);
        private static readonly Color ACCENT_V_HOVER = new(0.54f, 0.45f, 0.80f, 1f);
        private static readonly Color ACCENT_B_HOVER = new(0.52f, 0.74f, 1.00f, 1f);

        // Textures + styles (lazy)
        private Texture2D _texPanel, _texPanel2;
        private GUIStyle _titleStyle, _subtitleStyle, _cardHeaderStyle, _cardBodyStyle, _dirStyle;

        private Texture2D GetHeaderIcon() => Resources.Load<Texture2D>("Icon 160x160 - Diagram Generator");

        [MenuItem("Tools/Diagram Generator")]
        static void ShowWindow()
        {
            var w = GetWindow<ClassDiagramGenerator>("Diagram Generator");
            w.minSize = new Vector2(600, 420);
        }

        // --- remplace entièrement ta méthode OnEnable() ---
        private void OnEnable()
        {
            _bgTex = Resources.Load<Texture2D>("settings_bg");
            if (_bgTex == null)
                _bgTex = Resources.Load<Texture2D>("inspector_bg");

            // ⚠️ Ne PAS appeler EnsureThemeAssets() ici (provoque "You can only call GUI functions from inside OnGUI")

            // Auto-scan discret au démarrage (si Assets/Scripts existe)
            string root = AssetDatabase.IsValidFolder("Assets/Scripts") ? "Assets/Scripts" : null;
            if (!string.IsNullOrEmpty(root))
            {
                _scriptManager.Scan(root);
                _scriptsScanned = _scriptManager.Scripts.Count > 0;
                _status = _scriptsScanned
                    ? $"Scanned: {_scriptManager.Scripts.Count} files under '{root}'."
                    : "Drop a folder or .cs files below to start.";
            }
            else
            {
                _status = "Select a folder or drop files to begin.";
            }
        }

        // --- remplace entièrement ta méthode EnsureThemeAssets() ---
        private void EnsureThemeAssets()
        {
            // Ce helper manipule GUI.skin / EditorStyles → il DOIT être appelé depuis OnGUI()
            if (Event.current == null) return;

            if (_texPanel == null) _texPanel = MakeTex(2, 2, COL_PANEL);
            if (_texPanel2 == null) _texPanel2 = MakeTex(2, 2, COL_PANEL_2);

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(EditorStyles.boldLabel)
                    { alignment = TextAnchor.MiddleLeft, fontSize = 14, richText = true };
                _titleStyle.normal.textColor = COL_TEXT;
            }

            if (_subtitleStyle == null)
            {
                _subtitleStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft };
                _subtitleStyle.normal.textColor = COL_TEXT_SUB;
            }

            if (_cardHeaderStyle == null)
            {
                _cardHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                    { alignment = TextAnchor.MiddleLeft, fontSize = 12 };
                _cardHeaderStyle.normal.textColor = COL_TEXT;
            }

            if (_cardBodyStyle == null)
            {
                // GUI.skin.box est OK ici car on est dans OnGUI()
                _cardBodyStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(12, 12, 10, 12),
                    margin = new RectOffset(0, 0, 0, 0)
                };
                _cardBodyStyle.normal.background = _texPanel2;
            }

            if (_dirStyle == null)
            {
                _dirStyle = new GUIStyle(EditorStyles.miniLabel);
                _dirStyle.normal.textColor = new Color(0.76f, 0.78f, 0.82f, 1f);
            }
        }

        private void OnGUI()
        {
            EnsureThemeAssets();

            DrawBackground();
            DrawHeader();
            DrawToolbar();

            _outerScroll = EditorGUILayout.BeginScrollView(_outerScroll);
            DrawCard("Selection", DrawSelectionCard);
            DrawCard("Export Options", DrawExportCard);
            DrawCard("Advanced", DrawAdvancedCard);
            EditorGUILayout.EndScrollView();

            DrawFooter();
        }

        // ---------- Background ----------
        void DrawBackground()
        {
            var r = new Rect(0, 0, position.width, position.height);
            if (_bgTex != null)
            {
                GUI.DrawTexture(r, _bgTex, ScaleMode.ScaleAndCrop, true);
                EditorGUI.DrawRect(r, new Color(0, 0, 0, BG_OVERLAY_ALPHA));
            }
            else EditorGUI.DrawRect(r, COL_BG);
        }

        // ---------- Header ----------
        void DrawHeader()
        {
            GUILayout.Space(6);
            var icon = GetHeaderIcon();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (icon != null) GUILayout.Label(icon, GUILayout.Width(40), GUILayout.Height(40));
                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(2);
                    EditorGUILayout.LabelField("Class Diagram Generator", _titleStyle);
                    EditorGUILayout.LabelField("Generate UML (PlantUML) from your C# scripts", _subtitleStyle);
                }
            }

            var line = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(line, COL_BORDER);
            GUILayout.Space(2);
        }

        // ---------- Toolbar (lightweight) ----------
        void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Onglets format
                DrawFormatTabs();
                GUILayout.FlexibleSpace();

                // Bouton Generate (toolbar)
                if (ToolbarPrimary("Generate"))
                    GenerateDiagram(_exportFormat);
            }

            GUILayout.Space(4);
        }

        void DrawFormatTabs()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int i = 0; i < 2; i++)
                {
                    bool active = (int)_exportFormat == i;
                    var label = (i == 0) ? "File (.puml)" : "URL";

                    var content = new GUIContent(label);
                    var r = GUILayoutUtility.GetRect(content, EditorStyles.toolbarButton, GUILayout.Width(110),
                        GUILayout.Height(20));

                    // fond + état
                    var bg = active ? ACCENT_BLUE : new Color(0, 0, 0, 0f);
                    EditorGUI.DrawRect(r, bg);

                    // soulignement / hover
                    bool hover = r.Contains(Event.current.mousePosition);
                    if (active)
                    {
                        var underline = new Rect(r.x, r.yMax - 2, r.width, 2);
                        EditorGUI.DrawRect(underline, hover ? ACCENT_B_HOVER : ACCENT_VIOLET);
                    }
                    else if (hover)
                    {
                        var outline = new Rect(r.x, r.yMax - 1, r.width, 1);
                        EditorGUI.DrawRect(outline, COL_BORDER);
                    }

                    var style = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        fontStyle = active ? FontStyle.Bold : FontStyle.Normal,
                        alignment = TextAnchor.MiddleCenter
                    };
                    style.normal.textColor = active ? Color.white : COL_TEXT_SUB;

                    if (GUI.Button(r, content, style))
                        _exportFormat = (ExportFormat)i;

                    GUILayout.Space(2);
                }
            }
        }

        bool ToolbarPrimary(string label)
        {
            var r = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.toolbarButton, GUILayout.Width(92));
            bool hover = r.Contains(Event.current.mousePosition);
            EditorGUI.DrawRect(r, hover ? ACCENT_B_HOVER : ACCENT_BLUE);
            var s = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };
            s.normal.textColor = Color.white;
            return GUI.Button(r, label, s);
        }

        // ---------- Cards ----------
        void DrawCard(string title, Action body)
        {
            var header = GUILayoutUtility.GetRect(1, 24, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint) GUI.DrawTexture(header, _texPanel);
            GUI.Label(new Rect(header.x + 10, header.y, header.width - 20, header.height), title, _cardHeaderStyle);
            EditorGUI.DrawRect(new Rect(header.x, header.yMax - 1, header.width, 1), COL_BORDER);

            EditorGUILayout.BeginVertical(_cardBodyStyle);
            try
            {
                body?.Invoke();
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);
        }

        void DrawSelectionCard()
        {
            // --- Ligne "Select & Scan" (remise en place)
            using (new EditorGUILayout.HorizontalScope())
            {
                _scanTarget = (DefaultAsset)EditorGUILayout.ObjectField(
                    new GUIContent("Folder / .cs", "Choose a folder or a .cs file"),
                    _scanTarget, typeof(DefaultAsset), false,
                    GUILayout.ExpandWidth(true), GUILayout.MinWidth(200));

                GUILayout.Space(6);
                if (SecondaryButton("Scan", 20))
                    ScanSelectionTarget();
                GUILayout.FlexibleSpace();
            }

            // --- Drop zone visible
            GUILayout.Space(6);
            var dz = GUILayoutUtility.GetRect(1, 64, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(dz, COL_PANEL);
            EditorGUI.DrawRect(new Rect(dz.x, dz.y, dz.width, 1), COL_BORDER);
            EditorGUI.DrawRect(new Rect(dz.x, dz.yMax - 1, dz.width, 1), COL_BORDER);
            var dzLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 11 };
            dzLabel.normal.textColor = COL_TEXT_SUB;
            GUI.Label(dz, "Drop a folder to scan it • Drop .cs files to add them", dzLabel);
            HandleDragAndDrop(dz);

            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                _scriptSearch =
                    EditorGUILayout.TextField(new GUIContent("Search", "Filter by file name"), _scriptSearch);
                if (MiniButton("All", 56))
                    foreach (var s in _scriptManager.Scripts)
                        s.IsSelected = true;
                if (MiniButton("None", 56))
                    foreach (var s in _scriptManager.Scripts)
                        s.IsSelected = false;
                if (MiniButton("Invert", 64))
                    for (int i = 0; i < _scriptManager.Scripts.Count; i++)
                        _scriptManager.Scripts[i].IsSelected = !_scriptManager.Scripts[i].IsSelected;
            }

            GUILayout.Space(4);
            using (new EditorGUILayout.HorizontalScope())
            {
                int total = _scriptManager.Scripts.Count;
                int selected = _scriptManager.Scripts.Count(s => s.IsSelected);
                GUILayout.Label($"Total: {total}", EditorStyles.miniBoldLabel, GUILayout.Width(90));
                GUILayout.Label($"Selected: {selected}", EditorStyles.miniBoldLabel, GUILayout.Width(110));
                GUILayout.FlexibleSpace();
            }

            var filtered = string.IsNullOrWhiteSpace(_scriptSearch)
                ? _scriptManager.Scripts
                : _scriptManager.Scripts
                    .Where(s => s.FileName.IndexOf(_scriptSearch, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            _scriptScroll = EditorGUILayout.BeginScrollView(_scriptScroll, GUILayout.Height(240));
            if (filtered.Count == 0)
            {
                EditorGUILayout.HelpBox("Use Scan or drop content above to populate the list.", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < filtered.Count; i++)
                {
                    var s = filtered[i];
                    var row = GUILayoutUtility.GetRect(1, 20, GUILayout.ExpandWidth(true));
                    if (i % 2 == 0) EditorGUI.DrawRect(row, COL_ROW_EVEN);

                    var tRect = new Rect(row.x + 6, row.y + 2, 20, row.height - 2);
                    s.IsSelected = EditorGUI.Toggle(tRect, s.IsSelected);

                    var nameRect = new Rect(tRect.xMax + 4, row.y + 1, 320, row.height - 2);
                    var nameLabel = new GUIStyle(EditorStyles.label) { normal = { textColor = COL_TEXT } };
                    EditorGUI.LabelField(nameRect, s.FileName, nameLabel);

                    var dirRect = new Rect(nameRect.xMax + 8, row.y + 1, row.width - nameRect.width - 40,
                        row.height - 2);
                    var assetPath = AbsoluteToAssetPathSafe(s.Path);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        EditorGUIUtility.AddCursorRect(dirRect, MouseCursor.Link);
                        EditorGUI.LabelField(dirRect, assetPath, _dirStyle);
                        if (Event.current.type == EventType.MouseDown && dirRect.Contains(Event.current.mousePosition))
                        {
                            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                            if (obj != null)
                            {
                                Selection.activeObject = obj;
                                EditorGUIUtility.PingObject(obj);
                            }
                        }
                    }
                    else EditorGUI.LabelField(dirRect, $"[{s.Directory}]", _dirStyle);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawExportCard()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Output (.puml)", GUILayout.Width(120));
                _outputPath = EditorGUILayout.TextField(_outputPath);
                if (MiniButton("…", 28))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Save PlantUML file", "ClassDiagram", "puml", "");
                    if (!string.IsNullOrEmpty(path)) _outputPath = path;
                }
            }

            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (PrimaryButton("🛠️  Generate Diagram", 42)) GenerateDiagram(_exportFormat);
                GUILayout.FlexibleSpace();
            }

            if (_exportFormat == ExportFormat.PlantUML_URL && !string.IsNullOrEmpty(_lastDiagramURL))
            {
                GUILayout.Space(6);
                EditorGUILayout.SelectableLabel(_lastDiagramURL, EditorStyles.textField, GUILayout.Height(20));
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (SecondaryButton("📋 Copy URL")) EditorGUIUtility.systemCopyBuffer = _lastDiagramURL;
                    GUILayout.FlexibleSpace();
                }
            }
        }

        void DrawAdvancedCard()
        {
            _includeAssociations = EditorGUILayout.ToggleLeft(
                new GUIContent("Include associations (fields/parameters of other classes)"),
                _includeAssociations);

            GUILayout.Space(2);
        }

        // ---------- Footer ----------
        void DrawFooter()
        {
            var line = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(line, COL_BORDER);

            var box = new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fontSize = 12 };
            box.normal.textColor = COL_TEXT;
            var r = GUILayoutUtility.GetRect(1, 32, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint) GUI.DrawTexture(r, _texPanel);
            GUI.Label(r, string.IsNullOrEmpty(_status) ? "Ready." : _status, box);

            var sig = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                { alignment = TextAnchor.LowerCenter, fontStyle = FontStyle.Italic };
            sig.normal.textColor = COL_TEXT_SUB;
            GUILayout.Label("© 2025 ClassDiagramGenerator • v2.0.1", sig);
        }

        // ---------- Buttons ----------
        bool PrimaryButton(string label, float height)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.miniButton,
                GUILayout.Height(height), GUILayout.MinWidth(240), GUILayout.ExpandWidth(false));
            bool hover = rect.Contains(Event.current.mousePosition);

            var top = new Rect(rect.x, rect.y, rect.width, Mathf.Round(rect.height * 0.5f));
            EditorGUI.DrawRect(top, hover ? ACCENT_B_HOVER : ACCENT_BLUE);
            var bot = new Rect(rect.x, rect.y + top.height, rect.width, rect.height - top.height);
            EditorGUI.DrawRect(bot, hover ? ACCENT_V_HOVER : ACCENT_VIOLET);

            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), COL_BORDER);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), COL_BORDER);

            var style = new GUIStyle(GUI.skin.button)
                { fontSize = 15, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = Color.white;
            return GUI.Button(rect, label, style);
        }

        bool SecondaryButton(string label, float height = 28f)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.miniButton,
                GUILayout.Height(height), GUILayout.MaxWidth(160));
            EditorGUI.DrawRect(rect, COL_PANEL_2);
            var style = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = COL_TEXT;
            return GUI.Button(rect, label, style);
        }

        bool MiniButton(string label, float width)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.miniButton, GUILayout.Width(width));
            EditorGUI.DrawRect(rect, COL_PANEL_2);
            var s = new GUIStyle(EditorStyles.miniButton);
            s.normal.textColor = COL_TEXT;
            return GUI.Button(rect, label, s);
        }

        // ---------- Drag & Drop ----------
        void HandleDragAndDrop(Rect dropArea)
        {
            var e = Event.current;
            if (e.type != EventType.DragUpdated && e.type != EventType.DragPerform) return;
            if (!dropArea.Contains(e.mousePosition)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                if (DragAndDrop.paths.Length == 1 && AssetDatabase.IsValidFolder(DragAndDrop.paths[0]))
                {
                    var folder = DragAndDrop.paths[0];
                    _scriptManager.Scan(folder);
                    _scriptsScanned = true;
                    _status = $"Scanned: {_scriptManager.Scripts.Count} files under '{folder}'.";
                }
                else
                {
                    var toAdd = new List<string>();
                    foreach (var p in DragAndDrop.paths)
                    {
                        if (AssetDatabase.IsValidFolder(p))
                            toAdd.AddRange(Directory.GetFiles(p, "*.cs", SearchOption.AllDirectories));
                        else if (p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                            toAdd.Add(p);
                    }

                    AddCsFiles(toAdd);
                }
            }

            e.Use();
        }

        // ---------- Scan helper ----------
        void ScanSelectionTarget()
        {
            if (_scanTarget == null)
            {
                EditorUtility.DisplayDialog("Scan", "Select a folder or a .cs file first.", "OK");
                return;
            }

            var path = AssetDatabase.GetAssetPath(_scanTarget);
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Scan", "Invalid asset.", "OK");
                return;
            }

            if (AssetDatabase.IsValidFolder(path))
            {
                _scriptManager.Scan(path);
                _scriptsScanned = true;
                _status = $"Scanned: {_scriptManager.Scripts.Count} files under '{path}'.";
            }
            else if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            {
                AddCsFiles(new[] { path });
            }
            else
            {
                EditorUtility.DisplayDialog("Scan", "Please select a folder or a .cs file.", "OK");
            }

            Repaint();
        }

        void AddCsFiles(IEnumerable<string> paths)
        {
            var set = new HashSet<string>(_scriptManager.Scripts.Select(s => s.Path), StringComparer.OrdinalIgnoreCase);
            int added = 0;
            foreach (var p in paths.Distinct())
            {
                if (!p.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;
                if (set.Contains(p)) continue;
                _scriptManager.Scripts.Add(new ScriptEntry { Path = p.Replace('\\', '/'), IsSelected = true });
                set.Add(p);
                added++;
            }

            if (added > 0)
            {
                _scriptsScanned = true;
                _status = $"Added {added} file(s) to the list.";
            }
            else _status = "No new .cs files added.";

            Repaint();
        }

        // ---------- Utils ----------
        private static Texture2D MakeTex(int w, int h, Color c)
        {
            var tex = new Texture2D(w, h);
            var pix = new Color[w * h];
            for (int i = 0; i < pix.Length; i++) pix[i] = c;
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }

        private static string AbsoluteToAssetPathSafe(string absolute)
        {
            if (string.IsNullOrEmpty(absolute)) return null;
            absolute = absolute.Replace("\\", "/");
            var data = Application.dataPath.Replace("\\", "/");
            if (absolute.StartsWith(data))
                return "Assets" + absolute.Substring(data.Length);
            return null;
        }

        // ---------- Generation (inchangé) ----------
        private void GenerateDiagram(ExportFormat format)
        {
            if (!_scriptsScanned || _scriptManager.Scripts.Count == 0)
            {
                _status = "❌ No scripts scanned or added. Use Scan or drop files.";
                EditorUtility.DisplayDialog("Error", "No scripts have been scanned.", "OK");
                return;
            }

            var selectedScripts = _scriptManager.GetSelected();
            if (selectedScripts.Count == 0)
            {
                _status = "❗ No script selected!";
                EditorUtility.DisplayDialog("Error", "Select at least one script to include in the diagram.", "OK");
                return;
            }

            var parser = new CSharpParser();
            var umlClasses = new List<UmlClass>();

            foreach (var script in selectedScripts)
            {
                string content = File.ReadAllText(script.Path);
                umlClasses.AddRange(parser.ParseClasses(content));
            }

            if (umlClasses.Count == 0)
            {
                _status = "❗ No classes detected. Check your scripts or parser logic.";
                return;
            }

            string plantuml = PlantUmlGenerator.GeneratePlantUml(umlClasses, _includeAssociations);

            if (format == ExportFormat.PlantUMLFile)
            {
                File.WriteAllText(_outputPath, plantuml, Encoding.UTF8);
                AssetDatabase.Refresh();
                _status = $"✅ Diagram generated: {_outputPath}  •  Classes: {umlClasses.Count}";
                EditorUtility.DisplayDialog("Done!", $"Diagram generated:\n{_outputPath}\nOpen it with PlantUML!",
                    "OK");
            }
            else if (format == ExportFormat.PlantUML_URL)
            {
                _lastDiagramURL = PlantUMLTextToUrl(plantuml);
                _status = "✅ Diagram URL generated! Copy-paste it in your browser.";
                EditorUtility.DisplayDialog("Done!", "URL generated below!\nCopy-paste it in your browser.", "OK");
            }
        }

        public static string PlantUMLTextToUrl(string uml)
        {
            byte[] data = Encoding.UTF8.GetBytes(uml);
            using (var ms = new MemoryStream())
            {
                using (var ds = new System.IO.Compression.DeflateStream(ms,
                           System.IO.Compression.CompressionLevel.Optimal, true))
                {
                    ds.Write(data, 0, data.Length);
                }

                var deflated = ms.ToArray();
                string encoded = PlantUmlBase64Encode(deflated);
                return "https://www.plantuml.com/plantuml/uml/" + encoded;
            }
        }

        private static readonly string _encode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";

        public static string PlantUmlBase64Encode(byte[] data)
        {
            var sb = new StringBuilder();
            int curr = 0, bits = 0;
            foreach (byte b in data)
            {
                curr = (curr << 8) | b;
                bits += 8;
                while (bits >= 6)
                {
                    bits -= 6;
                    sb.Append(_encode[(curr >> bits) & 0x3F]);
                }
            }

            if (bits > 0) sb.Append(_encode[(curr << (6 - bits)) & 0x3F]);
            return sb.ToString();
        }

        // -------- Models / Parser / Generator (inchangé) --------
        public class UmlClass
        {
            public string Name;
            public string BaseClass;
            public List<string> Interfaces = new();
            public bool IsAbstract;
            public bool IsInterface;
            public List<UmlField> Fields = new();
            public List<UmlProperty> Properties = new();
            public List<UmlMethod> Methods = new();
            public string Summary;
        }

        public class UmlField
        {
            public string Name;
            public string Type;
            public string Visibility;
        }

        public class UmlProperty
        {
            public string Name;
            public string Type;
            public string Visibility;
        }

        public class UmlMethod
        {
            public string Name;
            public string ReturnType;
            public string Visibility;
            public List<UmlParameter> Parameters = new();
        }

        public class UmlParameter
        {
            public string Name;
            public string Type;
        }

        private static List<string> SafeSplitBaseTypes(string input)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(input)) return result;

            var sb = new StringBuilder();
            int depth = 0;
            foreach (char c in input)
            {
                if (c == '<')
                {
                    depth++;
                    sb.Append(c);
                }
                else if (c == '>')
                {
                    depth = Math.Max(0, depth - 1);
                    sb.Append(c);
                }
                else if (c == ',' && depth == 0)
                {
                    result.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else sb.Append(c);
            }

            if (sb.Length > 0) result.Add(sb.ToString().Trim());

            return result;
        }
        
        private static string StripGenerics(string typeName)
        {
            int i = typeName.IndexOf('<');
            return (i >= 0) ? typeName.Substring(0, i) : typeName;
        }

        public class CSharpParser
        {
            private static Regex ClassRegex =
                new(
                    @"(?:///(.*)\n)?\s*(public|private|internal|protected|abstract|sealed|static|partial|\s)*\s*(class|interface)\s+(\w+)(\s*:\s*([\w,\s<>]+))?",
                    RegexOptions.Multiline);

            private static Regex FieldRegex = new(@"(public|private|protected|internal)\s+([\w<>,\[\]]+)\s+(\w+)\s*;",
                RegexOptions.Multiline);

            private static Regex PropertyRegex =
                new(@"(public|private|protected|internal)\s+([\w<>,\[\]]+)\s+(\w+)\s*\{\s*get;\s*set;\s*\}",
                    RegexOptions.Multiline);

            private static Regex MethodRegex =
                new(@"(public|private|protected|internal)\s+([\w<>,\[\]]+)\s+(\w+)\s*\(([^)]*)\)",
                    RegexOptions.Multiline);

            public List<UmlClass> ParseClasses(string content)
            {
                var classes = new List<UmlClass>();
                foreach (Match match in ClassRegex.Matches(content))
                {
                    var summary = match.Groups[1].Success ? match.Groups[1].Value.Trim() : string.Empty;
                    var isInterface = match.Groups[3].Value == "interface";
                    var className = match.Groups[4].Value;
                    var bases = match.Groups[6].Success
                        ? SafeSplitBaseTypes(match.Groups[6].Value)
                        : null;
                    string baseClass = null;
                    List<string> interfaces = new();
                    if (bases != null && bases.Count > 0)
                    {
                        if (!isInterface)
                        {
                            baseClass = bases[0];
                            if (bases.Count > 1) interfaces.AddRange(bases.Skip(1));
                        }
                        else
                        {
                            interfaces.AddRange(bases);
                        }
                    }


                    var c = new UmlClass
                    {
                        Name = className,
                        BaseClass = baseClass,
                        Interfaces = interfaces,
                        IsAbstract = match.Value.Contains("abstract"),
                        IsInterface = isInterface,
                        Summary = summary
                    };

                    foreach (Match f in FieldRegex.Matches(content))
                        c.Fields.Add(new UmlField
                        {
                            Visibility = GetVisibilitySymbol(f.Groups[1].Value), Type = f.Groups[2].Value,
                            Name = f.Groups[3].Value
                        });

                    foreach (Match p in PropertyRegex.Matches(content))
                        c.Properties.Add(new UmlProperty
                        {
                            Visibility = GetVisibilitySymbol(p.Groups[1].Value), Type = p.Groups[2].Value,
                            Name = p.Groups[3].Value
                        });

                    foreach (Match m in MethodRegex.Matches(content))
                    {
                        if (c.Name == m.Groups[3].Value) continue;
                        c.Methods.Add(new UmlMethod
                        {
                            Visibility = GetVisibilitySymbol(m.Groups[1].Value),
                            ReturnType = string.IsNullOrWhiteSpace(m.Groups[2].Value) ? "void" : m.Groups[2].Value,
                            Name = m.Groups[3].Value,
                            Parameters = ParseParameters(m.Groups[4].Value)
                        });
                    }

                    classes.Add(c);
                }

                return classes;
            }

            private static string GetVisibilitySymbol(string kw)
            {
                if (kw.Contains("public")) return "+";
                if (kw.Contains("private")) return "-";
                if (kw.Contains("protected")) return "#";
                if (kw.Contains("internal")) return "~";
                return "";
            }

            private static List<UmlParameter> ParseParameters(string raw)
            {
                var list = new List<UmlParameter>();
                if (string.IsNullOrWhiteSpace(raw)) return list;
                foreach (var param in raw.Split(','))
                {
                    var parts = param.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                        list.Add(new UmlParameter { Type = parts[0], Name = parts[1] });
                }

                return list;
            }
        }

        public static class PlantUmlGenerator
        {
            public static string GeneratePlantUml(List<UmlClass> classes, bool includeAssociations)
            {
                var sb = new StringBuilder();
                sb.AppendLine("@startuml");
                foreach (var c in classes)
                {
                    string stereotype = c.IsInterface ? " <<interface>>" : (c.IsAbstract ? " <<abstract>>" : "");
                    sb.AppendLine($"class {c.Name}{stereotype} {{");
                    foreach (var f in c.Fields) sb.AppendLine($"    {f.Visibility} {f.Name} : {f.Type}");
                    foreach (var p in c.Properties)
                        sb.AppendLine($"    {p.Visibility} {p.Name} : {p.Type} {{ get; set; }}");
                    foreach (var m in c.Methods)
                    {
                        var plist = string.Join(", ", m.Parameters.Select(p => $"{p.Name} : {p.Type}"));
                        sb.AppendLine($"    {m.Visibility} {m.Name}({plist}) : {m.ReturnType}");
                    }

                    sb.AppendLine("}");
                    if (!string.IsNullOrWhiteSpace(c.Summary)) sb.AppendLine($"' {c.Name}: {c.Summary}");
                }

                foreach (var c in classes)
                {
                    if (!string.IsNullOrEmpty(c.BaseClass)) 
                        sb.AppendLine($"{StripGenerics(c.BaseClass)} <|-- {c.Name}");
                    foreach (var iface in c.Interfaces) 
                        sb.AppendLine($"{StripGenerics(iface)} <|.. {c.Name}");
                }

                if (includeAssociations)
                {
                    var names = new HashSet<string>(classes.Select(cl => StripGenerics(cl.Name)));
                    var added = new HashSet<string>();

                    foreach (var c in classes)
                    {
                        string className = StripGenerics(c.Name);

                        foreach (var f in c.Fields)
                        {
                            string typeName = StripGenerics(f.Type);
                            if (names.Contains(typeName) && typeName != className && added.Add($"{className}-{typeName}-f"))
                                sb.AppendLine($"{className} --> {typeName} : field");
                        }

                        foreach (var p in c.Properties)
                        {
                            string typeName = StripGenerics(p.Type);
                            if (names.Contains(typeName) && typeName != className && added.Add($"{className}-{typeName}-p"))
                                sb.AppendLine($"{className} --> {typeName} : property");
                        }

                        foreach (var m in c.Methods)
                        {
                            foreach (var param in m.Parameters)
                            {
                                string typeName = StripGenerics(param.Type);
                                if (names.Contains(typeName) && typeName != className && added.Add($"{className}-{typeName}-a"))
                                    sb.AppendLine($"{className} --> {typeName} : parameter");
                            }
                        }
                    }
                }

                sb.AppendLine("@enduml");
                return sb.ToString();
            }
        }
    }
}
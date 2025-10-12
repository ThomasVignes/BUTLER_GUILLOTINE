# ClassDiagramGenerator

A modern Unity Editor tool to **generate PlantUML class diagrams** from your C# scripts.
Pick any folder (or the whole project), select scripts, and export a `.puml` file or a shareable PlantUML URL — all from the Unity Editor.

---

## Installation

This tool ships as Editor scripts.

1. Copy the following files into your project:

* `Assets/ClassDiagramGenerator/Editor/ClassDiagramGenerator.cs`
* `Assets/ClassDiagramGenerator/Editor/ScriptSelectionManager.cs`

2. (Optional) Add a 160×160 logo at `Resources/Icon 160x160 - Diagram Generator.png` if you want the header icon.

> Unity will compile them as Editor scripts automatically.

---

## Usage

1. **Open the window**
   `Tools → 🧬 Diagram Generator`

2. **Choose where to scan**

   * Enter a folder path, click **Browse…**, or press **Whole project** to scan `Assets/`.
   * You can also **drag & drop**:

     * Drop a **folder** to set it as the scan root.
     * Drop one or more **`.cs` files** to add them directly to the selection.

3. **Scan & select scripts**

   * Click **🔍 Scan** to list scripts.
   * Use **Search**, **All**, **None** to curate the list.

4. **Options**

   * **Export format**: `📄 PlantUML File` or `🌐 PlantUML URL`.
   * **Advanced**: *Include associations* (detects references between types via fields, properties, and parameters).
   * **Output path**: pick where to save the `.puml` (when using file export).

5. **Generate**

   * Click **🛠️ Generate Diagram**.
   * If you exported a file, you can **Reveal file**.
   * If you generated a URL, **Copy URL** and open it in your browser.

---

## What it extracts

* **Types**: classes, interfaces, structs, enums
* **Namespaces** → rendered as **PlantUML packages**
* **Members**:

  * Fields, properties, methods, events
  * Visibility: `+` public, `-` private, `#` protected, `~` internal
  * Modifiers surfaced where stable in PlantUML (`{static}`, `{abstract}`)
* **Relationships**:

  * Inheritance & interface realization
  * **Associations** from member/parameter types
  * Generic & array hints (adds `*` multiplicity for collections like `List<T>`, arrays, etc.)

> Inside a package block, type names are local (no dots).
> All references (arrows) use fully qualified names to avoid PlantUML syntax issues.

---

## Features

* English UI, polished Editor window
* Pick any folder to scan or **scan the whole project**
* **Drag & drop** folders and `.cs` files
* Script **search** and **bulk select**
* Export as **`.puml` file** or **PlantUML URL**
* One-click **Reveal file** / **Copy URL**
* Persistent last scan root (via `EditorPrefs`)
* Fast regex-based parsing with per-type scoping

---

## Troubleshooting

* **“Syntax Error (Assumed diagram type: class)” in PlantUML**
  Usually caused by dotted names inside declarations. This tool emits **local names in packages** and **qualified names only in arrows**, which avoids that. If you copy/paste or edit the `.puml`, don’t put dots in class headers inside a `package`.

* **Weird self-links**
  The generator skips self-associations. If you see one after manual edits, remove the `X --> X` line.

* **Nothing appears**
  Ensure you scanned the right folder, at least one script is selected, and your scripts contain standard C# type declarations.

---

## Notes & Limits

* Parsing is **regex-based**; it aims for practical coverage, not full C# semantics.
* Shows `{static}` and `{abstract}` reliably. Other modifiers are detected but not all are rendered to keep PlantUML happy.
* Accessor-level visibility (e.g., `get; private set;`) is not yet shown explicitly.
* The URL export uses **deflate + PlantUML encoding** and targets the public PlantUML server.

---

## FAQ

**Q: Do I have to keep scripts in `Assets/Scripts`?**
A: No. You can scan **any folder** (or the **whole project**) and even drop individual `.cs` files.

**Q: Does it overwrite existing `.puml` files?**
A: Yes, if you save to the same path.

**Q: Does it modify my scripts?**
A: No. It only reads `.cs` files and generates output.

**Q: Can it export images (PNG/SVG) directly?**
A: Not yet. Generate a `.puml` and render it via PlantUML (local or online). If you want built-in PNG/SVG export, open an issue—we can add it.

---

## Changelog (highlights)

* **v1.4**

  * English UI
  * Folder picker, drag & drop folders/files
  * Namespaces as packages; local names inside packages, fully qualified names in relationships
  * Associations improved (generics/arrays, multiplicity)
  * Self-link & double-qualification fixes
  * Output path picker, Reveal file

* **v1.3**

  * First pass on folder selection & persistence, improved styling

---

## Support

Questions, suggestions, or bug reports:
**[jules.gilli@live.fr](mailto:jules.gilli@live.fr)**

---

## License

MIT License

---

ClassDiagramGenerator © 2025 JulesTools

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClassDiagramGenerator
{
    public class ScriptEntry
    {
        public string Path;
        public bool IsSelected = true;
        public string FileName => System.IO.Path.GetFileName(Path);
        public string Directory => System.IO.Path.GetDirectoryName(Path)?.Replace('\\','/');
    }

    public class ScriptSelectionManager
    {
        public List<ScriptEntry> Scripts { get; private set; } = new List<ScriptEntry>();

        public void Scan(string folder, string extension = ".cs")
        {
            Scripts.Clear();
            if (string.IsNullOrEmpty(folder)) return;
            if (!Directory.Exists(folder)) return;

            var files = Directory.GetFiles(folder, "*" + extension, SearchOption.AllDirectories);
            Scripts = files
                .Where(f => !f.EndsWith(".meta"))
                .Select(f => new ScriptEntry { Path = f.Replace('\\','/'), IsSelected = true })
                .ToList();
        }

        public void AddFiles(IEnumerable<string> paths)
        {
            foreach (var p in paths)
            {
                if (string.IsNullOrEmpty(p) || !p.EndsWith(".cs")) continue;
                var normalized = p.Replace('\\','/');
                if (Scripts.Any(s => s.Path == normalized)) continue;
                Scripts.Add(new ScriptEntry { Path = normalized, IsSelected = true });
            }
        }

        public List<ScriptEntry> GetSelected() => Scripts.Where(s => s.IsSelected).ToList();
    }
}
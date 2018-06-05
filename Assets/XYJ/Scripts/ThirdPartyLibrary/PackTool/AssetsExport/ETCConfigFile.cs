#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public partial class AssetsExport    
    {
        public class ETCFile
        {
            // 使用情况
            public string r = string.Empty;
            public string g = string.Empty;
            public string b = string.Empty;

            public void Reset()
            {
                r = string.Empty;
                g = string.Empty;
                b = string.Empty;
            }

            public void Init(string file)
            {
                Reset();
                if (File.Exists(file))
                {
                    string[] lines = File.ReadAllLines(file);
                    for (int i = 0; i < lines.Length; ++i)
                    {
                        switch (lines[i][0])
                        {
                        case 'r': r = lines[i].Substring(2); break;
                        case 'g': g = lines[i].Substring(2); break;
                        case 'b': b = lines[i].Substring(2); break;
                        }
                    }
                }
            }

            public ETCType has(string file)
            {
                if (r == file)
                    return ETCType.R;
                if (g == file)
                    return ETCType.G;
                if (b == file)
                    return ETCType.B;

                return ETCType.Null;
            }

            public void RemoveFile(string file)
            {
                if (r == file)
                {
                    r = "";
                }

                if (g == file)
                {
                    g = "";
                }

                if (b == file)
                {
                    b = "";
                }
            }

            public void Set(ETCType type, string file)
            {
                switch (type)
                {
                case ETCType.R: r = file; break;
                case ETCType.G: g = file; break;
                case ETCType.B: b = file; break;
                }
            }

            public void SaveFile(string file)
            {
                List<string> lines = new List<string>();
                if (r != string.Empty)
                {
                    lines.Add(string.Format("r={0}", r));
                }

                if (g != string.Empty)
                {
                    lines.Add(string.Format("g={0}", g));
                }

                if (b != string.Empty)
                {
                    lines.Add(string.Format("b={0}", b));
                }

                File.WriteAllLines(file, lines.ToArray());
            }

            public ETCType GetEmpty()
            {
                if (string.IsNullOrEmpty(r))
                    return ETCType.R;

                if (string.IsNullOrEmpty(g))
                    return ETCType.G;

                if (string.IsNullOrEmpty(b))
                    return ETCType.B;

                return ETCType.Null;
            }

            public bool isEmpty
            {
                get { return string.IsNullOrEmpty(r) && string.IsNullOrEmpty(g) && string.IsNullOrEmpty(b); }
            }
        }
    }
}
#endif
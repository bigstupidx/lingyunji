using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PackTool
{
    public partial class MonoScriptAutoGen
    {
        [MenuItem("Assets/PackTool/GenScript", false, 0)] 
        static public void GenMonoBehaviour()
        {
            Object obj = Selection.activeObject;
            MonoScript s = obj as MonoScript;
            if (s != null)
            {
                Gen(s);
                AnewBuild();
                UnityEngine.Debug.Log(s.name);
            }
        }

        static public List<string> GetAllCSFileList(string path)
        {
            int startIndex = path.Length + 1;
            if (!System.IO.Directory.Exists(path))
                return new List<string>();

            string[] files = System.IO.Directory.GetFiles(path, "*.*", System.IO.SearchOption.AllDirectories);
            List<string> resList = new List<string>();
            string tmp;
            string s;
            for (int i = 0; i < files.Length; ++i)
            {
                s = files[i].Replace('\\', '/').Substring(startIndex);
                tmp = s.ToLower();
                if (!tmp.EndsWith(".cs"))
                    continue;

                resList.Add(s);
            }

            return resList;
        }

        // 遍历
        [MenuItem("Assets/PackTool/GenScriptAll", false, 0)]
        static public void GenAll()
        {
            List<string> files = GetAllCSFileList(Application.dataPath);
            PackResources PrefabRes = new PackResources(); // 预置体资源
            PackResources.GetResources<Object>(PrefabRes, files, null, null, new string[] { ".cs" });

            List<MonoScript> css = PrefabRes.GetList<MonoScript>();
            foreach (MonoScript cs in css)
            {
                Gen(cs);
            }

            if (css.Count != 0)
            {
                AnewBuild();
                AssetDatabase.ImportAsset("Assets/Scripts/PackTool/Component/Auto");
                AssetDatabase.Refresh();
            }
        }
    }
}
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public class DependenceList
    {
        public class DepList
        {
            public HashSet<string> Dic = new HashSet<string>();

            public bool ContainsKey(string res)
            {
                if (Dic.Contains(res))
                    return true;

                return false;
            }
        }

        Dictionary<string, DepList> Dic = new Dictionary<string, DepList>();

        // 当前所有的Mono脚本
        Dictionary<string, System.Type> MonoList = new Dictionary<string, System.Type>();
        Dictionary<string, List<string>> ScriptDepList = new Dictionary<string, List<string>>();

        static bool isInherited(System.Type type, System.Type baseType)
        {
            if (type == null)
                return false;

            if (type == baseType)
                return true;

            if (type.BaseType == null)
                return false;

            if (type.BaseType == baseType)
                return true;

            return isInherited(type.BaseType, baseType);
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

        void InitAllMonoList()
        {
            List<string> files = GetAllCSFileList(Application.dataPath);
            PackResources PrefabRes = new PackResources(); // 预置体资源
            PackResources.GetResources<Object>(PrefabRes, files, null, null, new string[] { ".cs" });

            List<MonoScript> css = PrefabRes.GetList<MonoScript>();
            string key;
            for (int i = 0; i < css.Count; ++i)
            {
                key = AssetDatabase.GetAssetPath(css[i]);
                MonoList.Add(key, css[i].GetClass());
            }
        }

        static List<string> EmptyList = new List<string>();

        // 得到与此相关的脚本资源
        List<string> GetDepMonoList(string file)
        {
            List<string> dep = null;
            if (ScriptDepList.TryGetValue(file, out dep))
                return dep;

            System.Type type = null;
            if (!MonoList.TryGetValue(file, out type))
            {
                ScriptDepList.Add(file, EmptyList);
                return EmptyList;
            }

            List<string> files = new List<string>();
            foreach (KeyValuePair<string, System.Type> itor in MonoList)
            {
                if (isInherited(type, itor.Value))
                {
                    files.Add(itor.Key);
                }
            }

            ScriptDepList.Add(file, files);
            return files;
        }

        public DepList GetDepList(string src)
        {
            DepList dep = null;
            if (Dic.TryGetValue(src, out dep))
                return dep;

            dep = new DepList();

            if (MonoList.Count == 0)
            {
                InitAllMonoList();
            }

            List<string> ress = new List<string>(AssetDatabase.GetDependencies(new string[] { src }));
            foreach (string res in ress)
            {
                if (res.EndsWith(".cs", true, null))
                {
                    List<string> s = GetDepMonoList(res);
                    if (s != null)
                    {
                        for (int i = 0; i < s.Count; ++i)
                        {
                            dep.Dic.Add(s[i]);
                        }
                    }
                }
                dep.Dic.Add(res);
            }

            Dic.Add(src, dep);
            return dep;
        }

        public DepList GetDepsList(string[] assets)
        {
            DepList depList = new DepList();
            foreach (string asset in assets)
            {
                DepList dl = GetDepList(asset);
                foreach (string key in dl.Dic)
                    depList.Dic.Add(key);
            }

            return depList;
        }

        public DepList GetDepsList(Object[] objs)
        {
            List<string> deps = new List<string>();
            foreach (Object o in objs)
                deps.Add(AssetDatabase.GetAssetPath(o));

            return GetDepsList(deps.ToArray());
        }

        public void Release()
        {
            Dic.Clear();
        }
    }
}
#endif
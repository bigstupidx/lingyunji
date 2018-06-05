#if USE_ABL
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ABCheckEditor : EditorWindow
    {
        [MenuItem("PackTool/AB文件查看", false, 9)]
        static public void OpenAssetFileCheckEditor()
        {
            EditorWindow.GetWindow<ABCheckEditor>(false, "ABCheckEditor", true);
        }

        EditorPageBtn page = new EditorPageBtn();

        // 所有的类型
        List<string> types = new List<string>();
        int showType = -1;

        List<FileData> ShowAllFileList = new List<FileData>();
        long ShowTotal = 0;

        string[] ShowTypes_;

        string[] ShowTypes
        {
            get
            {
                return ShowTypes_;
            }
        }

        class AssetInfo
        {
            public HashSet<string> abs = new HashSet<string>();
        }

        static Dictionary<string, AssetInfo> DuplicateResources = new Dictionary<string, AssetInfo>();

        long GetSizeType(string type)
        {
            long t = 0;
            AllFileList.ForEach((fd) =>
            {
                if (fd.types.Contains(type))
                {
                    t += fd.DstSize;
                }
            });

            return t;
        }

        enum ShowTypeEnum
        {
            Size,
            Check,
        }

        ShowTypeEnum showTypeEnum = ShowTypeEnum.Size;

        void OnGUI()
        {
            if (GUILayout.Button("初始化", GUILayout.Height(50f)))
            {
                Init();
            }

            showTypeEnum = (ShowTypeEnum)EditorGUILayout.EnumPopup("显示类型", showTypeEnum);
            switch (showTypeEnum)
            {
            case ShowTypeEnum.Size:
                OnGUISize();
                break;
            case ShowTypeEnum.Check:
                OnGUICheck();
                break;
            }
        }

        EditorPageBtn check_page = new EditorPageBtn();
        void OnGUICheck()
        {
            List<KeyValuePair<string, AssetInfo>> Duplicates = new List<KeyValuePair<string, AssetInfo>>();
            foreach (var itor in DuplicateResources)
            {
                if (itor.Value.abs.Count != 1)
                    Duplicates.Add(itor);
            }

            GUILayout.Label(string.Format("重复打包资源:{0}", Duplicates.Count));
            check_page.total = Duplicates.Count;
            check_page.OnRender();
        }

        enum SortType
        {
            Null,
            Size,
            AssetCount,
        }

        SortType sortType = SortType.Null;

        void OnGUISize()
        {
            GUILayout.Label(string.Format("打包后大小:{0}", XTools.Utility.ToMb(totalDstSize)));
            if (ShowTypes != null)
            {
                var st = EditorGUILayout.MaskField("过滤", showType, ShowTypes);
                var sst = (SortType)EditorGUILayout.EnumPopup("排序", sortType);
                if (st != showType || ShowAllFileList.Count == 0 || sst != sortType)
                {
                    sortType = sst;
                    showType = st;
                    HashSet<string> sts = new HashSet<string>();
                    for (int i = 0; i < types.Count; ++i)
                    {
                        if ((st & (1 << i)) != 0)
                            sts.Add(types[i]);
                    }

                    ShowTotal = 0;
                    ShowAllFileList.Clear();
                    AllFileList.ForEach((fd) =>
                    {
                        for (int i = 0; i < fd.types.Count; ++i)
                        {
                            if (sts.Contains(fd.types[i]))
                            {
                                ShowTotal += fd.DstSize;
                                ShowAllFileList.Add(fd);
                                return;
                            }
                        }
                    });

                    switch (sst)
                    {
                    case SortType.Null:
                        break;
                    case SortType.Size:
                        ShowAllFileList.Sort((x, y) => { return y.DstSize.CompareTo(x.DstSize); });
                        break;
                    case SortType.AssetCount:
                        ShowAllFileList.Sort((x, y) => { return y.assets.Length.CompareTo(x.assets.Length); });
                        break;
                    }
                }
            }

            page.total = ShowAllFileList.Count;
            page.OnRender();

            GUILayout.Label(string.Format("当前大小:{0} {1}%", XTools.Utility.ToMb(ShowTotal), (100f * ShowTotal / totalDstSize).ToString("0.00")));
            page.ForEach((int index) =>
            {
                FileData fd = ShowAllFileList[index];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(string.Format("{0})", index), GUILayout.Width(40f));
                EditorGUILayout.LabelField(string.Format("T:{0}", fd.assets.Length), GUILayout.Width(50f));
                EditorGUILayout.LabelField(string.Format("size:{0}", XTools.Utility.ToMb(fd.DstSize)), GUILayout.Width(90f));
                EditorGUILayout.LabelField(string.Format("type:{0}", fd.type), GUILayout.Width(300f));
                if (GUILayout.Button(string.Format("file:{0}", fd.initfile)))
                {
                    EditorUtility.RevealInFinder(fd.initfile);
                }

                EditorGUILayout.EndHorizontal();
            },
            true);
        }

        class FileData
        {
            public string initfile = "";
            public long DstSize; // 文件大小

            // 资源类型
            public List<string> types = new List<string>();

            public bool isShow = false;

            public string[] assets { get; private set; }

            public string type { get; private set; }

            public bool init(string file)
            {
                if (file.EndsWith(".manifest"))
                    return false;

                initfile = file;
                DstSize = GetFileSize(file);
                if (!file.EndsWith(".unity", true, null))
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(file);
                    try
                    {
                        var objs = ab.LoadAllAssets();
                        foreach (var obj in objs)
                        {
                            var t = obj.GetType().Name;
                            if (types.Contains(t))
                                continue;

                            types.Add(t);
                        }

                        assets = ab.GetAllAssetNames();
                        foreach (var asset in assets)
                        {
                            AssetInfo info = null;
                            if (!DuplicateResources.TryGetValue(asset, out info))
                            {
                                info = new AssetInfo();
                                DuplicateResources.Add(asset, info);
                            }

                            info.abs.Add(file);
                        }
                    }
                    finally
                    {
                        ab.Unload(true);
                    }
                }
                else
                {
                    assets = new string[1] { "scene" };
                    types.Add("unity");
                }

                if (types.Count == 0)
                {
                    type = "null";
                    return true;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(types[0]);
                for (int i = 1; i < types.Count; ++i)
                    sb.AppendFormat(",{0}", types[i]);
                type = sb.ToString();

                return true;
            }
        }

        List<FileData> AllFileList = new List<FileData>();

        long totalDstSize = 0;

        static long GetFileSize(string file)
        {
            if (!File.Exists(file))
                return 0;

            System.IO.FileInfo info = new FileInfo(file);
            return info.Length;
        }

        static string Root_;
        static string Root
        {
            get
            {
                if (string.IsNullOrEmpty(Root_))
                    Root_ = ResourcesPath.LocalDataPath + "AB/";
                return Root_;
            }
        }

        class TypeInfo
        {
            public long totalSize = 0; // 总大小
            public long totalCount = 0; // 总文件数
        }

        // 初始化
        void Init()
        {
            string dstfilepath = Root;
            if (!Directory.Exists(dstfilepath))
                return;

            DuplicateResources.Clear();
            totalDstSize = 0;
            ShowTotal = 0;
            AllFileList.Clear();
            ShowAllFileList.Clear();
            types.Clear();
            ShowTypes_ = null;
            string[] files = Directory.GetFiles(dstfilepath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileData fd = new FileData();
                if (fd.init(file))
                {
                    AllFileList.Add(fd);
                    totalDstSize += fd.DstSize;

                    foreach (var t in fd.types)
                    {
                        if (types.Contains(t))
                            continue;
                        types.Add(t);
                    }
                }
            }

            //AllFileList.Sort((FileData x, FileData y) => { return y.DstSize.CompareTo(x.DstSize); });

            Dictionary<string, TypeInfo> type_to_info = new Dictionary<string, TypeInfo>();
            foreach (var fd in AllFileList)
            {
                foreach (var t in fd.types)
                {
                    TypeInfo v = null;
                    if (!type_to_info.TryGetValue(t, out v))
                    {
                        v = new TypeInfo();
                        type_to_info.Add(t, v);
                    }
                    
                    v.totalSize += fd.DstSize;
                    ++v.totalCount;
                }
            }

            types.Sort((string x, string y) => 
            {
                return type_to_info[y].totalSize.CompareTo(type_to_info[x].totalSize);
            });

            ShowTypes_ = new string[types.Count];
            for (int i = 0; i < types.Count; ++i)
            {
                var info = type_to_info[types[i]];
                ShowTypes_[i] = string.Format("{0}(Count:{1} {2} [{3}%])", types[i], info.totalCount, XTools.Utility.ToMb(info.totalSize), (100f * info.totalSize / totalDstSize).ToString("0.00"));
            }
        }
    }
}
#endif
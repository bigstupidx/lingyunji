using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ABSeeEditor : EditorWindow
    {
        [MenuItem("PackTool/ABSee文件查看", false, 9)]
        static public void OpenAssetFileCheckEditor()
        {
            EditorWindow.GetWindow<ABSeeEditor>(false, "ABSeeEditor", true);
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
            public List<FileData> abs = new List<FileData>();
            public bool isShow = false;
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

        static GUIEditor.EmptyObject Empty = new GUIEditor.EmptyObject();

        void OnGUICheck()
        {
            List<KeyValuePair<string, AssetInfo>> Duplicates = new List<KeyValuePair<string, AssetInfo>>();
            foreach (var itor in DuplicateResources)
            {
                if (itor.Value.abs.Count != 1)
                    Duplicates.Add(itor);
            }

            Duplicates.Sort((x, y)=> { return y.Value.abs.Count.CompareTo(x.Value.abs.Count); });

            var pl = paramList.Get<ParamList>("OnGUICheck");
            GUILayout.Label(string.Format("重复打包资源:{0}", Duplicates.Count));

            GUIEditor.GuiTools.ObjectFieldList(
                pl, 
                Duplicates, 
                (ator)=> 
                {
                    return AssetDatabase.LoadAssetAtPath<Object>(ator.Key);
                },
                false, 
                null,
                null,
                (ator)=> 
                {
                    if (GUILayout.Button(string.Format("{0}({1})", ator.Value.isShow ? "S" : "H", ator.Value.abs.Count), GUILayout.Width(50f)))
                        ator.Value.isShow = !ator.Value.isShow;

                    if (ator.Value.isShow)
                    {
                        EditorGUILayout.EndHorizontal();
                        GUIEditor.GuiTools.ObjectFieldList<FileData>(
                            pl.Get<ParamList>(ator.Key), 
                            ator.Value.abs, 
                            (assetPath)=> 
                            {
                                return Empty;
                            }, 
                            true, 
                            null, 
                            null,
                            (fd)=> 
                            {
                                fd.OnGUI(pl, -1);
                            }, 
                            null);
                        EditorGUILayout.BeginHorizontal();
                    }
                });
        }

        enum SortType
        {
            Null,
            Size,
            AssetCount,
        }

        SortType sortType = SortType.Null;

        ParamList paramList = new ParamList();

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
                        ShowAllFileList.Sort((x, y) => { return y.assets.Count.CompareTo(x.assets.Count); });
                        break;
                    }
                }
            }

            page.total = ShowAllFileList.Count;
            page.pageNum = 30;
            page.OnRender();

            GUILayout.Label(string.Format("当前大小:{0} {1}%", XTools.Utility.ToMb(ShowTotal), (100f * ShowTotal / totalDstSize).ToString("0.00")));
            page.ForEach((int index) =>
            {
                FileData fd = ShowAllFileList[index];
                fd.OnGUI(paramList, index);
            },
            true);
        }

        class FileData
        {
            public string initfile = "";
            public long DstSize; // 文件大小

            public bool isShowAssets = false;

            // 资源类型
            public List<string> types = new List<string>();

            public bool isShow = false;

            public List<string> assets { get; private set; }

            public string type { get; private set; }

            public bool init(string file)
            {
                file = file.Replace('\\', '/');
                string dstpath = ResourcesPath.LocalDataPath + file;
                initfile = file;
                DstSize = GetFileSize(dstpath);
                if (XTools.Utility.isAB(file))
                {
                    AssetBundle ab = null;
                    try
                    {
                        ab = AssetBundle.LoadFromFile(dstpath);
                        if (!ab.isStreamedSceneAssetBundle)
                        {
                            var objs = ab.LoadAllAssets();
                            foreach (var obj in objs)
                            {
                                var t = obj.GetType().Name;
                                if (t == "MonoScript")
                                    continue;

                                if (types.Contains(t))
                                    continue;

                                types.Add(t);
                            }
                        }
                        else
                        {
                            types.Add(typeof(UnityEngine.SceneManagement.Scene).Name);
                        }

                        assets = new List<string>();
                        foreach (var asset in ab.GetAllAssetNames())
                        {
                            if (asset.EndsWith(".cs", true, null) ||
                                asset.EndsWith(".js", true, null) ||
                                asset.EndsWith(".dll", true, null))
                                continue;

                            assets.Add(asset);
                            AssetInfo info = null;
                            if (!DuplicateResources.TryGetValue(asset, out info))
                            {
                                info = new AssetInfo();
                                DuplicateResources.Add(asset, info);
                            }

                            info.abs.Add(this);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                        Debug.LogErrorFormat("error:{0}", file);
                    }
                    finally
                    {
                        if (ab != null)
                            ab.Unload(true);
                    }
                }
                else
                {
                    assets = new List<string>();
                    types.Add("noab");
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

            public void OnGUI(ParamList pl, int index)
            {
                GUIEditor.GuiTools.HorizontalField(false, () =>
                {
                    if (index != -1)
                        EditorGUILayout.LabelField(string.Format("{0})", index), GUILayout.Width(40f));

                    if (GUILayout.Button(string.Format("{0}({1})", isShowAssets ? "S依赖" : "H依赖", assets.Count), GUILayout.Width(80f)))
                        isShowAssets = !isShowAssets;
                    EditorGUILayout.LabelField(string.Format("size:{0}", XTools.Utility.ToMb(DstSize)), GUILayout.Width(90f));
                    EditorGUILayout.LabelField(string.Format("type:{0}", type), GUILayout.Width(400f));
                    if (GUILayout.Button(string.Format("file:{0}", initfile)))
                        EditorUtility.RevealInFinder(Root + initfile);

                    if (isShowAssets)
                    {
                        EditorGUILayout.EndHorizontal();
                        GUIEditor.GuiTools.ObjectFieldList<string>(
                            pl.Get<ParamList>(initfile),
                            assets,
                            (asset) => { return AssetDatabase.LoadAssetAtPath<Object>(asset); ; },
                            false,
                            null,
                            null,
                            null);

                        EditorGUILayout.BeginHorizontal();
                    }

                });
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
                    Root_ = ResourcesPath.LocalDataPath;
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
            paramList.ReleaseAll();
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
                if (fd.init(file.Substring(dstfilepath.Length)))
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

                //if (AllFileList.Count >= 1000)
                //    break;
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
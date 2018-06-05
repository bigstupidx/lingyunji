#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GUIEditor;

namespace PackTool
{
    public class AssetFileCheckEditor : EditorWindow
    {
        [MenuItem("PackTool/打包后文件比较", false, 9)]
        static public void OpenAssetFileCheckEditor()
        {
            EditorWindow.GetWindow<AssetFileCheckEditor>(false, "AssetFileCheckEditor", true);
        }

        void OnGUI()
        {
            if (GUILayout.Button("初始化", GUILayout.Height(50f)))
            {
                Init();
            }

            GUILayout.Label(string.Format("源大小:{0} 打包后大小:{1}", XTools.Utility.ToMb(totalSrcSize), XTools.Utility.ToMb(totalDstSize)));

            GuiTools.ObjectFieldList<FileData>(
                mParamList, 
                AllFileList, 
                (FileData fd) => { return fd.asset; }, false, 
                (List<FileData> fds, ParamList pl)=>
                {
                    if (fds.Count != 0)
                    {
                        bool isSorted = pl.Get("isSorted", true);
                        isSorted = EditorGUILayout.Toggle("文件大小排序", isSorted);
                        pl.Set("isSorted", isSorted);
                        if (isSorted)
                        {
                            bool isReverse = pl.Get("isReverse", true);
                            isReverse = EditorGUILayout.Toggle("倒序", isReverse);
                            pl.Set("isReverse", isReverse);
                            if (isSorted)
                            {
                                fds.Sort((FileData x, FileData y) =>
                                {
                                    return x.DstSize.CompareTo(y.DstSize);
                                });

                                if (isReverse)
                                    fds.Reverse();
                            }
                        }
                    }
                },
                (FileData fd)=>
                {
                    //EditorGUILayout.TextField(fd.SrcPath);
                },
                (FileData fd) =>
                {
                    if (fd.asset == null)
                    {
                        EditorGUILayout.LabelField("src:" + fd.SrcPath);
                        EditorGUILayout.LabelField("dst:" + fd.DstPath);
                    }
                    else
                    {
                        if (GUILayout.Button("打包", GUILayout.Width(40f)))
                        {
                            AssetsExport.Export(fd.asset, BuildAssetBundleOptions.DeterministicAssetBundle);

                            fd.init(fd.initfile);
                        }

//                         float used = fd.SrcSize == 0f ? 1f : (100.0f * fd.DstSize / fd.SrcSize);
//                         EditorGUILayout.LabelField("used:" + used.ToString("f2") + "%", GUILayout.Width(80f));
                        EditorGUILayout.LabelField("src:" + XTools.Utility.ToMb(fd.SrcSize), GUILayout.Width(100f));
                        EditorGUILayout.LabelField("dst:" + XTools.Utility.ToMb(fd.DstSize), GUILayout.Width(100f));
                        if (GUILayout.Button(fd.DstPath))
                        {
                            UnityEditor.EditorUtility.RevealInFinder(AssetsExport.PackPath + fd.DstPath);
                            //Application.OpenURL(AssetsExport.PackPath + fd.DstPath);
                        }
                    }
                }, GUILayout.Width(350f));
        }

        ParamList mParamList = new ParamList();

        class FileData
        {
            public string SrcPath; // 源文件
            public int SrcSize; // 文件大小

            public string DstPath; // 压缩之后的文件
            public int DstSize; // 文件大小

            Object _asset = null;

            // 对应的源资源
            public Object asset
            { 
                get 
                {
                    if (_asset == null)
                    {
                        _asset = AssetDatabase.LoadAssetAtPath("Assets/" + SrcPath, typeof(Object));
                    }
                    return _asset;
                } 
            } 

            public bool isShow = false;

            public string initfile = "";

            public bool init(string file)
            {
                string dstfilepath = AssetsExport.PackPath;

                string sfile = file.Replace('\\', '/');
                string f = sfile.Substring(dstfilepath.Length);
                if (!f.Contains("_"))
                    return false;
                if (f.StartsWith("Data/"))
                    return false;

                if (f.StartsWith("resoruce_config") ||
                    f.StartsWith("sprites_atlas.b"))
                {
                    return false;
                }

                initfile = file;
                DstPath = f;
                DstSize = GetFileSize(sfile);

                string assetpath = ToSrcPath(f);
                SrcPath = Application.dataPath + "/" + assetpath;
                SrcSize = GetFileSize(SrcPath);
                SrcPath = assetpath;

                return true;
            }
        }

        List<FileData> AllFileList = new List<FileData>();

        int totalDstSize = 0;
        int totalSrcSize = 0;

        static string ToSrcPath(string file)
        {
            string srcfile = (Application.dataPath + "/" + file).Replace('\\', '/');
            if (File.Exists(srcfile))
                return file;

            srcfile = (Application.dataPath + "/__copy__/" + file).Replace('\\', '/');
            if (File.Exists(srcfile))
                return "__copy__/" + file;

            file = file.Substring(0, file.LastIndexOf('.'));
            int p = file.LastIndexOf('_');
            if (p != -1)
                file = file.Substring(0, p) + "." + file.Substring(p + 1);

            srcfile = Application.dataPath + "/" + file;
            if (File.Exists(srcfile))
                return file;

            srcfile = Application.dataPath + "/" + file.Replace("-", " ");
            if (File.Exists(srcfile))
                return file.Replace("-", " ");

            return file;
        }

        static int GetFileSize(string file)
        {
            if (!File.Exists(file))
                return 0;

            System.IO.FileInfo info = new FileInfo(file);
            return (int)info.Length;
        }

        // 初始化
        void Init()
        {
            string dstfilepath = AssetsExport.PackPath;
            if (!Directory.Exists(dstfilepath))
                return;

            mParamList.ReleaseAll();
            totalDstSize = 0;
            totalSrcSize = 0;
            AllFileList.Clear();
            string[] files = Directory.GetFiles(dstfilepath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (file.EndsWith(Suffix.PrefabDataByte) || file.EndsWith(Suffix.SceneDataByte) || file.EndsWith(Suffix.ScenePosByte))
                    continue;

                FileData fd = new FileData();
                if (fd.init(file))
                {
                    AllFileList.Add(fd);

                    totalDstSize += fd.DstSize;
                    totalSrcSize += fd.SrcSize;
                }
            }
        }
    }
}
#endif
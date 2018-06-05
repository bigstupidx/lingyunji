#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PackTool;
using XTools;

public class ArchiveEditorWindow : EditorWindow
{
    [MenuItem("PackTool/Archive", false, 9)]
    [MenuItem("Assets/PackTool/Archive", false, 0)]
    static public void OpenPackEditorWindow()
    {
        EditorWindow.GetWindow<ArchiveEditorWindow>(false, "ArchiveEditorWindow", true);
    }

    void OnDisable() 
    {
        if (archive != null)
            archive.Close();

        archive = null;
    }

    void OnEnable()
    {
        Logger.CreateInstance();
    }

    Archive archive = null;
    EditorPageBtn epg = new EditorPageBtn();
    Vector2 ScrollPosition;

    string SearchKey = "";
    string filename = "";

    void OnGUI()
    {
        GUIEditor.GuiTools.HorizontalField(false, 
            () => 
            {
                if (GUILayout.Button("读取文档", GUILayout.Height(50f)))
                {
                    filename = EditorPrefs.GetString("ArchiveEditorWindow.ReadFile", Application.dataPath + "/StreamingAssets/");
                    filename = EditorUtility.OpenFilePanel("读取资源包", filename, "");
                    if (string.IsNullOrEmpty(filename))
                        return;

                    EditorPrefs.SetString("ArchiveEditorWindow.ReadFile", filename);
                    if (archive != null)
                        archive.Close();

                    archive = new Archive(filename, 0);
                    archive.InitDefault();
                }

                if (GUILayout.Button("制作资源包", GUILayout.Height(50f)))
                {
                    string writefile = EditorPrefs.GetString("ArchiveEditorWindow.WriteFile", ResourcesPath.LocalBasePath);
                    writefile = EditorUtility.OpenFolderPanel("读取目录", writefile, "");
                    if (string.IsNullOrEmpty(writefile))
                        return;

                    EditorPrefs.SetString("ArchiveEditorWindow.WriteFile", writefile);
                    Archive.AssetPackArchive(writefile, writefile + AssetZip.patchsuffix, "");
                    EditorUtility.RevealInFinder(writefile + AssetZip.patchsuffix);
                }

                if (GUILayout.Button("读取apk当中的文档", GUILayout.Height(50f)))
                {
                    filename = EditorPrefs.GetString("ArchiveEditorWindow.ReadFile", Application.dataPath + "/StreamingAssets/");
                    filename = EditorUtility.OpenFilePanel("读取资源包", filename, "*.apk");
                    if (string.IsNullOrEmpty(filename))
                        return;

                    EditorPrefs.SetString("ArchiveEditorWindow.ReadFile", filename);
                    if (archive != null)
                        archive.Close();

                    AssetZip zip = new AssetZip();
                    zip.Init(filename, "");
                    ICSharpCode.SharpZipLib.Zip.ZipFile.PartialInputStream stream = zip.FindFileStream("assets/" + AssetZip.DefaultName) as ICSharpCode.SharpZipLib.Zip.ZipFile.PartialInputStream;

                    archive = new Archive(filename, (int)stream.StartPos);
                    archive.InitDefault();
                }
            });

        List<Archive.FileItem> tempitems = null;
        SearchKey = EditorGUILayout.TextField("搜索关键字", SearchKey);
        string sk = SearchKey.ToLower();
        if (!string.IsNullOrEmpty(sk))
        {
            tempitems = new List<Archive.FileItem>();
            foreach (Archive.FileItem item in archive.fileItems)
            {
                if (item.name.ToLower().Contains(sk.ToLower()))
                    tempitems.Add(item);
            }
        }
        else
        {
            if (archive != null && archive.fileItems != null)
                tempitems = new List<Archive.FileItem>(archive.fileItems);
            else
                tempitems = new List<Archive.FileItem>();
        }

        if (archive != null && GUILayout.Button("解压文档", GUILayout.Height(50f)))
        {
            string dstpath = EditorPrefs.GetString("ArchiveEditorWindow.SavePath", Application.dataPath);
            dstpath = EditorUtility.OpenFolderPanel("目录", Application.dataPath, "Data");
            if (string.IsNullOrEmpty(dstpath))
                return;
            EditorPrefs.SetString("ArchiveEditorWindow.SavePath", dstpath);
            TimeCheck tc = new TimeCheck(true);
            archive.Unarchive("", dstpath);
            Debug.Log("解压用时:" + tc.delay);
        }

        epg.total = tempitems.Count;
        epg.pageNum = 50;
        epg.OnRender();
        ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
        for (int i = epg.beginIndex; i < epg.endIndex; ++i)
        {
            Archive.FileItem item = tempitems[i];

            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
            GUI.backgroundColor = Color.white;
            GUILayout.Label((i+1).ToString(), GUILayout.Width(24f));
             
            EditorGUILayout.TextField(item.name);
            EditorGUILayout.TextField(string.Format("{0}({1})", item.size, XTools.Utility.ToMb(item.size)), GUILayout.Width(150f));
            EditorGUILayout.TextField(item.crc32.ToString("X"), GUILayout.Width(80f));
            EditorGUILayout.TextField((archive.data_offset + item.offset).ToString(), GUILayout.Width(80f));
            if (GUILayout.Button("导出", GUILayout.Width(40f)))
            {
                string path = item.name;
                if (path.LastIndexOf('/') != -1)
                    path = path.Substring(0, path.LastIndexOf('/'));

                path = EditorUtility.SaveFilePanel(
                    "导出文件",//不导出中文
                    System.IO.Path.GetFileNameWithoutExtension(filename) + "_un/" + path,
                    System.IO.Path.GetFileName(item.name),
                    (string.IsNullOrEmpty(System.IO.Path.GetExtension(item.name)) ? "" : System.IO.Path.GetExtension(item.name).Substring(1)));

                if (!string.IsNullOrEmpty(path))
                {
                    Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));

                    //bool islzma = Utility.isLzma(item.name);
                    //if (!islzma)
                    {
                        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                        fs.SetLength(item.size);
                        try
                        {
                            Utility.CopyStream(item.GetStream(archive), fs, new byte[1024 * 1024]);
                            fs.Close();
                        }
                        catch (System.Exception ex)
                        {
                            File.Delete(path);
                            Logger.LogException(ex);
                        }
                    }
                    //else
                    //{
                    //    lzma.DecodeFile(archive.data_file, item.offset + archive.data_offset, path);
                    //}

                    Application.OpenURL(path.Substring(0, path.LastIndexOf('/')));
                }
            }
            
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
#endif
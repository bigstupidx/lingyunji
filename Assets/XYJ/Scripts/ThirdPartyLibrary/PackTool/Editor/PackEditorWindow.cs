//#if USE_RESOURCESEXPORT
//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using PackTool;
//using UI;
//#if USE_UATLAS
//using ATLAS = UI.uAtlas;
//#else
//using ATLAS = UI.Atlas;
//#endif
//#pragma warning disable 618

//public class PackEditorWindow : EditorWindow
//{
//    [MenuItem("PackTool/Window", false, 9)]
//    [MenuItem("Assets/PackTool/Window", false, 0)]
//    static public void OpenPackEditorWindow()
//    {
//        EditorWindow.GetWindow<PackEditorWindow>(false, "PackWindow", true);
//    }

//    static public PackEditorWindow Instance;

//    void OnEnable() { Instance = this; }
//    void OnDisable() { Instance = null; }

//    [MenuItem("PackTool/Pause")]
//    public static void Pause()
//    {
//        if (Instance != null)
//        {
//            Instance.mAssetExportMgr.isPause = !Instance.mAssetExportMgr.isPause;
//        }
//    }

//    [MenuItem("PackTool/Step")]
//    public static void Step()
//    {
//        if (Instance != null)
//        {
//            Instance.mAssetExportMgr.isStop = true;
//        }
//    }

//    [MenuItem("PackTool/Relese")]
//    public static void Relese()
//    {
//        if (Instance != null)
//        {
//            Instance.mAssetExportMgr.Release();
//        }
//    }

//    // 当前要打包的路径
//    string PackPath = ""; // 当前要打包的路径
//    HelpShowEditor.AssetList<GameObject> mPrefabList = new HelpShowEditor.AssetList<GameObject>();

//    void ShowPrefabInfo()
//    {
//        string RealPath ;
//        PackPath = GUILayout.TextField(PackPath);
//        if (string.IsNullOrEmpty(PackPath))
//            RealPath = Application.dataPath;
//        else
//            RealPath = Application.dataPath + "/" + PackPath;

//        GUILayout.Label("真实路径:" + RealPath);
//        GUILayout.Label("存储路径:" + AssetsExport.PackPath + PackPath);

//        if (Directory.Exists(RealPath))
//        {
//            HelpShowEditor.ShowResourcesInfo<GameObject>(RealPath, GetDirectoryPrefabs, "预置", PackAllResources, mPrefabList);//不导出中文
//        }
//        else
//        {
//            GUILayout.Label("要打包的路径不存在！");
//        }
//    }

//    string ScenePackPath = "Scene/Levels";
//    HelpShowEditor.AssetList<Object> mSceneList = new HelpShowEditor.AssetList<Object>();

//    void ShowSceneInfo()
//    {
//        ScenePackPath = GUILayout.TextField(ScenePackPath);
//        string RealPath = Application.dataPath + "/" + ScenePackPath;
//        GUILayout.Label("场景路径:" + ScenePackPath);
//        GUILayout.Label("存储路径:" + AssetsExport.PackPath + ScenePackPath);

//        if (Directory.Exists(RealPath))
//        {
//            HelpShowEditor.ShowResourcesInfo<Object>(RealPath, GetDirectoryScenes, "场景", PackAllResources, mSceneList);//不导出中文
//        }
//        else
//        {
//            GUILayout.Label("要打包的路径不存在！");
//        }
//    }

//    HelpShowEditor.AssetList<AudioClip> mAudioClipList = new HelpShowEditor.AssetList<AudioClip>();

//    // 显示音频信息
//    void ShowAudioClipInfo()
//    {
//        HelpShowEditor.ShowResourcesInfo<AudioClip>(Application.dataPath, GetDirectoryAudioClip, "音效", PackAllResources, mAudioClipList);//不导出中文
//    }

//    // 显示材质信息
//    void ShowMaterialsInfo()
//    {
//        HelpShowEditor.ShowResourcesInfo<Material>(Application.dataPath, GetDirectoryMaterial, "材质", PackAllResources, MaterialAsset);//不导出中文
//    }

//    Vector2 mGlobalPos;
//    bool PackAllResources; // 打包所有资源

//    // 材质资源
//    HelpShowEditor.AssetList<Material> MaterialAsset = new HelpShowEditor.AssetList<Material>();

//    AssetsExport mAssetExportMgr = new AssetsExport();
//    PackCollectList mPackCollectList = new PackCollectList();

////     string UIPath = Application.dataPath + "/UIData";
////     HelpShowEditor.AssetList<UIFont> UIFontAsset = new HelpShowEditor.AssetList<UIFont>();

////     void ShowUIFontInfo()
////     {
////         HelpShowEditor.ShowResourcesInfo<UIFont>(UIPath, GetDirectoryUIFont, "字体", PackAllResources, UIFontAsset);//不导出中文
////     }

//    HelpShowEditor.AssetList<ATLAS> AtlasAsset = new HelpShowEditor.AssetList<ATLAS>();
//    string UIPath = Application.dataPath + "/Art/UIData/UData/Atlas";
//    void ShowAtlasInfo()
//    {
//        HelpShowEditor.ShowResourcesInfo<ATLAS>(UIPath, GetDirectoryAtlas, "图集", PackAllResources, AtlasAsset);//不导出中文
//    }

//    HelpShowEditor.AssetList<UIPanelBase> UIPanelBaseAsset = new HelpShowEditor.AssetList<UIPanelBase>();
//    void ShowUIPanelbaseInfo()
//    {
//        HelpShowEditor.ShowResourcesInfo<UIPanelBase>(UIPath, GetDirectoryUIPanel, "面板", PackAllResources, UIPanelBaseAsset);//不导出中文
//    }

//    HelpShowEditor.AssetList<Texture> UITextureAsset = new HelpShowEditor.AssetList<Texture>();
//    void ShowUITexutreInfo()
//    {
//        HelpShowEditor.ShowResourcesInfo<Texture>(UIPath, GetDirectoryTexture, "贴图", PackAllResources, UITextureAsset);//不导出中文
//    }
    
//    // 得到某个目录下的所有贴图
//    void GetDirectoryTexture(string path, List<Texture> textList, List<string> paths)
//    {
//        List<string> files = mFileList.GetFiles(path, "UIData/");

//        PackResources PrefabRes = new PackResources(); // 
//        PackResources.GetResources<Texture>(PrefabRes, files, null, new string[] { "NGUIResources/Textures/", "/NGUIResources/Textures/" }, null);
//        PrefabRes.GetList<Texture>(textList);
//    }

//    // 得到某个目录下的所有UI图集
//    void GetDirectoryAtlas(string path, List<ATLAS> uiAtlas, List<string> paths)
//    {
//        List<string> files = mFileList.GetFiles(path, "Art/UIData/UData/Atlas");
//        PackResources PrefabRes = new PackResources(); // 预置体资源
//        PackResources.GetResources<GameObject>(PrefabRes, files, null, null, new string[] { ".prefab" });
//        List<GameObject> prefabs = new List<GameObject>();
//        PrefabRes.GetList<GameObject>(prefabs);
//        uiAtlas.AddRange(XTools.Utility.GetComponent<ATLAS>(prefabs));
//    }

//    // 得到某个目录下的所有面板资源
//    void GetDirectoryUIPanel(string path, List<UIPanelBase> panelbases, List<string> paths)
//    {
//        List<string> files = mFileList.GetFiles(path, "UIData/");

//        PackResources PrefabRes = new PackResources(); // 预置体资源
//        PackResources.GetResources<GameObject>(PrefabRes, files, null, new string[] { "/NGUIResources/", }, new string[] { ".prefab" });
//        List<GameObject> prefabs = new List<GameObject>();
//        PrefabRes.GetList<GameObject>(prefabs);
//        panelbases.AddRange(XTools.Utility.GetComponent<UIPanelBase>(prefabs));
//    }

//    class FileList
//    {
//        Dictionary<string, List<string>> AllFileList = new Dictionary<string, List<string>>();

//        public void Clear()
//        {
//            AllFileList.Clear();
//        }

//        public List<string> GetFiles(string path, string prefix = null)
//        {
//            List<string> files;
//            if (AllFileList.TryGetValue(path, out files))
//                return files;

//            files = XTools.Utility.GetAllFileList(path);
//            if (!string.IsNullOrEmpty(prefix))
//            {
//                for (int i = 0; i < files.Count; ++i)
//                    files[i] = prefix + files[i];
//            }

//            AllFileList[path] = files;
//            return files;
//        }
//    }

//    FileList mFileList = new FileList();

//    int RepaintCount = 0;
//    void Update()
//    {
//        RepaintCount++;
//        if (RepaintCount >= 60)
//        {
//            RepaintCount = 0;
//            Repaint();
//        }
//    }

//    float PauseTime = 0f;
//    void OnGUI()
//    {
//        if (mPackCollectList.isDone == false)
//        {
//            GUILayout.Label("收集依赖当中...");
//            return;
//        }

//        if (mAssetExportMgr.isDone == false)
//        {
//            if (mAssetExportMgr.isPause)
//            {
//                if (GUILayout.Button("继续", GUILayout.Height(50f)))
//                {
//                    if (Time.time - PauseTime >= 2.0f)
//                    {
//                        mAssetExportMgr.isPause = false;
//                        PauseTime = Time.time;
//                    }
//                }
//            }
//            else
//            {
//                if (GUILayout.Button("暂停", GUILayout.Height(50f)))
//                {
//                    if (Time.time - PauseTime >= 2.0f)
//                    {
//                        mAssetExportMgr.isPause = true;
//                        PauseTime = Time.time;
//                    }
//                }
//            }
//            return;
//        }
////        mPackCollectList.isMd5File = false;

//        if (GUILayout.Button("是否打包所有资源!", GUILayout.Height(50f)))
//        {
//            PackAllResources = true;
////            mPackCollectList.isMd5File = true;
//        }

////         if (GUILayout.Button("精简打包所有资源", GUILayout.Height(50f)))
////         {
////             mPackCollectList.isMd5Check = true;
////             PackAllResources = true;
////             mPackCollectList.isMd5File = true;
////         }

//        mGlobalPos = GUILayout.BeginScrollView(mGlobalPos);
//        mFileList.Clear();
//        XTools.TimeCheck checkTime = new XTools.TimeCheck();
//        checkTime.begin();

//        ShowPrefabInfo();
//        ShowAudioClipInfo();
//        ShowMaterialsInfo();
//        ShowSceneInfo();
//        ShowAtlasInfo();
//        ShowUIPanelbaseInfo();
//        ShowUITexutreInfo();
//        GUILayout.EndScrollView();
//        if (PackAllResources == true)
//        {
//            PackAllResources = false;
//            Debug.Log("Find Pack Res! " + checkTime.delay);
//        }

//        checkTime.begin();
//        mPackCollectList.Clear();

//        if (mSceneList.mIsPack)
//        {
//            if (mSceneList.mList.Count != 0)
//            {
//                //EditorApplication.SaveCurrentSceneIfUserWantsTo();
//            }

//            string currentPath = EditorApplication.currentScene;
//            List<Object> scenePrefabList = new List<Object>();
//            foreach (Object scene in mSceneList.mList)
//            {
//                string path = AssetDatabase.GetAssetPath(scene);
////                 if (!AssetExportMgr.IsNeedUpdate(Application.dataPath + "/" + path.Substring(7)))
////                 {
////                     if (mPackCollectList.isMd5Check)
////                     {
////                         Debug.Log("场景:" + path + "不需要打包!");
////                         continue;
////                     }
////                 }

//                if (!EditorApplication.OpenScene(path))
//                {
//                    Debug.Log("打开场景:" + path + "失败!");
//                    continue;
//                }

//                PackTool.SceneBundler.PackCurrentScene(scenePrefabList);
//            }

//            if (scenePrefabList != null && scenePrefabList.Count > 0)
//            {
//                for (int i = 0; i < scenePrefabList.Count; ++i)
//                {
//                    mPackCollectList.CollectScene(scenePrefabList[i]);
//                }
//            }

//            if (!string.IsNullOrEmpty(currentPath))
//            {
//                EditorApplication.OpenScene(currentPath);
//            }
//        }

//        if (mPrefabList.mIsPack)
//        {
//            foreach (GameObject prefab in mPrefabList.GetAssetList())
//                mPackCollectList.CollectPrefab(prefab);
//        }

//        if (UIPanelBaseAsset.mIsPack)
//        {
//            foreach (UIPanelBase panel in UIPanelBaseAsset.GetAssetList())
//                mPackCollectList.CollectPrefab(panel.gameObject);
//        }

//        if (AtlasAsset.mIsPack)
//        {
//            foreach (ATLAS atlas in AtlasAsset.GetAssetList())
//                mPackCollectList.CollectAtlas(atlas);
//        }

//        if (MaterialAsset.mIsPack)
//        {
//            foreach (Material mat in MaterialAsset.GetAssetList())
//            {
//                mPackCollectList.CollectMaterial(mat);
//            }
//        }

//        if (UITextureAsset.mIsPack)
//        {
//            foreach (Texture texture in UITextureAsset.GetAssetList())
//                mPackCollectList.CollectTexture(texture);
//        }

//        if (mAudioClipList.mIsPack)
//        {
//            foreach (AudioClip audio in mAudioClipList.GetAssetList())
//            {
//                mPackCollectList.CollectSound(audio);
//            }
//        }

//        if (!mPackCollectList.isEmpty)
//        {
//            Debug.Log("Delay:" + checkTime.delay);
//            XTools.GlobalCoroutine.StartCoroutine(mPackCollectList.BeginCollect(mAssetExportMgr));
//        }
//    }

//    // 得到某个目录下的所有预设
//    void GetDirectoryPrefabs(string path, List<GameObject> prefabs, List<string> paths)
//    {
//        List<string> files = mFileList.GetFiles(path);
//        PackResources PrefabRes = new PackResources(); // 预置体资源
//        PackResources.GetResources<GameObject>(PrefabRes, files, null, new string[] { "/ResourcesExport/", }, new string[] { ".prefab" });

//        for (int i = 0; i < PrefabRes.Values.Count; ++i)
//            prefabs.Add(PrefabRes.Values[i] as GameObject);
//    }

//    // 得到某个目录下的场景
//    void GetDirectoryScenes(string path, List<Object> sceneList, List<string> paths)
//    {
//        int startIndex = Application.dataPath.Length - "Assets".Length;
//        string[] files = Directory.GetFiles(path, "*.unity", SearchOption.TopDirectoryOnly);
//        foreach (string file in files)
//        {
//            string s = file.Substring(startIndex);
//            Object o = (Object)AssetDatabase.LoadAssetAtPath(s, typeof(Object));
//            if (o != null)
//            {
//                sceneList.Add(o);
//                paths.Add(s.Substring(7));
//            }
//        }
//    }

//    // 得到某个目录下的音效
//    void GetDirectoryAudioClip(string path, List<AudioClip> audioClips, List<string> paths)
//    {
//        List<string> files = mFileList.GetFiles(path);

//        PackResources audioRes = new PackResources();
//        PackResources.GetResources<AudioClip>(audioRes, files, null, new string[] { "/ResourcesExport/" }, new string[] { ".mp3", ".ogg", ".wav" });
//        audioRes.GetList<AudioClip>(audioClips);
//    }

//    // 得到某个目录下的材质
//    void GetDirectoryMaterial(string path, List<Material> mats, List<string> paths)
//    {
//        List<string> files = mFileList.GetFiles(path);

//        PackResources matRes = new PackResources();
//        PackResources.GetResources<Material>(matRes, files, null, new string[] { "/MaterialExport/" }, new string[] { ".mat" });
//        matRes.GetList<Material>(mats);
//    }
//}
//#endif
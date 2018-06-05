//#define TESTRES
#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;
using UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

#if USE_UATLAS
using ATLAS = UI.uAtlas;
#else
using ATLAS = xys.UI.Atlas;
#endif

namespace PackTool
{ 
    // 打包
    public class ExportAllResources : ResourcesFind
    {
        public ExportAllResources(bool issimplify, bool showlog)
        {
            ShowLog = showlog;
            isSimplify = issimplify;
        }

        AssetsExport mAssetExportMgr = new AssetsExport(true);
        PackCollectList mPackCollectList = new PackCollectList();

        // 是否精简打包
        public bool isSimplify { get; protected set; }

        public bool ShowLog
        {
            get { return mAssetExportMgr.ShowLog; }
            set { mAssetExportMgr.ShowLog = mPackCollectList.ShowLog = value; }
        }

        // 查找所有资源
        public override void FindAllResources()
        {
            TimeCheck tc = new TimeCheck(true);
            base.FindAllResources();
            Debug.Log("查找所有资源用时:" + tc.renew);
#if TESTRES
            while (mPrefabRes.Keys.Count > 2)
                mPrefabRes.Keys.RemoveAt(mPrefabRes.Keys.Count - 1);

            while (mPrefabRes.Values.Count > 2)
                mPrefabRes.Values.RemoveAt(mPrefabRes.Values.Count - 1);

            while (mSceneList.Count > 1)
                mSceneList.RemoveAt(mSceneList.Count - 1);

            mSceneList.Clear();
            mSceneList.Add(new AssetObject<Object>("Assets/Art/Scenes/Fbxs/levels/Level_ZhuqueBB.unity"));

            mPrefabRes.Clear();
            mAudioRes.Clear();
            mMatRes.Clear();
            mTextRes.Clear();
#endif
        }

        static bool IsCanDelete(string filename)
        {
            string src_filename = filename;
            if (!filename.Contains("/"))
                return false;

            if (filename.StartsWith(xys.UI.CheckAllPanel.atlas_root.Substring(16)))
                return false;

            if (filename.StartsWith("etc/a/") || filename.StartsWith("etc/d/"))
                filename = filename.Substring(6);
            else if (filename.EndsWith(".pd"))
                filename = filename.Substring(0, filename.Length - 3);
            else if (filename.EndsWith(".x"))
                filename = filename.Substring(0, filename.Length - 2);
            else if (filename.EndsWith(".sbd"))
                filename = filename.Substring(0, filename.Length - 4);
            else if (filename.EndsWith(".sbp"))
                filename = filename.Substring(0, filename.Length - 4);
            else if (filename.EndsWith("_hd.png", true, null))
                filename = filename.Substring(0, filename.Length - 7) + ".png";
            else if (filename.EndsWith("_hd.mat", true, null))
                filename = filename.Substring(0, filename.Length - 7) + ".mat";

            string files;
            // 判断是否是需要存在的资源
            if (filename.StartsWith("Data/"))
            {
                files = Application.dataPath + "/../" + filename;
                if (System.IO.File.Exists(files))
                    return false;
            }
            else
            {
                files = Application.dataPath + "/" + filename;
                if (System.IO.File.Exists(files))
                    return false;
            }

            // 动画,网格资源
            if (filename.EndsWith(".anim") || filename.EndsWith(".asset"))
            {
                string cf = filename.Substring(0, filename.LastIndexOf('/'));
                files = Application.dataPath + "/" + cf + ".fbx";
                if (System.IO.File.Exists(files))
                    return false;

                files = Application.dataPath + "/" + cf + ".obj";
                if (System.IO.File.Exists(files))
                    return false;

                filename = cf;
                int lastpos = filename.LastIndexOf('/'); lastpos = filename.LastIndexOf('/', lastpos);
                cf = filename.Substring(0, lastpos) + "@" + filename.Substring(lastpos + 1);
                files = Application.dataPath + "/" + cf + ".fbx";
                if (System.IO.File.Exists(files))
                    return false;

                Debug.Log(string.Format("{0}->{1}", src_filename, cf + ".fbx"));

                return true;
            }
            else
            {
                Debug.Log(src_filename);
                return true;
            }
        }

        [MenuItem("Assets/PackTool/DeleteNoUseFile")]
        static void DeleteNoUseFile()
        {
            string[] allresfile = System.IO.Directory.GetFiles(AssetsExport.PackPath, "*", System.IO.SearchOption.AllDirectories);
            string copy_root = (Application.dataPath + "/../").Replace('\\', '/');
            int lenght = AssetsExport.PackPath.Length;
            for (int i = 0; i < allresfile.Length; ++i)
            {
                string filename = allresfile[i].Substring(lenght).Replace('\\', '/');
                if (IsCanDelete(filename))
                {
                    System.IO.File.Delete(allresfile[i]);
                    Debug.Log("Delete:" + allresfile[i]);

                    string copy_file = copy_root + filename;
                    if (System.IO.File.Exists(copy_file))
                        System.IO.File.Delete(copy_file);

                    if (System.IO.File.Exists(copy_file + ".meta"))
                        System.IO.File.Delete(copy_file + ".meta");
                }
            }
        }


        int collect_total = 0; // 总的收集数量

        bool NeedCollect(string assetsPath, bool isprefab)
        {
            bool b = CodeCheckAtler.isAlter(assetsPath);
            if (!b && isprefab)
            {
                string ap = assetsPath.Substring(7);
                string abf = AssetsExport.PackPath + ap;
                if (!File.Exists(abf))
                {
                    // 要打包的
                    b = true;
                }
                else
                {
                    string cf = string.Format("{0}/__copy__/{1}", Application.dataPath, ap);
                    if (!File.Exists(cf))
                        b = true;
                }
            }

            if (b)
            {
                ++collect_total;
                if (isSimplify)
                    Debug.Log(string.Format("精简打包，资源:{0}有变化!", assetsPath));
                b = true;
            }

            return isSimplify ? b : true;
        }

        bool NeedCollect(Object obj, bool isprefab)
        {
            return NeedCollect(AssetDatabase.GetAssetPath(obj), isprefab);
        }

        // 收集依赖
        void CollectResources()
        {
            // 收集场景
            CollectScene();

            // 收集预置体
            foreach (GameObject prefab in mPrefabRes.GetList<GameObject>())
            {
                if (NeedCollect(prefab, true))
                    mPackCollectList.CollectPrefab(prefab);
            }
            mPrefabRes.Clear();

            // 收集图集
            foreach (ComponentObject<ATLAS> atlas in mAtlas)
            {
                if (NeedCollect(atlas.GetValue(), false))
                    mPackCollectList.CollectAtlas(atlas.GetValue());
            }
            mAtlas.Clear();

            // 材质
            foreach (Material mat in mMatRes.GetList<Material>())
            {
                if (NeedCollect(mat, false))
                    mPackCollectList.CollectMaterial(mat);
            }
            mMatRes.Clear();

            // 贴图
            foreach (Texture texture in mTextRes.GetList<Texture>())
            {
                if (NeedCollect(texture, false))
                    mPackCollectList.CollectTexture(texture);
            }
            mTextRes.Clear();

            // 音频
            foreach (AudioClip audio in mAudioRes.GetList<AudioClip>())
            {
                if (NeedCollect(audio, false))
                    mPackCollectList.CollectSound(audio);
            }

            if (isSimplify)
            {
                Debug.Log("变化的资源数量:" + collect_total);
            }

            mAudioRes.Clear();
        }

        // 收集场景
        void CollectScene()
        {
            List<Object> scenePrefabList = new List<Object>();
            foreach (AssetObject<Object> scene in mSceneList)
            {
                if (!NeedCollect(scene.assetPath, false))
                    continue;

                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                Scene s = EditorSceneManager.OpenScene(scene.assetPath, OpenSceneMode.Single);
                if (!s.IsValid())
                {
                    Debug.LogError("打开场景:" + scene.assetPath + "失败!");
                    continue;
                }

                SceneBundler.PackCurrentScene(scenePrefabList);
            }

            if (scenePrefabList != null && scenePrefabList.Count > 0)
            {
                for (int i = 0; i < scenePrefabList.Count; ++i)
                {
                    mPackCollectList.CollectScene(scenePrefabList[i]);
                }
            }

            scenePrefabList.Clear();
        }

        public bool isDone { get; protected set; }

        public bool isError { get { return mAssetExportMgr.isError; } }

        // 导出资源
        public void Export()
        {
            CodeCheckAtler.Release();

            if (!isSimplify)
            {
                if (Directory.Exists(AssetsExport.PackPath))
                {
                    Debug.Log("非精简打包!删除目录:" + AssetsExport.PackPath);
                    Directory.Delete(AssetsExport.PackPath, true);
                }
            }
            else
            {
                if (!Directory.Exists(AssetsExport.PackPath))
                {
                    Debug.Log("没有基础数据，不可精简打包!");
                    isSimplify = false;
                }
            }

            isDone = false;

            FindAllResources();

            // 收集资源
            mAssetExportMgr.Release();
            mPackCollectList.Clear();

            // 收集所有依赖
            CollectResources();

            if (!mPackCollectList.isEmpty)
            {
                GlobalCoroutine.StartCoroutine(mPackCollectList.BeginCollect(mAssetExportMgr));
                GlobalCoroutine.StartCoroutine(CheckEnd());
            }
            else
            {
                isDone = true;
            }
        }

        IEnumerator CheckEnd()
        {
            while (!mPackCollectList.isDone)
                yield return 0;

            while (!mAssetExportMgr.isDone)
                yield return 0;

            //if (isSimplify)
            //    DeleteNoUseFile();

            // 复制配置文件到目录
            AssetsExport.CopyConfigFile();

            isDone = true;
        }
    }
}
#endif
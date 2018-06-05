using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#if USE_UATLAS
using ATLAS = UI.uAtlas;
#else
using ATLAS = xys.UI.Atlas;
#endif

namespace PackTool
{ 
    public class ResourcesFind
    {
        protected FileList mFileList = new FileList();

        protected PackResources mPrefabRes = new PackResources();
        protected PackResources mAudioRes = new PackResources();
        protected PackResources mMatRes = new PackResources();
        protected List<AssetObject<Object>> mSceneList = new List<AssetObject<Object>>();

        protected List<ComponentObject<ATLAS>> mAtlas = new List<ComponentObject<ATLAS>>(); // UGUI图集
        protected PackResources mTextRes = new PackResources(); // 贴图
        public List<Object> GetAllList()
        {
            List<Object> allList = new List<Object>();
            allList.Capacity = 4096;
            EachAllObject((Object o) => { allList.Add(o); });

            return allList;
        }

        public void EachAllObject(System.Action<Object> fun)
        {
            foreach (Object obj in mPrefabRes.Values)
                fun(obj);

            foreach (Object obj in mAudioRes.Values)
                fun(obj);

            foreach (Object obj in mMatRes.Values)
                fun(obj);

            foreach (Object obj in mTextRes.Values)
                fun(obj);

            foreach (ComponentObject<ATLAS> obj in mAtlas)
            {
                fun(obj.Get());
            }

            foreach (AssetObject<Object> obj in mSceneList)
            {
                fun(obj.Get());
            }
        }

        // 查找所有资源
        public virtual void FindAllResources()
        {
            mPrefabRes.Clear();
            mAudioRes.Clear();
            mMatRes.Clear();
            PackResources.GetResources<GameObject>(mPrefabRes, mFileList.GetFiles(Application.dataPath), null, new string[] { "/ResourcesExport/", }, new string[] { ".prefab" });
            PackResources.GetResources<AudioClip>(mAudioRes, mFileList.GetFiles(Application.dataPath), null, new string[] { "/AudioClipExport/" }, new string[] { ".mp3", ".ogg", ".wav" });
            PackResources.GetResources<Material>(mMatRes, mFileList.GetFiles(Application.dataPath), null, new string[] { "/MaterialExport/" }, new string[] { ".mat" });

            // 场景
            mSceneList.Clear();

            //string scenePath = EditorSceneManager.GetActiveScene().path;
            foreach (string path in BuildSceneList.GetAllSceneList())
            {
                mSceneList.Add(new AssetObject<Object>(path));
            }

            mAtlas.Clear();
            {
                List<string> files = mFileList.GetFiles(Application.dataPath + "/Art/UIData/UData/Atlas", "Art/UIData/UData/Atlas/");
                PackResources uiprefabs = new PackResources(); // 预置体资源
                PackResources.GetResources<GameObject>(uiprefabs, files, null, null, new string[] { ".prefab" });
                List<GameObject> prefabs = new List<GameObject>();
                uiprefabs.GetList(prefabs);

                mAtlas.AddRange(ComponentObject<ATLAS>.SwitchList(Utility.GetComponent<ATLAS>(prefabs)));
            }

            // 贴图
            mTextRes.Clear();
        }
    }
}
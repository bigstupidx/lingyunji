#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;
#if USE_UATLAS
using ATLAS = UI.uAtlas;
#else
using ATLAS = xys.UI.Atlas;
#endif

namespace PackTool
{
    public class PackCollectList
    {
        public PackCollectList()
        {
            isDone = true;
        }

        List<AssetObject<GameObject>> PrefabList = new List<AssetObject<GameObject>>();
        List<AssetObject<Object>> SceneList = new List<AssetObject<Object>>();
//         List<ComponentObject<UIFont>> UIFontList = new List<ComponentObject<UIFont>>();
//         List<ComponentObject<UIAtlas>> UIAtlasList = new List<ComponentObject<UIAtlas>>();
        List<AssetObject<Texture>> TextureList = new List<AssetObject<Texture>>();
        List<AssetObject<AudioClip>> AudioClipList = new List<AssetObject<AudioClip>>();
        List<AssetObject<Material>> MaterialList = new List<AssetObject<Material>>();
        List<ComponentObject<ATLAS>> AtlasList = new List<ComponentObject<ATLAS>>();

        // 是否显示日志
        public bool ShowLog = true;

        public bool isEmpty
        {
            get
            {
                if (PrefabList.Count == 0 &&
                    SceneList.Count == 0 &&
                    AtlasList.Count == 0 &&
                    //UIFontList.Count == 0 &&
                    //UIAtlasList.Count == 0 &&
                    TextureList.Count == 0 &&
                    AudioClipList.Count == 0 &&
                    MaterialList.Count == 0 )
                {
                    return true;
                }

                return false;
            }
        }

        public bool isDone { get; protected set; }

        public void Clear()
        {
            PrefabList.Clear();
            SceneList.Clear();
            AtlasList.Clear();
            TextureList.Clear();
            AudioClipList.Clear();
            MaterialList.Clear();
        }

        string AssetPath;

        public void CollectPrefab(GameObject asset)
        {
            PrefabList.Add(new AssetObject<GameObject>(asset));
        }

        public void CollectScene(Object asset)
        {
            SceneList.Add(new AssetObject<Object>(asset));
        }

        public void CollectAtlas(ATLAS asset)
        {
            AtlasList.Add(new ComponentObject<ATLAS>(asset));
        }

        public void CollectTexture(Texture asset)
        {
            TextureList.Add(new AssetObject<Texture>(asset));
        }

        public void CollectSound(AudioClip asset)
        {
            AudioClipList.Add(new AssetObject<AudioClip>(asset));
        }

        public void CollectMaterial(Material asset)
        {
            MaterialList.Add(new AssetObject<Material>(asset));
        }

        delegate void CollectObj<T>(T obj);

        bool isCollectEnd = false;
        IEnumerator CollectAsset<T, M>(List<M> objs, CollectObj<T> fun, string type) where T : Object where M : AssetObjectinterface
        {
            isCollectEnd = false;
            int num = 0;
            foreach (M asset in objs)
            {
                T obj = asset.Get() as T;
                if (ShowLog)
                {
                    Debuger.Log("收集" + type + ":" + obj.name + " " + num + ":" + objs.Count);
                    //yield return 0;
                }

                fun(obj);
                num++;
            }

            Debuger.Log("收集" + type + "结束!");
            yield return 0;
            isCollectEnd = true;
        }

        public IEnumerator BeginCollect(AssetsExport mgr)
        {
            isDone = false;
            yield return 0;

            Debuger.Log("开始收集资源依赖!");
            TimeCheck timecheck = new TimeCheck();
            timecheck.begin();

            GlobalCoroutine.StartCoroutine(CollectAsset<GameObject, AssetObject<GameObject>>(PrefabList, mgr.CollectPrefab, "预置"));//不导出中文
            while (!isCollectEnd)
                yield return 0;

            GlobalCoroutine.StartCoroutine(CollectAsset<Object, AssetObject<Object>>(SceneList, mgr.CollectScene, "场景"));//不导出中文
            while (!isCollectEnd)
                yield return 0;

            //GlobalCoroutine.StartCoroutine(CollectAsset<ATLAS, ComponentObject<ATLAS>>(AtlasList, mgr.CollectAtlas, "图集"));//不导出中文
            //while (!isCollectEnd)
            //    yield return 0;

            GlobalCoroutine.StartCoroutine(CollectAsset<Texture, AssetObject<Texture>>(TextureList, mgr.CollectTexture, "纹理"));//不导出中文
            while (!isCollectEnd)
                yield return 0;

            GlobalCoroutine.StartCoroutine(CollectAsset<AudioClip, AssetObject<AudioClip>>(AudioClipList, mgr.CollectSound, "音效"));//不导出中文
            while (!isCollectEnd)
                yield return 0;

            GlobalCoroutine.StartCoroutine(CollectAsset<Material, AssetObject<Material>>(MaterialList, mgr.CollectMaterial, "材质"));//不导出中文
            while (!isCollectEnd)
                yield return 0;

            float collectTime = timecheck.delay;
            Clear();
            isDone = true;
            
            GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            while (!mgr.isDone)
                yield return 0;

            Debug.Log("收集完毕!耗时:" + collectTime);
            Debug.Log("总耗时:" + timecheck.delay);
        }
    }
}

#endif
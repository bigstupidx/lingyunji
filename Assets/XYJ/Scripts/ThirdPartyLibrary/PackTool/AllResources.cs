#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    [ExecuteInEditMode]
    public class AllResources : MonoBehaviour
    {
#if USER_ALLRESOURCES
        static AllResources mInstance;
        public static AllResources Instance
        {
            get
            {
                return mInstance;
            }
        }

        public void Init()
        {
            mInstance = this;
            ResourcesGroup.Clear();
            Prefabs.Init();
            Textures.Init();
            AudioClips.Init();
            Materials.Init();

            SceneVersion.Init();
        }

#if UNITY_EDITOR
        //收集所有资源
        public void UpdateAllResource()
        {
            List<string> files = XTools.Utility.GetAllFileList(Application.dataPath);
            PackResources.GetResources<GameObject>(Prefabs, files, null, new string[] { "/ResourcesExport/", }, new string[] { ".prefab" });
            PackResources.GetResources<AudioClip>(AudioClips, files, null, new string[] { "/AudioClipExport/" }, new string[] { ".mp3", ".ogg", ".wav" });
            PackResources.GetResources<Material>(Materials, files, null, new string[] { "/MaterialExport/" }, new string[] { ".mat" });

            UnityEditor.EditorUtility.SetDirty(gameObject);
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();

            SpritesLoad.CreateIconAtlas();
        }
#endif
        // 保存所有的预设体
        public PackResources Prefabs = new PackResources();
        public GameObject GetPrefab(string key)
        {
            return Prefabs.Get<GameObject>(key);
        }

        // 保存所有的图片纹理
        [SerializeField]
        public PackResources Textures = new PackResources();
        public Texture GetTexture(string key)
        {
            return Textures.Get<Texture>(key);
        }

        // 保存所有的音效资源
        [SerializeField]
        public PackResources AudioClips = new PackResources();
        public AudioClip GetAudioClip(string key)
        {
            return AudioClips.Get<AudioClip>(key);
        }

        // 保存材质资源
        [SerializeField]
        public PackResources Materials = new PackResources();
        public Material GetMaterial(string key)
        {
            return Materials.Get<Material>(key);
        }

        public Object GetObject(string key)
        {
            Object obj = null;
            if ((obj = GetTexture(key)) != null)
                return obj;

            if ((obj = GetPrefab(key)) != null)
                return obj;

            if ((obj = GetAudioClip(key)) != null)
                return obj;

            if ((obj = GetMaterial(key)) != null)
                return obj;

            return null;
        }
#endif
    }
}

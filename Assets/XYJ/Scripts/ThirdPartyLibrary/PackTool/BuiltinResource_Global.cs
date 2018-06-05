#if USE_RESOURCESEXPORT
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PackTool
{
    // 保存有用到的内置资源，贴图，shader之类的
    public partial class BuiltinResource
    {
        public const string filepath = "BuiltinResource.prefab";

        static BuiltinResource()
        {
            Instance = null;
            isDone = false;
        }

        public static bool isDone { get; private set; }
        public static BuiltinResource Instance { get; private set; }

        public static void Init()
        {
            if (isDone)
                return;

            AssetBundleLoad.Load(filepath, OnLoadEnd);
        }

        static void CheckLoad(AssetBundle ab)
        {
            GameObject go = null;
            if (ab != null)
            {
                go = ab.LoadAsset<GameObject>("BuiltinResource") as GameObject;
                ab.Unload(false);
            }

            if (go == null || ((Instance = go.GetComponent<BuiltinResource>()) == null))
            {
                go = new GameObject("Auto-BuiltinResource");
                Instance = go.AddComponent<BuiltinResource>();
                Object.DontDestroyOnLoad(go);
            }
            else
            {
                Instance.InitMap();
            }

            isDone = true;
            PlayerPrefs.SetInt("BuiltinResource", 1);
            //yield break;
        }

        static void OnLoadEnd(AssetBundle ab)
        {
            CheckLoad(ab);
            //MagicThread.Instance.StartCoroutine();
        }

//#if UNITY_EDITOR
//        [UnityEditor.MenuItem("Assets/CheckBuiltin")]
//        static void CheckBuiltin()
//        {
//            BuiltinResource br = UnityEditor.AssetDatabase.LoadAssetAtPath<BuiltinResource>("Assets/__copy__/BuiltinResource.prefab");
//            for (int i = 0; i < br.SelfMaterialsData.Count;++i)
//            {
//                var mat = br.SelfMaterialsData[i];
//                UnityEditor.MaterialProperty[] mps = UnityEditor.MaterialEditor.GetMaterialProperties(new Object[] { mat.mat });
//                foreach (var mp in mps)
//                {
//                    if (mp.type == UnityEditor.MaterialProperty.PropType.Texture)
//                    {
//                        if (mat.mat.GetTexture(mp.name) != null)
//                        {
//                            Debug.LogFormat("mat:{0}", mat.mat.name);
//                        }
//                    }
//                }
//            }
//        }
//#endif
    }
}
#endif
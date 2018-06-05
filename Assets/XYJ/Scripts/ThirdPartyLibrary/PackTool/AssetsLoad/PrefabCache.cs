#if USE_RESOURCESEXPORT
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class PrefabCache : MonoBehaviour
    {
        static PrefabCache instance = null;

        public static PrefabCache Get()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("PrefabCache");
                instance = go.AddComponent<PrefabCache>();
                go.SetActive(false);
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }
}
#endif
#endif
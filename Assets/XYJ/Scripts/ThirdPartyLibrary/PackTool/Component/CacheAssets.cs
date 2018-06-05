#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CacheAssets : MonoBehaviour
{
    // 缓冲的对象列表
    public static Dictionary<int, Object> CacheList = new Dictionary<int, Object>();

    [PackTool.Pack]
    public Texture[] textures; // 缓冲的纹理

    void Awake()
    {
        if (textures == null || textures.Length == 0)
            return;

        int id = 0;
        foreach (Texture t in textures)
        {
            if (t == null || CacheList.ContainsKey((id = t.GetInstanceID())))
                continue;

            CacheList.Add(id, t);
            Debuger.DebugLog("缓冲纹理:" + t.name);
        }

        textures = null;
    }
}

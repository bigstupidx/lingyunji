#if MEMORY_CHECK
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XTools;
using PackTool;

public class RunningAssetsDiffer
{
    public AssetSyncInfo before; // 前数据
    public AssetSyncInfo current; // 数据数据

    public AssetSyncInfo addInfo { get; protected set; } // 新增的资源
    public AssetSyncInfo subInfo { get; protected set; } // 减少的资源
    public AssetSyncInfo sameInfo { get; protected set; } // 没变化的资源

    static bool StringCompare(string x, string y)
    {
        return x.CompareTo(y) == 0;
    }

    public void Differ(AssetSyncInfo b, AssetSyncInfo c)
    {
        before = b;
        current = c;

        addInfo = new AssetSyncInfo();
        subInfo = new AssetSyncInfo();
        sameInfo = new AssetSyncInfo();

        AddSubOrSame(before.textures, current.textures, addInfo.textures, subInfo.textures, sameInfo.textures, StringCompare);
        AddSubOrSame(before.materials, current.materials, addInfo.materials, subInfo.materials, sameInfo.materials, StringCompare);
        AddSubOrSame(before.atlas, current.atlas, addInfo.atlas, subInfo.atlas, sameInfo.atlas, StringCompare);
        AddSubOrSame(before.meshs, current.meshs, addInfo.meshs, subInfo.meshs, sameInfo.meshs, StringCompare);
        AddSubOrSame(before.controls, current.controls, addInfo.controls, subInfo.controls, sameInfo.controls, StringCompare);
        AddSubOrSame(before.avatars, current.avatars, addInfo.avatars, subInfo.avatars, sameInfo.avatars, StringCompare);
        AddSubOrSame(before.audioclips, current.audioclips, addInfo.audioclips, subInfo.audioclips, sameInfo.audioclips, StringCompare);
        AddSubOrSame(before.mPrefabs, current.mPrefabs, addInfo.mPrefabs, subInfo.mPrefabs, sameInfo.mPrefabs, (PrefabBeh.Data x, PrefabBeh.Data y)=> { return x.url == y.url; });
    }

    // t1和t2数据的差异
    // add t1没有，t2有的数据
    // sub t1有，t2没有的数据
    // save t1，t2都有的数据
    static void AddSubOrSame<T>(List<T> t1, List<T> t2, List<T> add, List<T> sub, List<T> same, Func<T, T, bool> fun)
    {
        for (int i = 0; i < t1.Count; ++i)
        {
            if (t2.FindIndex((T obj)=> { return fun(obj, t1[i]); }) != -1)
            {
                same.Add(t1[i]);
            }
            else
            {
                sub.Add(t1[i]);
            }
        }

        for (int i = 0; i < t2.Count; ++i)
        {
            if (t1.FindIndex((T obj) => { return fun(obj, t2[i]); }) != -1)
            {

            }
            else
            {
                add.Add(t2[i]);
            }
        }
    }
}
#endif
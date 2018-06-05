#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class PrefabCacheLoad
    {
        // 缓存的预置体列表
        static HashSet<string> CacheList = new HashSet<string>();

#if UNITY_EDITOR
        public static void GetAll(List<PrefabLoad> pls)
        {
            pls.Capacity = pls.Capacity + CacheList.Count;
            foreach (string key in CacheList)
                pls.Add(PrefabLoad.Get(key) as PrefabLoad);
        }
#endif

        // 资源加载
//         static public void LoadGroup(string group, string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true)
//         {
//             LoadFullPath(PrefabLoad.GetFullName(group, name), fun, funp, isinit);
//         }

        // 默认的资源组
        static public void Load(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true)
        {
            LoadFullPath(PrefabLoad.GetFullName(ResourcesGroup.Prefab, name), fun, funp, isinit);
        }
        
        // 需要完整的路径
        static public void LoadFullPath(string name, ResourcesEnd<GameObject> fun, object funp, bool isinit = true)
        {
            PrefabLoad.PrefabData pd = PrefabLoad.LoadAsyncFullPath(name, fun, funp, isinit);
            if (CacheList.Contains(name))
            {
                pd.SubRef();
            }
            else
            {
                // 未缓存的，计数+1
                pd.load.AddRef();
                pd.SubRef();
            }
        }

        // 清除所有的缓存
        static public void ClearAllCache()
        {
            PrefabLoad pl = null;
            foreach (string key in CacheList)
            {
                pl = PrefabLoad.Get(key) as PrefabLoad;
                if (pl != null)
                    pl.SufRef();
                else
                {
                    Debuger.ErrorLog("Cache not Find!url:{0}", key);
                }
            }

            CacheList.Clear();
        }

        static public void AddCache(GameObject go)
        {
#if USE_RESOURCESEXPORT
            if (go == null)
                return;

            PrefabBeh pb = go.GetComponent<PrefabBeh>();
            if (pb == null)
            {
                Debuger.ErrorLog("添加对象到缓存表，查找源预置体失败!");
                return;
            }

            if (CacheList.Contains(pb.url))
                return; // 已经添加到缓存列表当中了

            PrefabLoad pl = PrefabLoad.Get(pb.url) as PrefabLoad;
            if (pl == null)
            {
                Debuger.ErrorLog("添加对象到缓存表，查找源预置体失败!");
                return;
            }

            pl.AddRef();
            CacheList.Add(pb.url);

#if UNITY_EDITOR
            PrefabCache.Get();
#endif
#endif
        }
    }
}
#endif
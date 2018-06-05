using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class AssetsUnLoad
    {
        static AssetsUnLoad()
        {
            isDone = true;
        }

        // 是否卸载了所有资源
        static public bool isDone { get; private set; }

        // 把当前所有加载的资源记录到日志
        static public void AllResInLog()
        {
   //         System.Text.StringBuilder builder = new System.Text.StringBuilder();
			//if (SceneLoad.Current != null)
			//	builder.AppendLine (SceneLoad.Current.url);

//#if COM_DEBUG
//            XTools.AllBuff.GetBuffInfo(builder);
//#endif

#if MEMORY_CHECK
            MemoryObjectMgr.GetInfo(builder);
#endif
            //Logger.LogDebug("AllResInLog:\n" + builder.ToString());
        }

        // 卸载资源，是否根据当前内存剩余量决定是否要
        public static void UnloadUnusedAssets()
        {
            // 还在释放当中的
            if (isDone == false)
                return;

            isDone = false;
            MagicThread.Instance.StartCoroutine(Unload(false));
        }

        // 卸载资源，是否根据当前内存剩余量决定是否要
        public static void UnloadUnusedAssets(bool checkmesh)
        {
            // 还在释放当中的
            if (isDone == false)
                return;

            if (isMemoryEnough())
            {
                Debuger.DebugLog("内存足够，不需要卸载!");
                System.GC.Collect();

#if MEMORY_CHECK
                AllResInLog();
#endif
                return; // 内存足够，不处理
            }

            isDone = false;
            MagicThread.Instance.StartCoroutine(Unload(checkmesh));
        }

        const uint memory_level = 1024 * 1024 * 150;

        public static bool isMemoryEnough()
        {
            return false;
        }

#if ASSET_DEBUG
        static TimeTrack AssetsUnLoad_TimeTrack = TimeTrackMgr.Instance.Get("AssetsUnLoad");
        static TimeTrack AssetsUnLoad_Clear_TimeTrack = TimeTrackMgr.Instance.Get("AssetsUnLoad.Clear");
        static TimeTrack AssetsUnLoad_Recover_TimeTrack = TimeTrackMgr.Instance.Get("AssetsUnLoad.Recover");
        static TimeTrack AssetsUnLoad_UnloadUnusedAssets_TimeTrack = TimeTrackMgr.Instance.Get("AssetsUnLoad.UnloadUnusedAssets");
        static TimeTrack AssetsUnLoad_GC_Collect_TimeTrack = TimeTrackMgr.Instance.Get("AssetsUnLoad.GC.Collect()");
#endif

#if USE_RESOURCESEXPORT
        static List<AssetLoadObject> deleteList = new List<AssetLoadObject>(2048);

        const int Frame_Total_Object = 50; // 一桢最多删除50个资源

        // 正在加载当中的资源
        static HashSet<string> loadingList = new HashSet<string>();

        static HashSet<string> GetLoadingList()
        {
            loadingList.Clear();
            List<string> depens = null;
            foreach (KeyValuePair<string, AssetLoadObject> itor in AllAsset.AlreadLoadList)
            {
                AssetLoadObject alo = itor.Value;
                if (!alo.isDone)
                {
                    loadingList.Add(itor.Key);
                    depens = alo.GetDependences();
                    if (depens != null && depens.Count > 0)
                        loadingList.UnionWith(depens);
                }
            }

            return loadingList;
        }


        // 清除因静态合批导致没有引用的网格
        public static void ClearMeshByStaticBinding()
        {
            if (isDone == false)
                return;

            isDone = false;
            MagicThread.Instance.StartCoroutine(ClearMesh());
        }

        static IEnumerator ClearMesh()
        {
            MeshLoad.Clear();
            yield return 0;
            yield return 0;
            yield return Resources.UnloadUnusedAssets();
            yield return 0;
            yield return 0;
            MeshLoad.Recover();
            isDone = true;
        }
#endif
        static IEnumerator Unload(bool checkmesh)
        {
#if USE_RESOURCESEXPORT
            while (PrefabBeh.wait_total != 0)
                yield return 0;

            HashSet<string> loading = GetLoadingList();
            List<AssetLoadObject> deletes = deleteList;
            while (true)
            {
                bool checkend = false; // 是否检测完毕
                foreach (KeyValuePair<string, AssetLoadObject> itor in AllAsset.AlreadLoadList)
                {
                    if (loading.Contains(itor.Key))
                        continue;

                    if (itor.Value.isDone && itor.Value.Refcount <= 0 && itor.Value.request_count == 0)
                    {
                        deletes.Add(itor.Value);

                        if (itor.Value.SubDepRef())
                        {
                            checkend = true;
                        }
                    }
                }

                if (checkend)
                {
                    deletes.Clear();
                }
                else
                {
                    break;
                }
            }

            loading.Clear();
            for (int i = 0; i < deletes.Count; ++i)
            {
                deletes[i].DestroySelf();
            }

            Debuger.DebugLog("deletelist:{0}", deletes.Count);
            deletes.Clear();

            // 再删除Mesh资源
            if (checkmesh)
            {
                MeshLoad.Clear();
            }
#endif

#if USE_ABL
            yield return ABL.AssetsMgr.UnloadUnusedAssets();
#else
            yield return 0;
            yield return Resources.UnloadUnusedAssets();
            yield return 0;
#endif

#if USE_RESOURCESEXPORT
            if (checkmesh)
            {
                MeshLoad.Recover();
            }

            System.GC.Collect();
#endif
            isDone = true;

#if MEMORY_CHECK
            AllResInLog();
#endif
        }
    }
}
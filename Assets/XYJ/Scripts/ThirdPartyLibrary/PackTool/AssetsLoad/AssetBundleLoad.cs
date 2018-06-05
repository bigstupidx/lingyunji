#if USE_RESOURCESEXPORT
#define LoadFromFileAsync
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public static class AssetBundleLoad
    {
        public delegate void LoadEnd(AssetBundle assetBundle);

        class Wait
        {
            public AssetBundleCreateRequest abcr;
            public LoadEnd fun;

#if ASSET_DEBUG
            public string url;
#endif
            public Wait(AssetBundleCreateRequest abcr, LoadEnd fun)
            {
                this.abcr = abcr;
                this.fun = fun;
            }
        }

        // 待加载当中
        static LinkedList<Wait> WaitLists = new LinkedList<Wait>();

        static TimerFrame.Frame LateFrame = null;

        static void AddWait(Wait data)
        {
            if (LateFrame == null)
            {
                LateFrame = TimerMgrObj.Instance.AddLateUpdate(LateUpdate, null);
            }

            WaitLists.AddLast(data);
        }

        static bool LateUpdate(object p)
        {
            LinkedListNode<Wait> ator = WaitLists.First;
            while (ator != null)
            {
                Wait w = ator.Value;
                if (w.abcr.isDone)
                {
                    try
                    {
#if ASSET_DEBUG
                        AssetBundle_LoadFromFileAsync_End.Execution(w.url, () =>
                        {
#endif
                            w.fun(w.abcr.assetBundle);
#if ASSET_DEBUG
                        });
#endif
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }

                    var v = ator.Next;
                    WaitLists.Remove(ator);
                    ator = v;
                }
                else
                {
                    ator = ator.Next;
                }
            }

            if (WaitLists.Count == 0)
            {
                LateFrame = null;
                return false;
            }

            return true;
        }

#if ASSET_DEBUG
        static TimeTrack AssetBundle_LoadFromFileAsync_Begin = TimeTrackMgr.Instance.Get("AssetBundle.LoadFromFileAsync.Begin");
        static TimeTrack AssetBundle_LoadFromFileAsync_End = TimeTrackMgr.Instance.Get("AssetBundle.LoadFromFileAsync.End");
#endif
        // 注意，传递过来的路径为相对路径，不能是绝对路径
        public static void Load(string url, LoadEnd fun)
        {
            int lenght = 0;
            int offset = 0;
            string dst = ResourcesPack.Find(url, out offset, out lenght);
            if (string.IsNullOrEmpty(dst))
            {
                Debug.LogErrorFormat("AssetBundle:{0} file not find!", url);
                TimerMgrObj.Instance.AddLateUpdate((object p) =>
                {
                    fun(null);
                    return false;
                },
                null);
            }
            else
            {
#if ASSET_DEBUG
                AssetBundle_LoadFromFileAsync_Begin.Execution(url, ()=> 
                {
#endif
#if LoadFromFileAsync
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(dst, 0, (ulong)(offset));
                    var wait = new Wait(abcr, fun);
#if ASSET_DEBUG
                    wait.url = url;
#endif
                    AddWait(wait);
#else
                    AssetBundle assetBundle = AssetBundle.LoadFromFile(dst, 0, (ulong)offset);
                    TimerMgrObj.Instance.AddLateUpdate((object p) =>
                    {
#if ASSET_DEBUG
                        AssetBundle_LoadFromFileAsync_End.Execution(url, () => 
                        {
#endif
                            fun(assetBundle);
#if ASSET_DEBUG
                        });
#endif
                        return false;
                    }, null);
#endif
#if ASSET_DEBUG
                });
#endif
            }
        }

#if UNITY_EDITOR
        public static void Check(string url, AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                Debug.LogErrorFormat("url:{0} assetBundle == null", url);
                return;
            }

            string[] names = assetBundle.GetAllAssetNames();
            int index = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Format("CheckAssetBundle url:{0}", url));
            for (int i = 1; i < names.Length; ++i)
            {
                string n = names[i];
                if (n.EndsWith(".js") || n.EndsWith(".cs") || n.EndsWith(".dll") || n.EndsWith(".asset"))
                    continue;

                sb.AppendLine(string.Format("   {0}", n));
                ++index;
            }

            if (index >= 2)
            {
                Debuger.DebugLog(sb.ToString());
            }
        }
#endif
    }
}
#endif
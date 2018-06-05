#if USE_ABL
using XTools;
using System;
using PackTool;
using UnityEngine;
using System.Collections.Generic;

namespace ABL
{
    public static class AssetBundleLoad
    {
        static AssetBundleCreateRequest Load(string url)
        {
            Debug.LogFormat("ab:{0}", url);
            int lenght = 0;
            int offset = 0;
            string dst = ResourcesPack.Find("AB/" + url, out offset, out lenght);
            if (string.IsNullOrEmpty(dst))
            {
                Debug.LogErrorFormat("AssetBundle:{0} file not find!", url);
            }
            else
            {
                return AssetBundle.LoadFromFileAsync(dst, 0, (ulong)(offset));
            }

            return null;
        }

        class Data
        {
            public AssetBundleCreateRequest abcr;
            public Action<AssetBundle> onEnd;
        }

        static LinkedList<Data> DataList = new LinkedList<Data>();
        static TimerFrame.Frame update_frame = null;

        static void Reg()
        {
            if (update_frame != null)
                return;

            update_frame = XTools.TimerMgrObj.Instance.AddLateUpdate(UpdateCheck, null);
        }

        static bool UpdateCheck(object p)
        {
            Data d;
            var ator = DataList.First;
            while (ator != null)
            {
                d = ator.Value;
                if (d.abcr.isDone)
                {
                    try
                    {
                        d.onEnd(d.abcr.assetBundle);
                    }
                    catch (Exception e)
                    {
                        Debuger.LogException(e);
                    }

                    var n = ator.Next;
                    DataList.Remove(ator);
                    ator = n;
                }
                else
                {
                    ator = ator.Next;
                }
            }

            if (DataList.Count == 0)
            {
                update_frame = null;
                return false;
            }
            return true;
        }

        static public void LoadAsync(string url, Action<AssetBundle> action)
        {
            AssetBundleCreateRequest abcr = Load(url);
            DataList.AddLast(new Data() { abcr = abcr, onEnd = action });
            Reg();
        }
    }
}
#endif
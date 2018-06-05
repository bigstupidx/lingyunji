#if USE_RESOURCESEXPORT
using XTools;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class TMPFontLoad : AssetLoad<TMPro.TMP_FontAsset>
    {
        static public TMPFontLoad.Data Load(string name, ResourcesEnd<TMPro.TMP_FontAsset> fun, object funp)
        {
            return TMPFontLoad.LoadImp(name, fun, funp, Create);
        }

        static TMPFontLoad Create(string name)
        {
            return CreateAsset<TMPFontLoad>(name);
        }

#if ASSET_DEBUG
        static TimeTrack font_timetrack = TimeTrackMgr.Instance.Get("Resources.Load");
#endif

        static int fontlib_load_total = 0;

        bool CheckLoadEnd(object p)
        {
            object[] pp = p as object[];
            if (pp[1] == null)
            {
                if (fontlib_load_total == 0)
                {
                    // 可以加载
                    pp[1] = Resources.LoadAsync<TMPro.TMP_FontAsset>(pp[0] as string);
                    ++fontlib_load_total;
                    return true;
                }
                else
                {
                    // 下一桢再判断下是否可加载
                    return true;
                }
            }

            // 正在加载当中
            ResourceRequest rr = pp[1] as ResourceRequest;
            if (!rr.isDone)
                return true;

            asset = rr.asset as TMPro.TMP_FontAsset;

            OnEnd();
            --fontlib_load_total;
            return false;
        }

        protected override void LoadAsset(string name)
        {
            if (name[0] == ':')
            {
                string path = name.Substring(1, name.Length - 7);

#if UNITY_EDITOR
                AddRef();
#endif
                // 属于Resources下的资源
                TimerMgrObj.Instance.addFrameLateUpdate(
                    CheckLoadEnd,
                    new object[2] { path, null });
            }
            else
            {
                AssetBundleLoad.Load(name, LoadAssetEnd);
            }
        }

        void LoadAssetEnd(AssetBundle assetBundle)
        {
            isDone = true;
            if (assetBundle == null)
            {
                Debug.LogError(string.Format("LoadAssetEnd: {0} assetBundle == null!", url));
            }
            else
            {
                asset = assetBundle.mainAsset as TMPro.TMP_FontAsset;
                assetBundle.Unload(false);
            }

            OnEnd();
        }

        protected override void FreeSelf()
        {
            FreeAsset<TMPFontLoad>(this);
        }
    }
}
#endif
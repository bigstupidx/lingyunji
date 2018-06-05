#if USE_RESOURCESEXPORT
using XTools;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class FontLibLoad : AssetLoad<Font>
    {
        static public FontLibLoad.Data Load(string name, ResourcesEnd<Font> fun, object funp)
        {
            return FontLibLoad.LoadImp(name, fun, funp, Create);
        }

        static FontLibLoad Create(string name)
        {
            var fll = new FontLibLoad();
            fll.Reset(name);
            return fll;
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
                    pp[1] = Resources.LoadAsync<Font>(pp[0] as string);
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

            asset = rr.asset as Font;
            OnEnd();
            --fontlib_load_total;
            return false;
        }

        protected override void LoadAsset(string name)
        {
            AddRef(); // 内置字体不需要删除
            if (name[0] == ':')
            {
                string path = name.Substring(1, name.Length - 5);

				// 属于Resources下的资源
                TimerMgrObj.Instance.addFrameLateUpdate(
                    CheckLoadEnd,
                    new object[2] { path, null });
            }
            else if (name.LastIndexOf('.') == -1)
            {
                asset = BuiltinResource.Instance.GetFont(name);
                if (asset == null)
                {
                    Debuger.LogError(string.Format("BuiltinResource font:{0} not find!", name));
                }

                NextFrame();
                return;
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
                Debuger.LogError(string.Format("LoadAssetEnd: {0} assetBundle == null!", url));
            }
            else
            {
                asset = assetBundle.mainAsset as Font;
                assetBundle.Unload(false);
            }

            OnEnd();
        }

        protected override void FreeSelf()
        {

        }
    }
}
#endif
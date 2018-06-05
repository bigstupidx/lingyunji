using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.UI.Utility
{
    public static class TipContentUtil
    {
        public static void Show(string name, params object[] objs)
        {
            string msg = GenText(name, objs);
            if (!string.IsNullOrEmpty(msg))
            {
                xys.UI.SystemHintMgr.ShowHint(msg);
            }
        }

        public static string GenText(string name, params object[] objs)
        {
            Config.TipsContent cfg = Config.TipsContent.GetByName(name);
            if (null == cfg)
            {
                Log.Error(string.Format("TipContentUtil::GenText miss cfg {0}", name));
                return string.Empty;
            }

            string text = cfg.des;
            if (null != objs && objs.Length > 0)
            {
                try
                {
                    text = string.Format(cfg.des, objs);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("TipContentUtil::GenText error {0}-{1}, objs.Count={2}, e.Message {3}", cfg.id, cfg.name, objs.Length, e.Message));
                    text = string.Empty;
                }
            }

            return text;
        }
    }
}

#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot.UI
{
    class MainPanelItemListener
    {
        public Func<bool> itemShowFunc = null;
        public Func<bool> itemReadyFunc = null;
        public Action<object> onClickCallBack = null;
        public object arg;
        public MainPanelItemListener()
        {
        }
        public MainPanelItemListener(Func<bool> itemShowFunc, Func<bool> itemReadyFunc = null, Action<object> onClickCallBack = null, object arg = null)
        {
            this.itemShowFunc = itemShowFunc;
            this.itemReadyFunc = itemReadyFunc;
            this.onClickCallBack = onClickCallBack;
            this.arg = arg;
        }
    }
}
#endif
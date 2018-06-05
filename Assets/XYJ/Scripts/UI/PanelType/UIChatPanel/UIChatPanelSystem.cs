#if !USE_HOT
using System;
using Config;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using xys.hot.Event;
using xys.UI;

namespace xys.hot.UI
{
    [Serializable]
    class UIChatPanelSystem
    {
        [SerializeField]
        private VerticalScrollRectWrapper scrollRect;
        [SerializeField]
        private GameObject parent;
        private ChatMgr mgr;
        public void OnInit()
        {
            mgr = hotApp.my.GetModule<HotChatModule>().ChatMgr;
            scrollRect.OnInit();
        }

        public void OnShow()
        {
            parent.SetActive(true);
            var current = ChannelType.Channel_System;
            scrollRect.Init(mgr.GetStartIndex(current)
                , mgr.GetMaxNumber(current)
                , mgr.GetMaxIndex(current));
        }

        public void OnHide()
        {
            parent.SetActive(false);
            scrollRect.OnCellAdding = null;
            var current = ChannelType.Channel_System;
            mgr.SetMsgStartIndex(current, scrollRect.GetStartIndex());

            scrollRect.ClearCells();
        }

        public void AddData()
        {
            scrollRect.AddData();
        }

        public void SetLock(bool isLock)
        {
            scrollRect.IsLock = isLock;
        }

        public VerticalScrollRectWrapper GetVerticalWrapper()
        {
            return scrollRect;
        }
    }
}
#endif

#if !USE_HOT
using System;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using UI;
using UnityEngine.UI;
using xys.hot.Event;
using xys.UI;

namespace xys.hot.UI
{
    [Serializable]
    public class UIMainChatPanel
    {
        [SerializeField]
        private UIMainChatScrollView msgScrollView;
        [SerializeField]
        private UIMainChatIcons icons;

        private HotObjectEventAgent eventAgent;
        private ChatMgr mgr;
        #region Impl
        public void OnInit()
        {
            icons.OnInit();
            msgScrollView.OnInit();
        }

        public void OnShow()
        {
            msgScrollView.OnShow();
            icons.OnShow();

            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            // 刷新UI消息
            eventAgent.Subscribe<ChannelType>(EventID.ChatMainPanel_RefreshMainChatMsg, OnRefreshMainChatMsg);
        }

        public void OnHide()
        {
            msgScrollView.OnHide();
            icons.OnHide();

            eventAgent.Release();
            eventAgent = null;
        }
        #endregion

        #region Event
        private void OnRefreshMainChatMsg(ChannelType channel)
        {
            if(ChatUtil.ChatMgr.GetChannelCondition(channel))
            {
                msgScrollView.AddData(); 
            }
        }
        #endregion
    }

}
#endif
#if !USE_HOT
using System;
using UnityEngine;
using xys.hot.Event;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    internal class UIChatVoiceTipsPanel : HotPanelBase
    {
        [SerializeField]
        private StateRoot icon;

        private HotObjectEventAgent eventAgent;

        #region Impl
        public UIChatVoiceTipsPanel(UIHotPanel parent) : base(parent) { }
        public UIChatVoiceTipsPanel() : base(null) { }

        protected override void OnInit() { }

        protected override void OnShow(object p)
        {
            eventAgent = new HotObjectEventAgent(App.my.localPlayer.eventSet);
            eventAgent.Subscribe(EventID.ChatVoiceTips_Misc, SetVoiceWithMisc);
            eventAgent.Subscribe(EventID.ChatVoiceTips_Cancle, SetVoiceWithCancle);
            // 重复开启时复位
            icon.SetCurrentState(0, false);
        }

        protected override void OnHide()
        {
            eventAgent.Release();
            eventAgent = null;
        }

        #endregion

        public void SetVoiceWithMisc()
        {
            icon.SetCurrentState(0, false);
        }

        public void SetVoiceWithCancle()
        {
            icon.SetCurrentState(1, false);
        }
    }
}
#endif

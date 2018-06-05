#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using wTimer;
using xys.UI;


namespace xys.hot.UI
{
    class UIActivityUnlockedPanel : HotPanelBase
    {
        [SerializeField]
        private Transform m_Transform;

        [SerializeField]
        private Text m_NameText;
        [SerializeField]
        private Text m_DescText;

        private Config.ActivityDefine m_ActivityData;

        private SimpleTimer m_CloseTimer;        //计时器

        private int m_closeTime = 0;

        public UIActivityUnlockedPanel() : base(null) { }
        public UIActivityUnlockedPanel(UIHotPanel parent) : base(parent)
        {
        }

        protected override void OnInit()
        {
            m_CloseTimer = new SimpleTimer(App.my.mainTimer);
        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            m_ActivityData = Config.ActivityDefine.Get(((ActivityData)args).activityId);
            m_NameText.text = m_ActivityData.name;
            m_DescText.text = m_ActivityData.unlockDesc;
        }

        protected override void OnEndPlayAnimShow()
        {
            m_closeTime = m_CloseTimer.Register(3f, 1, ClosePanel);
        }

        // 3s自动关闭界面
        private void ClosePanel()
        {
            m_CloseTimer.Release();
            m_closeTime = 0;
            App.my.uiSystem.HidePanel(xys.UI.PanelType.UIActivityUnlockedPanel);
        }
    }
}
#endif
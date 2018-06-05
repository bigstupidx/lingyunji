#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/20


using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;


namespace xys.hot.UI
{
    class UIPlantTreeTipPanel : HotPanelBase
    {
        [SerializeField]
        private Text m_ShowText;

        [SerializeField]
        private Text m_DetailText;

        [SerializeField]
        private Button m_CloseBtn;

        [SerializeField]
        private Button m_GoBtn;

        [SerializeField]
        private Text m_CloseText;

        private int m_AutoHideTimer = 0;
        private int m_AutoCloseTime = 20;

        private OneTreeData treeData;

        public UIPlantTreeTipPanel() : base(null) { }
        public UIPlantTreeTipPanel(UIHotPanel parent) : base(parent)
        {
        }

        protected override void OnInit()
        {
            m_ShowText.text = xys.UI.Utility.TipContentUtil.GenText("moneyTree_mature_tip");
            m_DetailText.text = xys.UI.Utility.TipContentUtil.GenText("moneyTree_get_mind_tip");

            m_CloseBtn.onClick.AddListener(OnClickClose);
            m_GoBtn.onClick.AddListener(OnClickGoToTree);
        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            treeData = args as OneTreeData;
            m_AutoHideTimer = hotApp.my.mainTimer.Register(1, int.MaxValue, AutoPlayHideTimer);
        }

        private void AutoPlayHideTimer()
        {
            m_AutoCloseTime--;
            m_CloseText.text = "取消(" + m_AutoCloseTime + ")";

            if (m_AutoCloseTime <= 0)
                OnClickClose();
        }

        private void OnClickClose()
        {
            if (m_AutoHideTimer > 0)
            {
                hotApp.my.mainTimer.Cannel(m_AutoHideTimer);
                m_AutoHideTimer = 0;
            }

            m_AutoCloseTime = 20;
            App.my.uiSystem.HidePanel("UIPlantTreeTipPanel");
        }

        private void OnClickGoToTree()
        {
            // 自动寻路找到种植区域
            MoneyTreeMgr moneyTreeMgr = hotApp.my.GetModule<HotMoneyTreeModule>().moneyTreeMgr;
            if (moneyTreeMgr != null)
            {
                moneyTreeMgr.UseSeedToPlantTree(treeData.treeId, -1, false, treeData.uId);
            }

            OnClickClose();
        }
    }
}
#endif
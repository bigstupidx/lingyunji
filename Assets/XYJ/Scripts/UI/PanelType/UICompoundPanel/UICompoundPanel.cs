#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using Config;

    class UICompoundPanel : HotPanelBase
    {
        [SerializeField]
        Button m_ClosePanel;

        [SerializeField]
        GameObject m_StoneCompoundPanel;        // 强化石类型界面

        [SerializeField]
        GameObject m_CommonCompoundPanel;       // 通用类型界面

        [SerializeField]
        Text m_TilteText;

        [SerializeField]
        CompoundCommonPage m_CommonPageMgr;

        [SerializeField]
        CompoundStonePage m_StonePageMgr;

        xys.UI.UIHotPanel m_Panel;

        public UICompoundPanel() :base(null){ }
        public UICompoundPanel(xys.UI.UIHotPanel panel) : base(panel)
        {
            m_Panel = panel;
        }

        protected override void OnInit()
        {
            m_CommonPageMgr.OnInit(m_Panel);
            m_StonePageMgr.OnInit(m_Panel);
            RegistButton();
        }

        protected override void OnShow(object args)
        {
            ItemFuncObject obj = args as ItemFuncObject;

            Item itemConfig = Item.Get(obj.itemId);
            if (itemConfig == null)
                return;
            ItemCompound config = ItemCompound.Get(itemConfig.comId);
            if (config == null)
                return;

            if (config.type == ItemCompositeType.common)
            {
                m_StoneCompoundPanel.SetActive(false);
                m_CommonCompoundPanel.SetActive(true);
                m_CommonPageMgr.OnShow(itemConfig.comId);
                m_TilteText.text = "合成";
            }
            else if (config.type == ItemCompositeType.stone)
            {
                m_StoneCompoundPanel.SetActive(true);
                m_CommonCompoundPanel.SetActive(false);
                m_StonePageMgr.OnShow(obj.itemId);
                m_TilteText.text = "强化石合成";
            }
        }

        protected override void OnHide()
        {
            m_CommonPageMgr.OnHide();
            m_StonePageMgr.OnHide();
        }

        void RegistButton()
        {
            // 返回按钮
            m_ClosePanel.onClick.AddListener(() =>
            {
                App.my.uiSystem.HidePanel(xys.UI.PanelType.UICompoundPanel);
            });
        }

        void RegistEvent()
        {
        }
    }
}
#endif
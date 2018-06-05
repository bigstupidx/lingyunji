#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    class FunctionPanel : HotPanelBase
    {
        public FunctionPanel() : base(null)
        {

        }

        public FunctionPanel(xys.UI.UIFunctionPanel parent) : base(parent)
        {

        }

        [SerializeField]
        Button m_AppearanceBtn;

        [SerializeField]
        Button m_clanBtn;
        int click_id = 0;
        protected override void OnInit()
        {
            //Íâ¹Û°´Å¥
            if (m_AppearanceBtn != null)
            {
                m_AppearanceBtn.onClick.AddListener(
                    () =>
                    {
                        App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIAppearancePanel, null);
                    });
            }

            if (m_clanBtn != null)
            {
                m_clanBtn.onClick.AddListenerIfNoExist(() =>
                {
                    parent.Hide(false);
                    if (App.my.localPlayer.clanIdValue > 0)
                    {
                        App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIClanPanel, null);
                    }
                    else
                    {
                        App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIClanCreatePanel, null);
                    }
              
                });
            }
        }
        protected override void OnShow(object args)
        {
            click_id = xys.UI.EventHandler.pointerClickHandler.AddEnd(OnClick);
        }

        bool OnClick(GameObject obj, BaseEventData bed)
        {
            if (obj != null)
            {
                if (parent.transform.IsInherit(obj.transform))
                {
                    if (obj.GetComponent<xys.UI.OpenPanel>() != null)
                    {
                        parent.Hide(false);
                    }
                }
            }
            return true;
        }

        protected override void OnHide()
        {
            if (click_id != 0)
                xys.UI.EventHandler.pointerClickHandler.Remove(click_id);
            click_id = 0;
        }
    }
}

#endif
namespace xys.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

#if UNITY_EDITOR
    [SinglePanelType("xys.hot.UI.BloodTipsPanel")]
#endif
    public class UIBloodTipsPanel : UIHotPanel
    {
        int m_EventHanderId = -1;
        protected override void OnShow(object args)
        {
            base.OnShow(args);
        }

        protected override void OnHide()
        {
            base.OnHide();
        }
    }
}
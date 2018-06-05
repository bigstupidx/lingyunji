#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using UnityEngine;
    using xys.UI;
    using xys.UI.State;

    class UIEquipPanel : HotTablePanelBase
    {
        [SerializeField]
        ILMonoBehaviour m_ILEquipListView;
        UIEquipListView m_EquipListView;
        [SerializeField]
        Transform m_leftContentTrans;
        EquipModule m_Module;
        HotTablePanel m_Parent;
        string m_currentPageType;
        public enum EquipPageType
        {
            InfPage = 0,
            RefPage = 1,
        }
        UIEquipPanel() : base(null) { }
        UIEquipPanel(xys.UI.HotTablePanel parent) : base(parent)
        {
            m_Parent = parent;
        }

    
        protected override void OnInit()
        {
            this.m_Module = App.my.localPlayer.GetModule<EquipModule>();

            if (m_ILEquipListView != null)
            {
                m_EquipListView = (UIEquipListView)m_ILEquipListView.GetObject();
            }
        }

        protected override void OnShow(object args)
        {
            Event.Subscribe(EventID.Equip_RefreshEquipList, m_EquipListView.RefreshData);
            //Event.Subscribe(EventID.Equip_NoneEquiped, HidePageContent);
            m_leftContentTrans.gameObject.SetActive(true);
            m_EquipListView.currentShowType = UIEquipListView.ShowType.EquipView;
            m_EquipListView.OnShow();
        }

        //void HidePageContent()
        //{
        //    m_Parent.GetShowPage().Hide();

        //}

        protected override void OnHide()
        {
            m_EquipListView.OnHide();
        }

        protected override bool OnPreChange(HotTablePage page)
        {
            m_leftContentTrans.gameObject.SetActive(!(page.pageType == "EquipForgePage"));
            return true;
        }
    }
}

#endif
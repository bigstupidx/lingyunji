#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class AttributePage : HotTablePageBase
    {
        enum PageTye
        {
            PropertyPage = 0,
            SkillPage = 1,
        }

        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        UIPetsScrollView m_ScrollView;

        UIPetsInfomation m_Infos;

        [SerializeField]
        ILMonoBehaviour m_ILPropertyPage;
        UIPetsProperty m_PropertyPage;
        [SerializeField]
        ILMonoBehaviour m_ILSkillPage;
        UIPetsSkillPage m_SkillPage;

        [SerializeField]
        StateToggle m_StateToggle;
        AttributePage() : base(null) { }

        AttributePage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            if (m_ILScrollView != null)
                m_ScrollView = (UIPetsScrollView)m_ILScrollView.GetObject();

            if (m_ILPropertyPage != null)
                m_PropertyPage = (UIPetsProperty)m_ILPropertyPage.GetObject();

            if(m_ILSkillPage != null)
                m_SkillPage = (UIPetsSkillPage)m_ILSkillPage.GetObject();

            m_StateToggle.OnSelectChange = this.SelectPage;
        }

        protected override void OnShow(object args)
        {
            PetsInfos infos = args as PetsInfos;
            if (infos == null)
            {
                Debuger.LogError("Panel null");
                return;
            }

            int itemid = infos.itemId;

            this.m_Infos = infos.panel.infos;
            if (m_PropertyPage != null)
                m_PropertyPage.panel = infos.panel;
            if (m_SkillPage != null)
                m_SkillPage.panel = infos.panel;

            if (m_ILPropertyPage != null)
                m_ILPropertyPage.gameObject.SetActive(itemid == 0);
            if (m_ScrollView != null)
                m_ScrollView.selectedCallback = this.ResetPage;

            if(itemid != 0)
            {
                Config.Item itemData = Config.Item.Get(itemid);
                switch (itemData.sonType)
                {
                    case (int)Config.ItemChildType.petLockSkillItem:
                        m_StateToggle.Select = (int)PageTye.SkillPage;
                        break;
                    default:
                        m_StateToggle.Select = (int)PageTye.PropertyPage;
                        break;
                }
            }
            else
            {
                m_StateToggle.Select = (int)PageTye.PropertyPage;
            }
         
            Event.Subscribe(EventID.Package_UpdatePackage, ResetItemData);
        }


        protected override void OnHide()
        {
            if (m_ScrollView != null)
                m_ScrollView.selectedCallback = null;
        }

        void ResetPage()
        {
            if (m_ScrollView.selected == -1)
                return;
            //刷新信息面板
            m_Infos.ResetData(m_ScrollView.selected);
            m_PropertyPage.ResetData(m_ScrollView.selected);
            m_SkillPage.ResetData(m_ScrollView.selected);
        }
        void RefleshData()
        {
            m_Infos.ResetData(m_ScrollView.selected);
            m_PropertyPage.ResetData(m_ScrollView.selected);
            m_SkillPage.ResetData(m_ScrollView.selected);
        }

        void SelectPage(StateRoot state, int index)
        {
            if (m_ScrollView.selected == -1)
                return;
            switch (index)
            {
                case (int)PageTye.PropertyPage:
                    m_Infos.transform.gameObject.SetActive(true);
                    m_ILSkillPage.gameObject.SetActive(false);
                    m_ILPropertyPage.gameObject.SetActive(true);
                    m_PropertyPage.ResetData(m_ScrollView.selected);
                    break;
                case (int)PageTye.SkillPage:
                    m_Infos.transform.gameObject.SetActive(true);
                    m_ILPropertyPage.gameObject.SetActive(false);
                    m_ILSkillPage.gameObject.SetActive(true);
                    m_SkillPage.ResetData(m_ScrollView.selected);
                    break;
            }
        }

        void ResetItemData()
        {
            if(m_ILPropertyPage.gameObject.activeSelf)
                m_PropertyPage.ResetData(m_ScrollView.selected);
            else if (m_ILSkillPage.gameObject.activeSelf)
                m_SkillPage.ResetData(m_ScrollView.selected);
        }
    }

}
#endif
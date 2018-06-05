#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class TrainPage : HotTablePageBase
    {
        UIPetsInfomation m_Infos;

        [SerializeField]
        ILMonoBehaviour m_ILScroll;
        UIPetsScrollView m_ScrollView;
        [SerializeField]
        UITrainScrollView m_TrainScrollView;
        [SerializeField]
        UITrainDigestionInfo m_DigestionInfos;
        [SerializeField]
        GameObject m_DigestionObj;

        [SerializeField]
        ILMonoBehaviour m_ILNickTips;
        UIPetsNickNameTips m_NickNameTips;
        [SerializeField]
        ILMonoBehaviour m_AiTips;
        UIPetsAITips m_PetAITips;

        [SerializeField]
        StateToggle m_StateToggle;

        TrainPage() : base(null) { }
        TrainPage(HotTablePage page) : base(page) { }
        protected override void OnInit()
        {
            if (m_ILScroll != null) m_ScrollView = (UIPetsScrollView)m_ILScroll.GetObject();
            if (m_ILNickTips != null) m_NickNameTips = (UIPetsNickNameTips)m_ILNickTips.GetObject();
            if (m_AiTips != null) m_PetAITips = (UIPetsAITips)m_AiTips.GetObject();

            m_TrainScrollView.OnInit();
            m_DigestionInfos.OnInit();
        }

        protected override void OnShow(object args)
        {
            PetsInfos infos = args as PetsInfos;
            if(infos == null)
            {
                Debug.Log("Panel null");
                this.parent.gameObject.SetActive(false);
                return;
            }
            this.m_Infos = infos.panel.infos;
            this.m_DigestionInfos.panel = infos.panel;

            int itemId = 0;
            if (infos != null)
                itemId = infos.itemId;
            m_TrainScrollView.Show(itemId, this.SelectedCallBack);
            this.ResetPage();

            Event.Subscribe(EventID.Package_UpdatePackage, this.ResetItemData);
            this.m_ScrollView.selectedCallback = this.ResetPage;
            this.m_StateToggle.OnSelectChange = this.ChangePage;
        }
        protected override void OnHide()
        {
            this.m_ScrollView.selectedCallback = null;
            this.m_StateToggle.OnSelectChange = null;

            this.m_TrainScrollView.OnDisable();
        }

        void SelectedCallBack(int itemID)
        {
            m_DigestionInfos.Set(m_ScrollView.selected, itemID);
        }

        void ResetPage()
        {
            m_PetAITips.ReFresh();
            if (m_ILNickTips.gameObject.activeSelf)
                m_PetAITips.CloseTips();

            if (m_AiTips.gameObject.activeSelf)
                m_NickNameTips.CloseTips();

            m_Infos.ResetData(m_ScrollView.selected);
            m_DigestionInfos.Set(m_ScrollView.selected, m_TrainScrollView.SelectedItem);
        }

        void ChangePage(StateRoot sr,int index)
        {
            if (index == 0)
            {
                m_DigestionObj.SetActive(true);
                m_DigestionInfos.Set(m_ScrollView.selected, m_TrainScrollView.SelectedItem);
            }
            else
            {
                m_DigestionObj.SetActive(false);
                //UIHintManage.Instance.ShowHint("功能暂未开放");
            }
        }

        void ResetData()
        {
            m_DigestionInfos.Set(m_ScrollView.selected, m_TrainScrollView.SelectedItem);
            m_Infos.ResetData(m_ScrollView.selected);
        }

        void ResetItemData()
        {
            m_TrainScrollView.RefleshData();
        }
    }

}

#endif
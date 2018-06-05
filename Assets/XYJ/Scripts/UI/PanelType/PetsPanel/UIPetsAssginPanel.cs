#if !USE_HOT
using xys.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.hot.UI
{
    class UIPetsAssginPanel : HotTablePanelBase
    {
        public enum page
        {
            AssignPage,
            AutoAssignPage
        }

        xys.PetsModule m_Module;

        [SerializeField]
        UIPetsAutoAssignResult m_ResAutoAssignPage;
        [SerializeField]
        Button m_TipsBtn;
        [SerializeField]
        Button m_ReturnBtn;

        PetsPanel m_Panel;
        public PetsPanel panel { get { return m_Panel; } }
        UIPetsAssginPanel() :base(null) { }
        UIPetsAssginPanel(HotTablePanel parent) : base(parent)
        {

        }

        protected override void OnInit()
        {
            this.m_Module = App.my.localPlayer.GetModule<PetsModule>();

           // this.m_TipsBtn.onClick.AddListener(this.ShowAssignTips);
            this.m_ReturnBtn.onClick.AddListener(this.CloseEvent);
        }

        protected override void OnShow(object args)
        {
            this.m_Panel = args as PetsPanel;
            if (this.m_Panel != null)
            {
                //
                this.tableParent.ShowType((int)page.AssignPage, this.m_Panel);
            }
            else
            {
                Debuger.LogError("panel未找到");
                parent.Hide(true);
                return;
            }

            Event.Subscribe(EventID.Pets_SliderUI, this.RefreshSlider);
        }

        bool OnSelectChange(int pageIndex)
        {
            tableParent.ShowType(pageIndex, this.m_Panel);
            return false;
        }

        void CloseEvent()
        {
            string type = tableParent.GetPageList()[(int)page.AssignPage].Get().pageType;
            if (this.tableParent.CurrentPage != type)
            {
                this.tableParent.ShowType(type, this.m_Panel);
            }
            else
            {
                App.my.uiSystem.HidePanel("UIPetsAssginPanel");
            }
        }
        #region 事件区域
        //显示百分比分配界面
        void ShowAssignTips()
        {
            tableParent.ShowType((int)page.AutoAssignPage, this.m_Panel);
        }
        #endregion
        void RefreshSlider()
        {
            if (this.m_Panel.selectedPetObj == null)
                return;
            int[] tempPoint = new int[6];
            tempPoint[0] = this.m_Panel.selectedPetObj.power_slider_point;
            tempPoint[1] = this.m_Panel.selectedPetObj.intelligence_slider_point;
            tempPoint[2] = this.m_Panel.selectedPetObj.root_bone_slider_point;
            tempPoint[3] = this.m_Panel.selectedPetObj.bodies_slider_point;
            tempPoint[4] = this.m_Panel.selectedPetObj.agile_slider_point;
            tempPoint[5] = this.m_Panel.selectedPetObj.body_position_slider_point;

            m_ResAutoAssignPage.m_Transform.gameObject.SetActive(true);
            m_ResAutoAssignPage.Set(this,tempPoint);
        }
    }
}
#endif
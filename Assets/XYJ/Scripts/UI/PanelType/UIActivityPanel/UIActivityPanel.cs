#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using NetProto.Hot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;


namespace xys.hot.UI
{
    class UIActivityPanel : HotTablePanelBase
    {
        [SerializeField]
        private Button m_CloseBtn;

        [SerializeField]
        private Button m_WeekBtn;

        [SerializeField]
        private Button m_SettingBtn;

        [SerializeField]
        private Button m_QuestionBtn;

        [SerializeField]
        private Button m_QuestionClose;

        [SerializeField]
        private Text m_QuestionText;

        [SerializeField]
        private Transform m_Question;

        [SerializeField]
        private ILMonoBehaviour m_LWeekPanel;
        private UIActivityWeekPanel m_WeekPanel;

        [SerializeField]
        private ILMonoBehaviour m_LRemindPanel;
        private UIActivityPushPanel m_RemindPanel;

        private ActivityMgr m_ActivityData;

        HotTablePanel m_Parent;

        UIActivityPanel() : base(null) { }
        UIActivityPanel(HotTablePanel parent) : base(parent)
        {
            m_Parent = parent as HotTablePanel;
        }

        protected override void OnInit()
        {
            ActivityMgr activityMgr = hotApp.my.GetModule<HotActivityModule>().activityMgr;
            if (activityMgr != null)
            {
                m_ActivityData = activityMgr;
            }

            m_WeekPanel = (UIActivityWeekPanel)m_LWeekPanel.GetObject();

            m_RemindPanel = (UIActivityPushPanel)m_LRemindPanel.GetObject();

            m_QuestionText.text = GlobalSymbol.ToUT(Config.IntroduceData.Get(1).desc);
            SetBtnClickListener();
        }

        protected override void OnShow(object args)
        {
            m_Parent.Event.Subscribe<Dictionary<int, ActivityData>>(EventID.Activity_UpdatePushUI, RefreshRemindView);
        }

        protected override bool OnPreChange(HotTablePage page)
        {
            return true;
        }

        private void SetBtnClickListener()
        {
            m_CloseBtn.onClick.AddListener(() =>
            {
                App.my.uiSystem.HidePanel(PanelType.UIActivityPanel);
            });

            m_WeekBtn.onClick.AddListener(() =>
            {
                m_WeekPanel.SetData(m_ActivityData.m_ActivityDbData.listActivies);
            });

            m_SettingBtn.onClick.AddListener(() =>
            {
                m_RemindPanel.SetData(m_ActivityData.m_ActivityDbData.listActivies);
            });

            m_QuestionBtn.onClick.AddListener(()=> 
            {
                m_Question.gameObject.SetActive(true);
                AnimationHelp.PlayAnimation(m_Question.GetComponent<Animator>(), "open", "ui_Tankuang_Activity", null);
            });

            m_QuestionClose.onClick.AddListener(() =>
            {
                m_Question.gameObject.SetActive(false);
            });
        }

        private void RefreshRemindView(Dictionary<int, ActivityData> data)
        {
            m_RemindPanel.RefreshData(data);
        }
    }
}
#endif

#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;


namespace xys.hot.UI
{
    class UIActivityOpenPanel : HotPanelBase
    {
        [SerializeField]
        private Transform m_Transform;

        [SerializeField]
        private Button m_GoToBtn;
        [SerializeField]
        private Button m_CancelBtn;

        [SerializeField]
        private Text m_TitleText;
        [SerializeField]
        private Text m_TimeText;
        [SerializeField]
        private Text m_DescText;

        [SerializeField]
        private Image m_IconImage;

        private int m_AutoHideTimer = 0;

        private int m_AutoCloseTime = 30;

        private ActivityData m_ActivityData;

        public UIActivityOpenPanel() : base(null) { }
        public UIActivityOpenPanel(UIHotPanel parent) : base(parent)
        {
        }

        protected override void OnInit()
        {
            m_GoToBtn.onClick.AddListener(this.OnClickGoTo);
            m_CancelBtn.onClick.AddListener(this.OnClickCancel);
        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            m_ActivityData = (ActivityData)args;

            Config.ActivityDefine activityData = Config.ActivityDefine.Get(m_ActivityData.activityId);

            m_TitleText.text = activityData.name;
            m_DescText.text = activityData.unlockDesc;

            Helper.SetSprite(m_IconImage, activityData.icon);

            m_TimeText.text = m_AutoCloseTime.ToString();
            m_AutoHideTimer = hotApp.my.mainTimer.Register(1, int.MaxValue, AutoPlayHideTime);
        }

        private void OnClickGoTo()
        {
            CanGoing(m_ActivityData);

            OnClickCancel();
        }

        private void OnClickCancel()
        {
            if (m_AutoHideTimer > 0)
            {
                hotApp.my.mainTimer.Cannel(m_AutoHideTimer);
                m_AutoHideTimer = 0;
            }

            m_AutoCloseTime = 30;
            App.my.uiSystem.HidePanel(PanelType.UIActivityOpenPanel);
        }

        private void AutoPlayHideTime()
        {
            if (m_AutoCloseTime > 0)
            {
                m_AutoCloseTime -= 1;
                m_TimeText.text = m_AutoCloseTime.ToString();
            }
            else
                OnClickCancel();
        }

        // 判断是否能前往
        private void CanGoing(ActivityData activityData)
        {
            Config.ActivityDefine activityConf = Config.ActivityDefine.Get(activityData.activityId);
            if (activityConf == null)
            {
                XYJLogger.LogError("活动id不存在" + activityConf.id);
            }

            if (activityConf.notOpen == 1 || hotApp.my.localPlayer.levelValue < activityConf.requireLv) // 策划开关
            {
                xys.UI.Utility.TipContentUtil.Show("activity_not_open_tip");
            }

            // 需要切换场景
            if (activityConf.sceneId != 0 && activityConf.sceneId != App.my.localPlayer.GetModule<LevelModule>().levelId)
            {

            }

            // 打开界面
            if (activityConf.panelName != "")
            {
                App.my.uiSystem.ShowPanel(activityConf.panelName);
            }

            // 环任务
            if (activityConf.taskId > 0)
            {

            }

            // 如果有npcid，走到npc
            if (activityConf.npcId > 0)
            {

            }
        }
    }
}
#endif
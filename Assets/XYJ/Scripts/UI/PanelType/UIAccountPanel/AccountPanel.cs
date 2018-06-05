#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    class AccountPanel : HotPanelBase
    {
        [SerializeField]
        GameObject m_right;
        [SerializeField]
        GameObject m_awardPrefab;
        [SerializeField]
        GameObject m_awardParent;
        [SerializeField]
        GameObject m_itemAward;
        [SerializeField]
        GameObject m_overTime;
        [SerializeField]
        Button m_exitBtn;
        [SerializeField]
        Text m_time;

        int m_chapterId;

        public AccountPanel() : base(null)
        {

        }

        public AccountPanel(xys.UI.UIAccountPanel parent) : base(parent)
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInit()
        {
            m_exitBtn.onClick.AddListener(() => { OnClickExit(false); });
            //m_mono.SetAction(SetTime);
        }

        protected override void OnShow(object args)
        {
            m_chapterId = (int)args;
            RefreshUI();
            Debuger.LogWarning("OnShow  OnShow   OnShow");
        }

        protected override void OnHide()
        {

        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        void RefreshUI()
        {
            //判断是否能够主动退出
            if (App.my.localPlayer.GetModule<LevelModule>().CanInitiativeExit(false))
            {
                m_exitBtn.gameObject.SetActive(true);
            }
            else
            {
                m_exitBtn.gameObject.SetActive(false);
            }

            //判断是否是章节副本，章节副本有次数限制
            if(m_chapterId != 0)
            {
                //todo
            }

            m_overTime.SetActive(false);
            //显示奖励
            m_itemAward.transform.DestroyChildren();
            m_awardParent.transform.DestroyChildren();
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        void SetTime()
        {
            int time = App.my.localPlayer.GetModule<LevelModule>().GetLevelLastTime();
            System.TimeSpan tp = new System.TimeSpan(0, 0, time);
            string format = "";
            if (time > 3600)
            {
                format = "HH:mm:ss";
            }
            else if (time > 60 && time <= 3600)
            {
                format = "mm:ss";
            }
            else
            {
                format = "ss";
            }
            m_time.text = System.Convert.ToDateTime(tp.ToString()).ToString(format);
        }

        /// <summary>
        /// 点击离开
        /// </summary>
        /// <param name="autoExit">是否自动退出</param>
        void OnClickExit(bool autoExit)
        {
            parent.Hide(true);
            if (!autoExit)
            {
                //手动退出
                Event.fireEvent(EventID.Level_Exit);
            }
        }
    }
}
#endif
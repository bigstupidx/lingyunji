#if !USE_HOT
namespace xys.hot
{
    using Config;
    using System.Collections;
    using UnityEngine;
    using xys.UI;
    using Config;

    /// <summary>
    /// 普通副本
    /// </summary>
    public class LevelLogicDungeon : LevelLogicBase
    {
        //副本退出等待时间
        private const int EXITTIME = 15;
        private const int LOSEEXITTIME = 3;

        //是否开场动画暂停
        private bool m_startCg = false;
        //是否胜利
        private bool m_iswin = false;
        //章节id
        private int m_chapterId;

        /// <summary>
        /// 关卡真实创建
        /// </summary>
        public override void Init(int levelId)
        {
            base.Init(levelId);
            m_levelEvent.Subscribe(EventID.Level_FinishCg, OnFinishCg);

            m_iswin = false;
            m_chapterId = 0;

            OpenCountDown();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnUpdate()
        {

        }

        /// <summary>
        /// 胜利或者失败
        /// </summary>
        /// <param name="iswin"></param>
        /// <param name="chapterId"></param>
        public override void OnWinOrLost(bool iswin, int chapterId)
        {
            m_chapterId = chapterId;
            m_iswin = iswin;
            Account();
        }

        /// <summary>
        /// 结算
        /// </summary>
        void Account()
        {
            //判断关卡类型
            if(m_config.levelType == LevelType.StroyDuplicate)
            {
                if(m_iswin)
                {

                }
                else
                {

                }
            }
            else
            {
                if(m_iswin)
                {
                    //胜利
                    if(m_config.accountType == AccountType.Default)
                    {
                        //打开结算界面
                        App.my.mainCoroutine.StartCoroutine(OpenAccount());
                    }
                    else
                    {
                        //直接退出
                        //App.my.levelLogicMgr.ExitTheLevel();
                        App.my.eventSet.fireEvent(EventID.Level_Exit);
                    }
                }
                else
                {
                    //失败
                    LocalPlayer player = App.my.localPlayer;
                    if(player != null)
                    {
                        if(player.isAlive)
                        {
                            //主角活着才显示失败
                        }
                    }
                }

                //关卡结束计时退出副本
                int time = m_iswin ? EXITTIME : LOSEEXITTIME;
                m_logicCommon.m_countdown.Beign(time,
                    () =>
                    {
                        var panel = UISystem.Get(PanelType.UIAccountPanel);
                        if(null != panel && panel.state == UIPanelBase.State.Show)
                        {
                            App.my.uiSystem.HidePanel(PanelType.UIAccountPanel);
                        }
                        //App.my.levelLogicMgr.ExitTheLevel();
                        App.my.eventSet.fireEvent(EventID.Level_Exit);
                    });
            }
        }

        /// <summary>
        /// 打开结算界面
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerator OpenAccount()
        {
            int delay = int.Parse(kvClient.Get("Guanqiawindelay").value);
            yield return new WaitForSeconds(delay);
            App.my.uiSystem.ShowPanel(PanelType.UIAccountPanel, m_chapterId);
        }

        /// <summary>
        /// 开启倒计时
        /// </summary>
        /// <param name="levelInfo"></param>
        void OpenCountDown()
        {
            if (m_config.maxStayTime > 0)
                m_logicCommon.m_countdown.Beign((int)m_config.maxStayTime);
        }

        /// <summary>
        /// cg播放完
        /// </summary>
        void OnFinishCg()
        {
            if (null != m_config && m_startCg)
            {
                //OpenCountDown(m_prototype);
                //CheckDuplicate(m_prototype);
                m_startCg = false;
            }
        }
    }
}
#endif
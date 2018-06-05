#if !USE_HOT
namespace xys.hot
{
    using System;
    using wTimer;

    /// <summary>
    /// 关卡倒计时
    /// </summary>
    public class LevelLogicCountdown
    {
        //关卡计时器
        SimpleTimer m_timer = new SimpleTimer(App.my.mainTimer);
        //关卡剩余时间
        int m_levelLastTime = 0;
        //关卡计时结束的Action
        Action m_timerAction;

        public void Beign(int time, Action finishAction = null)
        {
            if (time > 0)
            {
                m_timer.Release();
                m_levelLastTime = time;
                m_timer.Register(1, time, LevelTimer);

                //显示主界面的倒计时
                //Utils.EventDispatcher.Instance.TriggerEvent<bool>(EventDefine.UIEvents.TriggerCountDown, true);
            }

            if (null != finishAction)
                m_timerAction = finishAction;
        }

        public void Exit()
        {
            //释放计时器
            m_timer.Release();
            m_timerAction = null;
            //Utils.EventDispatcher.Instance.TriggerEvent<bool>(EventDefine.UIEvents.TriggerCountDown, false);
        }

        public int GetLastTime()
        {
            return m_levelLastTime;
        }

        void LevelTimer()
        {
            m_levelLastTime--;
            if (m_levelLastTime <= 0)
            {
                if (null != m_timerAction)
                    m_timerAction();
                m_timerAction = null;
                m_levelLastTime = 0;

                //时间到隐藏主界面的倒计时
                //Utils.EventDispatcher.Instance.TriggerEvent<bool>(EventDefine.UIEvents.TriggerCountDown, false);

                //通知时间到了
                //LevelLogicManage.Instance.OnWinOrLost(LevelLogicManage.InstanceOverType.TimeOut, 0);
            }

            //刷新主界面的剩余时间
            //UIBattlePanel battlePanel = UI.UISystem.Get<UIBattlePanel>();
            //if (null != battlePanel && battlePanel.state == UI.UIPanelBase.State.Show)
            //{
            //    battlePanel.SetLevelTime(m_levelLastTime);
            //}
        }

        //计时器默认结束行为
        void DefaultFinishAction()
        {

        }

        //关卡时间增加事件
        public void AddTime(int time)
        {
            m_levelLastTime = m_levelLastTime + time;
        }
    }
}
#endif
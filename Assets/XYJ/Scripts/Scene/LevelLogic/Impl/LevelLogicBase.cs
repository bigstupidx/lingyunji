#if !USE_HOT
namespace xys.hot
{
    using Config;
    using xys.UI;

    public class LevelLogicBase
    {
        //事件定义
        protected EventAgent m_levelEvent;
        //关卡公用逻辑
        public LevelLogicCommon m_logicCommon;
        //关卡id
        protected int m_levelId;
        //关卡配置
        LevelDefine config_;
        protected LevelDefine m_config
        {
            get
            {
                if(config_ == null)
                {
                    config_ = LevelDefine.Get(m_levelId);
                }
                return config_;
            }
        }

        /// <summary>
        /// 关卡退出
        /// </summary>
        public virtual void OnExit()
        {
            m_logicCommon.m_countdown.Exit();

            var panel = UISystem.Get(PanelType.UIAccountPanel);
            if (null != panel && panel.state == UIPanelBase.State.Show)
            {
                App.my.uiSystem.HidePanel(PanelType.UIAccountPanel);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// 胜利失败回调：关卡成功失败(type=0时，表示后端回调，1表示时间到了，2表示玩家放弃复活 ，3表示点击退出按钮)
        /// </summary>
        /// <param name="isWin"></param>
        /// <param name="chapterId"></param>
        public virtual void OnWinOrLost(bool isWin, int chapterId = 0) { }

        /// <summary>
        /// 点击退出，不同的副本会有不同的操作
        /// </summary>
        public virtual void OnClickExit() { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="levelId"></param>
        public virtual void Init(int levelId)
        {
            m_levelId = levelId;
            m_logicCommon = new LevelLogicCommon();
            m_levelEvent = new EventAgent();
        }

        /// <summary>
        /// 获取关卡的剩余时间
        /// </summary>
        /// <returns></returns>
        public int GetLevelLastTime()
        {
            if (m_logicCommon != null && m_logicCommon.m_countdown != null)
            {
                return m_logicCommon.m_countdown.GetLastTime();
            }
            return 0;
        }
    }
}
#endif
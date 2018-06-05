using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
namespace xys.battle
{
    public class Buff
    {
        //结束时间
        float m_finishTime;
        public BuffConfig m_cfg { get; private set; }
        //上次触发时间
        public float m_nextTriggerTime { get; private set; }
        //叠加层数
        public int m_addCnt { get; private set; }
        //施法者
        public IObject m_source { get; private set; }
        protected IObject m_target;


        public void Enter(IObject source, IObject target, BuffConfig cfg)
        {
            m_source = source;
            m_target = target;
            m_cfg = cfg;
            m_addCnt = 1;
            RefreshTime();

            if (cfg.logic != null)
                cfg.logic.OnEnter(source, target);
        }

        //叠加，刷新时间
        public void AddCnt(IObject source)
        {
            m_source = source;
            RefreshTime();
            if (m_addCnt < m_cfg.addMaxCnt)
            {
                if (IsExcuteWhenAdd())
                    m_cfg.logic.OnEnter(source, m_target);
                m_addCnt++;
            }
        }

        //部分类型叠加时需要执行逻辑
        bool IsExcuteWhenAdd()
        {
            switch (m_cfg.type)
            {
                case BuffType.AddAttribute:
                    return true;
                default:
                    return false;
            }
        }

        void RefreshTime()
        {
            if (m_cfg.finishType == BuffConfig.FinishType.ByTime)
                m_finishTime = BattleHelp.timePass + m_cfg.timeLen;
            else
                m_finishTime = float.MaxValue;
        }

        public void Exit()
        {
            if (m_target == null)
                return;
            if (m_cfg.logic != null)
            {
                if (IsExcuteWhenAdd())
                {
                    for (int i = 0; i < m_addCnt; i++)
                        m_cfg.logic.OnExit(m_target);
                }
                else
                    m_cfg.logic.OnExit(m_target);
            }

            m_source = null;
            m_target = null;
            m_finishTime = BattleHelp.timePass - 1;

        }

        //是否结束
        public bool IsFinish()
        {
            return m_finishTime <= BattleHelp.timePass;
        }

        //更新间隔事件
        public void UpdateActionsTick()
        {
            if (m_cfg.intervalActionList.Count == 0)
                return;
            if (BattleHelp.timePass < m_nextTriggerTime)
                return;

            m_nextTriggerTime = BattleHelp.timePass + m_cfg.tickTime;
            ActionManager.HandleActionListAndSendMsg(null, m_source, m_target, m_cfg.intervalActionList, m_source.position, m_source.rotateAngle);
        }

        //buff剩余时间
        public float timeLeft { get { return m_finishTime - BattleHelp.timePass; } }
    }
}

using Config;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class IState
    {
        public virtual void OnEnter(IObject obj, StateType lastState, object para) { }
        public virtual void OnExit(IObject obj, StateType nextState) { }
        public virtual void OnUpdate(IObject obj) { }
    }

    public abstract class StateManager : IBattleComponent
    {
        Dictionary<StateType, IState> m_states = new Dictionary<StateType, IState>();
        public IState m_curSt { get; private set; }
        public StateType m_curStType { get; private set; }
        protected IObject m_obj;
        float m_stateFinishTime;
        float m_stateBeginTime;
        public bool isFastRun { get; private set; }


        protected abstract IState CreateState(StateType type);
        public virtual void OnAwake(IObject obj)
        {
            m_obj = obj;
            m_curSt = null;
            m_curStType = StateType.Empty;
            m_curSt = new IState();
        }
        public virtual void OnDestroy()
        {
            if (m_curSt != null)
                m_curSt.OnExit(m_obj, StateType.Destroy);
            m_curSt = null;
            m_curStType = StateType.Destroy;
            m_states.Clear();
            m_obj = null;
        }




        public virtual void OnStart()
        {

        }

        public void OnEnterScene()
        {

        }
        public void OnExitScene()
        {

        }

        //状态是否结束
        public bool IsStateFinish()
        {
            if (m_stateFinishTime < 0)
                return false;
            else
                return BattleHelp.timePass > m_stateFinishTime;
        }

        //设置状态结束
        public void SetStateFinish(float timedelay = -1)
        {
            m_stateFinishTime = BattleHelp.timePass + timedelay;
        }

        //状态剩余时间
        public float stateTimeLeft { get { return m_stateFinishTime - BattleHelp.timePass; } }

        public float stateTimePass { get { return BattleHelp.timePass - m_stateBeginTime; } }


        //切换被击状态
        public void ChangeHitState(StateType st, object para = null, float timeLen = -1)
        {
            if (StateChangeRule.IsCanChange(m_curStType, st))
                ChangeState(st, para, timeLen);
        }

        public void SetFastRun(bool run)
        {
            if (isFastRun != run)
            {
                if (BattleHelp.IsRunBattleLogic())
                {
                    if(run)
                        m_obj.battle.m_attrLogic.SetSpeed(m_obj.cfgInfo.speed * kvBattle.fastRunSpeedMultiple);
                    else
                        m_obj.battle.m_attrLogic.SetSpeed(m_obj.cfgInfo.speed);
                }

                //先后顺序不能换
                isFastRun = run;
                OnSetFastRun(run);
            }
        }

        protected virtual void OnSetFastRun(bool run)
        {

        }

        //切换指定状态
        public void ChangeState(StateType st, object para = null, float timeLen = -1)
        {
            switch (m_curStType)
            {
                //部分状态,不能自己切换自己
                case StateType.Idle:
                case StateType.Move:
                case StateType.Jump:
                    if (st == m_curStType)
                        return;
                    break;
                //当前是死亡状态不能再切换
                case StateType.Dead:
                    return;
            }

            if (timeLen <= 0)
                m_stateFinishTime = -1;
            else
                m_stateFinishTime = BattleHelp.timePass + timeLen;

            m_stateBeginTime = BattleHelp.timePass;
            StateType lastState = m_curStType;
            if (m_curSt != null)
                m_curSt.OnExit(m_obj, st);
            m_curStType = st;
            m_curSt = GetState(st);
            if (m_curSt != null)
                m_curSt.OnEnter(m_obj, lastState, para);
            OnChangeState(lastState, st);

            int addFlg = 0;
            //状态flg
            if (!StateChangeRule.IsCanMoveOrSkill(lastState))
                addFlg--;
            if (!StateChangeRule.IsCanMoveOrSkill(st))
                addFlg++;
            if (addFlg < 0)
            {
                m_obj.battle.m_buffMgr.RemoveFlag(BuffManager.Flag.NoMove);
                m_obj.battle.m_buffMgr.RemoveFlag(BuffManager.Flag.NoSkill);
                m_obj.battle.m_ai.PauseAI(false);
            }
            else if (addFlg > 0)
            {
                m_obj.battle.m_buffMgr.AddFlag(BuffManager.Flag.NoMove);
                m_obj.battle.m_buffMgr.AddFlag(BuffManager.Flag.NoSkill);
                m_obj.battle.m_ai.PauseAI(true);

            }
        }

        protected virtual void OnChangeState(StateType oldState, StateType newState)
        {

        }


        public IState GetState(StateType type)
        {
            IState st;
            if (!m_states.TryGetValue(type, out st))
            {
                st = CreateState(type);
                m_states.Add(type, st);
            }
            return st;
        }
    }
}

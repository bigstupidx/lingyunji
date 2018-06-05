using GameServer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class AIManager : IBattleComponent, IBattleUpdate
    {
        CheckInterval m_interval = new CheckInterval();
        protected ObjectBase m_obj;
        Dictionary<SimpleAIType, ISimpleAI> m_ais = new Dictionary<SimpleAIType, ISimpleAI>();
        protected ISimpleAI m_cuiai;

        public SimpleAIType m_curAiType { get; private set; }

        //ai暂停
        int m_pauseAI;

        protected SimpleAIType m_defaultIdleAI;
        protected object m_defaultIdleAIPara;

        protected SimpleAIType m_defaultBattleAI;
        protected object m_defaultBattleAIPara;

        public void OnAwake(IObject obj)
        {
            m_obj = obj as ObjectBase;
        }
        public virtual void OnDestroy()
        {
            if (m_cuiai != null)
                m_cuiai.OnExit();
            foreach (var p in m_ais)
                p.Value.OnDestroy();
            m_ais.Clear();
            m_obj = null;
        }

        public virtual void OnStart()
        { }

        //切换默认休闲ai
        public void ChangeIdleAI()
        {
            m_obj.battle.m_ai.ChangeAI(m_defaultIdleAI, m_defaultIdleAIPara);
        }

        //设置默认休闲ai
        public void SetIdleDefaultAI(SimpleAIType type, object para = null, bool change = false)
        {
            m_defaultIdleAI = type;
            m_defaultIdleAIPara = para;
            if (change)
                ChangeIdleAI();
        }

        //如果有进入战斗动画就先播放动画
        public void EnterBattleAI()
        {
            if (BattleHelp.IsRunBattleLogic() && !string.IsNullOrEmpty(m_obj.battle.m_attrMgr.postureCfg.enterBattle))
            {
                m_obj.battle.m_attrLogic.SetState(ObjectState.EnterBattle);
                ChangeAI(SimpleAIType.PlayAni, new PlayAniAI.Info() { bati = true, backToIdleAI = false, time = m_obj.cfgInfo.GetAniTime(m_obj.battle.m_attrMgr.postureCfg.enterBattle) });
            }
            else
                ChangeBattleAI();
        }

        //默认战斗ai
        public void ChangeBattleAI()
        {
            ChangeAI(m_defaultBattleAI, m_defaultBattleAIPara);
        }

        //设置默认战斗ai
        public void SetBattleDefaultAI(SimpleAIType type, object para = null, bool change = false)
        {
            m_defaultBattleAI = type;
            m_defaultBattleAIPara = para;
            if (change)
                ChangeBattleAI();
        }

        public virtual void OnEnterScene()
        {
        }
        public void OnExitScene() { }

        //切换ai
        public void ChangeAI(SimpleAIType aiid, object para = null)
        {
            if (m_cuiai != null)
                m_cuiai.OnExit();

            if (!m_ais.TryGetValue(aiid, out m_cuiai))
            {
                m_cuiai = SimpleAICreate.CreateSimpleAI(aiid);
                m_cuiai.OnCreate(m_obj);
            }

            if (m_cuiai != null)
                m_cuiai.OnEnter(para);
        }

        //暂停AI
        public void PauseAI(bool pause)
        {
            if (pause)
            {
                if (m_pauseAI++ == 0 && m_cuiai != null)
                    m_cuiai.OnPause(pause);
            }
            else
            {
                if (m_pauseAI-- == 1 && m_cuiai != null)
                    m_cuiai.OnPause(pause);
            }
        }

        public virtual void OnUpdate()
        {
#if COM_SERVER
            if (!m_interval.Check(0.5f))
                return;
#endif
            if (m_pauseAI == 0 && m_cuiai != null)
                m_cuiai.OnUpdate();
            m_obj.battle.m_stateMgr.m_curSt.OnUpdate(m_obj);
            OnHandleDefaltState();
        }

        //处理一些常规状态
        void OnHandleDefaltState()
        {
            if (m_obj.battle.m_stateMgr.IsStateFinish())
            {
                switch (m_obj.battle.m_stateMgr.m_curStType)
                {
                    case StateType.Skill:
                        m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                        break;
                    //浮空结束进入倒地
                    case StateType.Float:
                        m_obj.battle.m_stateMgr.ChangeState(StateType.KnockDown, null, AniConst.FloatToGroundStayTime);
                        break;
                    //死亡
                    case StateType.Dead:
                        break;
                    case StateType.Empty:
                        break;
                    default:
                        m_obj.battle.m_stateMgr.ChangeState(StateType.Idle, null, -1);
                        break;
                }
            }
        }
    }
}

using NetProto;
using xys.battle;
using System;
using System.Collections.Generic;
using UnityEngine;
using Config;


namespace xys.battle
{
    class LocalObjectSysPos
    {
        IObject         m_obj;
        StateType       m_lastStType = StateType.Empty;
        CheckInterval   m_interval = new CheckInterval();

        public LocalObjectSysPos(IObject obj)
        {
            m_obj = obj;
        }

        public void Clear()
        {
            m_obj = null;
        }

        public void OnUpdate( )
        {
            //状态特殊处理
            StateType curStType = m_obj.battle.m_stateMgr.m_curStType;
            switch (m_lastStType)
            {
                case StateType.Move:

                    //移动一段时间进入快跑
                    if (curStType == m_lastStType)
                    {
                        //非战斗模式进入快跑
                        if (m_obj.battle.m_stateMgr.stateTimePass > kvBattle.intoFastrunTime 
                            && !m_obj.battle.m_attrMgr.battleState 
                            && !m_obj.battle.m_stateMgr.isFastRun
                            && m_obj.type == ObjectType.Player
                            )
                            m_obj.battle.m_stateMgr.SetFastRun(true);
                        SysPos(m_lastStType);
                    }
                    //切换待机就广播停止
                    else if(curStType == StateType.Idle)
                    {
                        SysPos(m_lastStType, true);
                    }                       
                    break;
                //轻功
                case StateType.Jump:
                    SysPosJump();
                    break;
                //技能移动
                case StateType.Skill:                                            
                    if (m_obj.battle.m_skillMgr.IsPlaying())
                    {
                        SkillAniConfig.AniType aniType = m_obj.battle.m_skillMgr.GetAniType();
                        if (aniType == Config.SkillAniConfig.AniType.CastingContinueCanMove)
                            SysPos(m_lastStType);
                    }
                    break;

                default:
                    break;
            }
            m_lastStType = curStType;
        }

        void SysPosJump()
        {
            //正在等待技能返回的时候不要同步坐标
            if (((SkillManagerBase)m_obj.battle.m_skillMgr).m_requestSkill)
                return;

            if (!m_interval.Check(kvBattle.clientSysPosInterval))
                return;
            App.my.battleProtocol.Request_JumpSendPos(m_obj);
        }

        void SysPos(StateType stType, bool isStop = false)
        {
            //正在等待技能返回的时候不要同步坐标
            if (((SkillManagerBase)m_obj.battle.m_skillMgr).m_requestSkill)
                return;

            //停止已经立刻上传
            if (!isStop)
            {
                if (!m_interval.Check(kvBattle.clientSysPosInterval))
                    return;
            }

            App.my.battleProtocol.Request_MoveSendSys(m_obj, stType, isStop);
        }

    }
}

 

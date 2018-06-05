using Config;
using NetProto;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace xys.battle
{
    public class StateManagerBase:StateManager
    {
        protected override IState CreateState(StateType type)
        {
            switch (type)
            {
                case StateType.Idle: return new IdleState();
                case StateType.Move: return new MoveState();
                case StateType.Weak: return new WeakState();
                case StateType.BeHit: return new BeHitState();
                case StateType.Float: return new FloatState();
                case StateType.KnockDown: return new KnockDownState();
                case StateType.Skill: return new SkillState();
                case StateType.BeatBack: return new BeatBackState();
                case StateType.Dead: return new DeadState();
                case StateType.Dizzy: return new DizzyState();
                case StateType.Jump: return new JumpState();
                case StateType.Sing: return new SingState();
                case StateType.PlayAni: return new PlayAniState();
                default:
                    return new IState();
            }
        }

        protected override void OnChangeState(StateType oldState, StateType newState)
        {
            m_obj.eventSet.FireEvent(ObjEventID.ChangeState, newState);

            //本地玩家目标状态切换了，技能要重新选取
            if (m_obj == App.my.localPlayer.battle.GetTarget())
                ((SkillManagerLocal)App.my.localPlayer.battle.m_skillMgr).OnTargetChangeState(newState);
        }


        protected override void OnSetFastRun(bool run)
        {
            if (m_obj.battle.m_isAiByLocal)
                App.my.battleProtocol.Request_FastRun(m_obj,run);

            if(run)
                m_obj.battle.m_aniMgr.PlayStateAni(m_obj.battle.m_attrMgr.postureCfg.normalRunToFastRun);

            if ((m_obj is LocalPlayer))
            {
                float tempSpeed = Mathf.Abs((kvBattle.fastRunFov - App.my.cameraMgr.m_defaultFov) / 0.5f);
                float toFov = run ? kvBattle.fastRunFov : App.my.cameraMgr.m_defaultFov;
                App.my.cameraMgr.ChangeCameraFov(toFov, tempSpeed, 0);

                ((SkillManagerLocal)m_obj.battle.m_skillMgr).Switch_CheckAllSkill();
            }
        }
    }
}

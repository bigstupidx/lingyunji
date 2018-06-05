using NetProto;
using xys.battle;
using System;
using System.Collections.Generic;
using UnityEngine;
using Config;


namespace xys.battle
{

    class LocalPlayerInputAI : ISimpleAI
    {
        public override void OnUpdate()
        {
            bool moveInput = App.my.input.IsMoving();

            //状态特殊处理
            StateType curStType = m_obj.battle.m_stateMgr.m_curStType;


            //跟随目标或者寻路，摇杆能打断
            switch (m_obj.battle.m_moveMgr.GetMoveState())
            {
                case MoveManagerBase.MoveState.MoveToTarget:
                case MoveManagerBase.MoveState.PathMove:
                    if (moveInput && App.my.input.IsReInput(InputManager.InputFlg.MoveToSkillFlg))
                        m_obj.battle.m_moveMgr.StopMove();
                    return;
                default: break;

            }

            switch (curStType)
            {
                case StateType.Idle:
                    if (moveInput)
                        m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
                    return;
                case StateType.Move:
                    //摇杆移动
                    if (moveInput && m_obj.battle.IsCanMove())
                    {
                        float speed = m_obj.battle.speed;
                        ((MoveManagerLocal)m_obj.battle.m_moveMgr).JoystickMove(App.my.input.GetMoveWay() *speed);
                    }       
                    else
                        m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                    return;
                case StateType.Sing:
                    //摇杆移动
                    if (moveInput)
                        m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
                    break;
                //部分技能可以被移动中断
                case StateType.Skill:
                    //如果停止了技能，则有可能需要把当前槽的技能切换一下
                    if (moveInput && m_obj.battle.IsCanMove())
                    {
                        if (m_obj.battle.m_skillMgr.IsPlaying())
                        {
                            SkillAniConfig.AniType aniType = m_obj.battle.m_skillMgr.GetAniType();
                            //施法动作,持续施法 取消技能需要服务器反应
                            if (aniType == Config.SkillAniConfig.AniType.Casting || aniType == SkillAniConfig.AniType.CastingContinue)
                            {
                                //需要重新按摇杆
                                if (App.my.input.IsReInput(InputManager.InputFlg.SkillFlg))
                                {
                                    App.my.battleProtocol.Request_StopSkill(m_obj);
                                }
                            }
                            //技能移动
                            else if (aniType == Config.SkillAniConfig.AniType.CastingContinueCanMove)
                            {
                                ((MoveManagerLocal)m_obj.battle.m_moveMgr).JoystickMove(App.my.input.GetMoveWay() * m_obj.battle.speed);
                            }
                            //收招可以移动
                            else if (aniType == Config.SkillAniConfig.AniType.AttackAfter)
                            {
                                m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

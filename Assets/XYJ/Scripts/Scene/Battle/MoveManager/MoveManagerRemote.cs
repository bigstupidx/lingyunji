using xys.battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    /// <summary>
    /// 远程对象发送移动目标点
    /// </summary>
    public partial class MoveManagerBase
    {
        //距离小于这个则不移动
        const float NoMoveDistance = 0.5f;

        //两个topos的距离
        float m_remoteDistanceInToPos;

        //继续移动距离
        float m_continueMoveDistance;
        Vector3 m_toPos;
        //上次同步过来的点
        Vector3 m_remoteLastToPos;

        //远程对象移动
        public void Remote_SetMoveToPos(Vector3 toPos)
        {
            float dis = BattleHelp.GetDistance(toPos, m_obj.position);

            Vector3 lastMovePos = m_remoteLastToPos;
            m_remoteLastToPos = toPos;

            //由于移动会有移动预测，所以当前发送的目标点比上个发送的目标点更接近当前客户端坐标，则不要移动了
            if (m_state == MoveState.Remote_PosContinuous)
            {
                if (dis <= BattleHelp.GetDistance(m_obj.position, m_toPos))
                    return;
            }

            //两个topos的距离
            m_remoteDistanceInToPos = BattleHelp.GetDistance(toPos, lastMovePos);

            //不要往下走，不然会穿到地面
            if (toPos.y < m_obj.position.y)
                toPos.y = m_obj.position.y;

            //设置贴地
            BattleHelp.GetGroundPos(ref toPos);
            m_toPos = toPos;

            ChangeState(MoveState.Remote_MoveToPos);
            m_moveLine.BeginMoveBySpeed(m_obj.position, toPos, m_obj.battle.speed);
            BattleHelp.SetLookAt(m_obj, toPos);
        }


        //移动
        public void Remote_SetMoveStop(Vector3 topos)
        {
            float dis = BattleHelp.GetDistance(m_obj.position, topos);
            //如果距离很近，则不用移动了
            if (dis < NoMoveDistance)
            {
                Remote_ChangeObjectState();
                StopMove();
                return;
            }

            ChangeState(MoveState.Remote_MoveToStop);
            m_moveLine.BeginMoveBySpeed(m_obj.position, topos, m_obj.battle.speed);
            BattleHelp.SetLookAt(m_obj, topos);
        }

        //移动结束的时候会切换角色状态
        void Remote_ChangeObjectState()
        {
            switch (m_obj.battle.m_stateMgr.m_curStType)
            {
                //正常接待机
                case StateType.Move:
                    m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                    break;
                //技能则不切换动作
                case StateType.Skill:
                    break;
                default:
                    break;
            }
        }



        void Remote_OnUpdate( Vector3 move)
        {
            if(m_state == MoveState.Remote_MoveToPos)
            {
                //修改移动速度
                float mul;
                float speed;
                if (m_remoteDistanceInToPos == 0)
                    mul = 1;
                else
                    mul = m_moveLine.m_moveLenght/m_remoteDistanceInToPos;
                //需要加速
                if (mul > kvBattle.clientSysPosSpeedUpMul)
                    mul = kvBattle.clientSysPosSpeedUpMul;
                else if (mul < kvBattle.clientSysPosSpeedDownByDistance)
                {
                    mul/=kvBattle.clientSysPosSpeedDownByDistance;
                    if (mul < kvBattle.clientSysPosSpeedDownMul)
                        mul = kvBattle.clientSysPosSpeedDownMul;
                }

                speed = m_obj.battle.speed * mul;
                if (speed < 0)
                    speed = 0;
                m_moveLine.SetMoveSpeed(speed);
            }
            //移动预测时快速减速
            else if (m_state == MoveState.Remote_PosContinuous)
            {
                float mul = m_moveLine.m_moveLenght / m_continueMoveDistance;
                float speed = kvBattle.clientSysPosSpeedDownMul * mul * mul;
                m_moveLine.SetMoveSpeed(speed);
                m_moveLine.SetMoveSpeed(0);

            }

            m_obj.SetPosition(m_obj.position + move);
            //目前后续移动目标点没有设置贴地会导致不贴地,所以每帧都设置贴地，后面可以考虑优化
            if (move != Vector3.zero)
                BattleHelp.SetGround(m_obj);
        }


        void Remote_FinishMove()
        {
            switch (m_state)
            {
                //移动到目标点再按原方向继续移动一小段时间
                case MoveState.Remote_MoveToPos:
                    m_continueMoveDistance = kvBattle.clientSysPosMaxOff - 0.1f;
                    m_moveLine.MoveContinue(m_continueMoveDistance);
                    ChangeState(MoveState.Remote_PosContinuous);
                    return;
                case MoveState.Remote_MoveToStop:
                case MoveState.Remote_PosContinuous:
                    Remote_ChangeObjectState();
                    break;
                default:
                    break;
            }
            StopMove();
        }
    }
}
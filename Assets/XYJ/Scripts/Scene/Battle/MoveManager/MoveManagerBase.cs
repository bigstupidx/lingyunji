using xys.battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    public partial class MoveManagerBase : IBattleComponent, IBattleUpdate
    {
        public enum MoveState
        {
            Stop,
            MoveToPos,          //设置路径,移到目标点结束
            ForceMove,          //强制移动

            //本地玩家使用
            MoveToTarget,       //跟随对象
            PathMove,           //寻路

            //远程对象使用
            Remote_MoveToPos,   //移动到目标点
            Remote_MoveToStop,  //移动到目的地就转到休闲待机
            Remote_PosContinuous,
        };


        protected MoveState m_state;
        protected IObject m_obj;

        protected LineMoveHelp m_moveLine = new LineMoveHelp();
        MoveToTargetHelp m_moveToTarget = new MoveToTargetHelp();
        PathMoveHelp m_pathMove = new PathMoveHelp();

        Action<object> m_finishAction;
        object m_para;
        //强制移动，例如击退
        public void SetForceMove(Vector3 fromPos, Vector3 toPos, float moveLenght, float moveSpeed)
        {
            ChangeState(MoveState.ForceMove);
            m_moveLine.BeginMove(fromPos,toPos, moveLenght, moveSpeed);
        }

        //强制移动,例如冲锋
        public void PlayForceMovePos(Vector3 toPos, float time)
        {
            //距离很短就不用移动了
            if (BattleHelp.GetDistance(m_obj.position, toPos) <= 0.01f)
                return;
            ChangeState(MoveState.ForceMove); 
            m_moveLine.BeginMoveByTime(m_obj.position, toPos, time);
        }

        public MoveState GetMoveState()
        {
            return m_state;
        }

        public void StopMove()
        {
            if (m_obj == null)
                return;
            m_state = MoveState.Stop;
            m_finishAction = null;
            m_para = null;
            m_moveToTarget.Stop();

            m_obj.battle.m_stateMgr.SetFastRun(false);
        }


        //移动到指定位置
        public void SetMoveToPos(Vector3 toPos,Action<object> action = null, object para = null,float speed = 0)
        {
            ChangeState(MoveState.MoveToPos);
            m_finishAction = action;
            m_para = para;
            if (speed == 0)
                speed = m_obj.battle.speed;
            m_moveLine.BeginMoveBySpeed(m_obj.position, toPos, speed);
            BattleHelp.SetLookAt(m_obj, toPos);
        }


        public void SetMoveByPath(Vector3 toPos, Action<object> action = null, object para = null, float speed = 0)
        {
            ChangeState(MoveState.PathMove); 
            m_finishAction = action;
            m_para = para;
            if (speed == 0)
                speed = m_obj.battle.speed;
            m_pathMove.StartPath(m_obj.position, toPos, speed);
        }

        public void SetMoveToTarget(IObject target,float finishDistance, Action<object> action = null, object para = null,float speed=0)
        {
            if (speed == 0)
                speed = m_obj.battle.speed;
            ChangeState(MoveState.MoveToTarget);
            m_moveToTarget.PlayMove(target,speed, finishDistance);
            m_finishAction = action;
            m_para = para;
        }


        public bool IsFinish()
        {
            return m_state == MoveState.Stop;
        }

        protected void ChangeState( MoveState state )
        {
            m_state = state;
        }

        public void OnAwake(IObject obj)
        {
            m_obj = obj;
        }
        public void OnDestroy()
        {
            m_obj = null;
        }
        public void OnEnterScene()
        {

        }
        public void OnExitScene()
        {
            StopMove();
        }

        public void OnStart() { }


        public void OnUpdate()
        {
            Vector3 move;
            bool finish = false;
            if (m_state == MoveState.Stop)
                return;

            float deltaTime = Time.deltaTime;

            //开始移动
            if(m_state == MoveState.MoveToTarget)
            {
                move = m_moveToTarget.UpdateMove(m_obj.position,deltaTime);
                BattleHelp.SetLookAt(m_obj, m_obj.position + move);
                if (m_moveToTarget.IsFinish())
                    finish = true;
            }
            //寻路
            else if(m_state == MoveState.PathMove)
            {
                move = m_pathMove.UpdateMove(m_obj,deltaTime);
                //寻路是异步，失败直接返回，不执行回调
                if (m_pathMove.m_state == PathMoveHelp.State.PathFail)
                {
                    StopMove();
                    return;
                }
                else if (m_pathMove.m_state == PathMoveHelp.State.Stop)
                    finish = true;
                    
            }
            //直线移动
            else
            {
                move = m_moveLine.UpdateMove(deltaTime);
                if (m_moveLine.IsFinish())
                    finish = true;
            }
            //本地移动
            if(m_obj.battle.m_isAiByLocal)
            {
                m_obj.battle.actor.CCMove(move);
            }
            //远程对象移动
            else
            {
                Remote_OnUpdate(move);
            }


            if (finish)
            {
                if (m_finishAction != null)
                    m_finishAction(m_para);

                if (m_obj.battle.m_isAiByLocal)
                    StopMove();
                else
                    Remote_FinishMove();
            }
        }
    }
}
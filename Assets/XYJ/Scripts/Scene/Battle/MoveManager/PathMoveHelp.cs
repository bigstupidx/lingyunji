using Config;
using GameServer;
using NetProto;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class PathMoveHelp
    {
        public enum State
        {
            Stop,
            WaitPos,
            Moving,
            PathFail,
        }

        //移动速度
        public float m_moveSpeed { get; private set; }
        public State m_state { get; private set; }
        List<Vector3> m_posList;
        int m_curPosId;

        public void StartPath(Vector3 fromPos, Vector3 toPos,float moveSpeed)
        {
            m_moveSpeed = moveSpeed;
            m_state = State.WaitPos;
            AstarPath.StartPath(fromPos, toPos, BeginMove);
        }

        void BeginMove(List<Vector3> list)
        {
            if(list == null)
            {
                m_state = State.PathFail;
            }
            else
            {
                m_posList = list;
                m_curPosId = 0;
                m_state = State.Moving;
            }
        }

        //返回true表示移动结束
        bool MoveToNextPos( IObject obj)
        {
            if(m_curPosId>=m_posList.Count-1)
            {
                m_state = State.Stop;
                return true;
            }

            Vector3 fromPos = m_posList[m_curPosId];
            Vector3 toPos = m_posList[m_curPosId+1];
            m_curPosId++;
            obj.SetRotate(BattleHelp.Vector2Angle(toPos, fromPos));
            return false;
        }


        public Vector3 UpdateMove( IObject obj, float deltaTime)
        {
            if (m_state == State.Moving)
            {
                //刚开始移动
                if(m_curPosId == 0)
                    MoveToNextPos(obj);
                
                float movelen = m_moveSpeed * deltaTime;
                Vector3 toPos = m_posList[m_curPosId];
                Vector3 moveWay = ( toPos - obj.position).normalized;
                float dis = BattleHelp.GetDistance(toPos, obj.position);
                if (dis < movelen)
                {
                    movelen = dis;
                    //整条路径结束
                    if (m_curPosId >= m_posList.Count - 1)
                    {
                        m_state = State.Stop;
                    }
                    //还有路点可以走
                    else
                    {
                        MoveToNextPos(obj);
                    }
                }

                return moveWay * movelen;
            }
            else
                return Vector3.zero;
        }

        public void SetMoveSpeed(float speed)
        {
            m_moveSpeed = speed;
        }

    }
}


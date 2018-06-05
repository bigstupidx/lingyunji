using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{

    class TestMoveAI : ISimpleAI
    {
        Vector3 m_initPos;
        Vector3 m_toPos;
        public override void OnCreate(IObject obj)
        {
            m_obj = obj;

            m_initPos = obj.position;
            m_toPos = m_initPos + BattleHelp.RotateAngle(new Vector3(0, 0, 10), BattleHelp.Rand(0, 360));
            RandomMove();
        }

        void RandomMove()
        {
            Vector3 toPos;
            float speed;
            if (BattleHelp.GetDistance(m_obj.position, m_toPos) < 1)
            {
                speed = 1;
                toPos = m_initPos;
            }
            else
            {
                speed = 10;
                toPos = m_toPos;
            }

            m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
            m_obj.battle.m_moveMgr.SetMoveToPos(toPos, OnFinish,null, speed);
        }

        void OnFinish(object t)
        {
            //Logger.LogError("move finish ");
        }

        //由AI来决定下个状态
        public override void OnUpdate()
        {
            switch (m_obj.battle.m_stateMgr.m_curStType)
            {
                case StateType.Idle:
                    RandomMove();
                    break;
                case StateType.Move:
                    if (m_obj.battle.m_moveMgr.IsFinish())
                    {
                        //返回待机
                        //m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                        RandomMove();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class ObjectStateMove : IObjectState
    {
        Vector3 m_targetPos = Vector3.zero;
        float m_moveSpeed = 0.0f;

        bool Move2Target(StoryObjectBase obj)
        {
            Vector3 direction = m_targetPos - obj.ComHandler.position;
            if (direction == Vector3.zero)
                return true;
            direction.y = 0;
            direction.Normalize();
            obj.ComHandler.m_move.LookDirection(direction);
            Vector3 motion = direction * m_moveSpeed * Time.deltaTime + Vector3.down * m_moveSpeed * Time.deltaTime;
            obj.ComHandler.m_move.CCMove(motion);
            return false;
        }

        public override bool OnEnter(StoryObjectBase obj, StoryObjectState lastState, object para)
        {
            object[] ps = (object[])para;
            if (ps!=null && ps.Length == 2)
            {
                m_targetPos = (Vector3)ps[0];
                m_moveSpeed = (float)ps[1];
                if (!Move2Target(obj))
                {
                    obj.ComHandler.m_model.PlayStateAni();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override void OnExit(StoryObjectBase obj, StoryObjectState nextState)
        {

        }

        public override bool OnUpdate(StoryObjectBase obj)
        {
            Vector3 dis = m_targetPos - obj.ComHandler.position;
            dis.y = 0;

            // 判断是否到达目的地
            if (dis.magnitude < m_moveSpeed * Time.deltaTime)
            {
                Move2Target(obj);
                return false;
            }

            Move2Target(obj);
            return true;
        }

    }
}

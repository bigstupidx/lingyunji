using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class ObjectStateAnim : IObjectState
    {
        public class Cxt
        {
            public string name;
            public float speed = 1.0f;
            public bool isLoop = false;
            public float timeLen = -1.0f;
        }

        float m_lastPlayTime = 0.0f;
        float m_playTimeLen = 0.0f;

        public override bool OnEnter(StoryObjectBase obj, StoryObjectState lastState, object para)
        {
            Cxt cxt = (Cxt)para;
            obj.ComHandler.m_model.PlayAnim(cxt.name, cxt.speed, 0, cxt.isLoop);

            m_lastPlayTime = Time.timeSinceLevelLoad;
            if (cxt.timeLen < 0)
            {
                if (cxt.timeLen == -1)
                    m_playTimeLen = obj.ComHandler.m_model.GetAnimLength(cxt.name) / cxt.speed;
                else
                    m_playTimeLen = -1;
            }
            else
            {
                // 指定时间
                m_playTimeLen = cxt.timeLen;
            }
            return true;
        }

        public override void OnExit(StoryObjectBase obj, StoryObjectState nextState)
        {

        }

        public override bool OnUpdate(StoryObjectBase obj)
        {
            if (m_playTimeLen == -1)
                return true;

            if (Time.timeSinceLevelLoad >= (m_lastPlayTime + m_playTimeLen))
                return false;
            else
                return true;
        }
    }
}

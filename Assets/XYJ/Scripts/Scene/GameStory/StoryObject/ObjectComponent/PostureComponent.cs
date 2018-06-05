using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class PostureComponent : IObjectComponent
    {

        StoryObjectBase m_obj;

        public int posture { get; private set; }
        //姿态信息
        public Config.PostureConfig m_cfg { get; private set; }

        public void SetPosture(int posture)
        {
            if (this.posture != posture || m_cfg == null)
            {
                m_cfg = Config.PostureConfig.Get(posture);
                if (m_cfg == null)
                    m_cfg = Config.PostureConfig.Get(1);
                this.posture = posture;
            }
        }

        public void OnAwake(StoryObjectBase obj)
        {
            m_obj = obj;
            SetPosture(0);
        }

        public void OnStart()
        {

        }

        public void OnDestroy()
        {
            m_cfg = null;
        }

        public void OnUpdate()
        {

        }

    }

}

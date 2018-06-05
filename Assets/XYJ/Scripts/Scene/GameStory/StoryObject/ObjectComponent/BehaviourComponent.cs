using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{

    public abstract class IObjectBehaviour
    {
        protected StoryObjectBase m_obj;
        protected object param;

        public abstract void DoAction();
    }

    public class ObjectPlayAnim : IObjectBehaviour
    {
        public override void DoAction()
        {
            

        }
    }

    /// <summary>
    /// 主要用来实现随机行为
    /// </summary>
    public class BehaviourComponent : IObjectComponent
    {


        public void SetRandomBehaviours()
        {

        }


        StoryObjectBase m_obj;
        public void OnAwake(StoryObjectBase obj)
        {
            m_obj = obj;

            
        }

        public void OnStart()
        {
            
        }

        public void OnDestroy()
        {
            
        }

        public void OnUpdate()
        {
            
        }
    }
}
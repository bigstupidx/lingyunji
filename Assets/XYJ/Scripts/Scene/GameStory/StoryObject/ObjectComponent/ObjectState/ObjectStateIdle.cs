using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class ObjectStateIdle : IObjectState
    {

        public override bool OnEnter(StoryObjectBase obj, StoryObjectState lastState, object para)
        {
            obj.ComHandler.m_model.PlayStateAni();
            return true;
        }

        public override void OnExit(StoryObjectBase obj, StoryObjectState nextState)
        {

        }

        public override bool OnUpdate(StoryObjectBase obj)
        {
            return true;
        }

    }

}

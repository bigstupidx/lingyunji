using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class ObjectStatePath : IObjectState
    {

        public override bool OnEnter(StoryObjectBase obj, StoryObjectState lastState, object para)
        {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public abstract class IObjectState
    {
        public abstract bool OnEnter(StoryObjectBase obj, StoryObjectState lastState, object para);
        public abstract void OnExit(StoryObjectBase obj, StoryObjectState nextState);
        public abstract bool OnUpdate(StoryObjectBase obj);
    }

    /// <summary>
    /// 角色基本状态，不能同时存在两种状态
    /// </summary>
    public enum StoryObjectState
    {
        Empty = 0,//起止状态，该状态下，角色不可控
        Idle,
        Anim,
        Move,
        Path,
    }

    /// <summary>
    /// 角色的控制状态
    /// </summary>
    public class StateComponent : IObjectComponent
    {
        StoryObjectBase m_obj;

        Dictionary<StoryObjectState, IObjectState> m_objectStateCache = new Dictionary<StoryObjectState, IObjectState>();

        StoryObjectState m_curState = StoryObjectState.Empty;
        IObjectState m_objectState = null;

        public StoryObjectState CurState
        {
            get { return m_curState; }
        }

        public bool ChangeState(StoryObjectState state, object para = null)
        {
            if (m_objectState != null)
                m_objectState.OnExit(m_obj, state);
             
            if (state == StoryObjectState.Empty)
            {
                m_curState = StoryObjectState.Empty;
                m_objectState = null;
                return true;
            }

            m_objectState = GetObjectState(state);
            if (m_objectState != null)
            {
                StoryObjectState lastState = m_curState;
                m_curState = state;
                if (!m_objectState.OnEnter(m_obj, lastState, para))
                {
                    ChangeState(StoryObjectState.Idle);
                    return false;
                }
                return true;
            }
            return false;
        }

        IObjectState GetObjectState(StoryObjectState state)
        {
            IObjectState objectState;
            if (!m_objectStateCache.TryGetValue(state, out objectState))
            {
                objectState = CreateObjectState(state);
                m_objectStateCache.Add(state, objectState);
            }
            return objectState;
        }

        IObjectState CreateObjectState(StoryObjectState state)
        {
            IObjectState objectState = null;
            switch (state)
            {
                case StoryObjectState.Anim:
                    objectState = new ObjectStateAnim();
                    break;
                case StoryObjectState.Move:
                    objectState = new ObjectStateMove();
                    break;
                case StoryObjectState.Path:
                    objectState = new ObjectStatePath();
                    break;
                default:
                    objectState = new ObjectStateIdle();
                    break;
            }
            return objectState;
        }

        public void OnAwake(StoryObjectBase obj)
        {
            m_obj = obj;

            m_curState = StoryObjectState.Empty;
            m_objectState = null;
        }

        public void OnStart()
        {
            ChangeState(StoryObjectState.Idle);
        }

        public void OnDestroy()
        {
            ChangeState(StoryObjectState.Empty);

            m_obj = null;
        }

        public void OnUpdate()
        {
            if (m_objectState != null)
            {
                if (!m_objectState.OnUpdate(m_obj))
                    ChangeState(StoryObjectState.Idle);
            }
        }

    }

}
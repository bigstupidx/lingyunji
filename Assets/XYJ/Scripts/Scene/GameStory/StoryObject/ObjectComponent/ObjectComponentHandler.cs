using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public interface IObjectComponent
    {
        void OnAwake(StoryObjectBase obj);
        void OnStart();
        void OnDestroy();
        void OnUpdate();
    }

    public class ObjectComponentHandler
    {
        StoryObjectBase m_obj;
        List<IObjectComponent> m_components = new List<IObjectComponent>();

        public ModelComponent m_model;
        public StateComponent m_state;
        public MoveComponent m_move;
        public PostureComponent m_posture;

        

        public Transform rootObject
        {
            get { return m_move.rootObject; }
        }

        public Vector3 position
        {
            get { return rootObject.position; }
        }

        T AddCompoment<T>(T component) where T : IObjectComponent
        {
            m_components.Add(component);
            return component;
        }

        public void Init(StoryObjectBase obj)
        {
            m_obj = obj;

            m_model = AddCompoment(new ModelComponent());
            m_state = AddCompoment(new StateComponent());
            m_move = AddCompoment(new MoveComponent());
            m_posture = AddCompoment(new PostureComponent());

            for (int i = 0; i < m_components.Count; ++i)
            {
                m_components[i].OnAwake(obj);
            }
        }

        /// <summary>
        /// 播放剧情角色行为
        /// </summary>
        public void PlayBehaviour(StoryEventType type, object param)
        {
            object[] objs = (object[])param;
            switch (type)
            {
                case StoryEventType.角色动作:
                    m_state.ChangeState(StoryObjectState.Anim, param);
                    break;
                case StoryEventType.角色冒泡:
                    m_model.m_handler.ShowBubbling((string)objs[0], (float)objs[1]);
                    break;
                case StoryEventType.角色随机冒泡:
                    m_model.m_handler.SetRandomBubbling((string[])objs[0], (float)objs[1], (float)objs[2]);
                    break;
                case StoryEventType.角色特效:
                    m_model.PlayFx((string)objs[0], (int)objs[1], (string)objs[2]);
                    break;
                case StoryEventType.角色移动:
                    m_state.ChangeState(StoryObjectState.Move, param);
                    break;
                case StoryEventType.角色巡逻:

                    break;
                case StoryEventType.角色转向:
                    m_model.SetAngle((float)param);
                    break;
                default:
                    Debuger.LogError("角色没有实现剧情事件类型："+ type);
                    break;
            }
        }

        // Use this for initialization
        public void Start()
        {

            for (int i = 0; i < m_components.Count; ++i)
            {
                m_components[i].OnStart();
            }
        }

        // Update is called once per frame
        public void Update()
        {
            for (int i = 0; i < m_components.Count; ++i)
            {
                m_components[i].OnUpdate();
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < m_components.Count; ++i)
            {
                m_components[i].OnDestroy();
            }
        }
    }
}
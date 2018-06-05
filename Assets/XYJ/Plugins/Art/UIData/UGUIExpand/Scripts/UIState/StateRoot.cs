using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI.State
{
    public class StateRoot : MonoBehaviour
    {
        public List<Element> elements; // 元素列表

        static StateConfig[] Empty = new StateConfig[0];

        public StateConfig[] States = Empty; // 状态名

        [SerializeField]
        Button uButton;

        // 点击是否切换状态
        public bool isClickSwitchState = false;

        public Button uCurrentButton
        {
            get { return uButton; }
            set
            {
                if(uButton == value)
                    return;

                if(uButton != null)
                {
                    uButton.onClick.RemoveListener(OnButtonClick);
                }

                uButton = value;

#if UNITY_EDITOR
                if(!Application.isPlaying)
                    return;
#endif
                BindButtonEvent();
            }
        }

        void BindButtonEvent()
        {
            if(uButton != null)
            {
                uButton.onClick.AddListener(OnButtonClick);
            }
        }

        void OnButtonClick()
        {
            if(isClickSwitchState)
            {
                if(!NextState())
                {
                    SetState(0);
                }
            }

            onClick.Invoke();
        }

        public bool NextState()
        {
            return SetState(CurrentState + 1);
        }

        public bool FrontState()
        {
            return SetState(CurrentState - 1);
        }

        public void SetNextStateWithLoop(bool isNotify = false)
        {
            if(!SetCurrentState(CurrentState + 1, isNotify))
            {
                SetCurrentState(0, isNotify);
            }
        }

        public void SetFrontStateWithLoop(bool isNotify = false)
        {
            if(!SetCurrentState(CurrentState - 1, isNotify))
            {
                SetCurrentState(States.Length - 1, isNotify);
            }
        }

        public UnityEvent onStateChange = new UnityEvent();
        public UnityEvent onClick = new UnityEvent();

#if UNITY_EDITOR
        public string[] StateNames
        {
            get
            {
                string[] s = new string[States.Length];
                for(int i = 0 ; i < s.Length ; ++i)
                {
                    s[i] = States[i].Name;
                }

                return s;
            }
        }
#endif

        [SerializeField]
        int StateIndex; // 当前状态

        void Awake()
        {
            BindButtonEvent();
            SetState(CurrentState);
        }

        public int CurrentState
        {
            get { return StateIndex; }
            set
            {
                if(CurrentState == value)
                    return;

                SetState(value);
            }
        }

        public string CurrentStateName
        {
            get
            {
                return States[StateIndex].Name;
            }
        }

        public bool SetCurrentState(int value, bool isnotify)
        {
            if(value < 0 || value >= States.Length)
                return false;

            StateIndex = value;
            if(elements != null)
            {
                for(int i = 0 ; i < elements.Count ; ++i)
                {
                    elements[i].Agent.Set(elements[i], value);
                }
            }

            if(isnotify)
                onStateChange.Invoke();
            return true;
        }

        public bool SetCurrentState(string stateName, bool isnotify)
        {
            for(int i = 0 ; i < States.Length ; ++i)
            {
                if(stateName == States[i].Name)
                {
                    StateIndex = i;
                    if(elements != null)
                    {
                        for(int j = 0 ; j < elements.Count ; ++j)
                        {
                            elements[j].Agent.Set(elements[j], i);
                        }
                    }

                    if(isnotify)
                        onStateChange.Invoke();
                    return true;
                }
            }
            return false;
        }

        bool SetState(int value)
        {
            return SetCurrentState(value, true);
        }

#if UNITY_EDITOR
        public void AddElement(Type type)
        {
            int lenght = States.Length;

            Element element = new Element();
            element.type = type;
            element.stateData = new ElementStateData[lenght];
            for(int i = 0 ; i < lenght ; ++i)
            {
                element.stateData[i] = new ElementStateData();
                element.Agent.Init(element, element.stateData[i]);
            }

            if(elements == null)
                elements = new List<Element>();

            elements.Add(element);
        }

        public bool RemoveElement(Element element)
        {
            if(elements == null)
                return false;

            return elements.Remove(element);
        }

        public void AddState()
        {
            int lenght = States.Length;
            System.Array.Resize<StateConfig>(ref States, lenght + 1);

            States[lenght] = new StateConfig();
            States[lenght].Name = lenght.ToString();

            if(elements != null)
            {
                for(int i = 0 ; i < elements.Count ; ++i)
                    elements[i].AddState();
            }
        }

        public void RemoveState(int index)
        {
            XTools.Utility.ArrayRemove(ref States, index);
            for(int i = 0 ; i < elements.Count ; ++i)
            {
                elements[i].RemoveState(index);
            }
        }
#endif
    }
}
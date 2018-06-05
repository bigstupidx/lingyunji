#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{

    [AutoILMono]
    class UIColorBtn
    {
        public enum State
        {
            Null_Color,
            Unlock_Unselected,
            Unlock_Selected,
            Lock_Unselected,
            Lock_Selected,
        }

        [SerializeField]
        Image m_color;
        [SerializeField]
        public StateRoot m_stateRoot;

        public int m_index;
        private System.Action<UIColorBtn> m_clickEvent = null;
        private System.Action<UIColorBtn> m_stateChangeEvent = null;
        void Awake()
        {
            if(m_stateRoot!=null)
            {
                m_stateRoot.onClick.AddListener(OnClick);
                m_stateRoot.onStateChange.AddListener(OnStateChange);
            }
            
        }
        //有颜色
        public void Set(int index,State state,Color color, System.Action<UIColorBtn> clickEvent,
           System.Action<UIColorBtn> stateChangeEvent )
        {
            m_index = index;
            SetState(state, false);
            m_color.color = color;
            m_clickEvent = clickEvent;
            m_stateChangeEvent = stateChangeEvent;
        }
        //无颜色
        public void Set(int index, State state)
        {
            SetState(state, false);
            m_index = index;
            m_clickEvent = null;
            m_stateChangeEvent = null;
        }
        void OnClick()
        {
            if(m_clickEvent != null)
            {
                m_clickEvent(this);
            }
        }
        void OnStateChange()
        {
            if(m_stateChangeEvent != null)
            {
                m_stateChangeEvent(this);
            }
        }
        public void SetState(State state,bool isNotify)
        {
            m_stateRoot.SetCurrentState((int)state, isNotify);
        }
    }
}
#endif
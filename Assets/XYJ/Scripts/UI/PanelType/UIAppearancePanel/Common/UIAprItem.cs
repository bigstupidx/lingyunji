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
    class UIAprItem
    {

        public enum State
        {
            Lock_Unselected = 0,
            Lock_Selected = 1,
            Unlock_Unselected = 2,
            Unlock_Selected = 3,
            OutTime_Unselected = 4,
            OutTime_Selected = 5,
            Load_Unselected = 6,
            Load_Selected = 7,
        }

        [SerializeField]
        public StateRoot m_stateRoot;
        [SerializeField]
        Image m_icon;

        private System.Action<UIAprItem> m_clickEvent = null;
        private System.Action<UIAprItem> m_stateChangeEvent = null;
        public int m_index;
        void Awake()
        {
            m_stateRoot.onClick.AddListener(OnClick);
            m_stateRoot.onStateChange.AddListener(OnStateChange);
        }

        public void Set(int index, UIAprItem.State uiAprState, string icon, System.Action<UIAprItem> clickEvent, System.Action<UIAprItem> stateChangeEvent)
        {
            
            m_index = index;
            SetState(uiAprState, false);
            if(icon!=null)
            {
                Helper.SetSprite(m_icon, icon);
            } 
            m_clickEvent = clickEvent;
            m_stateChangeEvent = stateChangeEvent;
        }

        void OnClick()
        {
            if(m_clickEvent!=null)
            {
                m_clickEvent(this);
            }
        }
        void OnStateChange()
        {
            if(m_stateChangeEvent!=null)
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
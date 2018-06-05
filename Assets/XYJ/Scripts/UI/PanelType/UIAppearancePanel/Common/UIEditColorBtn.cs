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
    class UIEditColorBtn
    {
        //0: 有颜色 1：无颜色 2，使用中
        [SerializeField]
        public StateRoot m_stateRoot;
        [SerializeField]
        Image m_color;
        [SerializeField]
        Button m_delete;

        public int m_index;
        private System.Action<UIEditColorBtn> m_clickDeleteEvent = null;
        private System.Action<UIEditColorBtn> m_clickEvent = null;
        void Awake()
        {
            m_delete.onClick.AddListener(OnClickDelete);
            m_stateRoot.onClick.AddListener(OnClick);
        }
        public void Set(int index,int state , Color color, System.Action<UIEditColorBtn> clickDeleteEvent,
            System.Action<UIEditColorBtn> clickEvent)
        {
            m_index = index;
            m_stateRoot.SetCurrentState(state, false);
            m_color.color = color;
            m_clickDeleteEvent = clickDeleteEvent;
            m_clickEvent = clickEvent;
        }
        public void Set(int index)
        {
            m_index = index;
            m_stateRoot.SetCurrentState(1, false);
        }
        void OnClickDelete()
        {
            if(m_clickDeleteEvent != null)
            {
                m_clickDeleteEvent(this);
            }
        }
        void OnClick()
        {
            if(m_clickEvent!=null)
            {
                m_clickEvent(this);
            }
        }
    }
}
#endif

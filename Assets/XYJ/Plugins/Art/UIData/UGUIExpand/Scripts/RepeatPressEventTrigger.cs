using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace xys.UI
{
    public class RepeatPressEventTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("长按过程中响应的次数，0表无限次")]
        [SerializeField]
        uint m_InvokeCount = 1;
        [Tooltip("长按延迟秒数")]
        [SerializeField]
        uint m_Delay = 1;
        [Tooltip("事件触发时间间隔")]
        [SerializeField]
        uint m_Interval = 1;
        [SerializeField]
        UnityEvent m_RepeatEvent = new UnityEvent();

        private bool m_keyDown = false;
        private float m_lastDownTime;
        private uint m_curCount;
        private Action m_listener;

        void Update()
        {
            if (m_keyDown && m_curCount > 0)
            {
                if (Time.time - m_lastDownTime > m_Interval)
                {
                    m_RepeatEvent.Invoke();
                    if (m_listener != null)
                        m_listener();
                    m_lastDownTime = Time.time;
                    m_curCount--;
                }
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            m_lastDownTime = Time.time + m_Delay - m_Interval;
            m_curCount = m_InvokeCount;
            m_keyDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_keyDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_keyDown = false;
        }

        public void AddRepeatListener(Action listener)
        {
            m_listener += listener;
        }

        public void RemoveRepeatListener(Action listener)
        {
            m_listener -= listener;
        }

        public void ClearRepeatListener()
        {
            m_listener = null;
        }
    }
}

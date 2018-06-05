using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace xys
{
    /// <summary>
    ///长点击操作
    /// </summary>
    public class DPOnPointer: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField]
        float m_Interval = 0.5f;
        [SerializeField]
        float m_DeltaTime = 0.5f;

        bool isPointDown = false;

        float m_Delay = 0.0f;

        float lastInvokeTime = 0.0f;

        public UnityEvent onLongPress = new UnityEvent();

        void Start()
        {
            m_Interval = float.Parse(Config.kvClient.Get("LongPressTimeInterval").value);
            m_DeltaTime = m_Delay = float.Parse(Config.kvClient.Get("LongPressTimeDelta").value);
        }

        void Update()
        {
            if (isPointDown)
            {
                if ((m_Delay -= Time.deltaTime) > 0f)
                {
                    return;
                }

                if (Time.time - lastInvokeTime > m_Interval)
                {
                    //触发点击;
                    onLongPress.Invoke();
                    lastInvokeTime = Time.time;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointDown = true;
            m_Delay = m_DeltaTime;
            lastInvokeTime = Time.time;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointDown = false;
            m_Delay = m_DeltaTime;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointDown = false;
            m_Delay = m_DeltaTime;
        }
    }
}


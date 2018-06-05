#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace xys.hot.UI
{
    /// <summary>
    /// 主面板摇杆区域
    /// </summary>
    public class JoystickView
    {
        public RectTransform m_joystick_point;
        public RectTransform m_joystick_bg;

        float m_radius;

        public JoystickView(GameObject go)
        {
            m_joystick_bg = go.transform.Find("Offset/joystick").GetComponent<RectTransform>();
            m_joystick_point = go.transform.Find("Offset/joystick/point").GetComponent<RectTransform>();

            ScrollCircle scrollCircle = m_joystick_bg.AddComponentIfNoExist<ScrollCircle>();
            m_radius = scrollCircle.m_radius;

            scrollCircle.onBeginDrag = JoystickOnBeginDrag;
            scrollCircle.onDrag = JoystickOnDrag;
            scrollCircle.onEndDrag = JoystickOnEndDrag;
        }


        void JoystickOnDrag(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            App.my.input.SetMoveWay(m_joystick_point.anchoredPosition / m_radius);
        }

        void JoystickOnEndDrag(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            App.my.input.SetMoveWay(Vector2.zero);
        }

        void JoystickOnBeginDrag(PointerEventData eventData)
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            App.my.input.BeginInput();
        }
    }

}
#endif

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace xys.UI
{
    /// <summary>
    /// 触发长按就不再触发点击事件
    /// 防连击和双击是互斥的
    /// 抬起事件是一定会触发的
    /// </summary>
    public class ButtonEx : Selectable
    {
        /// <summary>
        /// 长按事件是否只调用一次
        /// </summary>
        public bool longPressInvokeOnce = true;
        /// <summary>
        /// 按下后超过这个时间则认定为"长按"
        /// </summary>
        public float longPressInterval = 0.5f;            
        /// <summary>
        /// 是否允许连击
        /// 如果不允许，则doubleClickInterval无效
        /// 如果允许，则comboDisableInterval无效
        /// </summary>
        public bool comboEnable = true;
        /// <summary>
        /// 低于此时间连击无效
        /// </summary>               
        public float comboDisableInterval = 1f;
        /// <summary>
        /// 两次点击要在此时间内才会触发双击
        /// </summary>            
        public float doubleClickInterval = 0.3f;        

        #region Event
        [SerializeField]
        private ButtonClickEvent m_onButtonClick = new ButtonClickEvent();
        [SerializeField]
        private ButtonClickWithoutDragEvent m_onButtonClickWithoutDrag = new ButtonClickWithoutDragEvent();
        [SerializeField]
        private ButtonDoubleClickEvent m_onDoubleClick = new ButtonDoubleClickEvent();
        [SerializeField]
        private ButtonLongPresskEvent m_onLongPress = new ButtonLongPresskEvent();
        [SerializeField]
        private ButtonPointDown m_onPointDown = new ButtonPointDown();
        [SerializeField]
        private ButtonPointExitEvent m_onPointExit = new ButtonPointExitEvent();
        [SerializeField]
        private ButtonPointEnterEvent m_onPointEnter = new ButtonPointEnterEvent();
        [SerializeField]
        private ButtonPointUpInBoundsEvent m_onPointUpInBounds = new ButtonPointUpInBoundsEvent();          // 在按钮中抬起
        [SerializeField]
        private ButtonPointUpExitBoundsEvent m_onPointUpExitBounds = new ButtonPointUpExitBoundsEvent();    // 在按钮外抬起

        public ButtonClickEvent OnClick
        {
            get { return m_onButtonClick; }
            set { m_onButtonClick = value; }
        }

        public ButtonClickWithoutDragEvent OnClickWithoutDrag
        {
            get { return m_onButtonClickWithoutDrag; }
            set { m_onButtonClickWithoutDrag = value; }
        }
        public ButtonDoubleClickEvent OnDoubleClick
        {
            get { return m_onDoubleClick; }
            set { m_onDoubleClick = value; }
        }
        public ButtonLongPresskEvent OnLongPress
        {
            get { return m_onLongPress; }
            set { m_onLongPress = value; }
        }
        public ButtonPointDown OnPointDown
        {
            get { return m_onPointDown; }
            set { m_onPointDown = value; }
        }
        public ButtonPointExitEvent OnPointExit
        {
            get { return m_onPointExit; }
            set { m_onPointExit = value; }
        }
        public ButtonPointEnterEvent OnPointEnter
        {
            get { return m_onPointEnter; }
            set { m_onPointEnter = value; }
        }
        public ButtonPointUpInBoundsEvent OnPointUpInBounds
        {
            get { return m_onPointUpInBounds; }
            set { m_onPointUpInBounds = value; }
        }
        public ButtonPointUpExitBoundsEvent OnPointUpExitBounds
        {
            get { return m_onPointUpExitBounds; }
            set { m_onPointUpExitBounds = value; }
        }
        #endregion

        // 是否已经调用过 
        private bool hadInvoke = false;                   
        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float recordTime;
        private bool isInBounds = true;
        private bool clickDisable = false;
        // 点击时间，用以判断连击时间间隔是否符合条件
        private float clickTime;
        // 点击时的落点，用以判断是否移动过
        private Vector2 pointDownData;
        // 抬起时的坐标，用以判断是否移动过
        private Vector2 pointUpData;
        void Update()
        {
            if(longPressInvokeOnce && hadInvoke)
                return;
            if(isPointerDown)
            {
                if(Time.time - recordTime > longPressInterval)
                {
                    longPressTriggered = true;
                    if(null != m_onLongPress)
                    {
                        m_onLongPress.Invoke();
                    }
                    hadInvoke = true;
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if(null != m_onPointDown)
            {
                m_onPointDown.Invoke();
            }
            isPointerDown = true;
            longPressTriggered = false;
            isInBounds = true;
            recordTime = Time.time;
            pointDownData = eventData.position;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            hadInvoke = false;
            pointUpData = eventData.position;

            if(null != m_onPointUpInBounds)
            {
                if(isInBounds)
                {
                    m_onPointUpInBounds.Invoke();
                }
                else if(null != m_onPointUpExitBounds)
                {
                    m_onPointUpExitBounds.Invoke();
                }
            }

            if(!longPressTriggered)
            {
                if(comboEnable)
                {
                    if(null != m_onDoubleClick)
                    {
                        if(IsInvoking("ClickInvoke"))
                        {
                            CancelInvoke("ClickInvoke");
                            m_onDoubleClick.Invoke();
                        }
                        else
                        {
                            Invoke("ClickInvoke", doubleClickInterval);
                        }
                    }
                    else
                    {
                        ClickInvoke();
                    }
                }
                else
                {
                    if(Time.time - clickTime > comboDisableInterval)
                    {
                        clickDisable = false;
                    }

                    if(clickDisable)
                    {
                        return;
                    }

                    ClickInvoke();
                    clickTime = Time.time;
                    clickDisable = true;
                }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            isInBounds = false;
            if(null != m_onPointExit)
            {
                m_onPointExit.Invoke();
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            isInBounds = true;
            if(null != m_onPointEnter)
            {
                m_onPointEnter.Invoke();
            }
        }

        private void ClickInvoke()
        {
            if(IsDraggingGameObject())
            {
                if(null != m_onDoubleClick)
                {
                    m_onButtonClick.Invoke();
                } 
            }
            else
            {
                if (null != m_onButtonClickWithoutDrag)
                {
                    m_onButtonClickWithoutDrag.Invoke();
                }
            }
        }

        private bool IsDraggingGameObject()
        {
            var x = Mathf.Abs(pointDownData.x - pointUpData.x) < 1f;
            var y = Mathf.Abs(pointDownData.y - pointUpData.y) < 1f;
            return !(x && y);
        }

        #region Defines
        [SerializeField]
        public class ButtonClickEvent : UnityEvent { }
        [SerializeField]
        public class ButtonDoubleClickEvent : UnityEvent { }
        [SerializeField]
        public class ButtonLongPresskEvent : UnityEvent { }
        [SerializeField]
        public class ButtonPointDown : UnityEvent { }
        [SerializeField]
        public class ButtonPointExitEvent : UnityEvent { }
        [SerializeField]
        public class ButtonPointEnterEvent : UnityEvent { }
        [SerializeField]
        public class ButtonPointUpInBoundsEvent : UnityEvent { }
        [SerializeField]
        public class ButtonPointUpExitBoundsEvent : UnityEvent { }
        [SerializeField]
        public class ButtonClickWithoutDragEvent : UnityEvent { }
        #endregion

    }
}

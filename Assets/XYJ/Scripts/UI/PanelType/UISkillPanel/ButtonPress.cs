using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace xys.UI
{
    public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool invokeOnce = false;                 // 是否只调用一次  
        private bool hadInvoke = false;                 // 是否已经调用过  

        public float interval = 1f;                     // 按下后超过这个时间则认定为"长按"  
        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float recordTime;

        public UnityEvent onPress = new UnityEvent();   // 按住时调用  
        public UnityEvent onRelease = new UnityEvent(); // 松开时调用  
        public UnityEvent onClick = new UnityEvent();   // 点击时调用  

        void Update()
        {
            if (invokeOnce && hadInvoke) return;
            if (isPointerDown)
            {
                if ((Time.time - recordTime) > interval)
                {
                    longPressTriggered = true;
                    onPress.Invoke();
                    hadInvoke = true;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            longPressTriggered = false;
            recordTime = Time.time;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            hadInvoke = false;
            onRelease.Invoke();

            if (!longPressTriggered)
                onClick.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;
            hadInvoke = false;
            onRelease.Invoke();
        }
    }
}
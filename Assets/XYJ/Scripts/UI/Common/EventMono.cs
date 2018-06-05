using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace xys.UI
{
    public class EventMono : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum ParamType
        {
            Null,
            Int,
            Str,
        }

        public ParamType type = ParamType.Null;
        public string eventName;
        public int intParam;
        public string strParam;
        public bool isAgain = false;
        public float interval = 0.2f;
        public bool isFireByKeyDown = true;//按下发送事件

        Coroutine CheckCoroutine;

        void DisableCheck()
        {
            if (CheckCoroutine != null)
            {
                StopCoroutine(CheckCoroutine);
                CheckCoroutine = null;
            }
        }

        void OnDisable()
        {
            DisableCheck();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!isAgain)
            {
                if (isFireByKeyDown)
                    OnFire();
                return;
            }

            DisableCheck();
            CheckCoroutine = StartCoroutine(Check());
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!isAgain)
            {
                if (!isFireByKeyDown)
                    OnFire();
                return;
            }

            DisableCheck();
        }

        IEnumerator Check()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(interval);
                OnFire();
            }
        }

        void OnFire()
        {
#if UNITY_EDITOR
            if (App.my == null)
                return;
#endif
            EventID eid;
            if (Str2Enum.To(eventName, out eid))
            {
                switch (type)
                {
                case ParamType.Null:
                    App.my.eventSet.fireEvent(eid);
                    break;
                case ParamType.Int:
                    App.my.eventSet.FireEvent(eid, intParam);
                    break;
                case ParamType.Str:
                    App.my.eventSet.FireEvent(eid, strParam);
                    break;
                }
            }
        }

    }
}

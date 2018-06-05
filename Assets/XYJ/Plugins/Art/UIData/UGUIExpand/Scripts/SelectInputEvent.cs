using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace xys.UI
{
    public class SelectInputEvent : MonoBehaviour, IDeselectHandler, ISelectHandler
    {
        [SerializeField]
        State.StateRoot stateRoot;

        bool isActive = false;

        public void OnDeselect(BaseEventData eventData)
        {
            isActive = false;
#if UNITY_IPHONE || UNITY_ANDROID
        stateRoot.CurrentState = 0;
#endif
        }

#if UNITY_IPHONE || UNITY_ANDROID
    IEnumerator UpdateKeyboard()
    {
        Rect currentRect = new Rect();
        while (true)
        {
            if (!isActive)
                yield break;

            Rect rect = TouchScreenKeyboard.area;
            if (rect.height == 0)
            {
                yield return 0;
                continue;
            }

            if (currentRect == rect)
            {
                yield return 0;
                continue;
            }

            currentRect = rect;
            for (int i = 0; i < stateRoot.elements.Count; ++i)
            {
                var el = stateRoot.elements[i];
                switch (el.type)
                {
                case State.Type.Pos:
                    {
                        if (el.stateData != null && el.stateData.Length >= 2)
                        {
                            el.stateData[1].vector3 = new Vector3(0f, rect.height, 0f);
                        }
                    }
                    break;
                }
            }

            stateRoot.SetCurrentState(1, false);
        }
    }
#endif

        public void OnSelect(BaseEventData eventData)
        {
            isActive = true;
#if UNITY_IPHONE || UNITY_ANDROID
        StartCoroutine(UpdateKeyboard());
#endif
        }
    }
}
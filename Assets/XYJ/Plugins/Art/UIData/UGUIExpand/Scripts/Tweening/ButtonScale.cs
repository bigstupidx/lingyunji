using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    [AddComponentMenu("UGUI/Interaction/Button Scale")]
    public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
    {
        static bool IsHighlighted(GameObject go)
        {
            if (EventSystem.current == null)
                return false;

            return EventSystem.current.currentSelectedGameObject == go;
        }

        public Transform tweenTarget;
        static Vector3 hover = new Vector3(1.0f, 1.0f, 1.0f);
        public Vector3 pressed = new Vector3(0.95f, 0.95f, 0.95f);
        public float duration = 0.0f;

        Vector3 mScale;
        bool mStarted = false;

        [SerializeField]
        bool isDisableByGrey = true; // 目标如果变灰，是否不生成

        bool targetIsGery
        {
            get
            {
                if (tweenTarget == null)
                    return false;

                Graphic g = tweenTarget.GetComponent<Graphic>();
                if (g == null)
                    return false;

                Material m = g.material;
                if (m != null && m.name == "UIGray")
                    return true;

                return false;
            }
        }

        bool isVaild
        {
            get
            {
                if (!enabled)
                    return false;

                if (!isDisableByGrey)
                    return true;

                if (targetIsGery)
                    return false;

                return true;
            }
        }

        public Vector3 Scale
        {
            set { mScale = value; }
        }

        void Start()
        {
            if (!mStarted)
            {
                mStarted = true;
                if (tweenTarget == null) tweenTarget = transform;
                mScale = tweenTarget.localScale;
            }
        }

        void OnEnable() { if (mStarted) OnHover(IsHighlighted(gameObject)); }

        void OnDisable()
        {
            if (mStarted && tweenTarget != null)
            {
                TweenScale tc = tweenTarget.GetComponent<TweenScale>();

                if (tc != null)
                {
                    tc.value = mScale;
                    tc.enabled = false;
                }
            }
        }

        void OnPress(bool isPressed)
        {
            if (isVaild)
            {
                if (!mStarted) Start();
                TweenScale.Begin(tweenTarget.gameObject, duration, isPressed ? Vector3.Scale(mScale, pressed) :
                    (IsHighlighted(gameObject) ? Vector3.Scale(mScale, hover) : mScale)).method = UITweener.Method.EaseInOut;
            }
        }

        void OnHover(bool isOver)
        {
            if (isVaild)
            {
                if (!mStarted) Start();
                TweenScale.Begin(tweenTarget.gameObject, duration, isOver ? Vector3.Scale(mScale, hover) : mScale).method = UITweener.Method.EaseInOut;
            }
        }

        void OnSelect(bool isSelected)
        {
            if (isVaild && (!isSelected))
                OnHover(isSelected);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnHover(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelect(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            OnSelect(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPress(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPress(false);
        }
    }
}
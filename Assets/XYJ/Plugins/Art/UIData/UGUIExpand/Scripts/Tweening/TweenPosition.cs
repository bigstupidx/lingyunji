using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

namespace UI
{
    [AddComponentMenu("uGUI/Tween/Tween Position")]
    public class UGUITweenPosition : UITweener
    {
        public Vector3 from;
        public Vector3 to;

        RectTransform mTrans;

        public RectTransform cachedTransform { get { if (mTrans == null) mTrans = GetComponent<RectTransform>(); return mTrans; } }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public Vector3 value
        {
            get
            {
                return cachedTransform.anchoredPosition;
            }
            set
            {
                cachedTransform.anchoredPosition = value;
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public UGUITweenPosition Begin(GameObject go, float duration, Vector3 pos)
        {
            UGUITweenPosition comp = UITweener.Begin<UGUITweenPosition>(go, duration);
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue() { from = value; }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue() { to = value; }

        [ContextMenu("Assume value of 'From'")]
        void SetCurrentValueToStart() { value = from; }

        [ContextMenu("Assume value of 'To'")]
        void SetCurrentValueToEnd() { value = to; }
    }
}
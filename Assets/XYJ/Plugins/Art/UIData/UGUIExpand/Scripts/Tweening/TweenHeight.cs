using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the widget's size.
/// </summary>

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("uGUI/Tween/Tween Height")]
    public class TweenHeight : UITweener
    {
        public int from = 100;
        public int to = 100;

        RectTransform mRect;

        public RectTransform cachedRect { get { if (mRect == null) mRect = GetComponent<RectTransform>(); return mRect; } }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public int value
        {
            get { return (int)cachedRect.rect.size.y; }
            set
            {
                cachedRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.RoundToInt(from * (1f - factor) + to * factor);
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public TweenHeight Begin(RectTransform rect, float duration, int height)
        {
            TweenHeight comp = UITweener.Begin<TweenHeight>(rect.gameObject, duration);
            comp.from = (int)rect.rect.size.y;
            comp.to = height;

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
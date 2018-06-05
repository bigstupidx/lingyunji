using UnityEngine;

/// <summary>
/// Tween the object's local scale.
/// </summary>

namespace UI
{
    [AddComponentMenu("UGUI/Tween/Tween Size Scale")]
    [RequireComponent(typeof(SizeScale))]
    public class TweenSizeScale : UITweener
    {
        public float from = 1f;
        public float to = 1f;

        SizeScale mSizeScale;

        public SizeScale cacheSizeScale { get { if (mSizeScale == null) mSizeScale = GetComponent<SizeScale>(); return mSizeScale; } }

        public float value { get { return cacheSizeScale.value; } set { cacheSizeScale.value = value; } }

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

        static public TweenSizeScale Begin(GameObject go, float duration, float scale)
        {
            TweenSizeScale comp = UITweener.Begin<TweenSizeScale>(go, duration);
            comp.from = comp.value;
            comp.to = scale;

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
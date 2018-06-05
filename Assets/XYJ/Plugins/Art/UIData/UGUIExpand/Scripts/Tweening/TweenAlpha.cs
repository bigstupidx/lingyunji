using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's alpha. Works with both UI widgets as well as renderers.
/// </summary>

namespace UI
{
    [AddComponentMenu("uGUI/Tween/Tween Alpha")]
    public class UGUITweenAlpha : UITweener
    {
        [Range(0f, 1f)]
        public float from = 1f;
        [Range(0f, 1f)]
        public float to = 1f;

        bool mCached = false;
        MaskableGraphic mGraphic;
        Material mMat;
        SpriteRenderer mSr;

        void Cache()
        {
            mCached = true;
            mGraphic = GetComponent<MaskableGraphic>();
            mSr = GetComponent<SpriteRenderer>();

            if (mGraphic == null && mSr == null)
            {
                Renderer ren = GetComponent<Renderer>();
                if (ren != null) mMat = ren.material;
                if (mMat == null) mGraphic = GetComponentInChildren<MaskableGraphic>();
            }
        }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public float value
        {
            get
            {
                if (!mCached) Cache();
                if (mGraphic != null) return mGraphic.color.a;
                if (mSr != null) return mSr.color.a;
                return mMat != null ? mMat.color.a : 1f;
            }
            set
            {
                if (!mCached) Cache();

                if (mGraphic != null)
                {
                    Color c = mGraphic.color;
                    c.a = value;
                    mGraphic.color = c;
                }
                else if (mSr != null)
                {
                    Color c = mSr.color;
                    c.a = value;
                    mSr.color = c;
                }
                else if (mMat != null)
                {
                    Color c = mMat.color;
                    c.a = value;
                    mMat.color = c;
                }
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public UGUITweenAlpha Begin(GameObject go, float duration, float alpha)
        {
            UGUITweenAlpha comp = UITweener.Begin<UGUITweenAlpha>(go, duration);
            comp.from = comp.value;
            comp.to = alpha;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        public override void SetStartToCurrentValue() { from = value; }
        public override void SetEndToCurrentValue() { to = value; }
    }
}
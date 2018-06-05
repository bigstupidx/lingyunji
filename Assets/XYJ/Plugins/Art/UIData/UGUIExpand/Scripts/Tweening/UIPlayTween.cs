using UnityEngine;
using UnityEngine.Events;
using UI.AnimationOrTween;

/// <summary>
/// Play the specified tween on click.
/// </summary>

namespace UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Interaction/Play Tween")]
    public class UIPlayTween : MonoBehaviour
    {
        static public UIPlayTween current;

        /// <summary>
        /// Target on which there is one or more tween.
        /// </summary>

        public GameObject tweenTarget;

        /// <summary>
        /// If there are multiple tweens, you can choose which ones get activated by changing their group.
        /// </summary>

        public int tweenGroup = 0;

        /// <summary>
        /// Direction to tween in.
        /// </summary>

        public Direction playDirection = Direction.Forward;

        /// <summary>
        /// Whether the tween will be reset to the start or end when activated. If not, it will continue from where it currently is.
        /// </summary>

        public bool resetOnPlay = false;

        /// <summary>
        /// Whether the tween will be reset to the start if it's disabled when activated.
        /// </summary>

        public bool resetIfDisabled = false;

        /// <summary>
        /// What to do if the tweenTarget game object is currently disabled.
        /// </summary>

        public EnableCondition ifDisabledOnPlay = EnableCondition.DoNothing;

        /// <summary>
        /// What to do with the tweenTarget after the tween finishes.
        /// </summary>

        public DisableCondition disableWhenFinished = DisableCondition.DoNotDisable;

        /// <summary>
        /// Whether the tweens on the child game objects will be considered.
        /// </summary>

        public bool includeChildren = false;

        /// <summary>
        /// Event delegates called when the animation finishes.
        /// </summary>

        public UnityEvent onFinished = new UnityEvent();

        UITweener[] mTweens;
        //bool mStarted = false;
        int mActive = 0;
        //bool mActivated = false;

        void Start()
        {
            //mStarted = true;

            if (tweenTarget == null)
            {
                tweenTarget = gameObject;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (disableWhenFinished != DisableCondition.DoNotDisable && mTweens != null)
            {
                bool isFinished = true;
                bool properDirection = true;

                for (int i = 0, imax = mTweens.Length; i < imax; ++i)
                {
                    UITweener tw = mTweens[i];
                    if (tw.tweenGroup != tweenGroup) continue;

                    if (tw.enabled)
                    {
                        isFinished = false;
                        break;
                    }
                    else if ((int)tw.direction != (int)disableWhenFinished)
                    {
                        properDirection = false;
                    }
                }

                if (isFinished)
                {
                    if (properDirection)
                        tweenTarget.SetActive(false);
                    mTweens = null;
                }
            }
        }

        /// <summary>
        /// Activate the tweeners.
        /// </summary>

        public void Play(bool forward)
        {
            mActive = 0;
            GameObject go = (tweenTarget == null) ? gameObject : tweenTarget;

            if (!go.activeSelf)
            {
                // If the object is disabled, don't do anything
                if (ifDisabledOnPlay != EnableCondition.EnableThenPlay) return;

                // Enable the game object before tweening it
                go.SetActive(true);
            }

            // Gather the tweening components
            mTweens = includeChildren ? go.GetComponentsInChildren<UITweener>() : go.GetComponents<UITweener>();

            if (mTweens.Length == 0)
            {
                // No tweeners found -- should we disable the object?
                if (disableWhenFinished != DisableCondition.DoNotDisable)
                    tweenTarget.SetActive(false);
            }
            else
            {
                bool activated = false;
                if (playDirection == Direction.Reverse) forward = !forward;

                // Run through all located tween components
                for (int i = 0, imax = mTweens.Length; i < imax; ++i)
                {
                    UITweener tw = mTweens[i];

                    // If the tweener's group matches, we can work with it
                    if (tw.tweenGroup == tweenGroup)
                    {
                        // Ensure that the game objects are enabled
                        if (!activated && !go.activeSelf)
                        {
                            activated = true;
                            go.SetActive(true);
                        }

                        ++mActive;

                        // Toggle or activate the tween component
                        if (playDirection == Direction.Toggle)
                        {
                            // Listen for tween finished messages
                            tw.onFinished.AddListener(OnFinished, 1);
                            tw.Toggle();
                        }
                        else
                        {
                            if (resetOnPlay || (resetIfDisabled && !tw.enabled))
                            {
                                tw.Play(forward);
                                tw.ResetToBeginning();
                            }
                            // Listen for tween finished messages
                            tw.onFinished.AddListener(OnFinished, 1);
                            tw.Play(forward);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Callback triggered when each tween executed by this script finishes.
        /// </summary>

        void OnFinished()
        {
            if (--mActive == 0 && current == null)
            {
                current = this;
                onFinished.Invoke();

                current = null;
            }
        }
    }
}
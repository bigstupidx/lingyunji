using UnityEngine;
using System.Collections;

namespace UI
{
    public class PanelAnimator
    {
#if UNITY_EDITOR
        public float speedScaler = 1f;
#endif

        public PanelAnimator(Animator anim)
        {
            openOrCloseAnimator = anim;
            if (openOrCloseAnimator != null)
                openOrCloseAnimator.enabled = false;
        }

        protected Animator openOrCloseAnimator; // 打开或关闭面板动画

        public IEnumerator PlayAnim(bool forward, System.Func<int> flag)
        {
            if (openOrCloseAnimator.runtimeAnimatorController != null)
            {
                int current = flag();

                openOrCloseAnimator.enabled = false;
                openOrCloseAnimator.Rebind();
                openOrCloseAnimator.updateMode = AnimatorUpdateMode.Normal;

                openOrCloseAnimator.Play(forward ? "open" : "close");
                openOrCloseAnimator.Update(0f);
                AnimatorStateInfo info = openOrCloseAnimator.GetCurrentAnimatorStateInfo(0);
                float lenght = info.length;
                yield return 0;

                if (current != flag())
                    yield break;

                float total = 0f;
                while (true)
                {
                    if (current != flag())
                        yield break;

                    float delta = Time.unscaledDeltaTime;
#if UNITY_EDITOR
                    delta *= speedScaler;
#endif
                    if (total + delta >= lenght)
                    {
                        // 结束了
                        openOrCloseAnimator.Update(lenght - total);
                        break;
                    }
                    else
                    {
                        openOrCloseAnimator.Update(delta);
                        total += delta;
                        yield return 0;
                    }
                }

                if (!forward)
                {
                    openOrCloseAnimator.Rebind();
                    openOrCloseAnimator.Play("close");
                    openOrCloseAnimator.Update(0f);
                }
            }
        }

        public void OnBeginShow(bool isPlayAnim)
        {
            if (openOrCloseAnimator != null)
            {
                if (isPlayAnim)
                {
                    // 最后一桢
                    openOrCloseAnimator.Rebind();
                    openOrCloseAnimator.Play("open");
                    openOrCloseAnimator.Update(openOrCloseAnimator.GetCurrentAnimatorStateInfo(0).length);
                }
                else
                {
                    // 第一桢
                    openOrCloseAnimator.Rebind();
                    openOrCloseAnimator.Play("open");
                    openOrCloseAnimator.Update(0);
                }
            }
        }
    }
}
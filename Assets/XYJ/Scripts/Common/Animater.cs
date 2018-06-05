#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class Animater : MonoBehaviour
    {
        Animator animator;

        public static Animater Get(Animator anim)
        {
            Animater animator = anim.GetComponent<Animater>();
            if (animator == null)
            {
                animator = anim.gameObject.AddComponent<Animater>();
            }

            return animator;
        }

        void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                animator.enabled = false;
            }
        }

        // 动画播放完成的回调
        System.Action OnEndFun = null;

        public void Play(string name, float speed, System.Action fun)
        {
            if (animator == null)
            {
                Awake();
            }

            enabled = true;
            animator.Rebind();
            Speed = speed;
            animator.Play(name);
            animator.Update(0f);

            // 这个是要倒着播放的，那么
            if (Speed < 0)
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                animator.Update(info.length);
            }

            OnEndFun = fun;
        }

        void Update()
        {
            if (animator == null)
                return;

            animator.Update(Speed * Time.deltaTime);
            if (CheckEnd())
            {
                var fun = OnEndFun;
                OnEndFun = null;
                enabled = false;

                if (fun != null)
                    fun();
            }
        }

        bool CheckEnd()
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            if (Speed > 0)
            {
                // 正向播放
                if (info.normalizedTime >= 1f)
                {
                    return true;
                }
            }
            else
            {
                if (info.normalizedTime <= 0f)
                {
                    return true;
                }
            }

            return false;
        }

        float Speed = 1f;

        public void Restore(float time = 0f)
        {
            animator.Rebind();
            animator.Update(time);
        }
    }
}

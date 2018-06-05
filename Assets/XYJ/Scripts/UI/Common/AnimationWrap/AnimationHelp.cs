using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace xys.UI
{

    public class AnimationHelp
    {
        //播放动画,播放完有回调，播放完会自动disable动画
        //返回的回调id可以用来取消回调。一般对象销毁或者重复播放的时候会需要取消回调
        //特效会创建多个的不要使用这个接口.其实这里一个bug，飘字的时候第二个创建出来的会吧第一个的动画重置为默认动画
        public static void PlayAnimation(Animator ani, string aniName, Action<object> callBack = null, object para = null)
        {
            if (ani != null)
            {
                AnimationClip[] animationClipArray = ani.runtimeAnimatorController.animationClips;
                AnimationClip targetClip = null;
                int clipLength = animationClipArray.Length;
                for (int index = 0; index < clipLength; index++)
                {
                    if (animationClipArray[index].name == aniName)
                    {
                        targetClip = animationClipArray[index];
                    }
                }
                if (targetClip == null)
                {
                    return;
                }

                int timerid;
                if (timerDic.TryGetValue(ani, out timerid))
                {
                    App.my.mainTimer.Cannel(timerid);
                    timerDic.Remove(ani);
                }

                /* bool hasAnimationOverEvent = ContainsAnimationEvent(targetClip.events, AnimationEventListener.ANIMATION_OVER);
                 if (!hasAnimationOverEvent)
                 {
                     AnimationEvent animationEvent = new AnimationEvent();
                     animationEvent.time = targetClip.length;
                     animationEvent.functionName = AnimationEventListener.ANIMATION_OVER;
                     animationEvent.stringParameter = targetClip.name;
                     targetClip.AddEvent(animationEvent);
                     //首次添加事件时，如果不提前update一下,填什么名字都只能播默认动画
                     //其实这里还有一个bug，飘字的时候第二个创建出来的会吧第一个的动画重置为默认动画
                     ani.Update(0);
                 }*/

                AnimationEventListener animationEventListener = ani.gameObject.AddMissingComponent<AnimationEventListener>();
                animationEventListener.SetCallBackData(ani, CallBack, callBack, para, targetClip.length);

                targetClip.wrapMode = WrapMode.Once;
                ani.enabled = true;
                ani.Play(aniName, 0, 0);
            }
        }

        public static void PlayNormalizedTime(Animator ani, string aniName,float normalizedTime)
        {
            ani.enabled = true;
            ani.Play(aniName, 0, normalizedTime);
            ani.Update(0);
            ani.enabled = false;
        }

        public static void PlayAnimation(Animator ani, string aniName, string clipName, Action<object> callBack = null, object para = null)
        {
            if (ani == null)
                return;
            AnimationClip[] animationClipArray = ani.runtimeAnimatorController.animationClips;
            AnimationClip targetClip = null;
            int clipLength = animationClipArray.Length;
            for (int index = 0; index < clipLength; index++)
            {
                if (animationClipArray[index].name == clipName)
                {
                    targetClip = animationClipArray[index];
                }
            }
            if (targetClip == null)
            {
                return;
            }

            int timerid;
            if (timerDic.TryGetValue(ani, out timerid))
            {
                App.my.mainTimer.Cannel(timerid);
                timerDic.Remove(ani);
            }
            AnimationEventListener animationEventListener = ani.gameObject.AddMissingComponent<AnimationEventListener>();
            animationEventListener.SetCallBackData(ani, CallBack, callBack, para, targetClip.length);

            targetClip.wrapMode = WrapMode.Once;
            ani.enabled = true;
            ani.Play(aniName, 0, 0);
        }

        private static bool ContainsAnimationEvent(AnimationEvent[] animationEventArray, string functionName)
        {
            if (animationEventArray == null)
            {
                return false;
            }
            for (int index = 0; index < animationEventArray.Length; index++)
            {
                if (animationEventArray[index].functionName == functionName)
                {
                    return true;
                }
            }
            return false;
        }
        #region 内部实现
        static void CallBack(Animator ani, Action<object> callBack, object para)
        {
            try
            {
                if (callBack != null)
                    callBack(para);
            }
            catch (Exception err)
            {
                Debug.LogException(err);
                throw new Exception("动画回调跑出异常 " + callBack.ToString());
            }

            if (ani != null && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                int timerId = xys.App.my.mainTimer.Register(0.00001f, 1, DelayClose, ani);
                timerDic.Add(ani, timerId);
            }
        }

        private static Dictionary<Animator, int> timerDic = new Dictionary<Animator, int>();
        private static void DelayClose(Animator animator)
        {
            if (animator != null)
            {
                animator.enabled = false;
                if (timerDic.ContainsKey(animator))
                {
                    timerDic.Remove(animator);
                }
            }

        }

        static float GetClipTime(Animator ani, string name)
        {
            if (null != ani && ani.layerCount >= 1)
            {
                AnimationClip[] info = ani.runtimeAnimatorController.animationClips;
                for (int i = 0; i < info.Length; i++)
                {
                    if (info[i].name == name)
                        return info[i].length;
                }
            }
            return 0;
        }
        #endregion
    }
}

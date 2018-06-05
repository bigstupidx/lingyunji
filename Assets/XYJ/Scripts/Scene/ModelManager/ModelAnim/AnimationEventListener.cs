using System;
using System.Collections.Generic;
using UnityEngine;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

public class AnimationEventListener : MonoBehaviour
{
    public const string UI_ANIMATION_EVENT = "UI_ANIMATION_EVENT_Animator";
    public const string ANIMATION_START = "OnAnimationStart";
    public const string ANIMATION_OVER = "OnAnimationOver";
    private Action<Animator, Action<object>, object> completeCallBackParams;
    private Action<object> completeCallBack;
    private object para;
    private Animator playAnimator;
    //private bool isPlay = true;
    public bool isInit = false;
    private float currentAnimationLength;
    public void SetCallBackData(Animator playAnimator, Action<Animator, Action<object>, object> completeCallBackParams, Action<object> completeCallBack, object para, float animationLength)
    {
        this.playAnimator = playAnimator;
        this.completeCallBackParams = completeCallBackParams;
        this.completeCallBack = completeCallBack;
        this.para = para;
        currentAnimationLength = animationLength;
        CancelInvoke("CheckAnimationOver");
        InvokeRepeating("CheckAnimationOver", currentAnimationLength, 0.05f);//理论上动画播放完的时候检查动画回调是否播放完
    }

    public void OnAnimationStart(string animationName)
    {

    }

    private void CheckAnimationOver()
    {
        if (playAnimator != null && playAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            if (completeCallBack != null)
            {
                completeCallBackParams(playAnimator, completeCallBack, para);
                completeCallBackParams = null;
                completeCallBack = null;
                playAnimator = null;
            }
            CancelInvoke("CheckAnimationOver");
        }
        //确保
    }


    public void OnAnimationOver(string animationName)
    {
        CancelInvoke("CheckAnimationOver");
        if (completeCallBack != null)
        {
            completeCallBackParams(playAnimator, completeCallBack, para);
            completeCallBackParams = null;
            completeCallBack = null;
            playAnimator = null;
        }
    }
}




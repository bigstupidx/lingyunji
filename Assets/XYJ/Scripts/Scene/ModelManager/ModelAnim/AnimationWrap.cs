/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：动画封装
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public abstract class AnimationWrap
#if MEMORY_CHECK
        : MemoryObject
#endif
{
    public abstract void Clear();
    //当前动画是否播放结束
    public abstract bool IsFinish();
    //是否循环
    public abstract bool IsLoop();
    //是否循环
    public abstract bool IsLoop(string name);

    //真正播放动画方法,会中断之前的动画
    public abstract void PlayAni(string name, float speed = 1.0f, float normalizedTime = 0, bool isLoop = false, bool crossFade = false, int layer = 0, float crossFadeNormalizedTime = -1);
    //获得当前动画播放时间(并不是真实帧的时间)
    public abstract float GetCurFrameTime();
    //获得当前动画真实帧
    public abstract int GetCurTrueAniFrame();
    //获得动画播放时长:效率不高，尽量不要每帧调用
    public abstract float GetLength(string name);
    //获得当前动画名字
    public abstract string GetCurName();
    //获得动画帧率
    public abstract float GetFrameRate(string name);
    //设置当前动画速度
    public abstract void SetCurSpeed(float speed);
    //获得当前动画速度
    public abstract float GetSpeed();
    //动画是否正在过渡
    public virtual bool IsInTransition() { return false; }
    //根据动画名获得一个动画片段
    public abstract AnimationClip GetClip(string name);
    public virtual void SetCullingMode(bool always) { ;}

    public static AnimationWrap Create(GameObject go)
    {
        //根据动画类型使用不同的管理器
        Animator animator = go.GetComponent<Animator>();
        if (animator != null)
            return new NewAnimation(animator);

        Animation animation = go.GetComponent<Animation>();
        if (animation != null)
            return new OldAnimation(animation);
        return null;
    }
}

public class PlayAniCxt
{
    public string name = string.Empty;
    public float speed = 1.0f;
    public float normalizedTime = 0;
    public float endNormalizedTime = 1.0f;
    public bool isLoop = false;
    public bool crossFade = false;
    public int layer = 0;
    public float crossFadeNormalizedTime = -1;

    public PlayAniCxt(string name, float speed = 1.0f, float normalizedTime = 0, bool isLoop = false, bool crossFade = false, int layer = 0, float crossFadeNormalizedTime = -1)
    {
        this.name = name;
        this.speed = speed;
        this.normalizedTime = normalizedTime;
        this.isLoop = isLoop;
        this.crossFade = crossFade;
        this.layer = layer;
        this.crossFadeNormalizedTime = crossFadeNormalizedTime;
    }
}
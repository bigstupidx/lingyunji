/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewAnimation : AnimationWrap
{
    public Animator m_animator { get; private set; }
    AnimationClip m_curClip;
    string m_curStateName;
#if UNITY_EDITOR
    bool m_isLoop;
#endif
    //当前速度的动画经过时间
    float m_curSpeedTimePass;
    //重新设置速度的时候，需要记录当前已经经过的时间,
    float m_lastSpeedTimePass;
    //过渡时间归一化
    const float CrossFadeNormalizedTime = 0.15f;
    //最长过渡时间
    const float maxCrossFadeTime = 0.02f;
    //当前过渡百分比
    float m_crossFadeNormalizedTime;
    Dictionary<string, AnimationClip> m_clips = new Dictionary<string, AnimationClip>();
    public NewAnimation(Animator ani)
    {
        m_animator = ani;
        if (m_animator.enabled == false)
        {
            string mod = "";
            if (ani.transform.parent != null)
                mod = ani.transform.parent.name;
            Debuger.LogError("Animator没有激活 mod=" + mod);
        }
    }


    public override void Clear()
    {
        m_animator = null;
    }

    public override void SetCullingMode( bool always )
    {
        if (always)
            m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        else
            m_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
    }


    //同一个动画不要重复报错，不然客户端很卡
    string m_errorAniName;
    //立马播放播放动画，会中断之前的动画
    public override void PlayAni(string name, float speed = 1.0f, float normalizedTime = 0, bool isLoop = false, bool crossfade = false, int layer = 0, float crossFadeNormalizedTime = CrossFadeNormalizedTime)
    {
        if (string.IsNullOrEmpty(name))
            return;
        if (null != m_animator && m_animator.runtimeAnimatorController != null && m_animator.layerCount > layer)
        {
            AnimationClip toClip = GetClip(name);

            if (crossfade)
            {
                if (crossFadeNormalizedTime == -1)
                    crossFadeNormalizedTime = CrossFadeNormalizedTime;

                //限制最长过渡时间
                if (toClip != null && toClip.length * crossFadeNormalizedTime > maxCrossFadeTime)
                    crossFadeNormalizedTime = maxCrossFadeTime / toClip.length;
                m_crossFadeNormalizedTime = crossFadeNormalizedTime;
                m_animator.CrossFade(name, crossFadeNormalizedTime, layer, normalizedTime);
            }

            else if (toClip!=null)
                m_animator.Play(name, layer, normalizedTime);

            if (layer != 0)
            {
                return;
            }

            //设置当前播放的速度
            m_animator.speed = speed;

            //设置循环
#if UNITY_EDITOR
            m_isLoop = isLoop;
#endif

            //要更新一帧，不然获取到的当前动画时间是错的
            m_animator.Update(0);

            m_curStateName = name;
            m_curClip = toClip;

            //找不到动画报错
            if (m_curClip == null)
            {
                m_curStateName = "";
                if (m_errorAniName != name)
                    Debuger.LogError(" 模型" + m_animator.transform.parent.name + "  找不到动画 " + name);
                m_errorAniName = name;
            }
            else
            {
#if UNITY_EDITOR
                if (m_isLoop && !m_curClip.isLooping)
                {
                    Debuger.LogWarning(" 模型" + m_animator.transform.parent.name + "  动画 " + name + " 没有勾选循环设置");
                }
#endif
                m_lastSpeedTimePass = 0;
                m_curSpeedTimePass = Time.time - m_curClip.length * normalizedTime;
            }
        }
    }

    //动画是否正在过渡
    public override bool IsInTransition()
    {
        return m_animator.IsInTransition(0);
    }

    public override bool IsLoop()
    {
        return m_animator.GetCurrentAnimatorStateInfo(0).loop;
    }

    //获得动画播放时长
    public override float GetLength(string name)
    {
        AnimationClip clip = GetClip(name);
        if (clip != null)
            return clip.length;
        else
            return 0;
    }

    //获得动画播放时长
    public override float GetFrameRate(string name)
    {
        AnimationClip clip = GetClip(name);
        if (clip != null)
            return clip.frameRate;
        else
            return -1;
    }

    public override bool IsLoop(string name)
    {
        AnimationClip clip = GetClip(name);
        if (clip != null)
            return clip.isLooping;
        else
            return false;
    }

    public override AnimationClip GetClip(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        if (null != m_animator && m_animator.layerCount >= 1)
        {
            AnimationClip clip;
            if (m_clips.TryGetValue(name, out clip))
                return clip;

            AnimationClip[] info = m_animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < info.Length; i++)
            {
                if (info[i].name == name)
                {
                    m_clips.Add(name, info[i]);
                    return info[i];
                }               
            }
        }
        return null;
    }

    public AnimationClip[] GetClips()
    {
        if (null != m_animator && m_animator.layerCount >= 1)
        {
            return m_animator.runtimeAnimatorController.animationClips;
        }
        return null;
    }

    //获得当前动画播放时间
    public override float GetCurFrameTime()
    {
        //正在过渡时，返回的时间不对
        if (m_curClip != null)
        {
            return (Time.time - m_curSpeedTimePass) * m_animator.speed + m_lastSpeedTimePass;
        }
        //没有动画直接返回结束了
        else if (m_curClip == null)
            return 10000;
        //其他情况当时间没有开始
        else
            return 0;

    }

    //获得动画真实帧
    public override int GetCurTrueAniFrame()
    {
        if (m_animator.IsInTransition(0))
        {
            return (int)(m_animator.GetAnimatorTransitionInfo(0).normalizedTime * m_crossFadeNormalizedTime);
        }
        else
        {
            AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
            return (int)(m_curClip.length * info.normalizedTime * xys.battle.AniConst.AnimationFrameRate);
        }
    }

    //当前动画是否播放结束
    public override bool IsFinish()
    {
        if (null != m_animator && m_curClip != null)
        {
            ////正在过渡不算结束
            //if (m_animator.IsInTransition(0))
            //    return false;
            //if (m_animator.GetCurrentAnimatorClipInfo(0).Length == 0)
            //    return true;
            if (GetCurFrameTime() >= m_curClip.length)
                return true;

            return false;
        }
        return true;
    }

    //获得当前动画名字
    public override string GetCurName()
    {
        return m_curStateName;
    }

    //设置当前动画速度
    public override void SetCurSpeed(float speed)
    {
        if (m_animator != null && m_curClip != null)
        {
            //重新记录时间
            m_lastSpeedTimePass = GetCurFrameTime();
            m_curSpeedTimePass = Time.time;

            m_animator.speed = speed;
            float normalizedTime = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            m_animator.Play(GetCurName(), 0, normalizedTime);
            m_animator.Update(0);
        }
    }

    //获得当前动画速度
    public override float GetSpeed()
    {
        if (m_animator != null)
            return m_animator.speed;
        else
            return 1;
    }

}


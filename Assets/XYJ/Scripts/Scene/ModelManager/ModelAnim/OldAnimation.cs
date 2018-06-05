/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;

public class OldAnimation : AnimationWrap
{
    Animation m_ani;
    AnimationState m_curst;
    public OldAnimation(Animation ani)
    {
        m_ani = ani;
    }

    public override void Clear()
    {

    }

    public override int GetCurTrueAniFrame()
    {
        return 0;
    }
    //播放动画
    public override void PlayAni(string name, float speed = 1.0f, float normalizedTime = 0, bool isLoop = false, bool isfade = false, int layer = 0, float crossFadeNormalizedTime = 0)
    {
        m_curst = m_ani[name];
        if (m_curst != null)
        {
            m_curst.speed = speed;
            m_ani.wrapMode = isLoop ? WrapMode.Loop : WrapMode.ClampForever;
            if (isfade)
            {
                m_ani.CrossFade(name); ;
            }
            else
            {
                m_ani.Stop();
                m_ani.Play(name);
            }
        }
    }

    public override bool IsLoop()
    {
        return m_ani.wrapMode == WrapMode.Loop;
    }

    public override bool IsLoop( string name)
    {
        AnimationState st = m_ani[name];
        if (st != null)
            return st.wrapMode == WrapMode.Loop;
        return false; 
    }

    //获得当前动画播放时间
    public override float GetCurFrameTime()
    {
        if (m_curst != null)
            return m_curst.time;
        else
            return 0;
    }

    public override string GetCurName()
    {
        if (m_curst != null)
            return m_curst.name;
        else
            return null;
    }
    //当前动画是否播放结束
    public override bool IsFinish()
    {
        if (m_curst != null)
            return m_curst.time >= m_curst.length || !m_curst.enabled;
        else
            return true;
    }


    //获得动画播放时长
    public override float GetLength(string name)
    {
        AnimationState st = m_ani[name];
        if (st != null)
            return st.length;
        return -1;
    }

    public override float GetFrameRate(string name)
    {
        AnimationState st = m_ani[name];
        if (st != null)
            return st.clip.frameRate;
        return -1;
    }

    //设置当前动画速度
    public override void SetCurSpeed(float speed)
    {
        if (m_curst!=null)
            m_curst.speed = speed;
    }

    //获得当前动画速度
    public override float GetSpeed()
    {
        if (m_curst != null)
            return m_curst.speed;
        else
            return 1;
    }

    public override AnimationClip GetClip(string name)
    {
        //TODO
        return null;
    }
}

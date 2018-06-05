using UnityEngine;
using System.Collections.Generic;
using Battle;
using xys.battle;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

/// <summary>
/// 姿态设置有两种情况，一种是在技能动作里添加，如果动作被中断了没添加出来，则在技能结束时会重新添加一个默认的
/// </summary>
public partial class AnimationEffectManage : MonoBehaviour
{
    //只会同时存在一个
    EffectPosture m_postrueEffect;

    //姿态切换时和技能结束时都会调用
    //设置姿态特效,需要把姿态特效销毁.技能结束的时候也会调用，这时候如果没有设置姿态特效就加上默认的
    public void SetPosture( int posture )
    {
        //销毁旧姿态
        if (m_postrueEffect != null && posture != m_postrueEffect.GetPosture())
        {
            m_postrueEffect.DestroyPostrue();
            m_postrueEffect = null;
        }

#if !SCENE_DEBUG
        //正在播放技能的时候不需要添加默认姿态特效，因为技能动作中可能会添加
        if (!m_role.battle.m_skillMgr.IsPlaying())
        {
            if (m_postrueEffect == null && posture == PostureType.JianKe_YuQi)
                XYJObjectPool.LoadPrefab("fx_nanjianke_flySowrd_rootA", OnLoadPostureEffect, posture);
        }
#endif
    }

    //加载姿态特效回调
    void OnLoadPostureEffect( GameObject go,object para )
    {
        EffectPosture effectPosture = go.GetComponent<EffectPosture>();
        if (effectPosture != null)
        {
            effectPosture.PlayEvent(this);
            //临时这样跟随
            EffectFollowBone.AddFollow(go,m_boneManage.GetBone("model"),true,Vector3.zero);
        }
    }

    //设置姿态特效
    public void SetCurrentPostureEffect(EffectPosture effectPosture)
    {
        ClearPosture();
        m_postrueEffect = effectPosture;
    }

    void ClearPosture()
    {
        if (m_postrueEffect != null)
            m_postrueEffect.DestroyPostrue();
        m_postrueEffect = null;
    }

    //和姿态特效交互,一般控制动画
    public void SendPostureEvent(EventTrigger.EventType msg, object para)
    {
        if (m_postrueEffect != null)
            m_postrueEffect.OnMessage(msg,null);
    }
}

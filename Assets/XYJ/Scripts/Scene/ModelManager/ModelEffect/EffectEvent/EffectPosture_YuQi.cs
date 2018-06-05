using UnityEngine;
using System.Collections;
using Battle;
using System.Collections.Generic;

/// <summary>
/// 御气特效
/// </summary>
public class EffectPosture_YuQi : EffectPosture
{
    AnimationEffectManage m_triggerManage;

    public override int GetPosture()
    {
        return xys.battle.PostureType.JianKe_YuQi;
    }

#if !SCENE_DEBUG
    //过渡用参数
    float lerpTime;
    float expandLength = 0.2f;
    float narrowLength = 2;
    AnimationState state;

    void Update()
    {
        //if (!state)
        //{
        //    return;
        //}
        //if (MainTime.timePass - lerpTime <= expandLength)
        //{
        //    state.speed = Mathf.Lerp(1f, 3f, (MainTime.timePass - lerpTime) / expandLength);
        //}
        //else if (MainTime.timePass - lerpTime - expandLength <= narrowLength)
        //{
        //    state.speed = Mathf.Lerp(3f, 1f, (MainTime.timePass - lerpTime - expandLength) / narrowLength);
        //}
    }
#endif


    protected override void PlayEvent(AnimationEffectManage effectmanage)
    {
        m_triggerManage = effectmanage;
        m_triggerManage.SetCurrentPostureEffect(this);
#if !SCENE_DEBUG
        //AddEvent(effectmanage.m_role);
#endif
        //if (m_flySowrdAni == null)
        //{ 
        //    m_flySowrdAni = transform.GetComponentInChildren<Animator>(true);
        //}
        //m_flySowrdAni.SetBool("PlayState", true);
    }

//    void Start()
//    {
//#if !SCENE_DEBUG
//        if (m_role != null)
//        { 
//            m_role.m_eventManage.AddEventListener<Skill>(RoleEventDefine.PlaySkill, OnSkillPlay);
//        }
//#endif
//    }
//    void OnDestory()
//    {
//#if !SCENE_DEBUG
//        m_role.m_eventManage.RemoveEventListener<Skill>(RoleEventDefine.PlaySkill, OnSkillPlay);
//        //m_flySowrdAni = null;
//#endif
//    }


    void OnDisable()
    {
#if !SCENE_DEBUG
        //RemoveEvent();
#endif
        m_triggerManage = null;
    }

    //接受消息
    protected override void OnMessage(EventTrigger.EventType msg, object para)
    {
    }



    #region 战斗消息交互
#if !SCENE_DEBUG
    //BaseRole m_role;
    //void AddEvent(BaseRole role)
    //{
    //    if (role == null)
    //        return;
    //    m_role = role;
    //    m_role.m_eventManage.AddEventListener<Skill>(RoleEventDefine.PlaySkill, OnPlaySkill);
    //}
    //void RemoveEvent()
    //{
    //    if (m_role == null || m_role.m_eventManage == null)
    //        return;
    //    m_role.m_eventManage.RemoveEventListener<Skill>(RoleEventDefine.PlaySkill, OnPlaySkill);
    //    m_role = null;
    //}

    //void OnPlaySkill(Skill skill)
    //{
    //    //if (m_flySowrdAni == null)
    //    //    return;
    //    //int layer = Random.Range(1, 6);
    //    //m_flySowrdAni.Play("attack_" + layer, layer);
    //}


    ////播放技能
    //void OnSkillPlay(Battle.Skill skill)
    //{
    //    //只有普攻才会切换特效状态
    //    if (skill == null || this == null || skill.prototype.type != SkillPrototypeManage.Type.NormalAttack)
    //        return;

    //    state = transform.FindChild("scaleGrp").GetComponentInChildren<Animation>()["loop"];
    //    lerpTime = MainTime.timePass;

    //}
#endif
    #endregion
}

using UnityEngine;
using System.Collections;
using xys;
#pragma warning disable


public partial class EventTriggerLogic : BaseEffectTriggerLogic
{
    EventTrigger m_data;
    bool m_isTrigger = false;   //效果是否触发了
    bool m_isFinish = false;

    public EventTriggerLogic(EventTrigger data)
    {
        m_data = data;
    }

    public override void Play(AnimationEffectManage triggerManage)
    {
        m_isTrigger = false;
        m_isFinish = false;
    }

    public override void Update(AnimationEffectManage triggerManage, int frame, int loopCnt, string ani)
    {
        if (m_data.m_eventType == EventTrigger.EventType.Null)
        {
            return;
        }
        if (m_isFinish)
            return;

        bool isFrame = (m_data.m_beginFrame <= frame);
        if (!m_isTrigger)
        {
            if (isFrame)
            {
                m_isTrigger = true;
                EventEnter(m_data.m_eventType, triggerManage);
            }
        }

        //到了结束帧
        if (m_data.m_stopFrame != -1 && frame >= m_data.m_stopFrame)
        {
            EventExit(m_data.m_eventType, triggerManage);
        }
    }

    public override void Finish(AnimationEffectManage triggerManage)
    {
        EventExit(m_data.m_eventType, triggerManage);
    }

    void EventEnter(EventTrigger.EventType eventType, AnimationEffectManage triggerManage)
    {
        //设置了本地玩家才有效
        if (m_data.m_isPlayOnlyMe && !triggerManage.m_isMe)
            return;
        InitEvent();
        BaseEventAction e;
        if (s_event.TryGetValue(eventType, out e))
            e.OnEnter(triggerManage, m_data);
    }

    void EventExit(EventTrigger.EventType eventType, AnimationEffectManage triggerManage)
    {
        if (m_isFinish || !m_isTrigger)
            return;
        m_isFinish = true;
        m_isTrigger = false;
#if !SCENE_DEBUG
        if (App.my.cameraMgr.m_mainCamera == null)
            return;
#endif
        //设置了本地玩家才有效
        if (m_data.m_isPlayOnlyMe && !triggerManage.m_isMe)
            return;

        InitEvent();
        BaseEventAction e;
        if (s_event.TryGetValue(eventType, out e))
            e.OnExit(triggerManage, m_data);
    }

    #region 事件类型处理
    //隐藏模型
    static public void HideModel(bool hide, GameObject role)
    {
        Renderer[] render = role.GetComponentsInChildren<Renderer>();
        ParticleEmitter[] particle = role.GetComponentsInChildren<ParticleEmitter>();

        for (int i = 0; i < render.Length; ++i)
        {
            render[i].enabled = !hide;
        }

        for (int i = 0; i < particle.Length; ++i)
        {
            particle[i].enabled = !hide;
        }
    }

    //给特效工具使用
    static public void HideModelWrap(bool hide, AnimationEffectManage triggerManage)
    {
#if SCENE_DEBUG
        HideModel(hide, triggerManage.gameObject);
#else
        triggerManage.m_role.battle.actor.SetHide(hide);
#endif
    }

    //隐藏部件
    static public void HidePart(bool hide, GameObject role,string partName )
    {
        Transform t = BoneManage.GetBone(role.transform,partName);
        if (t == null)
            return;
        t.gameObject.SetActive(!hide);
    }

    #endregion
}

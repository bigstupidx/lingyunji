using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//动作可以设置各种事件
[System.Serializable]
public partial class EventTrigger : BaseEffectTrigger
{
    /// <summary>
    /// 动作里面可以绑定事件,事件id定了以后就不能修改了
    /// </summary>
    public enum EventType
    {
        Null = 0,
        HideModel = 1,              //隐藏模型(结束帧填-1表示动作结束时不恢复回来）
        UI_MASK = 4,                //UI遮罩(必杀技)
        DestroyEffect = 5,            //删除特效(根据名字)
        HidePart = 6,                 //隐藏部件（结束帧填-1表示动作结束时不恢复回来）
    }

    public EventType m_eventType = EventType.Null;

    public int m_beginFrame;
    public int m_stopFrame = -1;
    public bool m_isPlayOnlyMe = false;     //只有本地玩家才生效
    public string m_para;//参数

}

public partial class EventTriggerLogic : BaseEffectTriggerLogic
{
    //事件基类
    class BaseEventAction
    {
        public virtual void OnEnter(AnimationEffectManage triggerManage, EventTrigger data) { }
        public virtual void OnExit(AnimationEffectManage triggerManage, EventTrigger data) { }
    }

    //隐藏模型
    class HideModelEvent : BaseEventAction
    {
        public override void OnEnter(AnimationEffectManage triggerManage, EventTrigger data)
        {
            HideModelWrap(true, triggerManage);
        }
        public override void OnExit(AnimationEffectManage triggerManage, EventTrigger data)
        {
            if(data.m_stopFrame != -1)
                HideModelWrap(false, triggerManage);
        }
    }

    //隐藏部件
    class HidePartEvent : BaseEventAction
    {
        public override void OnEnter(AnimationEffectManage triggerManage, EventTrigger data)
        {
            HidePart(true, triggerManage.gameObject, data.m_para);
        }
        public override void OnExit(AnimationEffectManage triggerManage, EventTrigger data)
        {
            HidePart(false, triggerManage.gameObject, data.m_para);
        }
    }

    //ui遮罩
    class UIMaskEvent : BaseEventAction
    {
#if !SCENE_DEBUG
        public override void OnEnter(AnimationEffectManage triggerManage, EventTrigger data)
        {
            //CameraManage.Instance.StartStuntSkill();
        }
        public override void OnExit(AnimationEffectManage triggerManage, EventTrigger data)
        {
            //CameraManage.Instance.FinishStuntSkill();
        }
#endif
    }

    //删除特效
    class DestroyEffectEvent : BaseEventAction
    {
        public override void OnEnter(AnimationEffectManage triggerManage, EventTrigger data)
        {
            triggerManage.DestroyEffectByName(data.m_para);
        }
    }

    //注册所有事件
    static void InitEvent()
    {
        if (s_isInit)
            return;
        s_isInit = true;
        s_event.Add(EventTrigger.EventType.HideModel, new HideModelEvent());
        s_event.Add(EventTrigger.EventType.UI_MASK, new UIMaskEvent());
        s_event.Add(EventTrigger.EventType.DestroyEffect, new DestroyEffectEvent());
        s_event.Add(EventTrigger.EventType.HidePart, new HidePartEvent());
    }
    static bool s_isInit;
    static Dictionary<EventTrigger.EventType, BaseEventAction> s_event = new Dictionary<EventTrigger.EventType, BaseEventAction>();
}

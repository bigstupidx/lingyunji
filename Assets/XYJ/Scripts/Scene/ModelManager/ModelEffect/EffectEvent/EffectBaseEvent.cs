using UnityEngine;
using System.Collections;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

//特效事件
public class EffecttBaseEvent : MonoBehaviour, IEffectBaseEvent
{
    //播放事件
    protected virtual void PlayEvent(AnimationEffectManage effectmanage )
    {
           
    }

    //接受消息
    protected virtual void OnMessage(EventTrigger.EventType msg, object para)
    {

    }

    public void PlayEvent(IEffectManage effectmanage)
    {
        PlayEvent((AnimationEffectManage)effectmanage);
    }

    public void OnMessage(object msg, object para)
    {
        OnMessage((EventTrigger.EventType)msg, para);
    }
}


//姿态特效，这些特效需要和动作有交互
public class EffectPosture : EffecttBaseEvent
{
    //获得特效姿态
    public virtual int GetPosture() { return -1; }

    //删除姿态特效
    public void DestroyPostrue()
    {
        XYJObjectPool.Destroy(gameObject);
    }
}

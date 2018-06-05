using UnityEngine;
using System.Collections;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

//特效事件
public interface IEffectBaseEvent
{
    void PlayEvent(IEffectManage effectmanage);
    void OnMessage(object msg, object para);
}

public interface IEffectManage
{
    GameObject gameObject { get; }
}
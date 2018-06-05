using UnityEngine;
using System.Collections;

//特效触发配置类
public class BaseEffectTrigger
#if MEMORY_CHECK
        : MemoryObject
#endif
{
}

//特效触发逻辑类
public class BaseEffectTriggerLogic
#if MEMORY_CHECK
        : MemoryObject
#endif
{
    public virtual void Play(AnimationEffectManage triggerManage) { }
    public virtual void Update(AnimationEffectManage triggerManage, int frame, int loopCnt, string ani) { }
    public virtual void Finish(AnimationEffectManage triggerManage) { }
}

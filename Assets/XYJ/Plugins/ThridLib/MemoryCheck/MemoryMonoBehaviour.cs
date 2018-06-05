#if MEMORY_CHECK
using UnityEngine;

public class MemoryMonoBehaviour : MonoBehaviour
{
    public MemoryMonoBehaviour()
    {
        MemoryObjectMgr.Add(this);
    }

    ~MemoryMonoBehaviour()
    {
        MemoryObjectMgr.Sub(this);
    }
}

#endif
using System;
using UnityEngine;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

public class UIHintBase<T> : MonoBehaviour
{
    private static T m_instance;
    public static T Instance()
    {
        return m_instance;
    }
    public static void SetInstance(T instance)
    {
        m_instance = instance;
    }

    void OnDestroy()
    {
        Release();
    }

    void Release()
    {
        m_instance = default(T);
    }
}


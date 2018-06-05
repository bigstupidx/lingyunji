#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
#if COM_DEBUG || UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class MissingScript : MonoBehaviour
{
    public static bool is_handle = false;

    public static bool isSet = false; // 是否有设置
}
#endif

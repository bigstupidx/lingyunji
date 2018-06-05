#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourcesRecords : MonoBehaviour
{
    //public byte[] bytes; // 总的数据
    public Component[] components; // 组件列表

#if UNITY_EDITOR
    [System.NonSerialized]
    public bool isRec = false;
#endif
}


#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneResRecords : MonoBehaviour
{
    // 这些数据保存到对应的场景文件.byte当中
    //public byte[] bytes; // 总的数据
    //public int[] positions; // 组件列表对应在数据当中的位置起始位置
    public Component[] components; // 组件列表

#if UNITY_EDITOR
    [System.NonSerialized]
    public bool isRec = false;
#endif
}


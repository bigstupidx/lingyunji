#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
#if USE_RESOURCESEXPORT && (UNITY_EDITOR || COM_DEBUG)
using UnityEngine;
using System.Collections;

public class ShaderExport : MonoBehaviour
{
    public Shader mShader; // 着色器
}
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LightInfoParent : MonoBehaviour
{
    [SerializeField]
    int lightmapIndex;

    [SerializeField]
    Vector4 lightmapScaleOffset;

    void OnEnable()
    {
        Renderer self = GetComponent<Renderer>();

#if UNITY_EDITOR
        Renderer parent = transform.parent.GetComponent<Renderer>();
        if (parent != null)
        {
            lightmapIndex = parent.lightmapIndex;
            lightmapScaleOffset = parent.lightmapScaleOffset;
        }
#endif
        if (self != null)
        {
            self.lightmapIndex = lightmapIndex;
            self.lightmapScaleOffset = lightmapScaleOffset;
        }

#if UNITY_EDITOR

#else
        Object.Destroy(this);
#endif
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LightUV : MonoBehaviour
{
    public Vector4 lightmapScaleOffset;

    public Vector4 currentLightmapScaleOffset;

    void OnEnable()
    {
        GetComponent<Renderer>().lightmapScaleOffset = lightmapScaleOffset;

        currentLightmapScaleOffset = GetComponent<Renderer>().lightmapScaleOffset;
    }
}

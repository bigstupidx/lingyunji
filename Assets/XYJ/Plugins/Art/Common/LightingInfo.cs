using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class LightingInfo : MonoBehaviour
{
#if UNITY_EDITOR
    [System.Serializable]
    class Info
    {
        public Renderer renderer;
        public int lightmapIndex;
        public Vector4 lightmapScaleOffset;
    }

    [SerializeField]
    List<Info> Infos = new List<Info>();

    // Use this for initialization
    void OnEnable()
    {
        Infos.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.lightmapIndex != -1)
            {
                Info i = new Info();
                i.renderer = r;
                i.lightmapIndex = r.lightmapIndex;
                i.lightmapScaleOffset = r.lightmapScaleOffset;

                Infos.Add(i);
            }
        }
    }
#endif
}

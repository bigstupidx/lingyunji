using UnityEngine;
using System.Collections;

public class EffectDynamicShadow : MonoBehaviour 
{
    void Awake()
    {
        ComLayer.SetSkinnedRenderLayer(this.gameObject,ComLayer.Layer_ProjectorRender);
    }

    void OnEnable()
    {
        DrawTargetObjectManage.AddShadow(this.transform);
    }

    void OnDisable()
    {
        DrawTargetObjectManage.RemoveShadow(this.transform);

    }
}

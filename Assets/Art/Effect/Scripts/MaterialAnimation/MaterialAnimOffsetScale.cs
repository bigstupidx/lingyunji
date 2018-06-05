using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MaterialAnimation))]
public class MaterialAnimOffsetScale : MaterialAnimBase
{
    public Vector2 offset;
    public Vector2 scale;

    public override void OnUpdate(Material mat)
    {
        mat.SetTextureOffset(propName, offset);
        mat.SetTextureScale(propName, scale);
    }
}
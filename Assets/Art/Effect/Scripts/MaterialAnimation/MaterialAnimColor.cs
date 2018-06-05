using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MaterialAnimation))]
public class MaterialAnimColor : MaterialAnimBase
{
    public Color color;

    public override void OnUpdate(Material mat)
    {
        mat.SetColor(propName, color);
    }
}
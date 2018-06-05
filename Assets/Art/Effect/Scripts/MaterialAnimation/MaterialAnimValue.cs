using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MaterialAnimation))]
public class MaterialAnimValue : MaterialAnimBase
{
    public float value;

    public override void OnUpdate(Material mat)
    {
        mat.SetFloat(propName, value);
    }
}
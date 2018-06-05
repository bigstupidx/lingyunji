using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MaterialAnimation))]
public class MaterialAnimVector : MaterialAnimBase
{
    public Vector4 value;

    public override void OnUpdate(Material mat)
    {
        mat.SetVector(propName, value);
    }
}
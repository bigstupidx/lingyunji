using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MaterialAnimation : MonoBehaviour
{
    // 源材质
    [PackTool.Pack]
    public Material srcMaterial;

    public virtual void ResetMaterial(Material mat)
    {

    }

    public virtual void LateUpdate()
    {

    }
}

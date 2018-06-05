using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MaterialAnimation))]
public abstract class MaterialAnimBase : MonoBehaviour
{
    public string propName;

    public abstract void OnUpdate(Material mat);
}

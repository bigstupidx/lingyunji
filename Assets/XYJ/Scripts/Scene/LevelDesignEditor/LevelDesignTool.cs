#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelDesignTool
{
    public static GameObject CreateNode(Transform parent, GameObject prefab = null)
    {
        GameObject child = null;
        if (prefab == null)
            child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        else
            child = GameObject.Instantiate(prefab);
        child.transform.parent = parent.transform;
        child.transform.position = XTools.Utility.GetViewScenePosition();

        return child;
    }
}
#endif
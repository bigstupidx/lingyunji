using UnityEngine;
using System.Collections.Generic;

public class ReflectionOpt : MonoBehaviour
{
    ReflectionProbe[] ReflectionProbes;

    private void Awake()
    {
        ReflectionProbes = GetComponentsInChildren<ReflectionProbe>();
    }

    private void LateUpdate()
    {
        Camera main = Camera.main;
        if (main == null)
            return;

        Vector3 pos = main.transform.position;
        for (int i = 0; i < ReflectionProbes.Length; ++i)
        {
            var bounds = ReflectionProbes[i].bounds;
            bounds.Expand(30);
            if (bounds.Contains(pos))
                ReflectionProbes[i].enabled = true;
            else
                ReflectionProbes[i].enabled = false;
        }
    }
}
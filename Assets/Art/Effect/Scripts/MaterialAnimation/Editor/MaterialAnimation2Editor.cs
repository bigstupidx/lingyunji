using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(MaterialAnimation2), true)]
public class MaterialAnimation2Editor : MaterialAnimationEditor
{
    public override void OnInspectorGUI()
    {
        MaterialAnimation ma = target as MaterialAnimation;
        Material old = ma.srcMaterial;

        ma.srcMaterial = (Material)EditorGUILayout.ObjectField("srcMat", ma.srcMaterial, typeof(Material), false);
        if (old != ma.srcMaterial)
            ma.ResetMaterial(ma.srcMaterial);

        List<MaterialAnimBase> mabs = new List<MaterialAnimBase>(ma.gameObject.GetComponents<MaterialAnimBase>());
        if (ma.srcMaterial == null)
        {
            foreach (MaterialAnimBase mab in mabs)
            {
                DestroyImmediate(mab);
            }

            return;
        }

        if (!ma.gameObject.activeInHierarchy)
            ma.LateUpdate();

        OnShowMaterailProperties(mabs, ma.srcMaterial);
    }
}
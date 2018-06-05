using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(MaterialAnimation), true)]
public class MaterialAnimationEditor : Editor
{
    protected static T AddMaterialAnimBase<T, T1, T2, T3, T4>(GameObject go) 
        where T : MaterialAnimBase
        where T1 : T
        where T2 : T
        where T3 : T
        where T4 : T
    {
        List<T> lists = new List<T>();
        go.GetComponents<T>(lists);
        if (lists.Count == 0)
            return go.AddComponent<T>();

        HashSet<System.Type> types = new HashSet<System.Type>();
        for (int i = 0; i < lists.Count; ++i)
            types.Add(lists[i].GetType());

        if (!types.Contains(typeof(T1)))
            return go.AddComponent<T1>();

        if (!types.Contains(typeof(T2)))
            return go.AddComponent<T2>();

        if (!types.Contains(typeof(T3)))
            return go.AddComponent<T3>();

        if (!types.Contains(typeof(T4)))
            return go.AddComponent<T4>();

        return null;
    }

    public override void OnInspectorGUI()
    {
        if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
        {
            base.OnInspectorGUI();
            return;
        }

        Material srcMat = null;
        MaterialAnimation ma = target as MaterialAnimation;
        Renderer r = ma.GetComponent<Renderer>();
        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
            if (r != null)
                EditorGUILayout.ObjectField("dstMat", r.material, typeof(Material), false);
            return;
        }

        List<MaterialAnimBase> mabs = new List<MaterialAnimBase>(ma.gameObject.GetComponents<MaterialAnimBase>());
        if (r == null || ((srcMat = r.sharedMaterial) == null))
        {
            foreach (MaterialAnimBase mab in mabs)
            {
                Object.DestroyImmediate(mab);
            }

            return;
        }

        if (srcMat == null)
            return;

        Material copy_mat = null;
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(srcMat)))
        {
            copy_mat = srcMat;
            srcMat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(srcMat.name));
        }

        EditorGUILayout.ObjectField("srcMat", srcMat, typeof(Material), false);
        if (srcMat == null)
        {
            return;
        }

        if (!ma.gameObject.activeInHierarchy)
            ma.LateUpdate();

        OnShowMaterailProperties(mabs, srcMat);
    }

    protected void OnShowMaterailProperties(List<MaterialAnimBase> mabs, Material srcMat)
    {
        MaterialAnimation ma = target as MaterialAnimation;
        MaterialProperty[] props = MaterialEditor.GetMaterialProperties(new Object[] { srcMat });
        foreach (MaterialProperty prop in props)
        {
            if (mabs.Find((MaterialAnimBase mab) => { return mab.propName == prop.name ? true : false; }))
                continue;

            switch (prop.type)
            {
            case MaterialProperty.PropType.Color:
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ColorField(prop.name, prop.colorValue);
                    if (GUILayout.Button("添加"))
                    {
                        MaterialAnimColor mac = AddMaterialAnimBase<MaterialAnimColor, MaterialAnimColor1, MaterialAnimColor2, MaterialAnimColor3, MaterialAnimColor4>(ma.gameObject);
                        mac.propName = prop.name;
                        mac.color = prop.colorValue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                break;
            case MaterialProperty.PropType.Vector:
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Vector4Field(prop.name, prop.vectorValue);
                    if (GUILayout.Button("添加"))
                    {
                        MaterialAnimVector mac = AddMaterialAnimBase<MaterialAnimVector, MaterialAnimVector1, MaterialAnimVector2, MaterialAnimVector3, MaterialAnimVector4>(ma.gameObject);
                        mac.propName = prop.name;
                        mac.value = prop.vectorValue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                break;
            case MaterialProperty.PropType.Float:
            case MaterialProperty.PropType.Range:
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.FloatField(prop.name, prop.floatValue);
                    if (GUILayout.Button("添加"))
                    {
                        MaterialAnimValue mac = AddMaterialAnimBase<MaterialAnimValue, MaterialAnimValue1, MaterialAnimValue2, MaterialAnimValue3, MaterialAnimValue4>(ma.gameObject);
                        mac.propName = prop.name;
                        mac.value = prop.floatValue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                break;
            case MaterialProperty.PropType.Texture:
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Vector4Field(prop.name, prop.textureScaleAndOffset);
                    if (GUILayout.Button("添加"))
                    {
                        MaterialAnimOffsetScale mac = AddMaterialAnimBase<MaterialAnimOffsetScale, MaterialAnimOffsetScale1, MaterialAnimOffsetScale2, MaterialAnimOffsetScale3, MaterialAnimOffsetScale4>(ma.gameObject);
                        mac.propName = prop.name;
                        Vector4 v = prop.textureScaleAndOffset;
                        mac.scale = new Vector2(v.x, v.y);
                        mac.offset = new Vector2(v.z, v.w);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                break;
            }
        }
    }
}
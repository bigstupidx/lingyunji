using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 该脚本需要renderer组件，并且只对一个renderer有效
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class MaterialAnimation1 : MaterialAnimation
{
    [SerializeField]
    string mat_guid; 

    MaterialAnimBase[] animBase = null;

    Renderer _renderer = null;
    Renderer mRenderer
    {
        get
        {
            if (_renderer == null)
                _renderer = GetComponent<Renderer>();

            return _renderer;
        }
    }

#if UNITY_EDITOR && !USE_RESOURCESEXPORT
    void OnDisable()
    {
        if (Application.isPlaying)
            return;

        ResetMaterial();
    }

    void ResetMaterial()
    {
        Material mat = mRenderer != null ? mRenderer.sharedMaterial : null;
        if (mat != null && string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(mat)))
        {
            Material src = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(UnityEditor.AssetDatabase.GUIDToAssetPath(mat.name));
            if (src != null)
            {
                mRenderer.sharedMaterial = src;
            }
        }
    }
#endif

    void SetMaterial()
    {
#if !UNITY_EDITOR
        animBase = GetComponents<MaterialAnimBase>();
#endif
    }

    void Awake()
    {
        if (mRenderer == null)
        {
#if !UNITY_EDITOR
            enabled = false;
#endif
            return;
        }

        if (srcMaterial != null && mRenderer.sharedMaterial == null)
        {
            mRenderer.sharedMaterial = srcMaterial;
        }

        SetMaterial();

#if UNITY_EDITOR && !USE_RESOURCESEXPORT
        ResetMaterial();
#endif
    }

    public override void LateUpdate()
    {
        if (mRenderer == null)
            return;

        Material mat = null;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (mRenderer != null && (mat = mRenderer.sharedMaterial) != null)
            {
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(mat);
                if (string.IsNullOrEmpty(assetPath))
                {

                }
                else
                {
                    mat_guid = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
                    mat = new Material(mat);
                    mat.name = mat_guid;

                    mRenderer.sharedMaterial = mat;
                }
            }
            else
            {
                mat_guid = null;
            }
        }
        else
        {
            mat = mRenderer.material;
        }
        animBase = GetComponents<MaterialAnimBase>();
#else
        mat = mRenderer.material;
#endif

        if (animBase == null || animBase.Length == 0 || mat == null)
            return;

        for (int i = 0; i < animBase.Length; ++i)
            animBase[i].OnUpdate(mat);
    }

#if UNITY_EDITOR && !USE_RESOURCESEXPORT
    void OnValidate()
    {
        if (!string.IsNullOrEmpty(mat_guid))
        {
            mRenderer.sharedMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(UnityEditor.AssetDatabase.GUIDToAssetPath(mat_guid));
        }
    }
#endif
}

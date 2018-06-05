#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 关卡编辑器   区域集
/// </summary>
[ExecuteInEditMode]
public class LevelDesignArea : MonoBehaviour
{
    public LevelDesignConfig.LevelAreaData m_data;

    void OnEnable()
    {
        if (null == m_data)
            m_data = new LevelDesignConfig.LevelAreaData();
    }

    public void SetData(LevelDesignConfig.LevelAreaData data)
    {
        m_data = data;
    }

    public void AddAreaToSet(bool isFirst = false)
    {
        GameObject child = LevelDesignTool.CreateNode(transform);
        Selection.activeGameObject = gameObject;

        child.gameObject.name = transform.childCount.ToString();
        //需要给对象增加碰撞类型，默认是矩形碰撞
        SetColliderType(child, LevelDesignConfig.LevelAreaData.AreaType.Rect);

        //移除不必要的控件
        DestroyImmediate(child.GetComponent<Renderer>());
        DestroyImmediate(child.GetComponent<MeshFilter>());
    }

    public LevelDesignConfig.LevelAreaData.AreaType GetAreaType(GameObject go)
    {
        Collider collider = go.GetComponent<Collider>();
        if (collider is BoxCollider)
            return LevelDesignConfig.LevelAreaData.AreaType.Rect;
        else
            return LevelDesignConfig.LevelAreaData.AreaType.Round;
    }

    //设置区域碰撞的类型
    void SetColliderType(GameObject go, LevelDesignConfig.LevelAreaData.AreaType type)
    {
        Collider collider = go.GetComponent<Collider>();
        switch (type)
        {
            case LevelDesignConfig.LevelAreaData.AreaType.Rect:
                {
                    // 添加一个box
                    if (collider != null)
                    {
                        if (collider is BoxCollider)
                            return;

                        collider.enabled = false;
                        collider.hideFlags = HideFlags.HideInInspector;
                        DestroyImmediate(collider);
                    }

                    go.AddComponent<BoxCollider>();
                }
                break;
            case LevelDesignConfig.LevelAreaData.AreaType.Round:
                {
                    // 添加一个球
                    if (collider != null)
                    {
                        if (collider is SphereCollider)
                            return;

                        collider.enabled = false;
                        collider.hideFlags = HideFlags.HideInInspector;
                        DestroyImmediate(collider);
                    }

                    go.AddComponent<SphereCollider>();
                }
                break;
        }
    }

    public void SetColliderType(GameObject go, int type)
    {
        SetColliderType(go, (LevelDesignConfig.LevelAreaData.AreaType)type);
    }

    void Update()
    {
        CollectData();
    }

    public void CollectData()
    {
        m_data.m_postions.Clear();
        m_data.m_dirs.Clear();
        m_data.m_scales.Clear();
        m_data.m_names.Clear();
        m_data.m_types.Clear();
        m_data.m_centers.Clear();
        m_data.m_sizes.Clear();
        m_data.m_radiuses.Clear();

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            m_data.m_postions.Add(child.position);
            m_data.m_dirs.Add(child.eulerAngles);
            m_data.m_scales.Add(child.localScale);
            m_data.m_names.Add(child.name);
            //区域类型
            LevelDesignConfig.LevelAreaData.AreaType areaType = GetAreaType(child.gameObject);
            m_data.m_types.Add(areaType);
            if (areaType == LevelDesignConfig.LevelAreaData.AreaType.Rect)
            {
                BoxCollider collider = child.GetComponent<BoxCollider>();
                m_data.m_centers.Add(collider.center);
                m_data.m_sizes.Add(collider.size);
                m_data.m_radiuses.Add(0);
            }
            else
            {
                SphereCollider collider = child.GetComponent<SphereCollider>();
                m_data.m_centers.Add(collider.center);
                m_data.m_sizes.Add(Vector3.zero);
                m_data.m_radiuses.Add(collider.radius);
            }
        }
    }
}
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 该脚本的renderer组件需要动态设置，并且支持多个,材质是叠加而不是替换
/// 查找父节点的所有子节点的renderer组件
/// </summary>
public class MaterialAnimation2 : MaterialAnimation
{
    Material copy_mat;
    MaterialAnimBase[] m_animBase;
    List<Renderer> m_renders;

    void OnEnable()
    {
        if (srcMaterial == null)
        {
            enabled = false;
            Debuger.ErrorLog("srcMaterial == null! name:{0}", name);
            return;
        }

        if (m_animBase == null)
        {
            m_animBase = GetComponents<MaterialAnimBase>();
            copy_mat = new Material(srcMaterial);
        }
    }

    public void Play()
    {
        //查找父节点的所有子节点的renderer组件.给每一个渲染都添加一个材质
        if (transform.parent != null && m_renders == null)
        {
            Renderer[] temRender = transform.parent.gameObject.GetComponentsInChildren<Renderer>();

            if (temRender != null)
            {
                m_renders = new List<Renderer>();
                for (int i = 0; i < temRender.Length; i++)
                {
                    if (temRender[i] is MeshRenderer || temRender[i] is SkinnedMeshRenderer)
                    {
                        m_renders.Add(temRender[i]);
                        ImplAddMaterial(m_renders[i], true, copy_mat);
                    }
                }
            }
        }
    }

    void OnDisable()
    {
        if (m_renders != null)
        {
            for (int i = 0; i < m_renders.Count;i++ )
                ImplAddMaterial(m_renders[i], false, copy_mat);
        }
        m_renders = null;
    }

    //增加或移除一个材质
    bool ImplAddMaterial(Renderer render, bool add, Material newMaterial)
    {
        if (render == null)
            return false;

        //优化内存
        Material[] curMaterials = render.sharedMaterials;

        //添加
        if (add)
        {
            Material[] tem = new Material[curMaterials.Length + 1];
            for (int i = 0; i < curMaterials.Length; i++)
                tem[i] = curMaterials[i];
            tem[tem.Length - 1] = newMaterial;
            render.sharedMaterials = tem;
        }
        //移除
        else
        {
            int findI = -1;
            for (int i = 0; i < curMaterials.Length; i++)
            {
                //正常情况应该是直接移动相同的实例
                if (curMaterials[i] == newMaterial)
                {
                    findI = i;
                    break;
                }
            }

            //找不到
            if (findI == -1)
                return false;

            Material[] tem = new Material[curMaterials.Length - 1];
            int index = 0;
            bool find = false;
            for (int i = 0; i < curMaterials.Length; i++)
            {
                if ((findI == i) && !find)
                    find = true;
                else
                    tem[index++] = curMaterials[i];
            }
            render.sharedMaterials = tem;
        }
        return true;
    }


    public override void LateUpdate()
    {
        if (srcMaterial == null || m_animBase == null || m_animBase.Length == 0 || m_renders == null || m_renders.Count==0)
        {
            return;
        }

        for (int i = 0; i < m_animBase.Length; ++i)
            m_animBase[i].OnUpdate(copy_mat);
    }
}

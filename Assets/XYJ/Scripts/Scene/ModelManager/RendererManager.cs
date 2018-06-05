using System;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    /// <summary>
    /// 角色材质管理
    /// </summary>
    public class RendererManager
    {

        // 记录材质球的渲染队列
        Dictionary<int, int> m_allDefaultRenderQueue = new Dictionary<int, int>();
        //每个渲染器可能有多个材质球，缓存下来
        Dictionary<Renderer, List<Material>> m_allDefaultMaterials = new Dictionary<Renderer, List<Material>>();
        //每个材质的原始颜色值
        private Dictionary<int, Color> m_allDefaultMaterialsColor = new Dictionary<int, Color>();
        //忽略的材质列表
        private List<int> m_ignoreMaterials = new List<int>();

        Transform m_root;
        public void Clear()
        {
            m_allDefaultRenderQueue.Clear();
            m_allDefaultMaterials.Clear();
            m_allDefaultMaterialsColor.Clear();
            m_ignoreMaterials.Clear();
        }

        //设置角色渲染顺序
        public void SetRenderQueue(int queue,bool isReset)
        {
            foreach (var kv in m_allDefaultMaterials)
            {
                foreach( var p in kv.Value)
                {
                    if(isReset)
                    {
                        if (m_allDefaultRenderQueue.TryGetValue(p.GetInstanceID(), out queue))
                            p.renderQueue = queue;
                    }
                    else
                        p.renderQueue = queue;
                }
            }
        }

        //设置颜色
        public void SetColor(Color color,bool reset)
        {
            foreach (var kv in m_allDefaultMaterials)
            {
                foreach (var p in kv.Value)
                {
                    if (p.HasProperty("_Color"))
                    {
                        if(reset)
                        {
                            if (m_allDefaultMaterialsColor.TryGetValue(p.GetInstanceID(),out color))
                                p.SetColor("_Color", color);
                        }                        
                        else
                            p.SetColor("_Color", color);
                    }                  
                }
            }
        }

        //增加一个材质
        public void AddMaterial(Material mat)
        {
            foreach (var kv in m_allDefaultMaterials)
            {
                List<Material> matList = new List<Material>();
                matList.AddRange(kv.Value);
                matList.Add(mat);
                kv.Key.materials = matList.ToArray();
            }
        }

        //切换材质
        public void ChangeMaterial(Material mat)
        {
            foreach (var kv in m_allDefaultMaterials)
            {
                kv.Key.material = mat;
            }
        }

        //还原材质
        public void ResetMaterial()
        {
            foreach (var kv in m_allDefaultMaterials)
            {
                kv.Key.materials = kv.Value.ToArray();
            }
        }


        //初始化渲染数据
        public void InitRenderPara( Transform root )
        {
            GameObject go = root.gameObject;
            Renderer[] renders = go.GetComponentsInChildren<Renderer>();

            //记录材质渲染queue
            m_allDefaultRenderQueue.Clear();
            if (renders != null)
            {
                for (int i = 0; i < renders.Length; i++)
                {
                    if (renders[i] is SkinnedMeshRenderer || renders[i] is MeshRenderer)
                    {
                        Material[] materials = renders[i].materials;
                        for (int j = 0; j < materials.Length; j++)
                        {
                            if (!m_ignoreMaterials.Contains(materials[j].GetInstanceID()))
                                m_allDefaultRenderQueue.Add(materials[j].GetInstanceID(), materials[j].renderQueue);
                        }
                    }
                }
            }

            //记录渲染器的材质列表
            m_allDefaultMaterials.Clear();
            m_allDefaultMaterialsColor.Clear();
            if (renders != null)
            {
                for (int i = 0; i < renders.Length; i++)
                {
                    if (renders[i] is SkinnedMeshRenderer || renders[i] is MeshRenderer)
                    {
                        Material[] materials = renders[i].materials;
                        m_allDefaultMaterials.Add(renders[i], new List<Material>(materials));
                        for (int j = 0; j < materials.Length; j++)
                        {
                            if (materials[j].HasProperty("_Color"))
                            {
                                m_allDefaultMaterialsColor.Add(materials[j].GetInstanceID(), materials[j].GetColor("_Color"));
                            }
                        }
                    }
                }
            }
        }

        public void SetIgnoreMaterials(int id)
        {
            if (!m_ignoreMaterials.Contains(id))
            {
                m_ignoreMaterials.Add(id);
            }
        }
    }
}

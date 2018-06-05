using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace UI
{
    public class SoftMaskModle : MonoBehaviour
    {
        static List<Renderer> Renderers = new List<Renderer>();

        public class Data
        {
            public Renderer renderer;
            //public Material[] srcMaterials; // 源材质
            //public Material[] dstMaterials; // 拷贝的材质

            const string mask_key = " Soft Mask";

            public void Restore()
            {
                Material[] materials = renderer.materials;
                for (int j = 0; j < materials.Length; ++j)
                {
                    Shader shader;
                    Material mat = materials[j];
                    if (mat != null && ((shader = mat.shader) != null))
                    {
                        string n = shader.name;
                        if (n.EndsWith(mask_key))
                        {
                            Shader s = Shader.Find(n.Remove(n.Length - mask_key.Length));
                            if (s != null)
                                mat.shader = s;
                        }
                    }
                }

                renderer.materials = materials;
            }

            public void Set(UI.RectSoftAlphaMask mask)
            {
                Material[] materials = renderer.materials;
                XTools.Utility.RelpaceShader(materials, mask_key);
                mask.UpdateMaterial(materials);

                renderer.materials = materials;
            }
        }

        public List<Data> mDatas = new List<Data>();

        public void Set()
        {
            if (mDatas.Count == 0)
                return;

            RectSoftAlphaMask mask = GetComponentInParent<RectSoftAlphaMask>();
            if (mask == null)
                return;

            for (int i = 0; i < mDatas.Count; ++i)
            {
                mDatas[i].Set(mask);
            }
        }

        void Restore()
        {
            for (int i = 0; i < mDatas.Count; ++i)
            {
                mDatas[i].Restore();
            }
        }

        void Start()
        {
            Renderers.Clear();
            gameObject.GetComponentsInChildren(true, Renderers);

            mDatas.Capacity = Renderers.Count;
            for (int i = 0; i < Renderers.Count; ++i)
            {
                Data d = new Data();
                d.renderer = Renderers[i];
                mDatas.Add(d);
            }

            Set();
        }

        void Update()
        {
            Set();
        }

//         void OnDisable()
//         {
//             Restore();
//         }

        void OnEnable()
        {
            Set();
        }
    }
}
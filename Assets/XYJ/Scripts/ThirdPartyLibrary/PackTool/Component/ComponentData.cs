using XTools;
using System.IO;
using UnityEngine;

namespace PackTool
{
#if USE_RESOURCESEXPORT
    // 组件所需要保存的数据
    public abstract partial class ComponentData
    {
        public static string GetSavePath(string src, string suffix)
        {
            int index = src.LastIndexOf('.');
            if (index != -1)
                src = src.Substring(0, index) + "_" + src.Substring(index + 1);

            src += suffix;
            return src;
        }

#if UNITY_EDITOR
        // 收集此组件的数据
        public abstract bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr);

        protected static Material[] CheckMaterialEditor(Material[] mats)
        {
            foreach (Material mat in mats)
            {
                if (mat != null && mat.shader != null)
                {
                    Shader s = Shader.Find(mat.shader.name);
                    if (s != null)
                        mat.shader = s;
                }
            }

            return mats;
        }

        protected static void CheckMaterialEditor(Renderer renderer)
        {
            renderer.sharedMaterials = CheckMaterialEditor(renderer.sharedMaterials); ;
        }
#endif

        // 组件数据加载完成
        public delegate void OnComponentEnd(object p);

        public sealed class Data
        {
            public Data()
            {

            }

            public void Reset(ParamData pd)
            {
                mParamData = pd;
                mTotal = 0;
                param = null;
            }

            public ParamData mParamData;
            public int mTotal = 0;

            // 附加参数
            public object param { get; set; }

            public Component mComponent { get { return mParamData.component; } }

            public void Release()
            {
                mParamData = null;
                mTotal = 0;
                param = null;

                Buff<Data>.Free(this);
            }

            public void OnEnd()
            {
                --mTotal;
                if (mTotal == 0)
                {
                    mParamData.OnEnd();

                    Release();
                }
                else if (mTotal < 0)
                {
                    Debuger.ErrorLog("mTotal < 0");
                }
            }
        }

        public class ParamData
        {
            public ParamData()
            {

            }

            public void Reset(Component c, OnComponentEnd e, object ep)
            {
                component = c;
                end = e;
                endp = ep;

                parent = null;
            }

            public Component component;
            public OnComponentEnd end;
            public object endp;

            public ComponentSave parent = null;

            internal void OnEnd()
            {
                if (end != null)
                {
                    end(endp);
                }

                end = null;
                endp = null;
                component = null;

                parent = null;
                Buff<ParamData>.Free(this);
            }
        }

        // 动态时资源加载,返回当前加载的资源数量
        public abstract Data LoadResources(ParamData pd, BinaryReader reader);
    }
#endif
}
#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Edelweiss.CloudSystem;

namespace PackTool
{
    public class CS_CloudPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            // 网格渲染器，需要保存下材质数据
            CS_Cloud com = component as CS_Cloud;
            Material mat = com.ParticleMaterial;
            if (__CollectMaterial__(ref mat, writer, mgr))
            {
                MatRefValue.SetValue(com, mat);
                return true;
            }
            return false;
        }
#endif

        static FieldInfo MatRefValue_ = null;

        static FieldInfo MatRefValue
        {
            get
            {
                if (MatRefValue_ == null)
                {
                    MatRefValue_ = typeof(Cloud<CS_Cloud, CS_ParticleData, CS_CreatorData>).GetField("m_ParticleMaterial", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
                }

                return MatRefValue_;
            }
        }

        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[] { data, 1 });

            return data;
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            CS_Cloud com = data.mComponent as CS_Cloud;
            switch (index)
            {

            case 1:
                MatRefValue.SetValue(com, mat);
                break;

            }
            data.OnEnd();
        }
    }
}
#endif
#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ParticleSystemData : RendererData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            ParticleSystem particleSystem = component as ParticleSystem;
            ParticleSystem.ShapeModule sm = particleSystem.shape;
            Mesh mesh = sm.mesh;
            if (__CollectMesh__(ref mesh, writer, mgr))
            {
                sm.mesh = null;
                return true;
            }
            return false;
        }
#endif
        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);
            __LoadMesh__(data, reader, LoadMeshEnd, data);

            return data;
        }

        static void LoadMeshEnd(Mesh mesh, object p)
        {
            Data data = p as Data;

            ParticleSystem particleSystem = data.mComponent as ParticleSystem;
            ParticleSystem.ShapeModule sm = particleSystem.shape;
            sm.mesh = mesh;

            data.OnEnd();
        }
    }
}
#endif
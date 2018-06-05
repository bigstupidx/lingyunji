#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class MeshFilterData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            // 网格渲染器，需要保存下材质数据
            MeshFilter meshFilter = component as MeshFilter;
            Mesh mesh = meshFilter.sharedMesh;
            if (__CollectMesh__(ref mesh, writer, mgr))
            {
                meshFilter.sharedMesh = null;
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

            MeshFilter meshFilter = data.mComponent as MeshFilter;
            meshFilter.sharedMesh = mesh;

            data.OnEnd();
        }
    }
}
#endif
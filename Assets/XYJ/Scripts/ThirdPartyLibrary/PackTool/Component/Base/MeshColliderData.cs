#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class MeshColliderData : ComponentData
    {
#if UNITY_EDITOR
        static Mesh DefaultMesh = null;
        static Mesh _DefaultMesh_
        {
            get
            {
                if (DefaultMesh == null)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DefaultMesh = obj.GetComponent<MeshFilter>().sharedMesh;
                    Object.DestroyImmediate(obj);
                }

                return DefaultMesh;
            }
        }

        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            // 网格渲染器，需要保存下材质数据
            MeshCollider meshCollider = component as MeshCollider;
            Mesh mesh = meshCollider.sharedMesh;
            if (__CollectMesh__(ref mesh, writer, mgr))
            {
                meshCollider.sharedMesh = _DefaultMesh_;
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

            MeshCollider meshCollider = data.mComponent as MeshCollider;
            meshCollider.sharedMesh = mesh;

            data.OnEnd();
        }
    }
}
#endif
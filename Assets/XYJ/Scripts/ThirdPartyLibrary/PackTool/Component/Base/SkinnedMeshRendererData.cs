#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class SkinnedMeshRendererData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            // 网格渲染器，需要保存下材质数据
            SkinnedMeshRenderer renderer = component as SkinnedMeshRenderer;

            // 先收集下材质
            Material[] mats = renderer.sharedMaterials;
            has |= __CollectList__<Material>(mats, writer, mgr, __CollectMaterial__);
            renderer.sharedMaterials = mats;

            Mesh mesh = renderer.sharedMesh;
            if (__CollectMesh__(ref mesh, writer, mgr))
            {
                renderer.sharedMesh = mesh;
                has = true;
            }

            return has;
        }

#endif
        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
#if UNITY_EDITOR
            CheckMaterialEditor(pd.component as Renderer);
#endif

            Data data = CreateData(pd);
            __LoadAssetList__<Material>(0, data, reader, __LoadMaterial__, LoadMaterialsEndList);
            __LoadMesh__(data, reader, LoadMeshEnd, data);

            return data;
        }

        public static void LoadMaterialsEndList(Material mat, object p)
        {
            object[] ps = (object[])p;
            Data data = (Data)ps[0];
            int index = (int)ps[1];

            SkinnedMeshRenderer com = data.mComponent as SkinnedMeshRenderer;
            Material[] mats = com.sharedMaterials;
            mats[index] = mat;
            com.sharedMaterials = mats;

            data.OnEnd();
        }

        public static void LoadMeshEnd(Mesh mesh, object p)
        {
            Data data = (Data)p;
            SkinnedMeshRenderer com = data.mComponent as SkinnedMeshRenderer;
            com.sharedMesh = mesh;

            data.OnEnd();
        }
    }
}
#endif
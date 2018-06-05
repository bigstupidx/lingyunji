#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ParticleSystemRendererData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            // 网格渲染器，需要保存下材质数据
            ParticleSystemRenderer renderer = component as ParticleSystemRenderer;
            Material[] mats = renderer.sharedMaterials;
            has |= __CollectList__<Material>(mats, writer, mgr, __CollectMaterial__);
            renderer.sharedMaterials = mats;

            Mesh[] meshs = new Mesh[renderer.meshCount];
            renderer.GetMeshes(meshs);
            if (__CollectList__(meshs, writer, mgr, __CollectMesh__))
            {
                renderer.SetMeshes(meshs);
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
            object[] param = new object[] { null, null };
            data.param = param;
            int num = __LoadAssetList__<Material>(0, data, reader, __LoadMaterial__, LoadMaterialsEndList);
            if (num > 0)
                param[0] = new Material[num];

            num = __LoadAssetList__<Mesh>(1, data, reader, __LoadMesh__, LoadMeshsEndList);
            if (num > 0)
                param[1] = new Mesh[num];

            return data;
        }

        static void LoadMeshsEndList(Mesh mesh, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            ParticleSystemRenderer com = data.mComponent as ParticleSystemRenderer;
            Mesh[] meshs = (Mesh[])(((object[])data.param)[1]);

            switch (type)
            {
            case 1:
                meshs[index] = mesh;
                break;
            }

            if (data.mTotal == 1)
            {
                com.SetMeshes(meshs);

                Material[] mats = (Material[])(((object[])data.param)[0]);
                if (mats != null)
                    com.sharedMaterials = mats;
            }

            data.OnEnd();
        }

        static void LoadMaterialsEndList(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            ParticleSystemRenderer com = data.mComponent as ParticleSystemRenderer;
            Material[] mats = (Material[])(((object[])data.param)[0]);

            switch (type)
            {
            case 0:
                mats[index] = mat;
                break;

            }

            if (data.mTotal == 1)
            {
                com.sharedMaterials = mats;

                Mesh[] meshs = (Mesh[])(((object[])data.param)[1]);
                if (meshs != null)
                    com.SetMeshes(meshs);
            }

            data.OnEnd();
        }
    }
}
#endif
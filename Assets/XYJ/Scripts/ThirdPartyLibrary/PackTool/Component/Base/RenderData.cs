#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class RendererData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            // 网格渲染器，需要保存下材质数据
            Renderer renderer = component as Renderer;
            Material[] mats = renderer.sharedMaterials;
            has |= __CollectList__< Material>(mats, writer, mgr, __CollectMaterial__);
            renderer.sharedMaterials = mats;

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
            int num = __LoadAssetList__<Material>(0, data, reader, __LoadMaterial__, LoadMaterialsEndList);
            if (num != 0)
                data.param = new Material[num];

            return data;
        }

        public static void LoadMaterialsEndList(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            //int type = (int)pp[2];
            Renderer com = data.mComponent as Renderer;
            Material[] mats = data.param as Material[];
            mats[index] = mat;

            if (data.mTotal == 1)
            {
                com.sharedMaterials = mats;
            }

            data.OnEnd();
        }
    }
}
#endif
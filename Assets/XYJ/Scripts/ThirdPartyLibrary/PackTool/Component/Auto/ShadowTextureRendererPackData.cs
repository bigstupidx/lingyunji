#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ShadowTextureRendererPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            DynamicShadowProjector.ShadowTextureRenderer com = component as DynamicShadowProjector.ShadowTextureRenderer;

            has |= __CollectMaterial__(ref com.m_blurShader, writer, mgr);

            has |= __CollectMaterial__(ref com.m_downsampleShader, writer, mgr);

            has |= __CollectMaterial__(ref com.m_copyMipmapShader, writer, mgr);

            has |= __CollectMaterial__(ref com.m_eraseShadowShader, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 0});

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 1});

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 2});

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 3});

            return data;
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            DynamicShadowProjector.ShadowTextureRenderer com = data.mComponent as DynamicShadowProjector.ShadowTextureRenderer;
            switch (index)
            {

            case 0:
                com.m_blurShader = mat;
                break;

            case 1:
                com.m_downsampleShader = mat;
                break;

            case 2:
                com.m_copyMipmapShader = mat;
                break;

            case 3:
                com.m_eraseShadowShader = mat;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif
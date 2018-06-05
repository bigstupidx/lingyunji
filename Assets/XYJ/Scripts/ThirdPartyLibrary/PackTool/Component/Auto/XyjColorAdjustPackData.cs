#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class XyjColorAdjustPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            XyjColorAdjust com = component as XyjColorAdjust;

            has |= __CollectTexture__(ref com.m_lutTexture, writer, mgr);

            has |= __CollectShader__(ref com.m_tonemapper, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{data, 0});

            __LoadShader__(data, reader, LoadShaderEnd, new object[]{data, 1});

            return data;
        }

        static void LoadTextureEnd(Texture texture, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            XyjColorAdjust com = data.mComponent as XyjColorAdjust;
            switch (index)
            {

            case 0:
                com.m_lutTexture = texture as Texture2D;
                break;

            }
            data.OnEnd();
        }

        static void LoadShaderEnd(Shader shader, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            XyjColorAdjust com = data.mComponent as XyjColorAdjust;
            switch (index)
            {

            case 1:
                com.m_tonemapper = shader;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif
#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DynamicShadowProjector;

namespace PackTool
{
    public class DrawTargetObjectPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            DrawTargetObject com = component as DrawTargetObject;

            DrawTargetObject.ReplaceShader[] dtr = com.replacementShaders;
            int lenght = (dtr == null ? 0 : dtr.Length);
            Shader[] shaders = new Shader[lenght];
            for (int i = 0; i < lenght; ++i)
            {
                shaders[i] = dtr[i].shader;
            }

            has |= __CollectList__<Shader>(shaders, writer, mgr, __CollectShader__);

            for (int i = 0; i < lenght; ++i)
            {
                dtr[i].shader = shaders[i];
            }

            Material mat = com.shadowShader;
            if (__CollectMaterial__(ref mat, writer, mgr))
            {
                com.shadowShader = mat;
                has = true;
            }

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadAssetList__<Shader>(0, data, reader, __LoadShader__, LoadShaderEndList);

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 1});

            return data;
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            DrawTargetObject com = data.mComponent as DrawTargetObject;
            switch (index)
            {

            case 1:
                com.shadowShader = mat;
                break;

            }
            data.OnEnd();
        }

        static void LoadShaderEndList(Shader shader, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            DrawTargetObject com = data.mComponent as DrawTargetObject;
            
            switch(type)
            {

            case 0:
                com.replacementShaders[index].shader = shader;
                break;

            }

            data.OnEnd();
        }

    }
}
#endif
#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class AfterimageEffectPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            AfterimageEffect com = component as AfterimageEffect;

            has |= __CollectMesh__(ref com.shaderMesh, writer, mgr);

            has |= __CollectMaterial__(ref com.shaderMaterial, writer, mgr);

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMesh__(data, reader, LoadMeshEnd, new object[]{data, 0});

            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{data, 1});

            return data;
        }

        static void LoadMaterialEnd(Material mat, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            AfterimageEffect com = data.mComponent as AfterimageEffect;
            switch (index)
            {

            case 1:
                com.shaderMaterial = mat;
                break;

            }
            data.OnEnd();
        }

        static void LoadMeshEnd(Mesh mesh, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            AfterimageEffect com = data.mComponent as AfterimageEffect;
            switch (index)
            {

            case 0:
                com.shaderMesh = mesh;
                break;

            }
            data.OnEnd();
        }

    }
}
#endif
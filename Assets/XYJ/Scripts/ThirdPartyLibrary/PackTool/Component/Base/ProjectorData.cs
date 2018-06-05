#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class ProjectorData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            // 网格渲染器，需要保存下材质数据
            Projector projector = component as Projector;
            Material mat = projector.material;
            if (__CollectMaterial__(ref mat, writer, mgr))
            {
                projector.material = null;
                has = true;
            }

            return has;
        }

#endif

        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadMaterial__(data, reader, LoadMaterialEnd, data);

            return data;
        }

        public static void LoadMaterialEnd(Material mat, object p)
        {
            Data data = p as Data;
            Projector com = data.mComponent as Projector;
            com.material = mat;
            data.OnEnd();
        }
    }
}
#endif
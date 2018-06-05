#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class RoleSkinConfigPackData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            RoleSkinConfig com = component as RoleSkinConfig;
            for (int i = 0; i < com.parts.Count; ++i)
            {
                RoleSkinPart part = com.parts[i];
                has |= __CollectTexture2DAsset__(ref part.orgTexture, writer, mgr);
                for (int j = 0; j < part.units.Count; ++j)
                {
                    var units = part.units[j];
                    has |= __CollectTexture2DAsset__(ref units.shareMask, writer, mgr);
                    for (int k = 0; k < units.texStyles.Count; ++k)
                    {
                        var texStyles = units.texStyles[k];
                        has |= __CollectTexture2DAsset__(ref texStyles.texName, writer, mgr);
                        has |= __CollectTexture2DAsset__(ref texStyles.maskName, writer, mgr);
                    }
                }
            }

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);
            RoleSkinConfig com = pd.component as RoleSkinConfig;
            for (int i = 0; i < com.parts.Count; ++i)
            {
                RoleSkinPart part = com.parts[i];
                __LoadT2DAsset__(data, reader, LoadEnd, new object[] { data, 0, part });
                for (int j = 0; j < part.units.Count; ++j)
                {
                    var units = part.units[j];
                    __LoadT2DAsset__(data, reader, LoadEnd, new object[] { data, 1, units });
                    for (int k = 0; k < units.texStyles.Count; ++k)
                    {
                        var texStyles = units.texStyles[k];
                        __LoadT2DAsset__(data, reader, LoadEnd, new object[] { data, 2, texStyles });
                        __LoadT2DAsset__(data, reader, LoadEnd, new object[] { data, 3, texStyles });
                    }
                }
            }

            return data;
        }

        static void LoadEnd(Texture2DAsset asset, object p)
        {
            object[] pp = p as object[];
            Data d = pp[0] as Data;
            int type = (int)pp[1];
            switch (type)
            {
            case 0:
                {
                    RoleSkinPart part = pp[2] as RoleSkinPart;
                    part.orgTexture = asset;
                }
                break;
            case 1:
                {
                    RoleSkinUnit unit = pp[2] as RoleSkinUnit;
                    unit.shareMask = asset;
                }
                break;
            case 2:
                {
                    RoleSkinTexStyle texStyles = pp[2] as RoleSkinTexStyle;
                    texStyles.texName = asset;
                }
                break;
            case 3:
                {
                    RoleSkinTexStyle texStyles = pp[2] as RoleSkinTexStyle;
                    texStyles.maskName = asset;
                }
                break;
            default:
                {
                    Debuger.ErrorLog("error type:{0}", type);
                }
                break;
            }

            d.OnEnd();
        }
    }
}
#endif
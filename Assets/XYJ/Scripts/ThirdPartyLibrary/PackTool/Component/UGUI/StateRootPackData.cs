#if USE_RESOURCESEXPORT
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using xys.UI.State;

namespace PackTool
{
    public class StateRootPackData : ComponentData
    {
#if UNITY_EDITOR
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            StateRoot sr = component as StateRoot;

            List<Element> elements = sr.elements;
            ushort elementCount = (ushort)elements.Count;
            writer.Write(elementCount);

            for (int i = 0; i < elementCount; ++i)
            {
                ElementStateData[] eds = elements[i].stateData;

                ushort edsl = (ushort)eds.Length;
                writer.Write(edsl);

                for (int m = 0; m < eds.Length; ++m)
                {
                    ElementStateData esd = eds[m];
                    has |= __CollectObject__(ref esd.obj, writer, mgr);
                }
            }

            return has;
        }
#endif
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            StateRoot sr = pd.component as StateRoot;
            List<Element> elements = sr.elements;
            ushort elementCount = reader.ReadUInt16();
            for (int i = 0; i < elementCount; ++i)
            {
                ElementStateData[] eds = elements[i].stateData;
                /*ushort edsl = */reader.ReadUInt16();

                for (int m = 0; m < eds.Length; ++m)
                {
                    ElementStateData esd = eds[m];
                    __LoadObject__(data, reader, LoadObjectEnd, new object[] { data, esd });
                }
            }

            return data;
        }

        static void LoadObjectEnd(Object obj, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            ElementStateData esd = pp[1] as ElementStateData;
            esd.obj = obj;

            data.OnEnd();
        }
    }
}
#endif
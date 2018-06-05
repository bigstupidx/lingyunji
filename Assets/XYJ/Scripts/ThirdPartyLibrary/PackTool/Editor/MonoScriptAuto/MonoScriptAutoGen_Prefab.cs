using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PackTool
{
    public partial class MonoScriptAutoGen
    {
        public partial class PrefabText
        {
            public const string CollectText = @"
            has |= __CollectPrefab__(ref com.{1}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadPrefab__(data, reader, PrefabLoadEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = go;
                break;
";

            public const string LoadEndFun =
    @"
        static void PrefabLoadEnd(GameObject go, object p)
        {{
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            {0} com = data.mComponent as {0};
            switch (index)
            {{
{1}
            }}
            data.OnEnd();
        }}
";
        }
        static void Gen_Prefab(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(PrefabText.CollectText, index, info.Name, "\"\""));

            // 加载接口
            data.loadtext.Append(string.Format(PrefabText.LoadText, index, "\".p\""));

            // 设置
            data.prefabloadend.Append(string.Format(PrefabText.LoadendText, index, info.Name));
        }

        static void Gen_PrefabFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(PrefabText.LoadEndFun, GetTypeName(type), data.prefabloadend);
        }
    }
}
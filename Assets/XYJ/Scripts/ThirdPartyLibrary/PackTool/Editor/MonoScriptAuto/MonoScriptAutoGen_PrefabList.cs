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
            public const string CollectTextList = @"
            has |= __CollectList__<GameObject>(com.{0}, writer, mgr, __CollectPrefab__);
";

            public const string LoadTextList =
    @"
            __LoadAssetList__<GameObject>({0}, data, reader, __LoadPrefab__, PrefabLoadEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = go;
                break;
";

            public const string LoadEndFunList =
    @"
        static void PrefabLoadEndList(GameObject go, object p)
        {{
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];
            int type = (int)pp[2];
            {0} com = data.mComponent as {0};
            
            switch(type)
            {{
{1}
            }}

            data.OnEnd();
        }}
";
        }
        static void Gen_PrefabList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(PrefabText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(PrefabText.LoadTextList, index));

            // 设置
            data.prefabloadendlist.Append(string.Format(PrefabText.LoadendTextList, index, info.Name));
        }

        static void Gen_PrefabFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(PrefabText.LoadEndFunList, GetTypeName(type), data.prefabloadendlist);
        }
    }
}
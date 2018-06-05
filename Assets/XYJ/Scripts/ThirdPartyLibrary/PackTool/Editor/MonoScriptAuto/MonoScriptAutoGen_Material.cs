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
        public partial class MaterialText
        {
            public const string CollectText = @"
            has |= __CollectMaterial__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadMaterial__(data, reader, LoadMaterialEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = mat;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadMaterialEnd(Material mat, object p)
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

        static void Gen_Material(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(MaterialText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(MaterialText.LoadText, index));

            // 设置
            data.materialloadend.Append(string.Format(MaterialText.LoadendText, index, info.Name));
        }

        static void Gen_MaterialFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(MaterialText.LoadEndFun, GetTypeName(type), data.materialloadend);
        }
    }
}
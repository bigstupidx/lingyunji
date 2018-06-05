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
            public const string CollectTextList = @"
            has |= __CollectList__<Material>(com.{0}, writer, mgr, __CollectMaterial__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<Material>({0}, data, reader, __LoadMaterial__, LoadMaterialsEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = mat;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadMaterialsEndList(Material mat, object p)
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

        static void Gen_MaterialList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(MaterialText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(MaterialText.LoadTextList, index));

            // 设置
            data.materialloadendlist.Append(string.Format(MaterialText.LoadendTextList, index, info.Name));
        }

        static void Gen_MaterialListFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(MaterialText.LoadEndFunList, GetTypeName(type), data.materialloadendlist);
        }
    }
}
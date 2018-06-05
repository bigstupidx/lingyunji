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
        public partial class ShaderText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<Shader>(com.{0}, writer, mgr, __CollectShader__);
";

            public const string LoadTextList =
    @"
            __LoadAssetList__<Shader>({0}, data, reader, __LoadShader__, LoadShaderEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = shader;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadShaderEndList(Shader shader, object p)
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

        static void Gen_ShaderList(FieldInfo info, Data data, int index)
        {
            string length = "";
            if (info.FieldType.IsArray)
            {
                length = "Length";
            }
            else
            {
                length = "Count";
            }
            
            // 收集接口
            data.collect.Append(string.Format(ShaderText.CollectTextList, info.Name, length, "\"/resources/\"", "\"\""));

            // 加载接口
            data.loadtext.Append(string.Format(ShaderText.LoadTextList, index, "\".s\""));

            // 设置
            data.shaderloadendlist.Append(string.Format(ShaderText.LoadendTextList, index, info.Name));
        }

        static void Gen_ShaderListFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(ShaderText.LoadEndFunList, type.Name, data.shaderloadendlist);
        }
    }
}
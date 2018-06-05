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
            public const string CollectText = @"
            has |= __CollectShader__(ref com.{1}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadShader__(data, reader, LoadShaderEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = shader;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadShaderEnd(Shader shader, object p)
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
        static void Gen_Shader(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(ShaderText.CollectText, index, info.Name, "\"/resources/\"", "\"\""));

            // 加载接口
            data.loadtext.Append(string.Format(ShaderText.LoadText, index, "\".shader\"", "\".s\""));

            // 设置
            data.shaderloadend.Append(string.Format(ShaderText.LoadendText, index, info.Name));
        }

        static void Gen_ShaderFun(Data data, System.Type type)
        {
            string ns = GetTypeName(type);
            data.loadendfun.AppendFormat(ShaderText.LoadEndFun, ns, data.shaderloadend);
        }
    }
}
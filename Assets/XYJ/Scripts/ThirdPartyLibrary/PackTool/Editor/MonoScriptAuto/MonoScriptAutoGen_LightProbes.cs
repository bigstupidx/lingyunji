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
        public partial class LightProbesText
        {
            public const string CollectText = @"
            has |= __CollectLightProbes__(ref com.{1}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadLightProbes__(data, reader, LoadLightProbesEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = lightProbes;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadLightProbesEnd(LightProbes lightProbes, object p)
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

        static void Gen_LightProbes(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(LightProbesText.CollectText, index, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(LightProbesText.LoadText, index));

            // 设置
            data.lightProbesloadend.Append(string.Format(LightProbesText.LoadendText, index, info.Name));
        }

        static void Gen_LightProbesFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(LightProbesText.LoadEndFun, GetTypeName(type), data.lightProbesloadend);
        }
    }
}
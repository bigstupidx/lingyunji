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
            public const string CollectTextList = @"
            has |= __CollectList__<LightProbes>(com.{0}, writer, mgr, __CollectLightProbes__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<LightProbes>({0}, data, reader, __LoadMaterial__, LoadLightProbesEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = lightProbes;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadLightProbesEndList(LightProbes lightProbes, object p)
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

        static void Gen_lightProbesList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(LightProbesText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(LightProbesText.LoadTextList, index));

            // 设置
            data.lightProbesloadlist.Append(string.Format(LightProbesText.LoadendTextList, index, info.Name));
        }

        static void Gen_lightProbesListFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(LightProbesText.LoadEndFunList, GetTypeName(type), data.lightProbesloadlist);
        }
    }
}
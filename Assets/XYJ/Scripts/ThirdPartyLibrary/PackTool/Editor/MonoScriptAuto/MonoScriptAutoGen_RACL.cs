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
        partial class RunAniConText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<RuntimeAnimatorController>(com.{0}, writer, mgr, __CollectAnimatorController__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<RuntimeAnimatorController>({0}, data, reader, __LoadAnimatorController__, OnRunAniConLoadEnd);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = rac;
                break;
";

            public const string LoadEndFunList =
    @"
        static void OnRunAniConLoadEnd(RuntimeAnimatorController rac, object p)
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

        static void Gen_RunAnimConList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(RunAniConText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(RunAniConText.LoadTextList, index));

            // 设置
            data.runanimconloadend.Append(string.Format(RunAniConText.LoadendTextList, index, info.Name));
        }

        static void Gen_RunAnimConListFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(RunAniConText.LoadEndFunList, GetTypeName(type), data.runanimconloadend);
        }
    }
}
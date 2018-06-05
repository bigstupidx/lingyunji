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
        public partial class RunAnimConText
        {
            public const string CollectText = @"
            has |= __CollectAnimatorController__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadAnimatorController__(data, reader, LoadRunAnimConEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = rac;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadRunAnimConEnd(RuntimeAnimatorController rac, object p)
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

        static void Gen_RunAnimCon(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(RunAnimConText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(RunAnimConText.LoadText, index));

            // 设置
            data.runanimconloadend.Append(string.Format(RunAnimConText.LoadendText, index, info.Name));
        }

        static void Gen_RunAnimConFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(RunAnimConText.LoadEndFun, GetTypeName(type), data.runanimconloadend);
        }
    }
}
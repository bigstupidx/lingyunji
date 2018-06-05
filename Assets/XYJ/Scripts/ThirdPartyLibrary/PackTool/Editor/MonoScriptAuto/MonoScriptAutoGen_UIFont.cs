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
        public partial class UIFontText
        {
            public const string CollectText = @"
            has |= __CollectUIFont__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadUIFont__(data, reader, LoadUIFontEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = font;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadUIFontEnd(UIFont font, object p)
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

        static void Gen_UIFont(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(UIFontText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(UIFontText.LoadText, index));

            // 设置
            data.uifontloadend.Append(string.Format(UIFontText.LoadendText, index, info.Name));
        }

        static void Gen_UIFontFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(UIFontText.LoadEndFun, GetTypeName(type), data.uifontloadend);
        }
    }
}
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
        public partial class FontText
        {
            public const string CollectText = @"
            has |= __CollectFont__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadFontlib__(data, reader, LoadFontEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = font;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadFontEnd(Font font, object p)
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

        static void Gen_Font(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(FontText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(FontText.LoadText, index));

            // 设置
            data.fontloadend.Append(string.Format(FontText.LoadendText, index, info.Name));
        }

        static void Gen_FontFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(FontText.LoadEndFun, GetTypeName(type), data.fontloadend);
        }
    }
}
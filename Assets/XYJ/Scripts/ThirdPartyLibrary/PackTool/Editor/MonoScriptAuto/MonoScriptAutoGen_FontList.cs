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
        partial class FontText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<Font>(com.{0}, writer, mgr, __CollectFont__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<Font>({0}, data, reader, __LoadFontlib__, LoadFontEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = font;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadFontEndList(Font font, object p)
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

        static void Gen_FontList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(FontText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(FontText.LoadTextList, index));

            // 设置
            data.fontloadlist.Append(string.Format(FontText.LoadendTextList, index, info.Name));
        }

        static void Gen_FontFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(FontText.LoadEndFunList, GetTypeName(type), data.fontloadlist);
        }
    }
}
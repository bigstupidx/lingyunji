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
        partial class UIFontText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<UIFont>(com.{0}, writer, mgr, __CollectUIFont__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<UIFont>({0}, data, reader, __LoadUIFont__, LoadUIFontEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = font;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadUIFontEndList(UIFont font, object p)
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

        static void Gen_UIFontList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(UIFontText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(UIFontText.LoadTextList, index));

            // 设置
            data.uifontloadlist.Append(string.Format(UIFontText.LoadendTextList, index, info.Name));
        }

        static void Gen_UIFontFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(UIFontText.LoadEndFunList, GetTypeName(type), data.uifontloadlist);
        }
    }
}
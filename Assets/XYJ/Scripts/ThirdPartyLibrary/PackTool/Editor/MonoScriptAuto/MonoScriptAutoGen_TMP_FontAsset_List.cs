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
        partial class TMP_FontAsset
        {
            public const string CollectTextList = @"
            has |= __CollectList__<TMPro.TMP_FontAsset>(com.{0}, writer, mgr, __CollectTMPFont__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<TMPro.TMP_FontAsset>({0}, data, reader, __LoadTMPFont__, LoadTMPFontEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = font;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadTMPFontEndList(TMPro.TMP_FontAsset font, object p)
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

        static void Gen_TMPFontList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(TMP_FontAsset.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(TMP_FontAsset.LoadTextList, index));

            // 设置
            data.tmpfontAssetloadlist.Append(string.Format(TMP_FontAsset.LoadendTextList, index, info.Name));
        }

        static void Gen_TMPFontFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(TMP_FontAsset.LoadEndFunList, GetTypeName(type), data.tmpfontAssetloadlist);
        }
    }
}
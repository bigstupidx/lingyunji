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
        public partial class TMP_FontAssets
        {
            public const string CollectText = @"
            has |= __CollectTMPFont__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadTMPFont__(data, reader, LoadTMPFontEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = font;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadTMPFontEnd(TMPro.TMP_FontAsset font, object p)
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

        static void Gen_TMPFont(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(TMP_FontAssets.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(TMP_FontAssets.LoadText, index));

            // 设置
            data.tmpfontAssetloadend.Append(string.Format(TMP_FontAssets.LoadendText, index, info.Name));
        }

        static void Gen_TMPFontFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(TMP_FontAssets.LoadEndFun, GetTypeName(type), data.tmpfontAssetloadend);
        }
    }
}
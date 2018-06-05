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
        public partial class UIAtlasText
        {
            public const string CollectText = @"
            has |= __CollectUIAtlas__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadNGUIAtlas__(data, reader, LoadUIAtlasEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = atlas;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadUIAtlasEnd(UIAtlas atlas, object p)
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

        static void Gen_UIAtlas(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(UIAtlasText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(UIAtlasText.LoadText, index));

            // 设置
            data.uiatlasloadend.Append(string.Format(UIAtlasText.LoadendText, index, info.Name));
        }

        static void Gen_UIAtlasFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(UIAtlasText.LoadEndFun, GetTypeName(type), data.uiatlasloadend);
        }
    }
}
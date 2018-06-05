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
        partial class UIAtlasText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<UIAtlas>(com.{0}, writer, mgr, __CollectUIAtlas__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<UIAtlas>({0}, data, reader, __LoadNGUIAtlas__, LoadUIAtlasEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = atlas;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadUIAtlasEndList(UIAtlas atlas, object p)
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

        static void Gen_UIAtlasList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(UIAtlasText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(UIAtlasText.LoadTextList, index));

            // 设置
            data.uiatlasloadlist.Append(string.Format(UIAtlasText.LoadendTextList, index, info.Name));
        }

        static void Gen_UIAtlasFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(UIAtlasText.LoadEndFunList, GetTypeName(type), data.uiatlasloadlist);
        }
    }
}
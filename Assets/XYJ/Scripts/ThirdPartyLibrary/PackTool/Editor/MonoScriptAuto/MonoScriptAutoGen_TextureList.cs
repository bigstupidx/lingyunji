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
        public partial class TextureText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<{0}>(com.{1}, writer, mgr, __CollectTexture__<{0}>);
";

            public const string LoadTextList =
    @"
            __LoadAssetList__<Texture>({0}, data, reader, __LoadTexture__, LoadTextureEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = texture;
                break;
";

            public const string LoadendTextListAs =
    @"
            case {0}:
                com.{1}[index] = texture as {2};
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadTextureEndList(Texture texture, object p)
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

        static void Gen_TextureList(FieldInfo info, Data data, int index)
        {
            System.Type element = GetArrayElementType(info.FieldType);

            // 收集接口
            data.collect.Append(string.Format(TextureText.CollectTextList, GetTypeName(element), info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(TextureText.LoadTextList, index));

            // 设置
            if (element == typeof(Texture))
                data.textureloadendlist.Append(string.Format(TextureText.LoadendTextList, index, info.Name));
            else
                data.textureloadendlist.Append(string.Format(TextureText.LoadendTextListAs, index, info.Name, element.Name));
        }

        static void Gen_TextureListFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(TextureText.LoadEndFunList, type.Name, data.textureloadendlist);
        }
    }
}
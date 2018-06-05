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
            public const string CollectText = @"
            has |= __CollectTexture__(ref com.{0}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadTexture__(data, reader, LoadTextureEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = texture;
                break;
";

            public const string LoadendTextAs =
    @"
            case {0}:
                com.{1} = texture as {2};
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadTextureEnd(Texture texture, object p)
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

        static void Gen_Texture(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(TextureText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(TextureText.LoadText, index));

            // 设置
            if (info.FieldType == typeof(Texture))
                data.textureloadend.Append(string.Format(TextureText.LoadendText, index, info.Name));
            else
                data.textureloadend.Append(string.Format(TextureText.LoadendTextAs, index, info.Name, info.FieldType.Name));
        }

        static void Gen_TextureFun(Data data, System.Type type)
        {
            string ns = GetTypeName(type);
            data.loadendfun.AppendFormat(TextureText.LoadEndFun, ns, data.textureloadend);
        }
    }
}
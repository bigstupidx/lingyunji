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
        public partial class SpriteText
        {
            public const string CollectText = @"
            has |= __CollectSprite__(ref com.{1}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadSprite__(data, reader, LoadSpriteEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = sprite;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadSpriteEnd(Sprite sprite, object p)
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

        static void Gen_Sprite(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(SpriteText.CollectText, index, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(SpriteText.LoadText, index));

            // 设置
            data.spriteloadend.Append(string.Format(SpriteText.LoadendText, index, info.Name));
        }

        static void Gen_SpriteFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(SpriteText.LoadEndFun, GetTypeName(type), data.spriteloadend);
        }
    }
}
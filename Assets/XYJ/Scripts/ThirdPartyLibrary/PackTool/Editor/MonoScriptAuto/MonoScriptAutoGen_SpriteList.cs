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
            public const string CollectTextList = @"
            has |= __CollectList__<Sprite>(com.{0}, writer, mgr, __CollectSprite__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<Sprite>({0}, data, reader, __LoadMesh__, LoadSpriteEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = sprite;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadSpriteEndList(Sprite sprite, object p)
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
        static void Gen_SpriteList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(SpriteText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(SpriteText.LoadTextList, index));

            // 设置
            data.spriteloadlist.Append(string.Format(SpriteText.LoadendTextList, index, info.Name));
        }

        static void Gen_SpriteFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(SpriteText.LoadEndFunList, GetTypeName(type), data.spriteloadlist);
        }
    }
}
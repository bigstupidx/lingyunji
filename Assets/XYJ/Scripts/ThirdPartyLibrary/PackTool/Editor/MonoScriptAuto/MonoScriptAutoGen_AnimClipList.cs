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
        partial class AnimClipText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<AnimationClip>(com.{0}, writer, mgr, __CollectAnimClip__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<AnimationClip>({0}, data, reader, __LoadAnimationClip__, LoadAnimClipEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = clip;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadAnimClipEndList(AnimationClip clip, object p)
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

        static void Gen_AnimClipList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(AnimClipText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(AnimClipText.LoadTextList, index));

            // 设置
            data.animcliploadlist.Append(string.Format(AnimClipText.LoadendTextList, index, info.Name));
        }

        static void Gen_AnimClipFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(AnimClipText.LoadEndFunList, GetTypeName(type), data.animcliploadlist);
        }
    }
}
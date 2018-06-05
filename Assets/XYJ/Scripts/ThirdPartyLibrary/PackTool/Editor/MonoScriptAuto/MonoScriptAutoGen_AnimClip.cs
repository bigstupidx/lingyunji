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
        public partial class AnimClipText
        {
            public const string CollectText = @"
            has |= __CollectAnimClip__(ref com.{0}, writer, mgr);
";

            public const string LoadText = @"
            __LoadAnimationClip__(data, reader, LoadAnimClipEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = clip;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadAnimClipEnd(AnimationClip clip, object p)
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

        static void Gen_AnimClip(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(AnimClipText.CollectText, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(AnimClipText.LoadText, index));

            // 设置
            data.animcliploadend.Append(string.Format(AnimClipText.LoadendText, index, info.Name));
        }

        static void Gen_AnimClipFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(AnimClipText.LoadEndFun, GetTypeName(type), data.animcliploadend);
        }
    }
}
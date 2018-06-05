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
        public partial class AvatarText
        {
            public const string CollectText = @"
            has |= __CollectAvatar__(ref com.{1}, writer, mgr);
";

            public const string LoadText =
    @"
            __LoadAvatar__(data, reader, LoadAvatarEnd, new object[]{{data, {0}}});
";

            public const string LoadendText =
    @"
            case {0}:
                com.{1} = avatar;
                break;
";

            public const string LoadEndFun =
    @"
        static void LoadAvatarEnd(Avatar avatar, object p)
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

        static void Gen_Avatar(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(AvatarText.CollectText, index, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(AvatarText.LoadText, index));

            // 设置
            data.meshloadend.Append(string.Format(AvatarText.LoadendText, index, info.Name));
        }

        static void Gen_AvatarFun(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(AvatarText.LoadEndFun, GetTypeName(type), data.meshloadend);
        }
    }
}
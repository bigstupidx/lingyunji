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
            public const string CollectTextList = @"
            has |= __CollectList__<Avatar>(com.{0}, writer, mgr, __CollectAvatar__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<Avatar>({0}, data, reader, __LoadAvatar__, LoadAvatarEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = avatar;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadAvatarEndList(Avatar avatar, object p)
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
        static void Gen_AvatarList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(AvatarText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(AvatarText.LoadTextList, index));

            // 设置
            data.meshloadendlist.Append(string.Format(AvatarText.LoadendTextList, index, info.Name));
        }

        static void Gen_AvatarFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(AvatarText.LoadEndFunList, GetTypeName(type), data.meshloadendlist);
        }
    }
}
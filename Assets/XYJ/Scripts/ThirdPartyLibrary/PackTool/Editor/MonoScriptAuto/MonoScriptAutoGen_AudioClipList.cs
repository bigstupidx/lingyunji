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
        partial class AudioClipText
        {
            public const string CollectTextList = @"
            has |= __CollectList__<AudioClip>(com.{0}, writer, mgr, __CollectAudioClip__);
";

            public const string LoadTextList = @"
            __LoadAssetList__<AudioClip>({0}, data, reader, __LoadAudioClip__, LoadAudioClipEndList);
";

            public const string LoadendTextList =
    @"
            case {0}:
                com.{1}[index] = clip;
                break;
";

            public const string LoadEndFunList =
    @"
        static void LoadAudioClipEndList(AudioClip clip, object p)
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

        static void Gen_AudioClipList(FieldInfo info, Data data, int index)
        {
            // 收集接口
            data.collect.Append(string.Format(AudioClipText.CollectTextList, info.Name));

            // 加载接口
            data.loadtext.Append(string.Format(AudioClipText.LoadTextList, index));

            // 设置
            data.audiocliploadlist.Append(string.Format(AudioClipText.LoadendTextList, index, info.Name));
        }

        static void Gen_AudioClipFunList(Data data, System.Type type)
        {
            data.loadendfun.AppendFormat(AudioClipText.LoadEndFunList, GetTypeName(type), data.audiocliploadlist);
        }
    }
}
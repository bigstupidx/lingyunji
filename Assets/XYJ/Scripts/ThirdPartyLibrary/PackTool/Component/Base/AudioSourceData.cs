#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class AudioSourceData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            // 网格渲染器，需要保存下材质数据
            AudioSource audio = component as AudioSource;

            AudioClip clip = audio.clip;
            if (__CollectAudioClip__(ref clip, writer, mgr))
            {
                audio.clip = null;
                has = true;
            }

            return has;
        }
#endif

        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadAudioClip__(data, reader, LoadAudioClipEnd, data);

            return data;
        }

        static void LoadAudioClipEnd(AudioClip clip, object p)
        {
            Data data = p as Data;
            AudioSource audio = data.mComponent as AudioSource;
            audio.clip = clip;

            data.OnEnd();
        }
    }
}
#endif
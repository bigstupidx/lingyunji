#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class AnimationData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            // 网格渲染器，需要保存下材质数据
            bool has = false;
            Animation anim = component as Animation;
            writer.Write(anim.clip != null ? anim.clip.name : "");

            AnimationClip[] clips = UnityEditor.AnimationUtility.GetAnimationClips(anim.gameObject);
            anim.clip = null;

            has |= __CollectList__<AnimationClip>(clips, writer, mgr, __CollectAnimClip__);

            if (has)
            {
                UnityEditor.AnimationUtility.SetAnimationClips(anim, new AnimationClip[0]);
            }

            return has;
        }
#endif

        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);
            string current = reader.ReadString();
            object[] ps = new object[2] { current, null };
            data.param = ps;

            int num = __LoadAssetList__<AnimationClip>(0, data, reader, __LoadAnimationClip__, LoadAnimationClipEnd);
            if (num != 0)
                ps[1] = new AnimationClip[num];

            return data;
        }

        static void LoadAnimationClipEnd(AnimationClip clip, object p)
        {
            object[] pp = p as object[];
            Data data = pp[0] as Data;
            int index = (int)pp[1];

            Animation com = data.mParamData.component as Animation;
            AnimationClip[] clips = ((AnimationClip[])(((object[])data.param)[1]));
            clips[index] = clip;

            if (data.mTotal == 1)
            {
                AnimationClip currentclip = null;
                string current = (string)(((object[])data.param)[0]);
                if (!string.IsNullOrEmpty(current))
                {
                    for (int i = 0; i < clips.Length; ++i)
                    {
                        if (clips[i] != null)
                        {
                            string name = clips[i].name;
                            com.AddClip(clips[i], name);
                            if (currentclip == null && current == name)
                            {
                                currentclip = clips[i];
                            }
                            clips[i] = null;
                        }
                        else
                            Debuger.LogError("LoadAnimationClipEnd AnimationClip == null");
                    }

                    if (currentclip != null)
                    {
                        com.clip = currentclip;
                    }
                }
                else
                {
                    for (int i = 0; i < clips.Length; ++i)
                    {
                        if (clips[i] != null)
                        {
                            string name = clips[i].name;
                            com.AddClip(clips[i], name);
                        }
                    }
                }
            }

            data.OnEnd();
        }
    }
}
#endif
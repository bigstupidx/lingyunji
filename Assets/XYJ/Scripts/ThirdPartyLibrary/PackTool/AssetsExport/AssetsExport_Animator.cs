#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public partial class AssetsExport
    {
        static Avatar CreateAvatar(Avatar avatar)
        {
            return CopyObjectClip<Avatar>(avatar, ".asset");
        }

        [MenuItem("Assets/PackTool/AnimatorController")]
        static void TestAnimatorController()
        {
            if (Selection.activeObject is RuntimeAnimatorController)
            {
                CodeCheckAtler.Release();
                AssetsExport mgr = new AssetsExport();
                mgr.CollectAnimator(Selection.activeObject as RuntimeAnimatorController);

                if (!mgr.isEmpty)
                {
                    XTools.GlobalCoroutine.StartCoroutine(mgr.BeginPack());
                }
            }
        }

        static bool IsFbxAnimation(RuntimeAnimatorController rac)
        {
            AnimationClip[] clips = rac.animationClips;
            for (int i = 0; i < clips.Length; ++i)
            {
                if (!AssetDatabase.GetAssetPath(clips[i]).EndsWith(".anim"))
                    return true;
            }

            return false;
        }

        static RuntimeAnimatorController CreateAnimatorController(RuntimeAnimatorController rac, AssetsExport mgr)
        {
            if (!IsFbxAnimation(rac))
                return rac;

            string assetPath = AssetDatabase.GetAssetPath(rac);
            string copypath = string.Format("Assets/__copy__/{0}", assetPath.Substring(7));
            RuntimeAnimatorController copy = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(copypath);
            if (copy != null && !IsNeedUpdate(assetPath.Substring(7)))
                return copy;

            string dst = (Application.dataPath + copypath.Substring(6)).Replace('\\', '/');
            Directory.CreateDirectory(dst.Substring(0, dst.LastIndexOf('/')));
            
//             string dcopydata = copypath.Substring(0, copypath.LastIndexOf('.')) + "_d.controller";
//             AssetDatabase.CopyAsset(assetPath, dcopydata);
//             AssetDatabase.Refresh();

//            rac = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(dcopydata);

            AssetDatabase.CopyAsset(assetPath, copypath);
            AssetDatabase.Refresh();
            copy = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(copypath);
            int count = 0;
            while (copy == null || ((copy as AnimatorController) == null))
            {
                AssetDatabase.ImportAsset(copypath);
                copy = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(copypath);
                if (copy != null)
                {
                    Debug.LogFormat("copy:{0} type:{1}", copypath, copy.GetType().Name);
                }
                ++count;
                if (count >= 100)
                {
                    Debug.LogErrorFormat("copypath:{0} load error!", copypath);
                    break;
                }
            }
            CopyAllAnimClip(copy as AnimatorController);
            AssetDatabase.SaveAssets();

            // 保存依赖
//             {
//                 string ddst = dst + Suffix.AnimConByte;
// 
//                 MemoryStream stream = new MemoryStream(1024);
//                 BinaryWriter writer = new BinaryWriter(stream);
// 
//                 AnimatorOverrideController dc = new AnimatorOverrideController();
//                 dc.runtimeAnimatorController = rac;
//                 AnimationClip[] clips = dc.animationClips;
//                 AnimationData.CollectAnimationList(clips, writer, mgr);
//                 Object.DestroyImmediate(dc);
// 
//                 byte[] bytes = new byte[stream.Length];
//                 stream.Position = 0;
//                 stream.Read(bytes, 0, (int)stream.Length);
//                 stream.Close();
//                 writer.Close();
// 
//                 File.WriteAllBytes(ddst, bytes);
//             }

            return copy;
        }

        static public Motion CopyMotion(Motion motion)
        {
            string path = AssetDatabase.GetAssetPath(motion);
            if (!path.ToLower().EndsWith(".anim"))
                motion = CopyObjectClip<Motion>(motion, ".anim");

            return motion;
        }

        static void CopyAllAnimClip(AnimatorController ac)
        {
            AnimatorControllerLayer[] layers = ac.layers;
            for (int i = 0; i < layers.Length; ++i)
            {
                AnimatorStateMachine asm = layers[i].stateMachine;
                if (asm != null)
                {
                    ChildAnimatorState[] casm = asm.states;
                    for (int j = 0; j < casm.Length; ++j)
                    {
                        Motion motion = casm[j].state.motion;
                        if (motion == null)
                            continue;

                        casm[j].state.motion = CopyMotion(motion);
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }

        static void RemoveAllAnimClip(AnimatorController ac)
        {
            AnimatorControllerLayer[] layers = ac.layers;
            for (int i = 0; i < layers.Length; ++i)
            {
                AnimatorStateMachine asm = layers[i].stateMachine;
                ChildAnimatorState[] casm = asm.states;
                for (int j = 0; j < casm.Length; ++j)
                {
                    Motion motion = casm[j].state.motion;
                    if (motion == null)
                        continue;

                    casm[j].state.motion = null;
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif
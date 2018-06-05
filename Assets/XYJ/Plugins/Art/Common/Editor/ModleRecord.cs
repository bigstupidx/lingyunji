using System.Text;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class ModleRecord
    {
        [System.Serializable]
        public class AnimData
        {
            [System.Serializable]
            public class Data
            {
                public string name;
                public float firstFrame;
                public float lastFrame;
                public bool loopTime;
            }

            public List<Data> anims = new List<Data>();
        }


        [MenuItem("Assets/模型/模型动画数据保存")]
        static void SaveRecord()
        {
            Utility.ForEachSelect(
                (ModelImporter mi) => 
                {
                    string file_path = "ModelAnim/" + mi.assetPath.Substring(7);

                    var clips = mi.clipAnimations;
                    AnimData data = new AnimData();
                    for (int i = 0; i < clips.Length; ++i)
                    {
                        var clip = clips[i];
                        var ad = new AnimData.Data();
                        ad.name = clip.name;
                        ad.firstFrame = clip.firstFrame;
                        ad.lastFrame = clip.lastFrame;
                        ad.loopTime = clip.loopTime;

                        data.anims.Add(ad);
                    }

                    Debug.LogFormat("{0}){1}", Path.GetFileNameWithoutExtension(mi.assetPath), JsonUtility.ToJson(data));
                    Directory.CreateDirectory(file_path.Substring(0, file_path.LastIndexOf('/')));

                    File.WriteAllText(file_path, JsonUtility.ToJson(data));
                },
                (string assetPath, string path) => 
                {
                    return assetPath.EndsWith(".fbx", true, null);
                });
        }

        [MenuItem("Assets/模型/模型动画数据恢复")]
        static void SaveRestore()
        {
            Utility.ForEachSelect(
                (ModelImporter mi) =>
                {
                    var anim = mi.defaultClipAnimations;
                    if (anim.Length == 0)
                        return;

                    string file_path = "ModelAnim/" + mi.assetPath.Substring(7);
                    if (!File.Exists(file_path))
                        return;

                    AnimData data = JsonUtility.FromJson<AnimData>(File.ReadAllText(file_path));

                    List<ModelImporterClipAnimation> mcas = new List<ModelImporterClipAnimation>();
                    for (int i = 0; i < data.anims.Count; ++i)
                    {
                        var mica = mi.defaultClipAnimations[0];
                        var d = data.anims[i];
                        mica.name = d.name;
                        mica.firstFrame = d.firstFrame;
                        mica.lastFrame = d.lastFrame;
                        mica.loopTime = d.loopTime;

                        mcas.Add(mica);
                    }

                    mi.clipAnimations = mcas.ToArray();
                    EditorUtility.SetDirty(mi);
                },
                (string assetPath, string path) =>
                {
                    return assetPath.EndsWith(".fbx", true, null);
                });
        }

        //[MenuItem("Assets/PackTool/模型动画复制")]
        //static void CopyRestore()
        //{
        //    Utility.ForEachSelect(
        //        (ModelImporter mi) =>
        //        {
        //            var anim = mi.clipAnimations;
        //            var objs = AssetDatabase.LoadAllAssetsAtPath(mi.assetPath);
        //            List<AnimationClip> clips = new List<AnimationClip>();
        //            foreach (var obj in objs)
        //            {
        //                if (obj is AnimationClip)
        //                {
        //                    if (obj.name.StartsWith("__preview__"))
        //                        continue;

        //                    clips.Add((AnimationClip)obj);
        //                }
        //            }

        //            string assetPath = mi.assetPath;
        //            assetPath = assetPath.Substring(0, assetPath.LastIndexOf('/')) + "/";
        //            for (int i = 0; i < clips.Count; ++i)
        //            {
        //                var clip = clips[i];
        //                Debug.LogFormat("{0}):{1}", clip.name, clip.length);

        //                clip = Object.Instantiate(clip);
        //                assetPath += (clip.name + ".asset");
        //                AssetDatabase.CreateAsset(clip, assetPath);
        //                Object.Destroy(clip);
        //            }

        //            for (int i = 0; i < anim.Length; ++i)
        //            {
        //                foreach (var curve in anim[i].curves)
        //                {
        //                    Debug.LogFormat("name:{0} lenght:{1}", curve.name, curve.curve.length);
        //                }
        //            }
        //        },
        //        (string assetPath, string path) =>
        //        {
        //            return assetPath.EndsWith(".fbx", true, null);
        //        });
        //}
    }
}
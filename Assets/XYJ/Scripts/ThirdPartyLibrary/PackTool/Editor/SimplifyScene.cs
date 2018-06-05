using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PackTool
{
    public class SimpleScene : TemplatesMarco
    {
        const string marco = "USE_SIMPLESCENE";
        const string path = "";

        [MenuItem("Help/场景精简/开启", true)]
        public static bool BufCannelToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return !macroDefine.has(marco);
        }

        [MenuItem("Help/场景精简/关闭", true)]
        public static bool BufOpenToggle()
        {
            MacroDefine macroDefine = new MacroDefine();
            return macroDefine.has(marco);
        }

        [MenuItem("Help/场景精简/关闭")]
        public static void BufCannel()
        {
            SetEnable(false, marco, path);
        }

        [MenuItem("Help/场景精简/开启")]
        public static void BufOpen()
        {
            SetEnable(true, marco, path);
            if (!Directory.Exists("Assets/__copy__/"))
                Update();
        }

        //#if USE_SIMPLESCENE
        [MenuItem("Help/场景精简/更新数据")]
        public static void Update()
        {
            Delete();
            XTools.GlobalCoroutine.StartCoroutine(Scene());
        }
        //#endif

        [MenuItem("Help/场景精简/清除数据")]
        public static void Delete()
        {
            XTools.Utility.DeleteFolder("Assets/__copy__");
        }

        static IEnumerator Scene()
        {
            XTools.TimeCheck tc = new XTools.TimeCheck(true);
            string current = EditorSceneManager.GetActiveScene().path;
            List<string> copy_scenes = new List<string>();
            IEnumerator ator = XTools.Utility.ForEachAsync("Assets", (AssetImporter ai) =>
            {

            },
            (string assetPath) =>
               {
                   if (assetPath.StartsWith("Assets/__copy__/"))
                       return false;

                   if (!XTools.Utility.HasExportScene(assetPath))
                       return false;

                   //                    if (!assetPath.EndsWith("Level_Shamo.unity"))
                   //                        return false;

                   var stream = File.OpenRead(assetPath);
                   long lenght = stream.Length;
                   if (lenght <= 1024 * 1024 * 5)
                       return false;

                   UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.OpenScene(assetPath);
                   string simplify = assetPath.Insert(7, "__copy__/");
                   Lightmapping.lightingDataAsset = null;
                   Lightmapping.realtimeGI = false;
                   Lightmapping.bakedGI = false;
                   foreach (var root in currentScene.GetRootGameObjects())
                   {
                       var lpgs = root.GetComponentsInChildren<LightProbeGroup>(true);
                       foreach (var lpg in lpgs)
                       {
                           lpg.probePositions = null;
                           XTools.Utility.Destroy(lpg);
                       }

                       var rps = root.GetComponentsInChildren<ReflectionProbe>(true);
                       foreach (var lpg in rps)
                       {
                           XTools.Utility.Destroy(lpg);
                       }

                       foreach (var c in root.GetComponentsInChildren<Component>())
                           XTools.Utility.CheckScene(c);

                       foreach (var light in root.GetComponentsInChildren<Light>())
                       {
                           XTools.Utility.Destroy(light);
                       }
                   }

                   //SceneRoot.SetCurrentBatching();
                   Directory.CreateDirectory(simplify.Substring(0, simplify.LastIndexOf('/')));
                   EditorSceneManager.SaveScene(currentScene, simplify);
                   copy_scenes.Add(simplify);

                   return false;
               });

            while (ator.MoveNext())
            {
                yield return 0;
            }

            foreach (var scene in copy_scenes)
            {
                UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.OpenScene(scene);
                Lightmapping.Bake();
                while (Lightmapping.isRunning)
                    yield return 0;
                EditorSceneManager.SaveScene(currentScene);
            }

            if (!string.IsNullOrEmpty(current))
                EditorSceneManager.OpenScene(current);

            Debug.LogFormat("totao time:{0}", tc.delay);
        }
    }
}
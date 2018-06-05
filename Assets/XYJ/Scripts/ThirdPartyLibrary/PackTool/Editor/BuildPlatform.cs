using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace PackTool
{
    public class BuildPlatform
    {
        [MenuItem("PackTool/设置需要加载的场景")]
        public static void SetBuildSettingsScene()
        {
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
            List<string> sceneList = new List<string>();
#if USE_RESOURCESEXPORT || USE_ABL
            // 场景打包模式,只需要加载这两个场景就OK了
            sceneList.Add("Assets/Art/UIData/Levels/StartInit.unity");
            sceneList.Add("Assets/Art/UIData/Levels/Empty.unity");
#elif USE_RESOURCES
            sceneList = BuildSceneList.GetAllSceneList();
#else
            sceneList = BuildSceneList.GetAllSceneList();
#endif
            List<EditorBuildSettingsScene> ebss = new List<EditorBuildSettingsScene>();
            foreach (string scene in sceneList)
                ebss.Add(new EditorBuildSettingsScene(scene, true));

            EditorBuildSettings.scenes = ebss.ToArray();
        }


        static public void DeleteAllChildDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            foreach (string file in Directory.GetFiles(path))
                File.Delete(file);

            foreach (string childpath in Directory.GetDirectories(path))
                Directory.Delete(childpath, true);
        }
    }
}
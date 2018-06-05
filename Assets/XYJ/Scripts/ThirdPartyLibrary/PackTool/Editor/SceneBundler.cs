#if USE_RESOURCESEXPORT

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PackTool
{
    // 场景打包
    public class SceneBundler
    {
        // 没有就复制个，有就返回
        static void GetCopyScene(Scene currentScene, string copyScene)
        {
            //string currentPath = currentScene.path;
            string absCopyPath = (Application.dataPath + "/" + copyScene.Substring(7)).Replace('\\', '/');
            Scene src_scene = currentScene;
            if (File.Exists(absCopyPath))
            {
                AssetDatabase.DeleteAsset(copyScene);
                AssetDatabase.Refresh();
            }

            string copypath = absCopyPath.Substring(0, absCopyPath.LastIndexOf('/'));
            if (!Directory.Exists(copypath))
            {
                Directory.CreateDirectory(copypath);
            }

            GameObject root = XTools.Utility.ResetSceneRoot(src_scene);
            root.AddComponent<SceneRoot>().SaveScene();

            EditorSceneManager.SaveScene(src_scene, copyScene);
            AssetDatabase.Refresh();
        }

        public static Object PackCurrentScene(List<Object> preList = null, bool bSave = false)
        {
            Scene currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!currentScene.IsValid())
            {
                Debug.Log("当前场景未保存，请先保存之后再打包!");
                return null;
            }
            else
            {
                string absScene = currentScene.path;
                string copyScenePath = "Assets/__copy__/" + absScene.Substring(7);

                GetCopyScene(currentScene, copyScenePath);
                EditorSceneManager.OpenScene(absScene);
                Object copyobj = AssetDatabase.LoadAssetAtPath("Assets/__copy__/" + absScene.Substring(7), typeof(Object));
                if (copyobj == null)
                    Debug.LogError("copyobj == null " + absScene);

                if (preList != null)
                    preList.Add(copyobj);

                return copyobj;
            }
        }

        [MenuItem("PackTool/PackScene")]
        public static void UnpackScene()
        {
            Scene currentScene = EditorSceneManager.GetActiveScene();
            string path = currentScene.path;
            Object scene = PackCurrentScene(null, true);
            if (scene != null)
            {
                AssetsExport mgr = new AssetsExport();
                mgr.CollectScene(scene);
                if (currentScene.IsValid())
                    EditorSceneManager.OpenScene(path);

                XTools.GlobalCoroutine.StartCoroutine(mgr.BeginPack());
            }
        }
    }
}
#endif
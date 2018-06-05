#if UNITY_EDITOR && SCENE_DEBUG
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace XTools
{
    [UnityEditor.InitializeOnLoad]
    public class SceneAutoSave : EditorWindow
	{
        const string timekey = "保存频率";
        const string enablekey = "开启";

        const bool default_enable = false;
        const float default_time = 120f;

        static SceneAutoSave()
        {
            EditorApplication.update += Check;
            last_time = -1;
        }

        static double last_time = -1;

        static string last_save_md5 = string.Empty; // 最后保存的文件的md5值

        public static string GetNowTime()
        {
            System.DateTime time = System.DateTime.Now;
            return string.Format("{0}-{1}-{2}-{3}-{4}-{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        // 每隔1分钟保存一次
        static void Check()
        {
            if (UnityEngine.Application.isPlaying)
                return;

            double time = EditorApplication.timeSinceStartup;
            if (last_time > time)
                return; // 下次保存的时间还没到

            last_time = time + EditorPrefs.GetFloat(timekey, default_time);
            if (EditorPrefs.GetBool(enablekey, default_enable))
            {
                SaveCurrent();
            }
        }

        static void SaveCurrent()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            if ((!scene.IsValid()) || string.IsNullOrEmpty(scene.path) || (!scene.isDirty))
                return;

            string assetPath = scene.path;
            if (assetPath.StartsWith(save_path))
                return;

            assetPath = assetPath.Substring(7);
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf('.'));
            string save_scene_path = string.Format("{0}/{1}_{2}.unity", save_path, assetPath, GetNowTime());

            EditorSceneManager.SaveScene(scene, save_scene_path, true);

            string md5value = Md5.GetFileMd5(save_scene_path);
            if (md5value == last_save_md5)
                AssetDatabase.DeleteAsset(save_scene_path);
            else
            {
                last_save_md5 = md5value;
            }
        }

        [MenuItem("Window/场景自动保存设置")]
        static void Open()
        {
            GetWindow<SceneAutoSave>(false, "SceneAutoSave", true);
        }

        // 自动保存的场景
        const string save_path = "Assets/__auto_scene__/";

        void OnGUI()
        {
            bool isenable = EditorPrefs.GetBool(enablekey, default_enable);
            isenable = EditorGUILayout.Toggle(enablekey, isenable);
            EditorPrefs.SetBool(enablekey, isenable);
            if (isenable)
            {
                float value = EditorPrefs.GetFloat(timekey, default_time);
                value = EditorGUILayout.FloatField(timekey + "(单位秒)", value);
                EditorPrefs.SetFloat(timekey, value);

                if (UnityEngine.GUILayout.Button("立即保存当前场景到临时目录"))
                {
                    SaveCurrent();
                }
            }
        }
	}
}

#endif
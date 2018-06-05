using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PackTool
{
    // 场景版本
    public class SceneVersion
    {
        const string filename = "sceneversion.txt";

        static SceneVersion()
        {
            SceneList = new HashSet<string>();
            svn_version = 0;
        }

#if UNITY_EDITOR
        static void InitEditor()
        {
            SceneList.Clear();
            foreach (EditorBuildSettingsScene ebss in EditorBuildSettings.scenes)
            {
                if (ebss.enabled)
                {
                    string path = ebss.path.Substring(7);
                    SceneList.Add(path);
                    ResourcesGroup.AddFile(path);
                }
            }

            if (!Application.isPlaying)
            {
                svn_version = svnversion.GetVersion();
            }
        }

        // 设置场景数据
        public static void SaveToFile()
        {
            JSONObject json = new JSONObject();
            svn_version = svnversion.GetVersion();
            json.put("svn_version", svn_version);

            JSONArray array = new JSONArray();
            json.put("scenes", array);
            foreach (string sp in BuildSceneList.GetAllSceneList())
                array.put(Path.GetFileNameWithoutExtension(sp));

            StringBuilder sb = new StringBuilder();
            json.GetText(sb);

            if (!Directory.Exists(ResourcesPath.streamingAssetsPath))
                Directory.CreateDirectory(ResourcesPath.streamingAssetsPath);

            File.WriteAllText(ResourcesPath.streamingAssetsPath + filename, sb.ToString());
        }
#endif
        public static void Init()
        {
#if UNITY_EDITOR
            InitEditor();
#else
            InitByFile();
#endif
            Debuger.Log("svn_version:" + svn_version);
        }

        static void InitByFile()
        {
            svn_version = 0;
            SceneList.Clear();
            Stream stream = StreamingAssetLoad.GetFile(filename);
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                JSONObject json = new JSONObject(reader.ReadToEnd());
                svn_version = json.getLong("svn_version");
                JSONArray array = json.getJSONArray("scenes");
                for (int i = 0; i < array.length(); ++i)
                    SceneList.Add(array.getString(i));

                reader.Close();
            }
        }

        static public long svn_version { get; protected set; }

        // 当前加载的场景列表
        static public HashSet<string> SceneList { get; protected set; }

        public static bool IsExist(string s)
        {
            return SceneList.Contains(s) ? true : false;
        }
    }
}
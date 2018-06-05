using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

namespace PackTool
{
    public class BuildSceneList
    {
        static HashSet<string> GetConfigScene()
        {
            string[] files = Directory.GetFiles("Data/Config/Edit/Level/LevelDesignConfig/");
            HashSet<string> scenes = new HashSet<string>();
            foreach (string file in files)
            {
                try
                {
                    LevelDesignConfig config = JsonUtility.FromJson<LevelDesignConfig>(File.ReadAllText(file));
                    foreach (var logic in config.m_levelLogicList)
                        scenes.Add(logic.m_scene);
                }
                catch(System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            return scenes;
        }

        static public List<string> GetAllSceneList()
        {
            HashSet<string> scenes = GetConfigScene();
            scenes.Add("main");
            scenes.Add("login");
            scenes.Add("level_renwujiemian");
            scenes.Add("Level_Nielianjiemian");
            scenes.Add("Level_Chuangjue");

            List<string> results = new List<string>();
            foreach (var scene in XTools.Utility.GetAllSceneList())
            {
                string name = Path.GetFileNameWithoutExtension(scene);
                if (name.EndsWith("_Opt"))
                    name = name.Substring(0, name.Length - 4);
                if (scenes.Contains(name))
                    results.Add(scene);
            }

            return results;
        }
    }
}

#endif
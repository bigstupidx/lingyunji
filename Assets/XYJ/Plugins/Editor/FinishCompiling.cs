using System;
using UnityEngine;
using UnityEditor;

//[InitializeOnLoad]
public class FinishCompiling
{
    const string compilingKey = "Compiling";
    static bool compiling;
    const string begin_time_Key = "begin_time_Key";
    static long begin_time = 0;
    static FinishCompiling()
    {
        compiling = EditorPrefs.GetBool(compilingKey, false);
        begin_time = long.Parse(EditorPrefs.GetString(begin_time_Key, "0"));
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (compiling && !EditorApplication.isCompiling)
        {
            var now = DateTime.Now;
            Debug.Log(string.Format("Compiling DONE {0}, total:{1}", now, ((now.Ticks - begin_time) * 0.0000001f).ToString("0.00")));
            compiling = false;
            EditorPrefs.SetBool(compilingKey, false);
            EditorPrefs.SetString(begin_time_Key, "0");
        }
        else if (!compiling && EditorApplication.isCompiling)
        {
            var now = DateTime.Now;
            begin_time = now.Ticks;
            Debug.Log(string.Format("Compiling START {0}", now));
            compiling = true;
            EditorPrefs.SetBool(compilingKey, true);
            EditorPrefs.SetString(begin_time_Key, begin_time.ToString());
        }
    }
}
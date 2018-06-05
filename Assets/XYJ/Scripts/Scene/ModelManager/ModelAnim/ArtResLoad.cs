/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：使用宏SCENE_DEBUG时美术场景的资源加载封装
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ArtResLoad 
{
#if !SCENE_DEBUG
    public static void LoadRes(string name, PackTool.ResourcesEnd<GameObject> fun,object para=null,bool isInit=true )
    {
        RALoad.LoadPrefab(name, fun, para, isInit);
    }

    public static void LoadCG(string name, PackTool.ResourcesEnd<GameObject> fun, object para = null)
    {
        RALoad.LoadPrefab(name, (GameObject go, object p) =>
        {
            GameObject obj = null;
            {
                bool isActive = go.activeSelf;
                if (isActive)
                    go.SetActive(false);

                {
                    obj = GameObject.Instantiate<GameObject>(go);
                    obj.name = go.name;
                }

                if (isActive)
                    go.SetActive(true);
            }

            xys.App.my.main.StartCoroutine(OnLoadEnd(obj, fun, para));
        }, 
        null, 
        false);
    }

    static IEnumerator OnLoadEnd(GameObject obj, PackTool.ResourcesEnd<GameObject> fun, object para)
    {
        var vvs = obj.GetComponentsInChildren<CGAnimatorBase>(true);
        for (int i = 0; i < vvs.Length; ++i)
        {
            while (vvs[i].GetAnimator() == null)
                yield return 0;
        }

        obj.SetActive(true);
        fun(obj, para);
    }

    public static void LoadResSync(string name, PackTool.ResourcesEnd<GameObject> fun, object para = null, bool isInit = true)
    {
        RALoad.LoadPrefabSync(name, fun, para, isInit);
    }

    public static void LoadAudioClip(string name, PackTool.ResourcesEnd<AudioClip> fun,object para)
    {

    }
#else

    static Dictionary<string, string> files = new Dictionary<string,string>();
    public static void LoadCG(string name, Action<GameObject, object> fun, object para = null)
    {
        LoadRes(name, fun, para, true);
    }

    public static void LoadResSync(string name, Action<GameObject,object> fun,object para=null,bool isInit=true )
    {
        LoadRes(name, fun, para, isInit);
    }

    public static void LoadAudioClip(string name, Action<AudioClip,object> fun,object para)
    {
        string fullName = null;
        name += ".wav";
        if (!files.TryGetValue(name, out fullName))
        {
            string[] filePaths = System.IO.Directory.GetFiles(Application.dataPath, name, System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i].Replace('\\', '/');
                if (filePath.EndsWith("AudioClipExport/" + name))
                {
                    fullName = filePath;
                    fullName = fullName.Substring(Application.dataPath.Length - 6);
                    files.Add(name, fullName);
                    break;
                }
            }
        }
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(fullName))
        {
            AudioClip asset = (AudioClip)UnityEditor.AssetDatabase.LoadAssetAtPath(fullName, typeof(AudioClip));
            if (asset != null && fun != null)
                fun(asset, para);
        }
#endif
    }

    public static void LoadRes(string name, Action<GameObject,object> fun, object para = null ,bool isInit=true)
    {
        string fullName = null;
        name += ".prefab";
        if(!files.TryGetValue(name,out fullName))
        {
            string[] filePaths = System.IO.Directory.GetFiles(Application.dataPath, name, System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i].Replace('\\', '/');
                if (filePath.EndsWith("ResourcesExport/" + name))
                {
                    fullName = filePath;
                    fullName = fullName.Substring(Application.dataPath.Length-6);
                    files.Add(name, fullName);
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(fullName))
        {
            Debuger.LogError("找不到资源 " + name);
        }
#if UNITY_EDITOR
        if(!string.IsNullOrEmpty(fullName))
        {
            GameObject asset = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(fullName, typeof(GameObject));
            GameObject go = isInit?GameObject.Instantiate(asset):asset;
            if (go != null && fun!=null)
                fun(go,para);
        }
#endif
    }


#endif
}

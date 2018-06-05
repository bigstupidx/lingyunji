using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityObject = UnityEngine.Object;
using PackTool;

#if USE_RESOURCESEXPORT

[CustomEditor(typeof(MonoBehaviour))]
public class MissingScriptResolver : Editor
{
	public override void OnInspectorGUI()
	{
        SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
        if (scriptProperty == null || scriptProperty.objectReferenceValue != null)
        {
            base.OnInspectorGUI();
            return;
        }
        else
        {
            GUILayout.Label("脚本丢失!");
        }

        if (MissingScript.is_handle)
        {
            UnityObject so = AssetDatabase.LoadAssetAtPath("Assets/Scripts/PackTool/MissingScript.cs", typeof(UnityObject));
            scriptProperty.objectReferenceValue = so as MonoScript;
            scriptProperty.serializedObject.ApplyModifiedProperties();
            scriptProperty.serializedObject.Update();
            EditorUtility.SetDirty(target);
            //EditorApplication.SaveAssets();
            MissingScript.isSet = true;
            //Debug.Log("脚本丢失!替换成功!");
        }
    }

    [MenuItem("PackTool/移除所有预置体丢失的脚本组件")]
    private static void RemoveMissingScript()
    {
        List<string> files = XTools.Utility.GetAllFileList(Application.dataPath);
        PackResources Prefabs = new PackResources();
        PackResources.GetResources<GameObject>(Prefabs, files, null, null, new string[] { ".prefab" });

        XTools.GlobalCoroutine.StartCoroutine(RemoveMissing(Prefabs.Keys));
    }

    [MenuItem("Assets/PackTool/移除丢失的脚本")]
    private static void RemoveSelectMissingScript()
    {
        List<string> selects = new List<string>();
        string path;
        foreach (GameObject go in Selection.gameObjects)
        {
            path = AssetDatabase.GetAssetPath(go);
            if (!string.IsNullOrEmpty(path) && path.StartsWith("Assets/"))
                selects.Add(path.Substring(7));
        }

        XTools.GlobalCoroutine.StartCoroutine(RemoveMissing(selects));
    }


    static IEnumerator RemoveMissing(List<string> prefabs)
    {
        List<string> missList = new List<string>();
        List<Component> cmps = new List<Component>();
        {
            for (int i = 0; i < prefabs.Count; ++i)
            {
                prefabs[i] = "Assets/" + prefabs[i];
                GameObject go = AssetDatabase.LoadAssetAtPath(prefabs[i], typeof(GameObject)) as GameObject;
                if (go == null)
                    continue;

                go.GetComponentsInChildren<Component>(true, cmps);
                if (cmps.Contains(null))
                    missList.Add(prefabs[i]);

                cmps.Clear();
            }
        }

        Debug.Log(string.Format("total:{0} miss num:{1}", prefabs.Count, missList.Count));
        List<Transform> trans = new List<Transform>(1024);
        List<MissingScript> scripts = new List<MissingScript>(1024);
        MissingScript.is_handle = true;
        for (int i = 0; i < missList.Count; ++i)
        {
            MissingScript.isSet = false;
            GameObject go = AssetDatabase.LoadAssetAtPath(missList[i], typeof(GameObject)) as GameObject;
            go.GetComponentsInChildren<Transform>(true, trans);
            for (int j = 0; j < trans.Count; ++j)
            {
                GameObject childgo = trans[j].gameObject;
                childgo.GetComponents<Component>(cmps);
                if (cmps.Contains(null))
                {
                    cmps.Clear();
                    Selection.activeGameObject = childgo;
                    yield return 0;
                }
                else
                {
                    cmps.Clear();
                }
            }

            trans.Clear();
            Selection.activeGameObject = null;

            go.GetComponentsInChildren<MissingScript>(true, scripts);
            foreach (MissingScript ms in scripts)
            {
                Debug.Log("组件名:" + ms.name);
                UnityEngine.Object.DestroyImmediate(ms, true);
            }

            UnityEditor.EditorUtility.SetDirty(go);
            Debug.Log(missList[i] + "移除丢失的脚本!" + go.name + " count:" + scripts.Count);
            yield return 0;
        }
        MissingScript.is_handle = false;

        Debug.Log("完成!");
        yield return 0;
    }
}

#endif
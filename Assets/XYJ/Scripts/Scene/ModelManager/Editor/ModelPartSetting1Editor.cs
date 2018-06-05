

#if SCENE_DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ModelPartSetting1))]
public class ModelPartSetting1Editor : Editor
{

    [MenuItem("GameObject/GameUnit/CreateModelPartSetting1Object")]
    static void CreateGameActor()
    {
        // Create GameObject
        GameObject go = new GameObject("ModelPartSetting1");
        go.transform.position = Vector3.zero;
        GameObject title = new GameObject("Title");
        title.transform.parent = go.transform;
        title.transform.localPosition = Vector3.zero;
        GameObject fx = new GameObject("transform");
        fx.transform.parent = go.transform;
        fx.transform.localPosition = Vector3.zero;

        // Add Components for Game Actor
        ModelPartSetting1 setting = go.AddComponent<ModelPartSetting1>();
        AnimationEffectManageData effectData = go.AddComponent<AnimationEffectManageData>();

        Selection.activeGameObject = go;
    }

    // =================================================

    ModelPartSetting1 setting;
    ModelPartManage manager;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (setting == null)
        {
            setting = target as ModelPartSetting1;
        }

        if (setting.RootObject == null)
        {
            return;
        }

        //if (manager==null)
        //{
        //    ModelPartTempObject tmp = setting.RootObject.GetComponent<ModelPartTempObject>();
        //    if (tmp != null)
        //    {
        //        manager = tmp.m_manager;
        //    }
        //}
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

}
#endif
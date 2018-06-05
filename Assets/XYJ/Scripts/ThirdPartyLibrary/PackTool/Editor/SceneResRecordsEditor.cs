using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(SceneResRecords), true)]
public class SceneResRecordsEditor : Editor
{
    //EditorPageBtn mPage = new EditorPageBtn();
    Dictionary<System.Type, int> TypeList = new Dictionary<System.Type, int>();

    static byte[] GetFileBytes(string files)
    {
        string f = Application.dataPath + "/" + files;
        if (File.Exists(f))
            return File.ReadAllBytes(f);

        f = Application.dataPath + "/__copy__/" + files;
        if (File.Exists(f))
            return File.ReadAllBytes(f);

        return null;
    }

    string filename = "";

    void OnEnable()
    {
        filename = AssetDatabase.GetAssetPath(target);
        if (string.IsNullOrEmpty(filename))
            return;

        filename = filename.Substring(7);
        if (filename.EndsWith(".scene.prefab"))
            filename = filename.Substring(0, filename.Length - 13) + ".unity";
    }

    public override void OnInspectorGUI()
    {
        SceneResRecords rrs = target as SceneResRecords;

        byte[] bytes = GetFileBytes(filename + PackTool.Suffix.SceneDataByte);
        if (bytes != null)
            EditorGUILayout.IntField("数据字节数", bytes.Length);
        else
            EditorGUILayout.IntField("数据字节数", 0);

        if (rrs.components != null)
            EditorGUILayout.IntField("组件数", rrs.components.Length);
        else
            EditorGUILayout.IntField("组件数", 0);

        bytes = GetFileBytes(filename + PackTool.Suffix.ScenePosByte);
        if (bytes != null)
            EditorGUILayout.IntField("标识数", bytes.Length / 4);
        else
            EditorGUILayout.IntField("标识数", 0);

        if (TypeList.Count == 0)
        {
            foreach (Component c in rrs.components)
            {
                if (c == null)
                    continue;

                int num = 0;
                if (TypeList.TryGetValue(c.GetType(), out num))
                    TypeList[c.GetType()] = num + 1;
                else
                    TypeList[c.GetType()] = 1;
            }
        }

        EditorGUILayout.LabelField("typenum:" + TypeList.Count);
        foreach (KeyValuePair<System.Type, int> itor in TypeList)
            EditorGUILayout.LabelField(itor.Key.Name + " Num:" + itor.Value);

        rrs.isRec = EditorGUILayout.Toggle("显示组件", rrs.isRec);
        if (rrs.isRec)
            base.OnInspectorGUI();
    }
}

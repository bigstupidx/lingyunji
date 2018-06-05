using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ResourcesRecords), true)]
public class ResourcesRecordsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ResourcesRecords rrs = target as ResourcesRecords;
//         if (rrs.bytes != null)
//             EditorGUILayout.IntField("数据字节数", rrs.bytes.Length);
//         else
//             EditorGUILayout.IntField("数据字节数", 0);

        if (rrs.components != null)
            EditorGUILayout.IntField("组件数", rrs.components.Length);
        else
            EditorGUILayout.IntField("组件数", 0);

        rrs.isRec = EditorGUILayout.Toggle("显示组件", rrs.isRec);
        if (rrs.isRec)
            base.OnInspectorGUI();
    }
}

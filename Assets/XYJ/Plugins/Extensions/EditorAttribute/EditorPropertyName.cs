#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

// The property drawer class should be placed in an editor script, inside a folder called Editor.
[CustomPropertyDrawer(typeof(EditorPropertyNameAttribute))]
public class EditorPropertyNameDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorPropertyNameAttribute fieldName = (EditorPropertyNameAttribute)attribute;

        //枚举的话要先获取菜单名字
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            string[] names = GetEnumDisplayNames(property, fieldInfo.FieldType);
            EditorGUI.BeginChangeCheck();

            int index = EditorGUI.Popup(position, fieldName.mGUILabel.text, property.enumValueIndex, names);
            if (EditorGUI.EndChangeCheck())
            {
                if (index >= 0)
                    property.enumValueIndex = index;
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, fieldName.mGUILabel, true);
        }
    }

    // The default is one line high.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var fieldName = (EditorPropertyNameAttribute)attribute;
        return EditorGUI.GetPropertyHeight(property, fieldName.mGUILabel);
    }

    // 缓存枚举名字列表
    static Dictionary<Type, string[]> _enumNameDict = new Dictionary<Type, string[]>();
    static string[] GetEnumDisplayNames(SerializedProperty property, Type t)
    {
        string[] names;
        if (_enumNameDict.TryGetValue(t, out names))
            return names;
        else
        {
            string[] enumDiplayNames = property.enumDisplayNames;
            string[] enumNames = property.enumNames;
            names = new string[enumNames.Length];
            for (int i = 0; i < enumNames.Length; ++i)
            {
                FieldInfo field = t.GetField(enumNames[i]);
                var attrs = field == null ? null : field.GetCustomAttributes(typeof(EditorPropertyNameAttribute), false);
                if (attrs == null || attrs.Length < 1)
                    names[i] = enumDiplayNames[i];
                else
                    names[i] = ((EditorPropertyNameAttribute)attrs[0]).mGUILabel.text;
            }
            _enumNameDict[t] = names;
        }
        return names;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class EditorPropertyNameAttribute : PropertyAttribute
{
    public GUIContent mGUILabel;

    public EditorPropertyNameAttribute (string label)
    {
        this.mGUILabel = new GUIContent(label);
    }
}
#endif
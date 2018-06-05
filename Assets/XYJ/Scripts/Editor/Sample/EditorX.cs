using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CanEditMultipleObjects]
public class EditorX<Target> : Editor where Target : Component
{
    protected GameObject gameobject;
    protected List<GameObject> objects = new List<GameObject>();
    protected List<Target> objTargets = new List<Target>();

    // 关联的编辑对象
    public new Target target { get { return base.target as Target; } }

	// undo 字符串
	public string undoString
	{
		get { return _undoString == null ? _undoString = target.ToString() : _undoString; }
		set { _undoString = value; }
	}
	string _undoString;

    protected virtual void OnEnable()
    {
        gameobject = target.gameObject;
        objects.Clear();
        objTargets.Clear();

        if (serializedObject.isEditingMultipleObjects)
        {
            for (int i = 0; i < serializedObject.targetObjects.Length; ++i)
            {
                Target obj = (Target)serializedObject.targetObjects[i];
                objTargets.Add(obj);
                objects.Add(obj.gameObject);
                MultipleEnable(obj, obj.gameObject.name);
            }
        }
        else
        {
            Target obj = gameobject.GetComponent<Target>();
            MultipleEnable(obj, gameobject.name);
        }
    }

    protected virtual void OnDisable()
    {
        if (serializedObject.isEditingMultipleObjects)
        {
            for (int i = 0; i < objects.Count; ++i)
            {
                if (objects[i] == null)
                {
                    DestroyTargetFunction(objTargets[i]);
                }
            }
        }
        else
        {
            if (null == gameobject)
            {
                DestroyTargetFunction(target);
            }
        }
    }

    protected virtual void DestroyTargetFunction(Target target)
    {

    }

    protected virtual void MultipleEnable(Target target, string name)
    {

    }

    // 调试输出
    public static void print(object message) { Debug.Log(message); }

	// 在绘制可交互的 Handle 之前调用此方法开始事件检查
	public static void BeginHandleEventCheck()
	{
		_lastHotControl = GUIUtility.hotControl;
	}
	static int _lastHotControl;

	// 在绘制可交互的 Handle 之后调用此方法获得事件类型
	public static HandleEvent EndHandleEventCheck()
	{
		return _lastHotControl == GUIUtility.hotControl ? HandleEvent.unknown
			: (_lastHotControl == 0 ? HandleEvent.select : HandleEvent.release);
	}
	public enum HandleEvent { unknown = 0, select, release }


	// 便捷编辑器绘制方法，如果检测到 draw 方法修改了值，将执行 apply 方法，并处理撤销和保存
	public void SmartDraw<T>(Func<T> draw, Action<T> apply)
	{
		EditorGUI.BeginChangeCheck();
		T value = draw();
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, undoString);
			apply(value);
			EditorUtility.SetDirty(target);
		}
	}

	#region 接口封装
	public void DrawVector3Field(Rect rect, Vector3 value, Action<Vector3> apply, string label = null)
	{
		SmartDraw(() => label == null
			? EditorGUI.Vector3Field(rect, GUIContent.none, value)
			: EditorGUI.Vector3Field(rect, label, value), apply);
	}

	public void DrawVector3FieldLayout(Vector3 value, Action<Vector3> apply, string label = null, params GUILayoutOption[] options)
	{
		SmartDraw(() => label == null
			? EditorGUILayout.Vector3Field(GUIContent.none, value, options)
			: EditorGUILayout.Vector3Field(label, value, options), apply);
	}

	public void DrawVector2Field(Rect rect, Vector2 value, Action<Vector2> apply, string label = null)
	{
		SmartDraw(() => label == null
			? EditorGUI.Vector2Field(rect, GUIContent.none, value)
			: EditorGUI.Vector2Field(rect, label, value), apply);
	}

	public void DrawVector2FieldLayout(Vector2 value, Action<Vector2> apply, string label = null, params GUILayoutOption[] options)
	{
		SmartDraw(() => label == null
			? EditorGUILayout.Vector2Field(GUIContent.none, value, options)
			: EditorGUILayout.Vector2Field(label, value, options), apply);
	}

	public void DrawEnumPopup(Rect rect, Enum value, Action<Enum> apply, string label = null)
	{
		SmartDraw(() => label == null
			? EditorGUI.EnumPopup(rect, GUIContent.none, value)
			: EditorGUI.EnumPopup(rect, label, value), apply);
	}

	public void DrawEnumPopupLayout(Enum value, Action<Enum> apply, string label = null, params GUILayoutOption[] options)
	{
		SmartDraw(() => label == null
			? EditorGUILayout.EnumPopup(GUIContent.none, value, options)
			: EditorGUILayout.EnumPopup(label, value, options), apply);
	}

    public void DrawPopupLayout(int index, string[] decs, Action<int> apply, string label = null, params GUILayoutOption[] options)
    {
        SmartDraw(() => EditorGUILayout.Popup(label, index, decs, options), apply);
    }

	public void DrawIntField(Rect rect, int value, Action<int> apply, string label = null)
	{
		SmartDraw(() => label == null
			? EditorGUI.IntField(rect, GUIContent.none, value)
			: EditorGUI.IntField(rect, label, value), apply);
	}

	public void DrawIntFieldLayout(int value, Action<int> apply, string label = null, params GUILayoutOption[] options)
	{
		SmartDraw(() => label == null
			? EditorGUILayout.IntField(GUIContent.none, value, options)
			: EditorGUILayout.IntField(label, value, options), apply);
	}

	public void DrawFloatField(Rect rect, float value, Action<float> apply, string label = null)
	{
		SmartDraw(() => label == null
			? EditorGUI.FloatField(rect, GUIContent.none, value)
			: EditorGUI.FloatField(rect, label, value), apply);
	}

	public void DrawFloatFieldLayout(float value, Action<float> apply, string label = null, params GUILayoutOption[] options)
	{
		SmartDraw(() => label == null
			? EditorGUILayout.FloatField(GUIContent.none, value, options)
			: EditorGUILayout.FloatField(label, value, options), apply);
	}

	public void DrawToggle(Rect rect, bool value, Action<bool> apply, string label = null)
	{
		SmartDraw(() => label == null
			? EditorGUI.Toggle(rect, GUIContent.none, value)
			: EditorGUI.Toggle(rect, label, value), apply);
	}

	public void DrawToggleLayout(bool value, Action<bool> apply, string label = null, params GUILayoutOption[] options)
	{
		SmartDraw(() => label == null
			? EditorGUILayout.Toggle(GUIContent.none, value, options)
			: EditorGUILayout.Toggle(label, value, options), apply);
	}

    public void DrawListLayout(SerializedProperty value, Action<bool> apply, params GUILayoutOption[] options)
    {
        SmartDraw(() => ShowElements(value), apply);
    }
    bool ShowElements(SerializedProperty list)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(list, true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
        return true;
    }

    public void DrawObjectLayout(UnityEngine.Object value, Type type, Action<object> apply, string label = null, params GUILayoutOption[] options)
    {
        SmartDraw(() => label == null
            ? EditorGUILayout.ObjectField(GUIContent.none, value, type, true, options)
            : EditorGUILayout.ObjectField(label, value, type, true, options), apply);
    }

    public void DrawTextLayout(string text, Action<string> apply, string label = null, params GUILayoutOption[] options)
    {
        SmartDraw(() => label == null
            ? EditorGUILayout.TextField(GUIContent.none, text, options)
            : EditorGUILayout.TextField(label, text, options), apply);
    }
	#endregion

    public Color SetColor(bool change)
    {
        if(change)
        {
            return Color.cyan;
        }
        return Color.white;
    }
}


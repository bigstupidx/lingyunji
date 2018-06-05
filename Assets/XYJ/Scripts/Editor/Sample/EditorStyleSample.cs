
using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System.Reflection;

public class EditorStyleSample : EditorWindow
{
    static EditorStyles _eStyles = new EditorStyles();
    static PropertyInfo[] _editorInfos = null;
    static PropertyInfo[] _skinInfos = null;

    [MenuItem("Tools/编辑器参考/编辑器控件样式(并带所有消息打印)")]
    static void ShowStyleSample()
    {
        EditorStyleSample iconWindow = GetWindow<EditorStyleSample>("控件样式", true);
        iconWindow.minSize = new Vector2(400, 600);
        iconWindow.autoRepaintOnSceneChange = true;
    }

    Vector2 m_mainScroll = Vector2.one;

    // Implement your own editor GUI here.
    void OnGUI()
    {
        using (new AutoEditorScrollView(ref m_mainScroll))
        {
            // 展示EditorStyles里的所有GUIStyle
            using (new AutoGUIColor(Color.yellow))
            {
                EditorGUILayout.LabelField("展示EditorStyles里的所有GUIStyle");
            }

            if (_editorInfos==null)
            {
                // 获取PropertyInfo
                _editorInfos = _eStyles.GetType().GetProperties(BindingFlags.Static | BindingFlags.Public);
                Debug.Log(string.Format("EditorStyles Properties Count : {0}", _editorInfos.Length));
            }

            if (_editorInfos != null)
            {
                for (int i = 0; i < _editorInfos.Length; ++i)
                {
                    object obj = _editorInfos[i].GetValue(_eStyles, null);

                    if (obj.GetType() == typeof(GUIStyle))
                    {
                        EditorGUILayout.Separator();
                        if (GUILayout.Button(_editorInfos[i].Name, (GUIStyle)obj))
                        {
                            ShowNotification(new GUIContent(_editorInfos[i].Name));
                        }
                    }

                }
            }

            EditorGUILayout.Separator();

            //展示GUISkin里的所有GUIStyle
            using (new AutoGUIColor(Color.yellow))
            {
                EditorGUILayout.LabelField("展示GUISkin里的所有GUIStyle");
            }

            if (_skinInfos == null)
            {
                _skinInfos = GUI.skin.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                Debug.Log(string.Format("GUISkin Properties Count : {0}", _editorInfos.Length));
            }

            if (_skinInfos != null)
            {
                for (int i = 0; i < _skinInfos.Length; ++i)
                {
                    object obj = _skinInfos[i].GetValue(GUI.skin, null);

                    if (obj.GetType() == typeof(GUIStyle))
                    {
                        EditorGUILayout.Separator();
                        if (GUILayout.Button(_skinInfos[i].Name, (GUIStyle)obj))
                        {
                            ShowNotification(new GUIContent(_skinInfos[i].Name));
                        }
                    }

                }
            }
        }
    }

    // 	Called as the new window is opened.
    void Awake()
    {
        Debug.Log("Messages:EditorWindow.Awake");
    }

    void OnDisable()
    {
        Debug.Log("Messages:EditorWindow.OnDisable");
    }

    void OnEnable()
    {
        Debug.Log("Messages:EditorWindow.OnEnable");
    }

    // 	OnDestroy is called when the EditorWindow is closed.
    void OnDestroy()
    {
        Debug.Log("Messages:EditorWindow.OnDestroy");
    }

    // Called when the window gets keyboard focus.
    void OnFocus()
    {
        Debug.Log("Messages:EditorWindow.OnFocus");
    }

    // Called when the window loses keyboard focus.
    void OnLostFocus()
    {
        Debug.Log("Messages:EditorWindow.OnLostFocus");
    }

    // Called whenever the scene hierarchy has changed.
    void OnHierarchyChange()
    {
        Debug.Log("Messages:EditorWindow.OnHierarchyChange");
    }

    // Called whenever the project has changed.
    void OnProjectChange()
    {
        Debug.Log("Messages:EditorWindow.OnProjectChange");
    }

    // Called whenever the selection has changed.
    void OnSelectionChange()
    {
        Debug.Log("Messages:EditorWindow.OnSelectionChange");
    }

    // OnInspectorUpdate is called at 10 frames per second to give the inspector a chance to update.
    void OnInspectorUpdate()
    {
        
    }

    void Update()
    {

    }

}

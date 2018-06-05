
using UnityEngine;
using UnityEditor;
using EditorExtensions;

public class EditorIconSample : EditorWindow
{

    [MenuItem("Tools/编辑器参考/编辑器图标样式")]
    static void ShowIconSample()
    {
        EditorIconSample iconWindow = EditorWindow.GetWindow<EditorIconSample>("图标样式", true);
        iconWindow.minSize = new Vector2(580, 300);
        iconWindow.autoRepaintOnSceneChange = true;
    }

    Vector2 m_mainScroll = Vector2.one;

    void OnGUI ()
    {
        using (new AutoEditorScrollView(ref m_mainScroll))
        {
            // 展示鼠标箭头样式
            EditorGUILayout.Separator();
            using (new AutoGUIColor(Color.yellow))
            {
                EditorGUILayout.LabelField("展示鼠标箭头样式:");
            }
            EditorGUILayout.Separator();

            // 展示鼠标箭头样式
            foreach (MouseCursor item in System.Enum.GetValues(typeof(MouseCursor)))
            {
                GUILayout.Button (System.Enum.GetName(typeof(MouseCursor), item));
                EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
                EditorGUILayout.Separator();
            }

            // 展示Unity内置图标
            EditorGUILayout.Separator();

            using (new AutoGUIColor(Color.yellow))
            {
                EditorGUILayout.LabelField("Unity内置IconContent:"+EditorIconContent.Count);
            }

            EditorGUILayout.Separator();

            const int rowSize = 10;
            int index = 0;
            EditorGUILayout.BeginHorizontal();
            foreach (EditorIconContentType item in System.Enum.GetValues(typeof(EditorIconContentType)))
            {
                if (index>0 && index%rowSize==0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                GUIContent content = EditorIconContent.GetSystem(item);
                if (content != null)
                {
                    if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(30)))
                    {
                        ShowNotification(new GUIContent (item.ToString()));
                        Debug.Log(item.ToString());
                    }
                    index++;
                }
            }
            EditorGUILayout.EndHorizontal();
            // end icon

            using (new AutoGUIColor(Color.yellow))
            {
                EditorGUILayout.LabelField("Unity内置IconTexture:" + EditorIconTexture.Count);
            }

            EditorGUILayout.Separator();

            index = 0;
            EditorGUILayout.BeginHorizontal();
            foreach (IconTextureType item in System.Enum.GetValues(typeof(IconTextureType)))
            {
                if (index > 0 && index % rowSize == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                GUIContent content = new GUIContent("", EditorIconTexture.GetSystem(item));
                if (content != null)
                {
                    if (GUILayout.Button(content, GUILayout.Width(50), GUILayout.Height(30)))
                    {
                        ShowNotification(new GUIContent(item.ToString()));
                        Debug.Log(item.ToString());
                    }
                    index++;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

        }
        
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

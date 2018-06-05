
using UnityEngine;
using UnityEditor;
using System;
using EditorExtensions;

/// <summary>
/// 用来编辑信息用
/// </summary>
public class EditorMessageBox : EditorWindow
{
    #region Internal Implement

    //private int m_editStyle = 0;

    private string m_tips;
    private string m_editLabel;
    private string m_editContent;

    private float m_labelLen = 60;

    public string m_okText;
    public string m_cancelText;
    private Action<string> m_onOK;
    private Action<string> m_onCancel;

    // Implement your own editor GUI here.
    void OnGUI()
    {
        EditorGUILayout.Separator();

        // Edit Message
        using (new AutoEditorLabelWidth(m_labelLen))
        {
            m_editContent = EditorGUILayout.TextField(new GUIContent(m_editLabel), m_editContent);
        }

        EditorGUILayout.Separator();

        // tips
        if (!string.IsNullOrEmpty(m_tips))
            EditorGUILayout.PrefixLabel(m_tips);

        EditorGUILayout.Separator();

        // buttons
        using (new AutoEditorHorizontal())
        {
            if (string.IsNullOrEmpty(m_cancelText))
                EditorGUILayout.Separator();

            if (GUILayout.Button(m_okText, GUILayout.Width(60)))
            {
                if (m_onOK != null)
                {
                    m_onOK(m_editContent);
                }
                m_onOK = null;
                Close();
            }

            EditorGUILayout.Separator();

            if (!string.IsNullOrEmpty(m_cancelText))
            {
                if (GUILayout.Button(m_cancelText, GUILayout.Width(60)))
                {
                    if (m_onCancel != null)
                    {
                        m_onCancel(m_editContent);
                    }
                    m_onCancel = null;
                    Close();
                }
            }
        }
            
    }

    // OnInspectorUpdate is called at 10 frames per second to give the inspector a chance to update.
    void OnInspectorUpdate()
    {
        Repaint();
    }

    // Called multiple times per second on all visible windows.
    void Update()
    {

    }

    void OnLostFocus()
    {
        Close();
    }

    void OnDestroy()
    {
        if (m_onCancel != null)
        {
            m_onCancel(m_editContent);
        }
    }

    #endregion

    /// <summary>
    /// 显示编辑信息窗口
    /// </summary>
    #region Static Methods

    public static EditorMessageBox Display(string title, string starLable, string startContent, Action<string> okFun)
    {
        return Display(title, starLable, startContent, "", "", okFun, "", null);
    }

    public static EditorMessageBox Display(string title, string starLable, string startContent, string tips, Action<string> okFun)
    {
        return Display(title, starLable, startContent, tips, "", okFun, "", null);
    }

    public static EditorMessageBox Display(string title, string starLable, string startContent, Action<string> okFun, Action<string> cancelFun)
    {
        return Display(title, starLable, startContent, "", "", okFun, "", cancelFun);
    }

    /// <summary>
    /// 显示编辑信息窗口
    /// </summary>
    /// <param name="title"></param>
    /// <param name="starLable"></param>
    /// <param name="startContent"></param>
    /// <param name="tips"></param>
    /// <param name="ok"></param>
    /// <param name="okFun"></param>
    /// <param name="cancel"></param>
    /// <param name="cancelFun"></param>
    /// <returns></returns>
    public static EditorMessageBox Display(string title, string starLable, string startContent, string tips, string ok, Action<string> okFun, string cancel, Action<string> cancelFun)
    {
        EditorMessageBox instance = GetWindow<EditorMessageBox>(title, true);
        instance.name = title;
        instance.minSize = new Vector2(400, 100);
        instance.maxSize = new Vector2(400, 120);

        instance.m_onOK = okFun;
        instance.m_onCancel = cancelFun;

        instance.m_okText = string.IsNullOrEmpty(ok) ? "确定" : ok;
        instance.m_cancelText = (cancelFun != null && string.IsNullOrEmpty(cancel)) ? "取消" : cancel;

        instance.m_editLabel = starLable;
        instance.m_editContent = startContent;

        instance.m_labelLen = starLable.Length * 15.0f;

        instance.m_tips = tips;

        instance.Show();
        return instance;
    }

    #endregion

}

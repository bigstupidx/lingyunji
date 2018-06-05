using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorExtensions;
using xys.json;
public abstract class JsonDirectoryEditorBase<T> : EditorWindow where T : IJsonFile, new()
{
    // 配置数据
    protected JsonDirectory<T> _jsonDirectoryConfigs;
    protected JsonDirectory<T> JsonDirectoryConfigs
    {
        get
        {
            if (_jsonDirectoryConfigs==null)
            {
                _jsonDirectoryConfigs = new JsonDirectory<T>(GetJsonDirectoryPath(), true);
            }
            return _jsonDirectoryConfigs;
        }
    }

    // 配置路径
    protected abstract string GetJsonDirectoryPath();

    // 编辑用的变量
    protected List<string> m_editConfigNameList;
    protected T m_curEditConfig;
    protected int m_curEditIndex = -1;// -1，表示没有可编辑的配置

    string m_searchConfigName = string.Empty;

    //绘制工具栏
    protected void DrawToolbarView()
    {
        // 展示教学引导列表
        if (GUILayout.Button(!HasCurEditConfig() ? "选择配置" : GetCurEditConfigName(), EditorStyles.toolbarPopup, GUILayout.Width(180)))
        {
            GenericMenu teachMenu = new GenericMenu();
            if (m_editConfigNameList != null && m_editConfigNameList.Count > 0)
            {
                for (int i = 0; i < m_editConfigNameList.Count; ++i)
                {
                    string teachName = m_editConfigNameList[i];
                    if (!string.IsNullOrEmpty(m_searchConfigName) && !teachName.Contains(m_searchConfigName))
                        continue;
                    bool selected = (m_editConfigNameList[m_curEditIndex] == teachName);
                    //string contentName = teachName;
                    teachMenu.AddItem(new GUIContent(teachName), selected, (obj) =>
                    {
                        SelectConfig((int)obj);
                    }, i);
                }
            }
            teachMenu.AddSeparator(string.Empty);
            teachMenu.AddItem(new GUIContent("添加新配置"), false, () => {
                OnAddNewConfig();
            });
            teachMenu.ShowAsContext();
        }

        // 查找
        using (new AutoEditorHorizontal(GUILayout.Width(80)))
        {
            m_searchConfigName = EditorGUILayout.TextField(GUIContent.none, m_searchConfigName, "ToolbarSeachTextField", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("", string.IsNullOrEmpty(m_searchConfigName) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
            {
                m_searchConfigName = "";
                EditorGUIUtility.editingTextField = false;
            }
        }

        // 添加
        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            OnAddNewConfig();
        }

        // 删除
        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Trash), EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            if (EditorUtility.DisplayDialog("删除配置", string.Format("是否确定要删除配置:\"{0}\"?", GetCurEditConfigName()), "是", "否"))
            {
                RemoveConfig();
            }
        }

        // 刷新
        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Refresh), EditorStyles.toolbarButton, GUILayout.Width(30)))
        {
            RecordLastConfigIndex();
            ReloadAll();
        }

        EditorGUILayout.Separator();

        if (GUILayout.Button("重命名", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            //Rename();
            EditorMessageBox.Display("文件重命名", "新文件名", GetCurEditConfigName(), (name) => { m_curEditConfig.SetKey(name); Rename(); });
        }

        if (GUILayout.Button("保存", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            SaveCurrentConfig();
        }

        DrawToolBarEx();

        GUILayout.Space(5);

    }

    /// <summary>
    /// 通过事件处理重命名
    /// </summary>
    protected void HandleEnterRenameEvent()
    {
        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
        {
            string newName = m_curEditConfig.GetKey();
            if (newName != GetCurEditConfigName())
            {
                Rename();
                Repaint();
            }
        }
    }

    protected void CheckSaveOnClose()
    {
        if (GUI.changed)
        {
            if (EditorUtility.DisplayDialog ("保存配置", "是否保存当前配置？", "确定", "取消"))
            {
                SaveCurrentConfig();
            }
        }
    }

    protected string LastConfigIndexPrefsKey
    {
        get
        {
            return string.Format("JsonLastConfigIndex:{0}", typeof(T).Name);
        }
    }
    /// <summary>
    /// 在关闭的时候记录一些信息
    /// </summary>
    protected void RecordLastConfigIndex()
    {
        EditorPrefs.SetInt(LastConfigIndexPrefsKey, m_curEditIndex);
    }

    protected int GetLastConfigIndex()
    {
        return EditorPrefs.GetInt(LastConfigIndexPrefsKey);
    }

    /// <summary>
    /// 加载
    /// </summary>
    protected void ReloadAll()
    {
        JsonDirectoryConfigs.LoadAll();

        EditorGUIUtility.editingTextField = false;
        RefreshConfigNameList(GetLastConfigIndex());
    }

    protected bool HasCurEditConfig()
    {
        if (m_curEditIndex != -1 && m_curEditConfig != null)
            return true;

        return false;
    }

    protected void RefreshConfigNameList (int defaultIndex=0)
    {
        m_editConfigNameList = JsonDirectoryConfigs.GetDirectoryFileList();
        SelectConfig(defaultIndex);
    }

    public string GetCurEditConfigName()
    {
        if (m_editConfigNameList != null && m_curEditIndex >= 0 && m_curEditIndex < m_editConfigNameList.Count)
            return m_editConfigNameList[m_curEditIndex];
        else
            return string.Empty;
    }

    /// <summary>
    /// 选择配置
    /// </summary>
    /// <param name="index"></param>
    protected void SelectConfig(int index)
    {
        EditorGUIUtility.editingTextField = false;
        if (m_editConfigNameList != null && m_editConfigNameList.Count>0)
        {
            if (index < 0 || index >= m_editConfigNameList.Count)
            {
                index = 0;
            }
            string configName = m_editConfigNameList[index];
            if (JsonDirectoryConfigs.Get(configName, out m_curEditConfig))
            {
                m_curEditIndex = index;
                ActionAfterSelect();
                return;
            }
        }
        m_curEditIndex = -1;
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    protected virtual void SaveCurrentConfig()
    {
        ActionBeforeSave();
        EditorGUIUtility.editingTextField = false;
        JsonDirectoryConfigs.Set(GetCurEditConfigName(), m_curEditConfig);
    }

    /// <summary>
    /// 重命名文件
    /// </summary>
    protected void Rename()
    {
        EditorGUIUtility.editingTextField = false;
        string newName = m_curEditConfig.GetKey();
        if (JsonDirectoryConfigs.Rename(GetCurEditConfigName(), newName))
        {
            m_editConfigNameList = JsonDirectoryConfigs.GetDirectoryFileList();
            m_curEditIndex = -1;
            for (int i = 0; i < m_editConfigNameList.Count; ++i)
            {
                if (m_editConfigNameList[i].Equals(newName))
                {
                    m_curEditIndex = i;
                    ActionAfterRename();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 添加配置
    /// </summary>
    /// <param name="configName"></param>
    /// <returns></returns>
    protected bool AddConfig(string configName)
    {
        T newObj = new T();
        return AddConfig(configName, newObj);
    }
    protected bool AddConfig(string configName, T config)
    {
        EditorGUIUtility.editingTextField = false;
        if (JsonDirectoryConfigs.Add(configName, config))
        {
            m_editConfigNameList = JsonDirectoryConfigs.GetDirectoryFileList();
            JsonDirectoryConfigs.Get(configName, out m_curEditConfig);
            m_curEditIndex = -1;
            for (int i = 0; i < m_editConfigNameList.Count; ++i)
            {
                if (m_editConfigNameList[i].Equals(configName))
                {
                    m_curEditIndex = i;
                    ActionAfterAdd();
                    break;
                }
            }
            return true;
        }

        EditorUtility.DisplayDialog("添加配置失败", "不能添加已存在的文件配置！同名文件："+ configName, "确定");
        return false;
    }

    /// <summary>
    /// 删除配置
    /// </summary>
    protected void RemoveConfig()
    {
        JsonDirectoryConfigs.Remove(m_curEditConfig);
        EditorGUIUtility.editingTextField = false;
        RefreshConfigNameList();
    }

    /// <summary>
    /// 添加新配置
    /// </summary>
    protected virtual void OnAddNewConfig()
    {
        //AddConfig(string.Format("rs_{0}", System.DateTime.Now.Ticks));
        EditorMessageBox.Display("新建文件", "文件名", string.Empty, (name) =>
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("配置文件名不能为空！");
                EditorUtility.DisplayDialog("添加配置失败", "配置文件名不能为空！", "确定");
                return;
            }
            AddConfig(name);
        });
    }

    /// <summary>
    /// 选择配置之后
    /// </summary>
    protected virtual void ActionAfterSelect()
    {

    }

    /// <summary>
    /// 保存配置之前
    /// </summary>
    protected virtual void ActionBeforeSave()
    {

    }

    /// <summary>
    /// 新增配置之后
    /// </summary>
    protected virtual void ActionAfterAdd()
    {

    }

    /// <summary>
    /// 重命名之后
    /// </summary>
    protected virtual void ActionAfterRename()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void DrawToolBarEx()
    {

    }
}

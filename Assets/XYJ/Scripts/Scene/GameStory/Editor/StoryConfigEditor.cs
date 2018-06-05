using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using xys.GameStory;
using System;
using EditorExtensions;


public class StoryConfigEditor : JsonDirectoryEditorBase<StoryConfig>
{

    protected override string GetJsonDirectoryPath()
    {
        return "Data/Config/Edit/GameStory/Story/";
    }

    [MenuItem("Tools/剧情编辑/游戏剧情配置", false, 100)]
    static void ShowStoryConfigEditor()
    {
        StoryConfigEditor window = GetWindow<StoryConfigEditor>(" 游戏剧情");
        window.minSize = new Vector2(720.0f, 480.0f);
        window.titleContent.image = EditorIconContent.GetSystem(EditorIconContentType.Clipboard).image;
    }

    Vector2 m_leftScrollPos = Vector3.zero;
    Vector2 m_rightScrollPos = Vector2.zero;

    int m_toolbarCurSelected = 0;
    GUIContent[] m_toolbarContents = new GUIContent[] {
        new GUIContent("事件"),
        new GUIContent("对象"),
        new GUIContent("点集"),
        new GUIContent("路点"),
        new GUIContent("镜头"),};

    bool m_eventListToggle = true;
    bool m_objectListToggle = true;
    GUIToggleTween m_baseInfoToggle = new GUIToggleTween();

    protected override void ActionAfterSelect()
    {
        if (Application.isPlaying && StoryPlayer.Current != null && StoryPlayer.Current.IsRunning)
        {
            StoryPlayer.Current.Stop();
        }
        if (m_curEditConfig != null)
        {
            m_curEditConfig.InitEvents();
        }
    }

    protected override void ActionAfterAdd()
    {
        if (m_curEditConfig != null)
        {
            m_curEditConfig.InitEvents();
        }
    }

    void OnEnable()
    {
        // 打开就加载
        ReloadAll();

        m_toolbarCurSelected = 0;
        m_eventListToggle = true;
    }

    void OnDisable()
    {
        RecordLastConfigIndex();
        // 关闭时，如果有修改就提示是否保存
        CheckSaveOnClose();
    }

    void Update()
    {
        Repaint();
    }

    // Implement your own editor GUI here.
    void OnGUI()
    {
        using (new AutoEditorVertical())//GUILayout.MinWidth(600)
        {
            using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
            {
                // 工具栏
                DrawToolbarView();
            }

            // 是否有编辑内容提示
            if (!HasCurEditConfig())
            {
                EditorGUILayout.HelpBox("请选择或添加配置进行编辑！", MessageType.Warning);
                return;
            }

            // 显示编辑内容
            using (new AutoEditorHorizontal())
            {
                DrawLeftView();

                DrawRightView();// 做分页
            }
        }

        //if (GUI.changed)
        //{
        //    EditorUtility.SetDirty(target);
        //}
    }

    void DrawLeftView()
    {
        using (new AutoEditorVertical(EditorStylesEx.PreferencesSectionBox, GUILayout.Width(240)))
        {

            // 剧情基本信息
            using (new AutoEditorScrollView(ref m_leftScrollPos))
            {
                using (new AutoEditorToggleArea(m_baseInfoToggle, "基本信息"))
                {
                    // 剧情描述
                    using (new AutoEditorLabelWidth(80))
                    {
                        EditorGUILayout.PrefixLabel(new GUIContent("剧情描述:"));

                        m_curEditConfig.description = EditorGUILayout.TextArea(m_curEditConfig.description, GUILayout.Height(30));

                        m_curEditConfig.stopGameLogic = EditorGUILayout.Toggle("暂停游戏逻辑", m_curEditConfig.stopGameLogic);

                        m_curEditConfig.isCanSkip = EditorGUILayout.Toggle("允许跳过剧情", m_curEditConfig.isCanSkip);

                        m_curEditConfig.shieldActorsType = EditorGUILayout.IntField("屏蔽角色类型", m_curEditConfig.shieldActorsType);
                    }
                }
            }

            // 剧情编辑工具
            using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
            {
                // 播放和停止
                GUIContent playBtn = EditorIconContent.GetSystem(EditorIconContentType.PlayButton);
                if (Application.isPlaying && StoryPlayer.Current != null && StoryPlayer.Current.IsRunning)
                {
                    playBtn = EditorIconContent.GetSystem(EditorIconContentType.PlayButton_On);
                }

                if (GUILayout.Toggle(false, playBtn, EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    if (Application.isPlaying)
                    {
                        if (StoryPlayer.Current != null && StoryPlayer.Current.IsRunning)
                        {
                            StoryPlayer.Current.Stop();
                        }
                        else if (StoryPlayer.Current == null || !StoryPlayer.Current.IsRunning)
                        {
                            GameStoryMgr.BeginStory(m_curEditConfig);
                        }
                    }
                    else
                    {
                        ShowNotification(new GUIContent("运行时才能执行！"));
                    }
                }

                // 暂停和恢复
                //GUIContent pauseBtn = EditorIconContent.GetSystem(EditorIconContentType.PauseButton);
                //if (Application.isPlaying && StoryPlayer.Current != null && StoryPlayer.Current.IsRunning && StoryPlayer.Current.EventTimeLine.IsPause)
                //{
                //    pauseBtn = EditorIconContent.GetSystem(EditorIconContentType.PauseButton_On);
                //}

                //if (GUILayout.Toggle(false, pauseBtn, EditorStyles.toolbarButton, GUILayout.Width(30)))
                //{
                //    if (Application.isPlaying && StoryPlayer.Current != null && StoryPlayer.Current.IsRunning)
                //    {
                //        if (StoryPlayer.Current.EventTimeLine.IsPlaying)
                //            StoryPlayer.Current.Pause();
                //        else if (StoryPlayer.Current.EventTimeLine.IsPause)
                //            StoryPlayer.Current.Resume();
                //    }
                //    else
                //    {
                //        ShowNotification(new GUIContent("运行时才能执行！"));
                //    }
                //}

                EditorGUILayout.Separator();

                //GUILayout.Button("关卡编辑", EditorStyles.toolbarButton, GUILayout.Width(60));
            }
        }
    }

    void DrawRightView()
    {
        using (new AutoEditorVertical(EditorStylesEx.PreferencesSectionBox))
        {

            switch (m_toolbarCurSelected)
            {
                case 0:
                    DrawEventList();
                    break;
                case 1:
                    DrawObjectList();
                    break;
                case 2:
                    DrawPointsList();
                    break;
                case 3:
                    DrawWayPointsList();
                    break;
                case 4:
                    DrawCamPointsList();
                    break;
            }

            // 剧情事件编辑工具
            using (new AutoEditorHorizontal(EditorStyles.toolbar))
            {
                int tmpSelected = GUILayout.Toolbar(m_toolbarCurSelected, m_toolbarContents, EditorStyles.toolbarButton);
                if (tmpSelected != m_toolbarCurSelected)
                {
                    m_toolbarCurSelected = tmpSelected;
                    EditorGUIUtility.editingTextField = false;
                }

                EditorGUILayout.Separator();
                // 播放
                if (GUILayout.Toggle(false, EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Plus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    if (m_toolbarCurSelected == 0)
                        m_curEditConfig.AddEvent();
                    else if (m_toolbarCurSelected == 1)
                        m_curEditConfig.AddObject();
                    else if (m_toolbarCurSelected == 2)
                        m_curEditConfig.AddPoints<Points>();
                    else if (m_toolbarCurSelected == 3)
                        m_curEditConfig.AddPoints<WayPoints>();
                    else if (m_toolbarCurSelected == 4)
                        m_curEditConfig.AddPoints<CamPoints>();

                }
                if (GUILayout.Toggle(false, EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Trash), EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    if (m_toolbarCurSelected == 0)
                        m_curEditConfig.ClearEvents();
                    else if (m_toolbarCurSelected == 1)
                        m_curEditConfig.ClearObjects();
                    else if (m_toolbarCurSelected == 2)
                        m_curEditConfig.ClearPoints<Points>();
                    else if (m_toolbarCurSelected == 3)
                        m_curEditConfig.ClearPoints<WayPoints>();
                    else if (m_toolbarCurSelected == 4)
                        m_curEditConfig.ClearPoints<CamPoints>();

                }

            }
        }

    }

    void DrawEventList()
    {
        using (new AutoEditorHorizontal(EditorStyles.toolbar))
        {

            bool tmp = EditorGUILayout.Toggle(m_eventListToggle, GUILayout.Width(15));
            if (m_eventListToggle != tmp)
            {
                m_eventListToggle = tmp;
                for (int i = 0; i < m_curEditConfig.eventList.Count; ++i)
                {
                    StoryEventElement element = m_curEditConfig.eventList[i];
                    element.toggle = tmp;
                }
            }

            GUILayout.Label(new GUIContent("事件类型"), EditorStyles.toolbarButton, GUILayout.Width(120));

            GUILayout.Label(new GUIContent("Start"), EditorStyles.toolbarButton, GUILayout.Width(44));

            GUILayout.Label(new GUIContent("End"), EditorStyles.toolbarButton, GUILayout.Width(44));

            GUILayout.Label(new GUIContent("描述信息"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUILayout.Label(new GUIContent("编辑工具"), EditorStyles.toolbarButton, GUILayout.Width(90));
        }

        // 剧情事件列表
        using (new AutoEditorScrollView(ref m_rightScrollPos))
        {
            for (int i = 0; i < m_curEditConfig.eventList.Count; ++i)
            {
                StoryEventElement element = m_curEditConfig.eventList[i];
                using (new AutoEditorVertical(EditorStylesEx.FrameArea))
                {
                    using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
                    {
                        element.toggle = EditorGUILayout.Toggle(element.toggle, GUILayout.Width(15));
                        // Type
                        StoryEventType type = (StoryEventType)EditorGUILayout.EnumPopup(element.type, EditorStyles.toolbarPopup, GUILayout.Width(120));
                        if (type != element.type)
                        {
                            element.SetType(type);
                        }

                        // start time
                        element.startTime = EditorGUILayout.FloatField(element.startTime, EditorStyles.toolbarTextField, GUILayout.Width(40));

                        // end time
                        element.endTime = EditorGUILayout.FloatField(element.endTime, EditorStyles.toolbarTextField, GUILayout.Width(40));

                        // describe
                        element.describe = EditorGUILayout.TextField(element.describe, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));

                        // tools
                        // delete
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            m_curEditConfig.RemoveEventAt(i);
                        }

                        // copy
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.TreeEditor_Duplicate), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            m_curEditConfig.CloneEvent(element);
                        }

                        // move
                        if (GUILayout.Button(new GUIContent("↕"), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            GenericMenu moveMenu = new GenericMenu();
                            for (int tmp = 0; tmp < m_curEditConfig.eventList.Count; ++tmp)
                            {
                                if (tmp == i)
                                    continue;
                                int movePos = tmp;
                                if (tmp > i)
                                    movePos += 1;
                                moveMenu.AddItem(new GUIContent(string.Format("移动{0}位置", tmp)), false, (obj) =>
                                {
                                    m_curEditConfig.MoveEvent((int)obj, element);
                                }, movePos);
                            }
                            moveMenu.ShowAsContext();
                        }
                    }

                    // 事件的详细编辑
                    if (element.toggle)
                    {
                        StoryEventDataEditor.DrawEventData(element);
                    }
                }
            }
        }
    }

    void DrawObjectList()
    {
        using (new AutoEditorHorizontal(EditorStyles.toolbar))
        {

            bool tmp = EditorGUILayout.Toggle(m_objectListToggle, GUILayout.Width(15));
            if (m_objectListToggle != tmp)
            {
                m_objectListToggle = tmp;
                for (int i = 0; i < m_curEditConfig.objectList.Count; ++i)
                {
                    StoryObjectElement element = m_curEditConfig.objectList[i];
                    element.toggle = tmp;
                }
            }

            GUILayout.Label(new GUIContent("角色ID"), EditorStyles.toolbarButton, GUILayout.Width(120));

            GUILayout.Label(new GUIContent("角色名"), EditorStyles.toolbarButton, GUILayout.Width(90));

            GUILayout.Label(new GUIContent("角色模型"), EditorStyles.toolbarButton, GUILayout.Width(90));

            GUILayout.Label(new GUIContent("描述信息"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUILayout.Label(new GUIContent("编辑工具"), EditorStyles.toolbarButton, GUILayout.Width(90));
        }

        // 处理id
        Dictionary<string, int> idList = new Dictionary<string, int>();
        for (int i = 0; i < m_curEditConfig.objectList.Count; ++i)
        {
            string id = m_curEditConfig.objectList[i].storyObjectID;
            if (!string.IsNullOrEmpty(id))
            {
                if (!idList.ContainsKey(id))
                    idList.Add(id, 1);
                else
                    idList[id] += 1;
            }
        }

        // 剧情事件列表
        using (new AutoEditorScrollView(ref m_rightScrollPos))
        {
            for (int i = 0; i < m_curEditConfig.objectList.Count; ++i)
            {
                StoryObjectElement element = m_curEditConfig.objectList[i];
                using (new AutoEditorVertical(EditorStylesEx.FrameArea))
                {
                    using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
                    {
                        element.toggle = EditorGUILayout.Toggle(element.toggle, GUILayout.Width(15));
                        // ID
                        element.storyObjectID = EditorGUILayout.TextField(element.storyObjectID, EditorStyles.toolbarTextField, GUILayout.Width(120));

                        // name
                        element.objectDefine.name = EditorGUILayout.TextField(element.objectDefine.name, EditorStyles.toolbarTextField, GUILayout.Width(90));

                        // model
                        element.objectDefine.model = EditorGUILayout.TextField(element.objectDefine.model, EditorStyles.toolbarTextField, GUILayout.Width(90));

                        // describe
                        element.describe = EditorGUILayout.TextField(element.describe, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));

                        // tools
                        // delete
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            m_curEditConfig.RemoveObjectAt(i);
                        }

                        // 编辑
                        if (GUILayout.Button("编辑位置", EditorStyles.toolbarButton, GUILayout.Width(60)))
                        {
                            PointSetObjectEditorWindow.OpenWindow(element.points, ObjectPointsCallback);
                        }
                    }

                    if (string.IsNullOrEmpty(element.storyObjectID) || (idList.ContainsKey(element.storyObjectID) && idList[element.storyObjectID] > 1))
                    {
                        EditorGUILayout.HelpBox("刷新点ID不能为空或重复！", MessageType.Error);
                    }

                    if (element.points.Count < 1)
                    {
                        EditorGUILayout.HelpBox("刷新点没有编辑位置信息！", MessageType.Warning);
                    }

                    // 事件的详细编辑
                    if (element.toggle && element.points.Count > 0)
                    {
                        for (int p = 0; p < element.points.Count; ++p)
                        {
                            element.points.positions[p] = EditorGUILayout.Vector3Field(string.Format("Position({0})", p), element.points.positions[p]);
                        }
                    }
                }
            }
        }
    }

    void ObjectPointsCallback(Points data)
    {

    }

    void DrawPointsList()
    {
        using (new AutoEditorHorizontal(EditorStyles.toolbar))
        {

            //bool tmp = EditorGUILayout.Toggle(m_eventListToggle, GUILayout.Width(15));
            //if (m_eventListToggle != tmp)
            //{
            //    m_eventListToggle = tmp;
            //    for (int i = 0; i < m_curEditConfig.pointsList.Count; ++i)
            //    {
            //        Points element = m_curEditConfig.pointsList[i];
            //        element.toggle = tmp;
            //    }
            //}

            GUILayout.Label(new GUIContent("点集ID"), EditorStyles.toolbarButton, GUILayout.Width(120));

            GUILayout.Label(new GUIContent("描述信息"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUILayout.Label(new GUIContent("数量"), EditorStyles.toolbarButton, GUILayout.Width(90));

            GUILayout.Label(new GUIContent("编辑工具"), EditorStyles.toolbarButton, GUILayout.Width(90));
        }

        // 处理id
        Dictionary<string, int> idList = new Dictionary<string, int>();
        for (int i = 0; i < m_curEditConfig.pointsList.Count; ++i)
        {
            string id = m_curEditConfig.pointsList[i].id;
            if (!string.IsNullOrEmpty(id))
            {
                if (!idList.ContainsKey(id))
                    idList.Add(id, 1);
                else
                    idList[id] += 1;
            }
        }

        // 剧情事件列表
        using (new AutoEditorScrollView(ref m_rightScrollPos))
        {
            for (int i = 0; i < m_curEditConfig.pointsList.Count; ++i)
            {
                Points element = m_curEditConfig.pointsList[i];
                using (new AutoEditorVertical(EditorStylesEx.FrameArea))
                {
                    using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
                    {
                        //element.toggle = EditorGUILayout.Toggle(element.toggle, GUILayout.Width(15));
                        // ID
                        element.id = EditorGUILayout.TextField(element.id, EditorStyles.toolbarTextField, GUILayout.Width(120));

                        // describe
                        element.describe = EditorGUILayout.TextField(element.describe, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));

                        // count
                        GUILayout.Label (string.Format("Count:{0}",element.Count), GUILayout.Width(90));

                        // tools
                        // delete
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            m_curEditConfig.RemovePointsAt<Points>(i);
                        }

                        // 编辑
                        if (GUILayout.Button("编辑", EditorStyles.toolbarButton, GUILayout.Width(60)))
                        {
                            PointSetObjectEditorWindow.OpenWindow(element, PointSetCallback);
                        }
                    }

                    if (string.IsNullOrEmpty(element.id) || (idList.ContainsKey(element.id) && idList[element.id] > 1))
                    {
                        EditorGUILayout.HelpBox("点集ID不能为空或重复！", MessageType.Error);
                    }

                    if (element.Count < 1)
                    {
                        EditorGUILayout.HelpBox("该点集还没编辑！", MessageType.Warning);
                    }

                    // 事件的详细编辑
                    //if (element.toggle)
                    //{
                    //    StoryEventDataEditor.DrawEventData(element);
                    //}
                }
            }
        }
    }

    void PointSetCallback(Points data)
    {
        
    }

    void DrawWayPointsList()
    {
        using (new AutoEditorHorizontal(EditorStyles.toolbar))
        {

            //bool tmp = EditorGUILayout.Toggle(m_eventListToggle, GUILayout.Width(15));
            //if (m_eventListToggle != tmp)
            //{
            //    m_eventListToggle = tmp;
            //    for (int i = 0; i < m_curEditConfig.wayPointsList.Count; ++i)
            //    {
            //        Points element = m_curEditConfig.wayPointsList[i];
            //        element.toggle = tmp;
            //    }
            //}

            GUILayout.Label(new GUIContent("路点ID"), EditorStyles.toolbarButton, GUILayout.Width(120));

            GUILayout.Label(new GUIContent("描述信息"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUILayout.Label(new GUIContent("点数量"), EditorStyles.toolbarButton, GUILayout.Width(90));

            GUILayout.Label(new GUIContent("编辑工具"), EditorStyles.toolbarButton, GUILayout.Width(90));
        }

        // 处理id
        Dictionary<string, int> idList = new Dictionary<string, int>();
        for (int i = 0; i < m_curEditConfig.wayPointsList.Count; ++i)
        {
            string id = m_curEditConfig.wayPointsList[i].id;
            if (!string.IsNullOrEmpty(id))
            {
                if (!idList.ContainsKey(id))
                    idList.Add(id, 1);
                else
                    idList[id] += 1;
            }
        }

        // 剧情事件列表
        using (new AutoEditorScrollView(ref m_rightScrollPos))
        {
            for (int i = 0; i < m_curEditConfig.wayPointsList.Count; ++i)
            {
                WayPoints element = m_curEditConfig.wayPointsList[i];
                using (new AutoEditorVertical(EditorStylesEx.FrameArea))
                {
                    using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
                    {
                        //element.toggle = EditorGUILayout.Toggle(element.toggle, GUILayout.Width(15));
                        // ID
                        element.id = EditorGUILayout.TextField(element.id, EditorStyles.toolbarTextField, GUILayout.Width(120));

                        // describe
                        element.describe = EditorGUILayout.TextField(element.describe, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));

                        // count
                        GUILayout.Label(string.Format("Count:{0}", element.Count), GUILayout.Width(90));

                        // tools
                        // delete
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            m_curEditConfig.RemovePointsAt<WayPoints>(i);
                        }

                        // 编辑
                        if (GUILayout.Button("编辑", EditorStyles.toolbarButton, GUILayout.Width(60)))
                        {
                            WayPointsObjectEditorWindow.OpenWindow(element, WayPointsEditCallback);
                        }
                    }

                    if (string.IsNullOrEmpty(element.id) || (idList.ContainsKey(element.id) && idList[element.id] > 1))
                    {
                        EditorGUILayout.HelpBox("路点ID不能为空或重复！", MessageType.Error);
                    }

                    if (element.Count <= 1)
                    {
                        EditorGUILayout.HelpBox("该路点信息还没编辑或路点少于2！", MessageType.Warning);
                    }

                    // 事件的详细编辑
                    //if (element.toggle)
                    //{
                    //    StoryEventDataEditor.DrawEventData(element);
                    //}
                }
            }
        }
    }

    void WayPointsEditCallback(WayPoints data)
    {

    }

    void DrawCamPointsList()
    {
        using (new AutoEditorHorizontal(EditorStyles.toolbar))
        {

            //bool tmp = EditorGUILayout.Toggle(m_eventListToggle, GUILayout.Width(15));
            //if (m_eventListToggle != tmp)
            //{
            //    m_eventListToggle = tmp;
            //    for (int i = 0; i < m_curEditConfig.camPointsList.Count; ++i)
            //    {
            //        CamPoints element = m_curEditConfig.camPointsList[i];
            //        element.toggle = tmp;
            //    }
            //}

            GUILayout.Label(new GUIContent("镜头ID"), EditorStyles.toolbarButton, GUILayout.Width(120));

            GUILayout.Label(new GUIContent("描述信息"), EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));

            GUILayout.Label(new GUIContent("点数量"), EditorStyles.toolbarButton, GUILayout.Width(90));

            GUILayout.Label(new GUIContent("编辑工具"), EditorStyles.toolbarButton, GUILayout.Width(90));
        }

        // 处理id
        Dictionary<string, int> idList = new Dictionary<string, int>();
        for (int i = 0; i < m_curEditConfig.camPointsList.Count; ++i)
        {
            string id = m_curEditConfig.camPointsList[i].id;
            if (!string.IsNullOrEmpty(id))
            {
                if (!idList.ContainsKey(id))
                    idList.Add(id, 1);
                else
                    idList[id] += 1;
            }
        }

        // 剧情事件列表
        using (new AutoEditorScrollView(ref m_rightScrollPos))
        {
            for (int i = 0; i < m_curEditConfig.camPointsList.Count; ++i)
            {
                CamPoints element = m_curEditConfig.camPointsList[i];
                using (new AutoEditorVertical(EditorStylesEx.FrameArea))
                {
                    using (new AutoEditorHorizontal(EditorStyles.toolbarButton))
                    {
                        //element.toggle = EditorGUILayout.Toggle(element.toggle, GUILayout.Width(15));
                        // ID
                        element.id = EditorGUILayout.TextField(element.id, EditorStyles.toolbarTextField, GUILayout.Width(120));

                        // describe
                        element.describe = EditorGUILayout.TextField(element.describe, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));

                        // count
                        GUILayout.Label(string.Format("Count:{0}", element.Count), GUILayout.Width(90));

                        // tools
                        // delete
                        if (GUILayout.Button(EditorIconContent.GetSystem(EditorIconContentType.Toolbar_Minus), EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            m_curEditConfig.RemovePointsAt<CamPoints>(i);
                        }

                        // 编辑
                        if (GUILayout.Button("编辑", EditorStyles.toolbarButton, GUILayout.Width(60)))
                        {
                            CamPointsObjectEditorWindow.OpenWindow(element, CamAnimEditCallback);
                        }
                    }

                    if (string.IsNullOrEmpty(element.id) || (idList.ContainsKey(element.id) && idList[element.id]>1))
                    {
                        EditorGUILayout.HelpBox("镜头ID不能为空或重复！", MessageType.Error);
                    }

                    if (element.Count <= 1)
                    {
                        EditorGUILayout.HelpBox("该镜头路点信息还没编辑或路点少于2！", MessageType.Warning);
                    }

                    // 事件的详细编辑
                    //if (element.toggle)
                    //{
                    //    StoryEventDataEditor.DrawEventData(element);
                    //}
                }
            }
        }
    }

    // Callback Methods

    void CamAnimEditCallback(CamPoints data)
    {
        Debug.LogWarning("count:" + data.Count);
    }

}

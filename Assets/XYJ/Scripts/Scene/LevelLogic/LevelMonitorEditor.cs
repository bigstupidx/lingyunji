#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using xys;
using EditorExtensions;
using GUIEditor;

public class LevelMonitorEditor : EditorWindow
{
    static public LevelMonitorEditor Instance { get; protected set; }

    //滚动位置
    Vector2 m_scrollPos;
    //全局事件
    List<LevelDesignConfig.LevelEventObjData> oaEventList;
    //局部事件
    List<LevelDesignConfig.LevelEventObjData> eventList;
    Dictionary<string, Config.EventMonitorVo> conditionVoDic;
    Enum2Type.Etype eConditionType;
    Enum2Type.Etype eActionType;

    static public void OpenLevelMonitor()
    {
        Instance = GetWindow<LevelMonitorEditor>(false, "LevelMonitorEditor", true);
        Instance.minSize = new Vector2(640.0f, 480.0f);

        Instance.titleContent.image = EditorIconContent.GetSystem(EditorIconContentType.Terrain_Icon).image;
    }

    void OnEnable()
    {
        oaEventList = null;
        eventList = null;
        conditionVoDic = null;
    }

    //绘制事件列表
    void OnGUI()
    {
        using (new AutoEditorScrollView(ref m_scrollPos))
        {
            if (GUILayout.Button("刷新数据"))
            {
                if (App.my != null && App.my.appStateMgr != null && App.my.appStateMgr.curState == AppStateType.GameIn)
                {
                    GetData();
                }
            }
            if (App.my != null && App.my.appStateMgr != null && App.my.appStateMgr.curState == AppStateType.GameIn)
            {
                if (null != oaEventList)
                {
                    using (new AutoEditorVertical(EditorStylesEx.BoxArea))
                    {
                        EditorGUILayout.LabelField("全局事件");
                        GUILayout.Space(5);

                        int index = 0;
                        foreach (var itor in oaEventList)
                        {
                            GUI.color = SetColor(index % 2 != 0);
                            Show(itor, GUI.color);
                            index++;
                        }
                    }
                }
                GUILayout.Space(20);
                if (null != eventList)
                {
                    using (new AutoEditorVertical(EditorStylesEx.BoxArea))
                    {
                        EditorGUILayout.LabelField("局部事件");
                        GUILayout.Space(5);

                        int index = 0;
                        foreach (var itor in eventList)
                        {
                            GUI.color = SetColor(index % 2 == 0);
                            Show(itor, GUI.color);
                            index++;
                        }
                    }
                }
            }
        }
    }

    void Show(LevelDesignConfig.LevelEventObjData itor, Color color)
    {
        string eventId = itor.m_eventId;
        List<LevelDesignConfig.LevelEventCondition> conditionList = itor.m_conditions;
        List<LevelDesignConfig.LevelEventAction> actionList = itor.m_actions;
        using (new AutoEditorVertical(EditorStylesEx.BoxArea))
        {
            using (new AutoEditorHorizontal())
            {
                EditorGUILayout.LabelField("事件id: " + itor.m_eventId + "   ", GUILayout.Width(150));
                itor.m_showMonitor = EditorGUILayout.Toggle(itor.m_showMonitor, GUILayout.Width(50));
            }

            if (!itor.m_showMonitor)
                return;

            EditorGUILayout.LabelField("条件");
            for (int i = 0; i < conditionList.Count; ++i)
            {
                bool meet = false;
                if (IsConditionMeet(itor.m_eventId, i))
                {
                    //条件已经成立
                    meet = true;
                }

                //显示条件描述
                using (new AutoEditorHorizontal())
                {
                    GUI.color = color;
                    EditorGUILayout.LabelField(eConditionType.etype.showTextList[eConditionType.etype.FindIndex(conditionList[i].m_conditionType)]);
                    string text = "";
                    if (meet)
                    {
                        GUI.color = Color.green;
                        text = "已达成";
                    }
                    else
                    {
                        GUI.color = Color.red;
                        text = "未达成";
                    }
                    EditorGUILayout.LabelField(text);
                    GUI.color = color;
                }
            }

            GUILayout.Space(5);
            EditorGUILayout.LabelField("事件");
            for (int i = 0; i < actionList.Count; ++i)
            {
                bool meet = false;
                if (IsActionMeet(itor.m_eventId, i))
                {
                    //条件已经成立
                    meet = true;
                }

                //显示条件描述
                using (new AutoEditorHorizontal())
                {
                    GUI.color = color;
                    EditorGUILayout.LabelField(eActionType.etype.showTextList[eActionType.etype.FindIndex(actionList[i].m_actionType)]);
                    string text = "";
                    if (meet)
                    {
                        GUI.color = Color.green;
                        text = "已触发";
                    }
                    else
                    {
                        GUI.color = Color.red;
                        text = "未触发";
                    }
                    EditorGUILayout.LabelField(text);
                    GUI.color = color;
                }
            }
        }
    }

    void GetData()
    {
        LocalPlayer m_mainPlayer = App.my.localPlayer;
        if (m_mainPlayer == null)
            return;
            
        //全局事件
        oaEventList = m_mainPlayer.GetModule<LevelModule>().GetEventList(true);
        //局部事件
        eventList = m_mainPlayer.GetModule<LevelModule>().GetEventList(false);
        conditionVoDic = m_mainPlayer.GetModule<LevelModule>().GetConditionVo();

        eConditionType = Enum2Type.To(typeof(LevelDesignConfig.ConditionType));
        eActionType = Enum2Type.To(typeof(LevelDesignConfig.ActionType));
    }

    //判断条件是否满足
    bool IsConditionMeet(string eventId, int index)
    {
        Config.EventMonitorVo vo = GetConditionVo(eventId);
        if(vo != null)
        {
            if(vo.conditionIndexList.Contains(index))
            {
                return true;
            }
        }
        return false;
    }

    bool IsActionMeet(string eventId, int index)
    {
        Config.EventMonitorVo vo = GetConditionVo(eventId);
        if (vo != null)
        {
            if (vo.actionIndexList.Contains(index))
            {
                return true;
            }
        }
        return false;
    }

    Config.EventMonitorVo GetConditionVo(string eventId)
    {
        Config.EventMonitorVo vo;
        if (conditionVoDic.TryGetValue(eventId, out vo))
        {
            return vo;
        }
        return null;
    }

    public Color SetColor(bool change)
    {
        if (change)
        {
            return Color.cyan;
        }
        return Color.white;
    }
}
#endif
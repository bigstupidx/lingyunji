using UnityEngine;
using UnityEditor;
using EditorExtensions;
using System;
using GUIEditor;
using System.Collections.Generic;

[CustomEditor(typeof(LevelDesignEventObj))]
[CanEditMultipleObjects]
public class LevelDesignEventObjEditor : EditorX<LevelDesignEventObj>
{
    bool m_isOverAll;
    Transform m_parent;

    protected override void OnEnable()
    {
        gameobject = target.gameObject;
        m_parent = gameobject.transform.parent;
        m_isOverAll = LevelDesignEditor.Instance.IsOverallEvent(gameobject.transform);
        base.OnEnable();
        //target.m_data.m_eventId = gameobject.name;
        //Add();

        //LevelDesignEditor.Instance.Repaint();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LevelDesignEditor.Instance.Repaint();
    }

    protected override void MultipleEnable(LevelDesignEventObj target, string name)
    {
        if (m_isOverAll)
        {
            target.m_data.m_eventId = name;
            //if(LevelDesignEditor.Instance.AddOverallEvent(target.m_data))
            {
                LevelDesignEditor.Instance.SetOverallEventData();
                LevelDesignEditor.Instance.SortOverallEvent();
            }
        }
        else
        {
            if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
            {
                LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
                if (logic != null)
                {
                    target.m_data.m_eventId = name;
                    //if(LevelDesignEditor.Instance.AddEventObj(target.m_data, logic.m_data))
                    {
                        LevelDesignEditor.Instance.SetEventData();
                        LevelDesignEditor.Instance.SortEvent();
                    }
                    return;
                }
            }
        }
    }

    protected override void DestroyTargetFunction(LevelDesignEventObj target)
    {
        if(m_isOverAll)
        {
            //LevelDesignEditor.Instance.MinusOverallEvent(target.m_data);

            LevelDesignEditor.Instance.SetOverallEventData();
            LevelDesignEditor.Instance.SortOverallEvent();
        }
        else
        {
            //LevelDesignEditor.Instance.MinusEventObj(target.m_data);
            LevelDesignEditor.Instance.SetEventData();
            LevelDesignEditor.Instance.SortEvent();
        }
    }

    public override void OnInspectorGUI()
    {
        using (new AutoEditorDisabledGroup(true))
        {
            target.m_data.m_eventId = EditorGUILayout.TextField("事件id:", target.m_data.m_eventId);
        }

        //增加条件
        if (GUILayout.Button("增加条件", GUILayout.Height(30)))
        {
            target.m_data.m_conditions.Add(new LevelDesignConfig.LevelEventCondition());
        }
        //绘制条件列表
        DrawCondition();
        GUI.color = Color.white;
        //增加行为
        if (GUILayout.Button("增加行为", GUILayout.Height(30)))
        {
            target.m_data.m_actions.Add(new LevelDesignConfig.LevelEventAction());
        }
        //绘制行为列表
        DrawAction();

        if (target.m_data.m_eventId != gameobject.name)
        {
            target.m_data.m_eventId = gameobject.name;
            if (m_isOverAll)
                LevelDesignEditor.Instance.SortOverallEvent();
            else
                LevelDesignEditor.Instance.SortEvent();
            LevelDesignEditor.Instance.Repaint();
        }
    }

    void DrawCondition()
    {
        if (target.m_data.m_conditions.Count > 0)
        {
            target.m_data.m_conditions.Remove(null);
            Enum2Type.Etype etype = Enum2Type.To(typeof(LevelDesignConfig.ConditionType));

            //绘制条件列表
            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                for (int i = 0; i < target.m_data.m_conditions.Count;)
                {
                    GUI.color = SetColor(i % 2 != 0);
                    using (new AutoEditorVertical(EditorStylesEx.BoxArea))
                    {
                        LevelDesignConfig.LevelEventCondition condition = target.m_data.m_conditions[i];
                        LevelDesignConfig.ConditionType conditionType = condition.m_conditionType;
                        using (new AutoEditorHorizontal())
                        {
                            //或
                            EditorGUILayout.LabelField("或成立", GUILayout.Width(50));
                            condition.m_isOr = EditorGUILayout.Toggle(condition.m_isOr, GUILayout.Width(20));

                            //选择条件
                            DrawPopupLayout((int)conditionType, etype.etype.showTextList.ToArray(), value => target.SetConditionType(i, value), "", GUILayout.Width(250));

                            if (GUILayout.Button("删除", GUILayout.Width(50)))
                            {
                                target.m_data.m_conditions.RemoveAt(i);
                            }
                            else
                            {
                                ++i;
                            }
                        }
                        //参数列表
                        Enum2Type.E enumV = etype.etype.FindE((System.Enum)conditionType);
                        List<Enum2Type.ParamValue> paramList = enumV.paramList;
                        if (null != paramList && paramList.Count > 0)
                        {
                            for (int j = 0; j < paramList.Count; ++j)
                            {
                                Enum2Type.ParamValue param = paramList[j];
                                ShowParam(param, condition);
                            }
                        }
                    }

                }
            }
                
        }
    }

    void DrawAction()
    {
        if (target.m_data.m_actions.Count > 0)
        {
            target.m_data.m_actions.Remove(null);
            Enum2Type.Etype etype = Enum2Type.To(typeof(LevelDesignConfig.ActionType));

            using (new AutoEditorVertical(EditorStylesEx.BoxArea))
            {
                for (int i = 0; i < target.m_data.m_actions.Count;)
                {
                    GUI.color = SetColor(i % 2 != 0);
                    using (new AutoEditorVertical(EditorStylesEx.BoxArea))
                    {
                        LevelDesignConfig.LevelEventAction action = target.m_data.m_actions[i];
                        using (new AutoEditorHorizontal())
                        {
                            EditorGUILayout.LabelField("延迟(秒)", GUILayout.Width(50));
                            action.m_delay = EditorGUILayout.FloatField(action.m_delay, GUILayout.Width(50));
                            action.m_actionType = 
                                (LevelDesignConfig.ActionType)GuiTools.EnumPopup(false, action.m_actionType);
                            if (GUILayout.Button("删除", GUILayout.Width(50)))
                            {
                                target.m_data.m_actions.RemoveAt(i);
                            }
                            else
                            {
                                ++i;
                            }
                        }
                        //参数列表
                        Enum2Type.E enumV = etype.etype.FindE((System.Enum)action.m_actionType);
                        List<Enum2Type.ParamValue> paramList = enumV.paramList;
                        if (null != paramList && paramList.Count > 0)
                        {
                            for (int j = 0; j < paramList.Count; ++j)
                            {
                                Enum2Type.ParamValue param = paramList[j];
                                ShowParam(param, null, action);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ShowParam(Enum2Type.ParamValue param, LevelDesignConfig.LevelEventCondition condition = null, LevelDesignConfig.LevelEventAction action = null)
    {
        if (condition == null && action == null)
            return;

        string key = param.Key;
        string text = param.text;
        DataType dataType = param.dataType;
        Type realType = param.RealType;
        Value value = param.CloneValue;
        string initValue = param.InitValue;

        switch (key)
        {
            case "p1":
                if (condition != null)
                    value.value.SetValue(dataType, string.IsNullOrEmpty(condition.m_param1) ? initValue : condition.m_param1, realType);
                else
                    value.value.SetValue(dataType, string.IsNullOrEmpty(action.m_param1) ? initValue : action.m_param1, realType);
                break;
            case "p2":
                if (condition != null)
                    value.value.SetValue(dataType, string.IsNullOrEmpty(condition.m_param2) ? initValue : condition.m_param2, realType);
                else
                    value.value.SetValue(dataType, string.IsNullOrEmpty(action.m_param2) ? initValue : action.m_param2, realType);
                break;
            case "p3":
                if (condition != null)
                    value.value.SetValue(dataType, string.IsNullOrEmpty(condition.m_param3) ? initValue : condition.m_param3, realType);
                else
                    value.value.SetValue(dataType, string.IsNullOrEmpty(action.m_param3) ? initValue : action.m_param3, realType);
                break;
            case "p4":
                if (condition != null)
                    value.value.SetValue(dataType, string.IsNullOrEmpty(condition.m_param4) ? initValue : condition.m_param4, realType);
                else
                    value.value.SetValue(dataType, string.IsNullOrEmpty(action.m_param4) ? initValue : action.m_param4, realType);
                break;
            case "p5":
                if (condition != null)
                    value.value.SetValue(dataType, string.IsNullOrEmpty(condition.m_param5) ? initValue : condition.m_param5, realType);
                else
                    value.value.SetValue(dataType, string.IsNullOrEmpty(action.m_param5) ? initValue : action.m_param5, realType);
                break;
        }

        switch (dataType)
        {
            case DataType.Null:
                break;
            case DataType.Bool:
                value.value.SetBool(EditorGUILayout.Toggle(text, value.value.GetBool(false)));
                break;
            case DataType.Float:
                value.value.SetFloat(EditorGUILayout.FloatField(text, value.value.GetFloat(0.0f)));
                break;
            case DataType.Int:
                value.value.SetInt(EditorGUILayout.IntField(text, value.value.GetInt(0)));
                break;
            case DataType.String:
                value.value.SetString(EditorGUILayout.TextField(text, value.value.GetString("")));
                break;
            case DataType.Enum:
                break;
            default:
                break;
        }

        switch(key)
        {
            case "p1":
                if(condition != null)
                    condition.m_param1 = value.value.value.ToString();
                else
                    action.m_param1 = value.value.value.ToString();
                break;
            case "p2":
                if (condition != null)
                    condition.m_param2 = value.value.value.ToString();
                else
                    action.m_param2 = value.value.value.ToString();
                break;
            case "p3":
                if (condition != null)
                    condition.m_param3 = value.value.value.ToString();
                else
                    action.m_param3 = value.value.value.ToString();
                break;
            case "p4":
                if (condition != null)
                    condition.m_param4 = value.value.value.ToString();
                else
                    action.m_param4 = value.value.value.ToString();
                break;
            case "p5":
                if (condition != null)
                    condition.m_param5 = value.value.value.ToString();
                else
                    action.m_param5 = value.value.value.ToString();
                break;
        }
    }

    void Add()
    {
        //需要判断是全局事件还是局部事件
        if(m_isOverAll)
        {
            LevelDesignEditor.Instance.AddOverallEvent(target.m_data);
        }
        else
        {
            if (gameobject.transform.parent != null && gameobject.transform.parent.parent != null)
            {
                LevelDesignLogic logic = gameobject.transform.parent.parent.GetComponent<LevelDesignLogic>();
                if (logic != null)
                {
                    LevelDesignEditor.Instance.AddEventObj(target.m_data, logic.m_data);
                    return;
                }
            }
        }
    }
}

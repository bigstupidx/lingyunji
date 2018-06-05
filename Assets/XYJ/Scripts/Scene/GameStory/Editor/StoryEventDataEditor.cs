using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using xys.GameStory;
using EditorExtensions;

/// <summary>
/// 剧情事件数据编辑
/// </summary>
public class StoryEventDataEditor
{

	public static void DrawEventData(StoryEventElement element)
    {
        using (new AutoEditorVertical())//EditorStylesEx.BoxArea
        {
            using (new AutoEditorLabelWidth(60))
            {

                // start time
                element.startTime = EditorGUILayout.Slider("开始时间", element.startTime, 0, 100.0f);
                // end time
                element.endTime = EditorGUILayout.Slider("结束时间", element.endTime, 0, 100.0f);

                switch(element.type)
                {
                    // ------------
                    // 角色行为相关事件，都需要指定角色
                    case StoryEventType.角色动作:
                        DrawEventDataObjectAnim(element);
                        break;
                    case StoryEventType.角色冒泡:
                        DrawEventDataObjectBubble(element);
                        break;
                    case StoryEventType.角色随机冒泡:
                    case StoryEventType.角色特效:
                        DrawEventDataParam(element);
                        break;
                    case StoryEventType.角色移动:
                        DrawEventDataObjectMove(element);
                        break;

                    // -----------
                    // 刷新点相关事件
                    case StoryEventType.角色创建:
                        DrawEventDataParam(element, "角色ID");
                        break;
                    case StoryEventType.角色删除:
                        DrawEventDataParam(element, "角色ID");
                        break;

                    // -----------
                    // 整合的全局模块事件
                    case StoryEventType.点集特效:
                        DrawPointsFx(element);
                        break;
                    case StoryEventType.CG动画:
                        DrawEventDataParam(element, "动画名：");
                        break;
                    case StoryEventType.镜头动画:
                        DrawEventDataParam(element, "镜头ID：");
                        break;
                    case StoryEventType.对白事件:
                        DrawEventDataParam(element, "对白ID：");
                        break;
                    case StoryEventType.个性事件选项:
                        DrawEventDataParam(element, "个性事件ID：");
                        break;
                }

                // describe
                EditorGUILayout.TextField("数据：", element.eventCxt, EditorStyles.toolbarTextField, GUILayout.ExpandWidth(true));
            }
        }
    }

    static void DrawEventDataParam(StoryEventElement element, string textName = "参数：")
    {
        if (element.m_eventData is StoryEventDataParam)
        {
            StoryEventDataParam data = element.m_eventData as StoryEventDataParam;
            string param = EditorGUILayout.TextField(textName, data.m_param);
            if (param != data.m_param)
            {
                data.m_param = param;
                element.SaveEventData();
            }
        }
    }

    static void DrawEventDataObjectMove(StoryEventElement element)
    {
        if (element.m_eventData is StoryEventDataObjectMove)
        {
            StoryEventDataObjectMove data = element.m_eventData as StoryEventDataObjectMove;

            string param = EditorGUILayout.TextField("角色ID：", data.m_refreshId);
            if (param != data.m_refreshId)
            {
                data.m_refreshId = param;
                element.SaveEventData();
            }

            string pointsId = EditorGUILayout.TextField("点集ID：", data.m_pointsId);
            if (pointsId != data.m_pointsId)
            {
                data.m_pointsId = pointsId;
                element.SaveEventData();
            }

        }
    }

    static void DrawEventDataObjectBubble(StoryEventElement element)
    {
        if (element.m_eventData is StoryEventDataObjectBubble)
        {
            StoryEventDataObjectBubble data = element.m_eventData as StoryEventDataObjectBubble;
            string param = EditorGUILayout.TextField("角色ID", data.m_refreshId);
            if (param != data.m_refreshId)
            {
                data.m_refreshId = param;
                element.SaveEventData();
            }

            string text = EditorGUILayout.TextField("冒泡内容", data.m_bubbleText);
            if (text != data.m_bubbleText)
            {
                data.m_bubbleText = text;
                element.SaveEventData();
            }

            float timeLen = EditorGUILayout.FloatField("时长", data.m_playTime);
            if (timeLen != data.m_playTime)
            {
                data.m_playTime = timeLen;
                element.SaveEventData();
            }
        }
    }

    static void DrawEventDataObjectAnim(StoryEventElement element)
    {
        if (element.m_eventData is StoryEventDataObjectAnim)
        {
            StoryEventDataObjectAnim data = element.m_eventData as StoryEventDataObjectAnim;
            string param = EditorGUILayout.TextField("角色ID", data.m_refreshId);
            if (param != data.m_refreshId)
            {
                data.m_refreshId = param;
                element.SaveEventData();
            }

            string anim = EditorGUILayout.TextField("动画", data.m_animName);
            if (anim != data.m_animName)
            {
                data.m_animName = anim;
                element.SaveEventData();
            }

            bool loop = EditorGUILayout.Toggle("循环", data.m_isLoop);
            if (loop != data.m_isLoop)
            {
                data.m_isLoop = loop;
                element.SaveEventData();
            }

            float timeLen = EditorGUILayout.FloatField("时长", data.m_playTime);
            if (timeLen != data.m_playTime)
            {
                data.m_playTime = timeLen;
                element.SaveEventData();
            }
        }
    }

    #region 点集特效

    static StoryEventElement PointsFxEventElement;
    static void DrawPointsFx (StoryEventElement element)
    {
        if (element.m_eventData is StoryEventDataPointsFx)
        {
            StoryEventDataPointsFx data = element.m_eventData as StoryEventDataPointsFx;

            string name = EditorGUILayout.TextField("特效名：", data.fxName);
            if (name != data.fxName)
            {
                data.fxName = name;
                element.SaveEventData();
            }

            string pointsId = EditorGUILayout.TextField("点集ID：", data.pointsId);
            if (pointsId!=data.pointsId)
            {
                data.pointsId = pointsId;
                element.SaveEventData();
            }

            float time = EditorGUILayout.FloatField("销毁时间：", data.destroyTime);
            if (time!=data.destroyTime)
            {
                data.destroyTime = time;
                element.SaveEventData();
            }
            
        }
    }

    #endregion

}

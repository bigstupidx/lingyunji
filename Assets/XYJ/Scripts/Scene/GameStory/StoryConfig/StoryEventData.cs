using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public interface IStoryEventData
    {
        /// <summary>
        /// 把结构转为json字符串
        /// </summary>
        /// <returns></returns>
        string ToJson();

        /// <summary>
        /// 转结构设置数据
        /// </summary>
        /// <param name="cxt"></param>
        void ParseJson(string cxt);
    }

    /// <summary>
    /// 基类：StoryEventData，默认数据内容为空
    /// </summary>
    public class StoryEventData : IStoryEventData
    {

        
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void ParseJson(string cxt)
        {
            
        }
    }

    public class StoryEventDataParam : IStoryEventData
    {
        public string m_param;// 默认参数

        /// <summary>
        /// 把结构转为json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// 转结构设置数据
        /// </summary>
        /// <param name="cxt"></param>
        public void ParseJson(string cxt)
        {
            if (string.IsNullOrEmpty(cxt))
                return;

            // 一个参数
            StoryEventDataParam data = JsonUtility.FromJson<StoryEventDataParam>(cxt);
            if (data != null)
            {
                m_param = data.m_param;
            }
        }
    }

    /// <summary>
    /// 工具集
    /// </summary>
    public class StoryEventDataUtility
    {

        public static IStoryEventData Create (StoryEventType type, string eventCxt="")
        {
            IStoryEventData obj = null;

            switch (type)
            {
                // ------------
                // 角色行为相关事件，都需要指定角色
                case StoryEventType.角色动作:
                    obj = new StoryEventDataObjectAnim();
                    break;
                case StoryEventType.角色冒泡:
                    obj = new StoryEventDataObjectBubble();
                    break;
                case StoryEventType.角色随机冒泡:
                case StoryEventType.角色特效:
                case StoryEventType.角色移动:
                    obj = new StoryEventDataObjectMove();
                    break;
                case StoryEventType.角色巡逻:
                    obj = new StoryEventDataParam();
                    break;

                // -----------
                // 刷新点相关事件
                case StoryEventType.角色创建:
                case StoryEventType.角色删除:
                    obj = new StoryEventDataParam();
                    break;

                // -----------
                // 整合的全局模块事件
                case StoryEventType.点集特效:
                    obj = new StoryEventDataPointsFx();
                    break;
                case StoryEventType.镜头动画:
                case StoryEventType.CG动画:
                case StoryEventType.对白事件:
                case StoryEventType.个性事件选项:
                    obj = new StoryEventDataParam();
                    break;
                default:
                    obj = new StoryEventData();
                    break;
            }

            if (!string.IsNullOrEmpty(eventCxt))
                obj.ParseJson(eventCxt);
            return obj;
        }

        public static string ToJson<T>(T obj) where T : StoryEventData
        {
            return JsonUtility.ToJson(obj);
        }

        public static T FromJson<T>(string cxt) where T : StoryEventData
        {
            return JsonUtility.FromJson<T>(cxt);
        }

    }

}

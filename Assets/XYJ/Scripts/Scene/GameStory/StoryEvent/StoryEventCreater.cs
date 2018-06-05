using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{

    /// <summary>
    /// 剧情事件类型，顺序不能变，不然配置表的类型数据不对
    /// </summary>
    public enum StoryEventType
    {
        //NULL = -1,
        空 = 0,// 空事件，可以用来控制剧情的结束时间

        // ------------
        // 角色行为相关事件，都需要指定角色
        
        角色动作 = 100,
        //角色技能,// 暂时不实现
        角色冒泡,
        角色随机冒泡,
        角色特效,
        角色移动,
        角色巡逻,
        角色转向,
        //角色姿态切换,
        //角色头顶血条,
        //角色任务标记,
        //角色显示,
        //角色隐藏,

        // -----------
        // 刷新点相关事件
        角色创建 = 200,
        角色删除,

        // 区域相关事件
        //激活区域,
        //关闭区域,

        // -----------
        // 其它表现类型事件
        逻辑停止 = 300,
        逻辑继续,
        屏蔽玩家操作,
        屏蔽角色,// 按角色类型隐藏角色


        // -----------
        // 整合的全局模块事件
        点集特效 = 400,
        CG动画,// CG动画
        镜头动画,
        对白事件,//剧情对话
        个性事件选项,
    }

    public class StoryEventCreater
    {
        public static StoryEventBase Gen(StoryEventElement element, StoryPlayer player)
        {
            StoryEventType type = element.type;
            StoryEventBase eventData = null;
            switch (type)
            {
                // ------------
                // 角色行为相关事件，都需要指定角色
                case StoryEventType.角色动作:
                    eventData = new StoryEventObjectAnim();
                    break;
                case StoryEventType.角色冒泡:
                    eventData = new StoryEventObjectBubble();
                    break;
                case StoryEventType.角色随机冒泡:
                    eventData = new StoryEventObjectRandomBubble();
                    break;
                case StoryEventType.角色移动:
                    eventData = new StoryEventObjectMove();
                    break;
                case StoryEventType.角色特效:
                    eventData = new StoryEventObjectFx();
                    break;
                case StoryEventType.角色转向:
                    eventData = new StoryEventObjectRotate();
                    break;

                // -----------
                // 刷新点相关事件
                case StoryEventType.角色创建:
                    eventData = new StoryEventCreateObjects();
                    break;
                case StoryEventType.角色删除:
                    eventData = new StoryEventDeleteObjects();
                    break;

                // -----------
                // 整合的全局模块事件
                case StoryEventType.点集特效:
                    eventData = new StoryEventPlayFx();
                    break;
                case StoryEventType.CG动画:
                    eventData = new StoryEventPlayCG();
                    break;
                case StoryEventType.镜头动画:
                    eventData = new StoryEventPlayCamAnim();
                    break;
                case StoryEventType.对白事件:
                    eventData = new StoryEventPlayDialogue();
                    break;
                case StoryEventType.个性事件选项:
                    eventData = new StoryEventPlayPersonality();
                    break;
                case StoryEventType.空:
                default:
                    eventData = new StoryEventEmpty();
                    break;
            }
            if (eventData == null)
            {
                Debuger.LogError("程序没有实现剧情事件类型：" + type);
                return null;
            }
            eventData.Init(element, player);
            return eventData;
        }
    }
}

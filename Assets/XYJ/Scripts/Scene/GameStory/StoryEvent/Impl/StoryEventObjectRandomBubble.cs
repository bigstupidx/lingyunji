using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryEventDataObjectRandomBubble : IStoryEventData
    {
        public string m_refreshId;
        public string m_bubbles;
        public float m_playTime = 3;
        public float m_intervalTime = 2;// 间隔时间

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void ParseJson(string cxt)
        {
            if (string.IsNullOrEmpty(cxt))
                return;

            // 一个参数
            StoryEventDataObjectRandomBubble data = JsonUtility.FromJson<StoryEventDataObjectRandomBubble>(cxt);
            if (data != null)
            {
                m_refreshId = data.m_refreshId;
                m_bubbles = data.m_bubbles;
                m_playTime = data.m_playTime;
                m_intervalTime = data.m_intervalTime;
            }
        }
    }

    public class StoryEventObjectRandomBubble : StoryEventBase
    {

        /// <summary>
        /// 执行事件
        /// </summary>
        public override void OnFire()
        {
            Debug.Log("StoryEvent.Fire : " + type);
            StoryEventDataObjectRandomBubble config = eventData as StoryEventDataObjectRandomBubble;
            if (config == null)
                return;

            List<StoryObjectBase> objects = GetObjects(config.m_refreshId);
            if (objects == null || objects.Count == 0)
                return;
            string[] texts = config.m_bubbles.Split('|');
            if (texts == null || texts.Length == 0)
                return;

            for (int i = 0; i < objects.Count; ++i)
            {
                objects[i].ComHandler.PlayBehaviour(StoryEventType.角色随机冒泡, new object[] { texts, config.m_playTime, config.m_intervalTime });
            }
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        public override void OnExit()
        {
            Debug.Log("StoryEvent.Exit : " + type);
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="timePass"></param>
        public override void Update(float timePass)
        {
            //Debug.Log("StoryEvent.Update : " + type);
        }

        /// <summary>
        /// 暂停事件
        /// </summary>
        public override void OnPause()
        {
            Debug.Log("StoryEvent.OnPause : " + type);
        }

        /// <summary>
        /// 恢复事件
        /// </summary>
        public override void OnResume()
        {
            Debug.Log("StoryEvent.OnResume : " + type);
        }

        /// <summary>
        /// 停止事件
        /// </summary>
        public override void OnStop()
        {
            Debug.Log("StoryEvent.OnStop : " + type);
        }
    }
}

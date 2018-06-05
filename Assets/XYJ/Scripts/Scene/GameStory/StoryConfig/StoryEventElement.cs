using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    [System.Serializable]
    public class StoryEventElement
    {
        [SerializeField]
        int m_ID = 0;// 内部用

        bool m_toggle = true;// 编辑器用
        [System.NonSerialized]
        public IStoryEventData m_eventData;// 编辑器用
        Dictionary<StoryEventType, IStoryEventData> m_dataCache;// 编辑器用

        #region 数据部分

        // 事件开始和结束时间
        [SerializeField]
        float m_StartTime;
        [SerializeField]
        float m_EndTime;
        public float startTime
        {
            get { return m_StartTime; }
            set
            {
                m_StartTime = value;
                if (m_StartTime < 0.0f)
                    m_StartTime = 0.0f;
                if (m_StartTime > m_EndTime)
                    m_EndTime = m_StartTime;
            }
        }
        public float endTime
        {
            get { return m_EndTime; }
            set
            {
                m_EndTime = value;
                if (m_EndTime < 0.0f)
                    m_EndTime = 0.0f;
                if (m_EndTime < m_StartTime)
                    m_StartTime = m_EndTime;
            }
        }

        // 事件类型
        public StoryEventType type = StoryEventType.空;

        public string describe;// 描述用的信息
        public string eventCxt;// 事件类型的内容，json结构，可以序列化对应事件类型的对象

        #endregion

        public int id
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public bool toggle
        {
            get { return m_toggle; }
            set { m_toggle = value; }
        }

        public StoryEventElement() {
            Init();
        }

        public StoryEventElement (StoryEventElement source)
        {
            this.m_ID = source.id;
            this.m_toggle = source.toggle;

            this.startTime = source.startTime;
            this.endTime = source.endTime;

            this.describe = source.describe;
            this.type = source.type;
            this.eventCxt = source.eventCxt;

            Init();
        }

        void Init()
        {
            m_eventData = StoryEventDataUtility.Create(this.type, this.eventCxt);

            m_dataCache = new Dictionary<StoryEventType, IStoryEventData>();
            m_dataCache.Add(this.type, m_eventData);
        }

        // 把编辑的数据保存为字符串
        public void SaveEventData()
        {
            eventCxt = m_eventData.ToJson();
        }

        /// <summary>
        /// 获取事件数据对象
        /// </summary>
        /// <returns></returns>
        public IStoryEventData GetEventData()
        {
            if (m_dataCache.ContainsKey(this.type))
            {
                m_eventData = m_dataCache[this.type];
                m_eventData.ParseJson(eventCxt);
            }
            else
            {
                m_eventData = StoryEventDataUtility.Create(this.type, eventCxt);
                m_dataCache.Add(this.type, m_eventData);
            }

            return m_eventData;
        }

        /// <summary>
        /// 设置 event type 并把eventCxt初始化eventData结构
        /// </summary>
        /// <param name="eventType"></param>
        public void SetType(StoryEventType eventType)
        {
            if (type == eventType)
                return;

            this.type = eventType;
            GetEventData();
            
            SaveEventData();
        }

    }
}

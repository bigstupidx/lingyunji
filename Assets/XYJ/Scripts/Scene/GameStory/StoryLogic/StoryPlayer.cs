using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryPlayer
    {
        // 获取当前的StoryPlayer
        public static StoryPlayer Current
        {
            get; private set;
        }

        /// <summary>
        /// 用来判断该StoryPlayer是否在执行
        /// </summary>
        public bool IsRunning { get; private set; }

        // 剧情id
        private string m_storyId = string.Empty;
        public string storyID { get { return m_storyId; } }

        // 剧情配置信息
        private StoryConfig m_configData;
        public StoryConfig ConfigData { get { return m_configData; } }

        // 剧情事件时间
        private StoryTimelineEvent m_timelineEvent = new StoryTimelineEvent();
        public StoryTimelineEvent EventTimeLine { get { return m_timelineEvent; } }

        // 剧情对象管理
        private StoryObjectMgr m_objectMgr = new StoryObjectMgr();
        public StoryObjectMgr ObjectMgr { get { return m_objectMgr; } }


        public StoryPlayer(string storyId)
        {
            m_storyId = storyId;
            StoryConfig config = null;
            // 通过id获取配置信息
            if (StoryConfig.TryGet(storyId, out config))
            {
                Init(config);
            }
        }

        public StoryPlayer(StoryConfig config)
        {
            m_storyId = config.storyID;
            Init(config);
        }

        void Init(StoryConfig config)
        {
            Current = this;
            m_configData = config;
            IsRunning = false;

            // 初始化对象管理
            m_objectMgr.Init(this);
            // 初始化事件
            EventTimeLine.Init(this);
        }

        void Exit()
        {
            Debug.LogWarning("StoryPlayer.Exit : " + m_storyId);
            IsRunning = false;
            ObjectMgr.OnStoryEnd();
        }

        public void Play()
        {
            IsRunning = true;
            ObjectMgr.OnStoryStart();
            EventTimeLine.StartTimeline();
            Debug.LogWarning("StoryPlayer.Play : " + m_storyId);
        }

        public void Stop()
        {
            Debug.LogWarning("StoryPlayer.Stop : " + m_storyId);
            EventTimeLine.StopTimeline();
            Exit();
        }

        public void Update()
        {
            if (IsRunning)
            {
                ObjectMgr.OnStoryUpdate();
                EventTimeLine.Process();
                if (EventTimeLine.IsStop)
                {
                    Exit();
                }
            }
        }

    }

}

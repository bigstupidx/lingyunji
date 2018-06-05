using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;

namespace xys.GameStory
{

    public class StoryTimelineEvent : StoryTimeline
    {

        // 已执行时间
        private float m_elapsedTime = 0.0f;

        // 事件执行列表
        private List<StoryEventBase> m_eventList = new List<StoryEventBase>();

        void ResetData()
        {
            m_elapsedTime = 0.0f;
            m_state = TimelineState.Stop;
            m_eventList.Clear();
        }

        /// <summary>
        /// 初始化事件配置信息
        /// </summary>
        /// <param name="configList"></param>
        public void Init(StoryPlayer player)
        {
            ResetData();

            this.Player = player;
        }

        public override void StartTimeline()
        {
            m_state = TimelineState.Playing;

            List<StoryEventElement> eventConfigs = Player.ConfigData.eventList;
            for (int i = 0; i < eventConfigs.Count; ++i)
            {
                StoryEventElement element = eventConfigs[i];
                StoryEventBase storyEvent = StoryEventCreater.Gen(element, Player);
                if (storyEvent != null)
                    m_eventList.Add(storyEvent);
            }
        }

        public override void StopTimeline()
        {
            if (IsStop)
                return;

            for (int i = m_eventList.Count - 1; i >= 0; --i)
            {
                StoryEventBase baseEvent = m_eventList[i];
                if (baseEvent.HasFire)
                    baseEvent.OnStop();
            }
            ResetData();
        }

        public override void PauseTimeline()
        {
            if (!IsPlaying)
                return;
            m_state = TimelineState.Pause;
            for (int i = m_eventList.Count - 1; i >= 0; --i)
            {
                StoryEventBase baseEvent = m_eventList[i];
                if (baseEvent.HasFire)
                    baseEvent.OnPause();
            }
        }

        public override void ResumeTimeline()
        {
            if (!IsPause)
                return;
            m_state = TimelineState.Playing;
            for (int i = m_eventList.Count - 1; i >= 0; --i)
            {
                StoryEventBase baseEvent = m_eventList[i];
                if (baseEvent.HasFire)
                    baseEvent.OnResume();
            }
        }

        public override void Process()
        {
            if (!IsPlaying)
                return;

            if (IsPause)
                return;

            m_elapsedTime += Time.deltaTime;
            
            for (int i=m_eventList.Count-1; i>=0; --i)
            {
                StoryEventBase baseEvent = m_eventList[i];

                if (!baseEvent.HasFire && m_elapsedTime >= baseEvent.FireTime)
                {
                    baseEvent.Fire();
                }
                else if (baseEvent.HasFire && m_elapsedTime > baseEvent.FireTime && m_elapsedTime<=baseEvent.EndTime)
                {
                    baseEvent.Update(m_elapsedTime);
                }
                else if (baseEvent.HasFire && m_elapsedTime >= baseEvent.EndTime)
                {
                    baseEvent.OnExit();
                    m_eventList.RemoveAt(i);
                }

                if (IsPause)
                    break;
            }

            if (m_eventList.Count==0)
            {
                ResetData();
            }
        }
    }
}

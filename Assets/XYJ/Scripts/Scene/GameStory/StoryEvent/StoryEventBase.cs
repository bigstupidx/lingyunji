using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    /// <summary>
    /// 剧情事件
    /// </summary>
    public abstract class StoryEventBase
    {
        StoryPlayer _storyPlayer;


        protected StoryTimelineEvent EventTimeLine
        {
            get { return _storyPlayer.EventTimeLine; }
        }

        protected StoryConfig ConfigData
        {
            get { return _storyPlayer.ConfigData; }
        }

        protected StoryObjectMgr ObjectMgr
        {
            get { return _storyPlayer.ObjectMgr; }
        }

        protected string storyID
        {
            get { return _storyPlayer.storyID; }
        }

        StoryEventType m_Type = StoryEventType.空;
        object m_Data;

        /// <summary>
        /// 事件类型
        /// </summary>
        protected StoryEventType type
        {
            get { return m_Type; }
            private set { m_Type = value; }
        }

        /// <summary>
        /// 事件数据
        /// </summary>
        protected object eventData
        {
            get { return m_Data; }
            private set { m_Data = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public float FireTime
        {
            get; protected set;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public float EndTime
        {
            get; protected set;
        }

        public bool HasFire { get; private set; }

        public void Init(StoryEventElement element, StoryPlayer player)
        {
            this._storyPlayer = player;

            m_Type = element.type;

            FireTime = element.startTime;
            EndTime = element.endTime;
            HasFire = false;

            m_Data = element.GetEventData();
        }

        protected void Pause()
        {
            EventTimeLine.PauseTimeline();
            Debug.LogWarning("StoryPlayer.Pause : " + storyID);
        }

        protected void Resume()
        {
            EventTimeLine.ResumeTimeline();
            Debug.LogWarning("StoryPlayer.Resume : " + storyID);
        }

        protected List<StoryObjectBase> GetObjects(string refreshId)
        {
            return ObjectMgr.GetRefreshObjects(refreshId);
        }

        protected Points GetPoints(string id)
        {
            return ConfigData.GetPoints(id);
        }

        protected WayPoints GetWayPoints(string id)
        {
            return ConfigData.GetWayPoints(id);
        }

        protected CamPoints GetCamPoints(string id)
        {
            return ConfigData.GetCamPoints(id);
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        public void Fire()
        {
            HasFire = true;
            OnFire();
        }

        public abstract void OnFire();

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="timePass"></param>
        public virtual void Update(float timePass) { }

        /// <summary>
        /// 退出事件
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// 暂停事件
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        /// 恢复事件
        /// </summary>
        public virtual void OnResume() { }

        /// <summary>
        /// 停止事件
        /// </summary>
        public virtual void OnStop() { }

    }
}

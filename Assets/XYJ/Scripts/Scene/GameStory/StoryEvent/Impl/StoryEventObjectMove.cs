using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryEventDataObjectMove : IStoryEventData
    {
        public string m_refreshId;
        public string m_pointsId;
        public float m_moveSpeed = 5f;
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void ParseJson(string cxt)
        {
            if (string.IsNullOrEmpty(cxt))
                return;

            // 一个参数
            StoryEventDataObjectMove data = JsonUtility.FromJson<StoryEventDataObjectMove>(cxt);
            if (data != null)
            {
                m_refreshId = data.m_refreshId;
                m_pointsId = data.m_pointsId;
                m_moveSpeed = data.m_moveSpeed;
            }

        }
    }

    public class StoryEventObjectMove : StoryEventBase
    {

        public override void OnFire()
        {
            Debug.Log("StoryEvent.Fire : " + type);
            StoryEventDataObjectMove config = eventData as StoryEventDataObjectMove;
            if (config == null)
                return;
            
            Points ps = GetPoints(config.m_pointsId);
            if (ps == null || ps.Count == 0)
                return;

            List<StoryObjectBase> objects = GetObjects(config.m_refreshId);
            if (objects == null || objects.Count == 0)
                return;

            for (int i = 0; i < objects.Count; ++i)
            {
                objects[i].ComHandler.m_state.ChangeState(StoryObjectState.Move, new object[]{ps.positions[0], config.m_moveSpeed});
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

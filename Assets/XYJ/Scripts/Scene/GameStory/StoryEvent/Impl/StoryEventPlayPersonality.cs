using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryEventPlayPersonality : StoryEventBase
    {

        /// <summary>
        /// 执行事件
        /// </summary>
        public override void OnFire()
        {
            Debug.Log("StoryEvent.Fire : " + type);
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

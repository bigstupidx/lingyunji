#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto.Hot;
    using Network;

    public partial class LevelMgr
    {
        public LevelData levelData { get; private set; }
        public C2ALevelRequest request { get; private set; }

        public void OnInit(Event.HotObjectEventSet localEvent)
        {
            request = new C2ALevelRequest(hotApp.my.gameRPC);

            RegistMsg();
            RegistEvent(localEvent);
        }
        
        void RegistMsg()
        {
            hotApp.my.handler.RegHot<LevelFinish>(NetProto.Protoid.A2C_LevelFinish, OnLevelFinish);
            hotApp.my.handler.RegHot<LevelTriggerEvent>(NetProto.Protoid.A2C_LevelTriggerEvent, OnLevelTriggerEvent);
            hotApp.my.handler.RegHot<LevelEventNotice>(NetProto.Protoid.A2C_LevelEventNotice, OnLevelEventNotice);
        }

        /// <summary>
        /// 关卡结束
        /// </summary>
        void OnLevelFinish(LevelFinish data)
        {
            //request.LevelExit(null, null);
            LevelFinish(data);
        }

        /// <summary>
        /// 关卡触发了事件
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="data"></param>
        void OnLevelTriggerEvent(LevelTriggerEvent data)
        {
            LevelTriggerEvent(data);
        }

        /// <summary>
        /// 关卡条件成立通知前端
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="data"></param>
        void OnLevelEventNotice(LevelEventNotice data)
        {
            if (data.noticeType == 1)
                Debuger.LogWarning(string.Format("事件 {0} 的条件 {1} 达成", data.eventId, data.index));
            else
                Debuger.LogWarning(string.Format("事件 {0} 的行为 {1} 触发", data.eventId, data.index));
            LevelEventNotice(data);
        }
    }
}
#endif
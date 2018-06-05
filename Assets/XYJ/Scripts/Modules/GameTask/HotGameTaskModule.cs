#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    using UnityEngine;
    using System.Collections.Generic;

    class HotGameTaskModule : HotModuleBase
    {

        public GameTaskMgr gameTaskMgr = new GameTaskMgr();

        public HotGameTaskModule(xys.GameTaskModule m)
            : base(m)
        {

        }

        #region HotModule Call Methods

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            if (output == null)
                return;

            if (this.gameTaskMgr.m_taskData == null)
                this.gameTaskMgr.m_taskData = new TaskData();
            this.gameTaskMgr.m_taskData.MergeFrom(output);
        }

        protected override void OnAwake()
        {
            gameTaskMgr.OnAwake();

        }

        #endregion

        public TaskData GetGameTaskData()
        {
            return gameTaskMgr.m_taskData;
        }

        public TaskDbRecord GetMainDbRecord()
        {
            return gameTaskMgr.m_taskData.main.curTask;
        }

        public TaskDbRecord GetLoopDbRecord(int loopId)
        {
            for (int i = 0; i < gameTaskMgr.m_taskData.loops.Count; ++i)
            {
                if (gameTaskMgr.m_taskData.loops[i].loopId == loopId)
                {
                    return gameTaskMgr.m_taskData.loops[i].curTask;
                }
            }
            return null;
        }

        public List<TaskDbRecord> GetCurTaskRecords()
        {
            List<TaskDbRecord> records = new List<TaskDbRecord>();
            records.Add(gameTaskMgr.m_taskData.main.curTask);

            for (int i = 0; i < gameTaskMgr.m_taskData.loops.Count; ++i)
            {
                TaskDbRecord record = gameTaskMgr.m_taskData.loops[i].curTask;
                if (record != null && record.taskId!=0)
                {
                    records.Add(record);
                }
            }

            return records;
        }

    }
}
#endif

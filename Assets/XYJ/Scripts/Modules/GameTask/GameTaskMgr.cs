#if !USE_HOT
namespace xys.hot
{
    using UnityEngine;
    using NetProto;

    class GameTaskMgr
    {
        public TaskData m_taskData;

        C2ATaskRequest request { get; set; }
        C2ATaskGMRequest gmRequest { get; set; }

        public GameTaskMgr()
        {
            m_taskData = null;
        }

        public void OnAwake()
        {
            request = new C2ATaskRequest(App.my.game.local);
            gmRequest = new C2ATaskGMRequest(App.my.game.local);

            // 注册推送监听
            hotApp.my.handler.Reg<NetProto.TaskStatusChangeRespon>(Protoid.A2C_TaskStatusChange, ReceiveTaskStatusChange);
            hotApp.my.handler.Reg<NetProto.TaskDbRecord>(Protoid.A2C_TaskRecordPush, ReceiveTaskDbRecord);
            hotApp.my.handler.Reg<NetProto.TaskLoopData>(Protoid.A2C_TaskLoopDataPush, ReceiveTaskLoopData);

            // 监听事件
            hotApp.my.eventSet.Subscribe(EventID.Task_ClearAllTasks, ClearAllTasksReq);
            hotApp.my.eventSet.Subscribe<int>(EventID.Task_AddTask, AddTaskReq);
            hotApp.my.eventSet.Subscribe<int>(EventID.Task_GiveupTask, GiveupTaskReq);
            hotApp.my.eventSet.Subscribe<int>(EventID.Task_AcceptedTask, AcceptedTaskReq);
            hotApp.my.eventSet.Subscribe<int>(EventID.Task_CompletedTask, CompletedTaskReq);
            hotApp.my.eventSet.Subscribe<int>(EventID.Task_SubmitTask, SubmitTaskReq);
        }

        #region 操作数据内部方法

        TaskDbRecord GetMainDbRecord()
        {
            return m_taskData.main.curTask;
        }

        TaskDbRecord GetLoopDbRecord(int loopId)
        {
            for (int i = 0; i < m_taskData.loops.Count; ++i)
            {
                if (m_taskData.loops[i].loopId == loopId)
                {
                    return m_taskData.loops[i].curTask;
                }
            }
            return null;
        }

        TaskLoopData GetLoopData(int loopId)
        {
            for (int i = 0; i < m_taskData.loops.Count; ++i)
            {
                if (m_taskData.loops[i].loopId == loopId)
                {
                    return m_taskData.loops[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 设置主线任务，直接替换
        /// </summary>
        /// <param name="dbRecord"></param>
        void SetMainTaskRecord(TaskDbRecord dbRecord)
        {
            m_taskData.main.curTask = dbRecord;
            // 设置其他数据
        }

        /// <summary>
        /// 设置换任务
        /// </summary>
        /// <param name="loopId"></param>
        /// <param name="dbRecord"></param>
        void SetLoopTaskRecord(int loopId, TaskDbRecord dbRecord)
        {
            TaskLoopData loopData = null;
            for (int i = 0; i < m_taskData.loops.Count; ++i)
            {
                loopData = m_taskData.loops[i];
                if (loopData != null && loopData.loopId == loopId)
                {
                    loopData.curTask = dbRecord;
                    return;
                }
            }

            // 如果没有找到该环类型，需要添加新类型
            loopData = new TaskLoopData();
            loopData.loopId = loopId;
            loopData.curLoopNum = 0;
            loopData.curRound = 1;
            loopData.curTask = dbRecord;
            m_taskData.loops.Add(loopData);
        }
        void SetLoopTaskRecord(TaskDbRecord dbRecord)
        {
            Config.TaskDefine define = Config.TaskDefine.Get(dbRecord.taskId);
            if (define!=null)
            {
                SetLoopTaskRecord(define.loopId, dbRecord);
            }
        }

        void SetTaskLoopData(TaskLoopData loopData)
        {
            for (int i = 0; i < m_taskData.loops.Count; ++i)
            {
                loopData = m_taskData.loops[i];
                if (m_taskData.loops[i] != null && m_taskData.loops[i].loopId == loopData.loopId)
                {
                    m_taskData.loops[i] = loopData;
                    return;
                }
            }

            // 如果没有找到该环类型，需要添加新类型
            m_taskData.loops.Add(loopData);
        }

        #endregion


        #region 接收推送信息

        void ReceiveTaskStatusChange(TaskStatusChangeRespon pushData)
        {
            // 主线任务
            if (pushData.taskType == TaskType.Main)
            {
                TaskDbRecord dbRecord = GetMainDbRecord();
                dbRecord.status = pushData.status;
            }
            else
            {
                
            }
        }

        void ReceiveTaskDbRecord(TaskDbRecord pushData)
        {
            if (pushData.type == TaskType.Main)
                SetMainTaskRecord(pushData);
            else
                SetLoopTaskRecord(pushData);
        }

        void ReceiveTaskLoopData(TaskLoopData pushData)
        {
            SetTaskLoopData(pushData);
        }

        #endregion

        #region 协议相关

        // 获取当前主线任务信息
        public void GetTaskMainRecordReq()
        {
            request.GetTaskMainReq(new None(), GetTaskMainRecordRespon);
        }
        void GetTaskMainRecordRespon(wProtobuf.RPC.Error error, TaskRecordRespon returnData)
        {
            if (returnData.code == ReturnCode.ReturnCode_OK)
                m_taskData.main.curTask = returnData.dbRecord;
        }

        // 获取环任务信息
        public void GetTaskLoopReq(int loopId)
        {
            NetProto.Int32 input = new Int32 (){value = loopId};
            request.GetTaskLoopReq(input, GetTaskLoopRespon);
        }
        void GetTaskLoopRespon(wProtobuf.RPC.Error error, TaskLoopRecordRespon returnData)
        {
            if (returnData.code == ReturnCode.ReturnCode_OK)
            {
                for (int i = 0; i < m_taskData.loops.Count; ++i)
                {
                    if (m_taskData.loops[i].loopId == returnData.loop.loopId)
                    {
                        m_taskData.loops[i] = returnData.loop;
                        return;
                    }
                }
                m_taskData.loops.Add(returnData.loop);
            }
        }

        // 请求接收任务
        public void AcceptedTaskReq(int taskId)
        {
            NetProto.Int32 input = new Int32 (){value = taskId};
            request.AcceptedTaskReq(input, AcceptedTaskRespon);
        }
        void AcceptedTaskRespon(wProtobuf.RPC.Error error, ReturnCodeData returnData)
        {
            
        }

        // 请求提交任务
        public void SubmitTaskReq(int taskId)
        {
            NetProto.Int32 input = new Int32() { value = taskId };
            request.SubmitTaskReq(input, SubmitTaskRespon);
        }
        void SubmitTaskRespon(wProtobuf.RPC.Error error, ReturnCodeData returnData)
        {
            GetTaskMainRecordReq();
        }

        // 请求放弃任务
        public void GiveupTaskReq(int taskId)
        {
            NetProto.Int32 input = new Int32() { value = taskId };
            request.GiveupTaskReq(input, GiveupTaskRespon);
        }
        void GiveupTaskRespon(wProtobuf.RPC.Error error, ReturnCodeData returnData)
        {
            GetTaskMainRecordReq();
        }

        #endregion

        #region GM指令相关

        public void ClearAllTasksReq()
        {
            gmRequest.ClearAllTasksReq(new None(), ClearAllTasksRespon);
        }
        void ClearAllTasksRespon(wProtobuf.RPC.Error error, ReturnCodeData returnData)
        {
            if (returnData.code == ReturnCode.ReturnCode_OK)
            {
                m_taskData.main.curTask = null;
                m_taskData.loops.Clear();
                GetTaskMainRecordReq();
            }
        }

        public void AddTaskReq(int taskId)
        {
            NetProto.Int32 input = new Int32() { value = taskId };
            gmRequest.AddTaskReq(input, AddTaskRespon);
        }
        void AddTaskRespon(wProtobuf.RPC.Error error, ReturnCodeData returnData)
        {
            GetTaskMainRecordReq();
        }

        public void CompletedTaskReq(int taskId)
        {
            NetProto.Int32 input = new Int32() { value = taskId };
            gmRequest.CompletedTaskReq(input, CompletedTaskRespon);
        }
        void CompletedTaskRespon(wProtobuf.RPC.Error error, ReturnCodeData returnData)
        {
            GetTaskMainRecordReq();
        }

        #endregion

    }
}
#endif

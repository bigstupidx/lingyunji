using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.gm
{
    public class GM_TaskCmdPage : GM_IPage
    {

        GM_CmdInputRecord addTask;
        GM_CmdInputRecord giveupTask;
        GM_CmdInputRecord acceptedTask;
        GM_CmdInputRecord completedTask;
        GM_CmdInputRecord submitTask;

        GM_CmdInputRecord taskDialogTest;

        public string GetTitle()
        {
            return "任务";
        }

        public void OnOpen()
        {
            addTask = new GM_CmdInputRecord("gmCmd_addTask", AddTask, "", "添加");
            giveupTask = new GM_CmdInputRecord("gmCmd_giveupTask", GiveupTask, "", "放弃");
            acceptedTask = new GM_CmdInputRecord("gmCmd_acceptedTask", AcceptedTask, "", "接收");
            completedTask = new GM_CmdInputRecord("gmCmd_completedTask", CompletedTask, "", "完成");
            submitTask = new GM_CmdInputRecord("gmCmd_submitTask", SubmitTask, "", "提交");

            taskDialogTest = new GM_CmdInputRecord("gmCmd_taskDialogTest", TaskDialogTest, "", "播放对白");
        }

        public void OnClose()
        {
            
        }

        public void OnGUI(Rect beginArea)
        {
            GUILayout.BeginArea(beginArea);

            if (!Application.isPlaying)
                return;
            if (GUILayout.Button("清空所有任务", GUILayout.Width(240)))
            {
                ClearAllTasks();
            }
            
            GUILayout.BeginVertical();
            addTask.OnGUI();
            giveupTask.OnGUI();
            acceptedTask.OnGUI();
            completedTask.OnGUI();
            submitTask.OnGUI();

            GUILayout.Label("===========================");

            taskDialogTest.OnGUI();

            GUILayout.EndVertical();

            GUILayout.EndArea();
        }

        void ClearAllTasks()
        {
            App.my.eventSet.fireEvent(EventID.Task_ClearAllTasks);
        }

        void AddTask(string cmd)
        {
            int id = 0;
            if (int.TryParse(cmd, out id))
            {
                App.my.eventSet.FireEvent<int>(EventID.Task_AddTask, id);
            }
        }

        void GiveupTask(string cmd)
        {
            int id = 0;
            if (int.TryParse(cmd, out id))
            {
                App.my.eventSet.FireEvent<int>(EventID.Task_GiveupTask, id);
            }
        }

        void AcceptedTask(string cmd)
        {
            int id = 0;
            if (int.TryParse(cmd, out id))
            {
                App.my.eventSet.FireEvent<int>(EventID.Task_AcceptedTask, id);
            }
        }

        void CompletedTask(string cmd)
        {
            int id = 0;
            if (int.TryParse(cmd, out id))
            {
                App.my.eventSet.FireEvent<int>(EventID.Task_CompletedTask, id);
            }
        }

        void SubmitTask(string cmd)
        {
            int id = 0;
            if (int.TryParse(cmd, out id))
            {
                App.my.eventSet.FireEvent<int>(EventID.Task_SubmitTask, id);
            }
        }

        void TaskDialogTest(string cmd)
        {
            int taskId = 0;
            if (int.TryParse(cmd, out taskId))
            {
                TaskDialogMgr.Instance.PlayDialogs(taskId);
            }
        }

    }
}
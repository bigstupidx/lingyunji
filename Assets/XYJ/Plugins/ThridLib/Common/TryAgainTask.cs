using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public abstract class TryAgainTask : ASyncOperation
    {
        public TryAgainTask(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }

        public abstract ASyncOperation CreateTask(); // 创建任务

        public abstract void UpdateTask(ASyncOperation task); // 任务的桢更新

        public abstract void TaskError(ASyncOperation task); // 失败了

        public bool isRun = true;

        // 当前任务
        public ASyncOperation Current { get; protected set; }

        // 开始任务
        public IEnumerator Begin()
        {
            isRun = true;
            while (true)
            {
                if (isRun == false)
                {
                    yield return 0;
                    continue;
                }

                progress = 0f;
                isRun = false;
                Current = CreateTask(); // 创建一个任务
                while (!Current.isDone) // 等待任务完成
                {
                    progress = Current.progress;
                    UpdateTask(Current);
                    yield return 0;
                }

                if (!string.IsNullOrEmpty(Current.error))
                {
                    TaskError(Current);
                }
                else
                {
                    isDone = true;
                    yield break;
                }
            }
        }
    }

    public class DTATask : TryAgainTask
    {
        public DTATask(string name, OnCreateTask task, Action<ASyncOperation> up, Action<DTATask> error)
            : base(name)
        {
            mCreateTask = task;
            mOnUpdateTask = up;
            mOnErrorTask = error;
        }

        public delegate ASyncOperation OnCreateTask();

        OnCreateTask mCreateTask = null;
        Action<ASyncOperation> mOnUpdateTask = null;
        Action<DTATask> mOnErrorTask = null;

        // 创建任务
        public override ASyncOperation CreateTask()
        {
            return mCreateTask();
        }

        // 任务的桢更新
        public override void UpdateTask(ASyncOperation task)
        {
            mOnUpdateTask(task);
        }

        // 失败了
        public override void TaskError(ASyncOperation task)
        {
            if (mOnErrorTask != null)
                mOnErrorTask(this);
            else
                isRun = true;
        }
    }
}
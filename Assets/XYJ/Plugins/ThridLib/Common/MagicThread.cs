using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace PackTool
{
    public class BackgroundTask : YieldInstruction
    {

    };

    public class ForegroundTask : YieldInstruction
    {

    };

    // 函数回调
    public delegate void MagicThreadUpdate();
    public delegate void MagicThreadParamUpdate(object p);

    /// <summary>
    /// Magic thread allow a coroutine to go into the background and return to foreground using yield statements.
    /// </summary>
    public class MagicThread : SingletonMonoBehaviour<MagicThread>
    {
        static IEnumerator TheadCallback(MagicThreadUpdate update)
        {
            yield return 0;
            update();
        }

        static IEnumerator ThreadParamUpdate(MagicThreadParamUpdate update, object p)
        {
            yield return 0;
            update(p);
        }

        // 后台线程
        public static void StartBackground(IEnumerator task)
        {
            lock (Instance.backgroundTasks)
            {
                Instance.backgroundTasks.Add(task);
            }
        }

        public static void StartBackground(MagicThreadUpdate update)
        {
            IEnumerator itor = TheadCallback(update);
            lock (Instance.backgroundTasks)
            {
                Instance.backgroundTasks.Add(itor);
            }
        }

        public static void StartBackground(MagicThreadParamUpdate update, object p)
        {
            IEnumerator itor = ThreadParamUpdate(update, p);
            lock (Instance.backgroundTasks)
            {
                Instance.backgroundTasks.Add(itor);
            }
        }

        // 主线程
        public static void StartForeground(IEnumerator task)
        {
            lock (Instance.foregroundTasks)
            {
                Instance.foregroundTasks.Add(task);
            }
        }

        public static void StartForeground(MagicThreadUpdate update)
        {
            IEnumerator itor = TheadCallback(update);
            lock (Instance.foregroundTasks)
            {
                Instance.foregroundTasks.Add(itor);
            }
        }

        public static void StartForeground(MagicThreadParamUpdate update, object p)
        {
            IEnumerator itor = ThreadParamUpdate(update, p);
            lock (Instance.foregroundTasks)
            {
                Instance.foregroundTasks.Add(itor);
            }
        }

        List<IEnumerator> foregroundTasks = new List<IEnumerator>();
        List<IEnumerator> backgroundTasks = new List<IEnumerator>();

        List<IEnumerator> newTasks = new List<IEnumerator>();
        void Update()
        {
            if (foregroundTasks.Count > 0)
            {
                lock (foregroundTasks)
                {
                    newTasks.AddRange(foregroundTasks);
                    foregroundTasks.Clear();
                }

                for (int i = 0; i < newTasks.Count; ++i)
                {
                    StartCoroutine(HandleCoroutine(newTasks[i]));
                    newTasks[i] = null;
                }

                newTasks.Clear();
            }

            if (backgroundTasks.Count > 0)
            {
                lock (backgroundTasks)
                {
                    newTasks.AddRange(backgroundTasks);
                    backgroundTasks.Clear();
                }

                for (int i = 0; i < newTasks.Count; ++i)
                {
                    ThreadPool.QueueUserWorkItem(GlobalHandleThread, newTasks[i]);
                    newTasks[i] = null;
                }

                newTasks.Clear();
            }
        }

        void GlobalHandleThread(object state)
        {
            HandleThread(state as IEnumerator);
        }

        IEnumerator HandleCoroutine(IEnumerator task)
        {
            while (task.MoveNext())
            {
                object t = task.Current;
                if ((t as BackgroundTask) == null)
                    yield return t;
                else
                {
                    lock (backgroundTasks)
                    {
                        backgroundTasks.Add(task);
                    }

                    yield break;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            isRunning = false;
        }

        protected override void OnApplicationQuit()
        {
            base.OnDestroy();
            isRunning = false;
        }

        bool isRunning = true;

        void HandleThread(IEnumerator task)
        {
            try
            {
                while (task.MoveNext())
                {
                    Thread.Sleep(10);
                    if (!isRunning)
                        return;

                    object t = task.Current;
                    if ((t as ForegroundTask) != null)
                    {
                        lock (foregroundTasks)
                        {
                            foregroundTasks.Add(task);
                        }
                        break;
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                XYJLogger.LogError("ThreadAbortException in MagicThread:{0} StackTrace:{1}", e.ToString(), e.StackTrace);
                return;
            }
            catch (System.Exception e)
            {
                XYJLogger.LogError("Exception in MagicThread: {0} StackTrace:{1}", e.ToString(), e.StackTrace);
                return;
            }
        }
    }
}

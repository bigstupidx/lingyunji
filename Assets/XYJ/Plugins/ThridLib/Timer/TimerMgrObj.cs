using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    [ExecuteInEditMode]
    public class TimerMgrObj
#if UNITY_EDITOR
        : SingletonMonoBehaviour<TimerMgrObj>
#else
        : Singleton<TimerMgrObj>
#endif
    {
        public static int frameCount { get; protected set; }

        TimerFrame mTimerFrameUpdate;
        TimerFrame mTimerFrameLateUpdate;

        // 注意Frame用对象池管理，在update返回为false之后，会被回收重新利用!!!!
        public TimerFrame.Frame AddUpdate(TimerFrame.UPDATE f, System.Object p)
        {
#if UNITY_EDITOR
            if (mTimerFrameUpdate == null && Application.isEditor)
            {
                Init();
            }
#endif
            return mTimerFrameUpdate.Add(f, p);
        }

        // 注意Frame用对象池管理，在update返回为false之后，会被回收重新利用!!!!
        public TimerFrame.Frame AddLateUpdate(TimerFrame.UPDATE f, System.Object p)
        {
#if UNITY_EDITOR
            if (mTimerFrameUpdate == null && Application.isEditor)
            {
                Init();
            }
#endif
            return mTimerFrameLateUpdate.Add(f, p);
        }

        // 添加一个桢更新回调
        public void addFrameUpdate(TimerFrame.UPDATE f, System.Object p)
        {
            AddUpdate(f, p);
        }

        // 添加一个桢更新回调
        public void addFrameLateUpdate(TimerFrame.UPDATE f, System.Object p)
        {
            AddLateUpdate(f, p);
        }

#if UNITY_EDITOR
        protected override void Init()
        {
            frameCount = 0;
            mTimerFrameUpdate = new TimerFrame();
            mTimerFrameLateUpdate = new TimerFrame();

            if (isRunning)
            {
                GlobalCoroutine.StartCoroutine(LateUpdateItor());
                GlobalCoroutine.StartCoroutine(UpdateItor());
            }
        }
#else
        public TimerMgrObj()
        {
            frameCount = 0;
            mTimerFrameUpdate = new TimerFrame();
            mTimerFrameLateUpdate = new TimerFrame();
        }
#endif

        public void Update()
        {
#if UNITY_EDITOR
            if (mTimerFrameUpdate == null && Application.isEditor)
            {
                Init();
            }
#endif
            ++frameCount;
            mTimerFrameUpdate.checkFrameUpdate();
        }

#if UNITY_EDITOR
        IEnumerator LateUpdateItor()
        {
            yield return 0;
            while (true)
            {
                if (this == null)
                    yield break;

                if (!isRunning)
                {
                    yield return 0;
                    continue;
                }

                LateUpdate();
                yield return 0;
            }
        }

        bool isRunning { get { return !Application.isPlaying; } }

        IEnumerator UpdateItor()
        {
            yield return 0;
            while (true)
            {
                if (this == null)
                    yield break;

                if (!isRunning)
                {
                    yield return 0;
                    continue;
                }

                Update();
                yield return 0;
            }
        }
#endif

        public void LateUpdate()
        {
#if UNITY_EDITOR
            if (mTimerFrameUpdate == null && Application.isEditor)
            {
                Init();
            }
#endif
            mTimerFrameLateUpdate.checkFrameUpdate();
        }

#if (!COM_DEBUG) && ASSET_DEBUG
        void OnGUI()
        {
            float w = Screen.width / 1280.0f;
            Rect r = new Rect(400 * w, 10 * w, 80 * w, 40 * w);
            float dw = 80 * w;
            if (GUI.Button(r, "生成消耗文件"))
            {
                TimeTrackMgr.Instance.Save();
            }

            r.x += dw;
            if (GUI.Button(r, "清除数据"))
            {
                TimeTrackMgr.Instance.Clear();
            }

        }
#endif
    }
}
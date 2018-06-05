#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using System;
using PackTool;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    public class LoadingMgr : MonoBehaviour
    {
        [SerializeField]
        Image mProgress;

        [SerializeField]
        Text mText;

        private Action m_finishAction;
        private float fakeWaitTime;
        /// <summary>
        /// 是否假加载画面
        /// </summary>
        private bool isFakeLoading;

        public bool isShow;

        public float progress
        {
            get { return mProgress.fillAmount; }
            set
            {
                mProgress.fillAmount = value;
                mText.text = (((int)(value * 10000))*0.01f).ToString();
            }
        }

        GameObject m_Root;

        GameObject Root
        {
            get
            {
                if (m_Root == null)
                    m_Root = gameObject;

                return m_Root;
            }
        }

        public void Show()
        {
            isShow = true;
            Root.SetActive(true);
            isFakeLoading = false;
        }

        public void Hide()
        {
            isShow = false;
            isFakeLoading = false;
            if (m_Root == null)
                return;

            m_Root.SetActive(false);
        }

        void Awake()
        {
            if (m_Root == null)
            {
                Root.SetActive(false);
            }
        }

        void OnEnable()
        {
            progress = 0f;
        }

        /// <summary>
        /// 使用假加载画面
        /// </summary>
        /// <param name="waitTime">加载时间</param>
        /// <param name="finishAction">完成回调</param>
        public void StartFakeLoading(float waitTime, Action finishAction)
        {
            nowFakeWaitTime = fakeWaitTime = waitTime;
            m_finishAction = finishAction;
            progress = 0;
            Show();
            isFakeLoading = true;
        }

        private float nowFakeWaitTime;
        void Update()
        {
            if (isFakeLoading)
            {
                if (nowFakeWaitTime > 0)
                {
                    nowFakeWaitTime -= Time.unscaledDeltaTime;
                    progress = (fakeWaitTime - nowFakeWaitTime)/fakeWaitTime;
                }
                else
                {
                    if (m_finishAction != null)
                    {
                        m_finishAction();
                        m_finishAction = null;
                    }
                    Hide();
                }
            }
        }
        //void LateUpdate()
        //{
        //    SceneLoad sl = SceneLoad.Current;
        //    if (sl == null)
        //        return;

        //    progress = sl.progress;
        //}
    }
}

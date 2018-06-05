using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.UI
{
    public class ModelUIBubbling : MonoBehaviour
    {

        #region UI Objects Reference

        //public GameObject m_bubblingObject;// 冒泡对象，如果冒泡就激活
        public GameObject m_titleObject;

        public StateRoot m_thisState;// 0:无冒泡，1:默认冒泡，2:带任务标识冒泡
        public StateRoot m_taskIconState;// 任务图标状态--0:可接任务，1:已完成任务（可交），2:有在执行任务

        public Text m_nameText;
        public WXB.SymbolText m_bubblingText;

        public Text m_nameTextWithTask;
        public WXB.SymbolText m_bubblingTextWithTask;

        #endregion

        public bool HasBubble
        {
            get { return !string.IsNullOrEmpty(m_curBubblingStr); }
        }

        // 任务状态
        // 0:无任务，1:可接任务，2:已完成任务（可交），3:有在执行任务
        int TaskStatus
        {
            get;
            set;
        }

        bool m_shieldRandomBubbling = false;// 屏蔽随机冒泡

        float m_curBubblingTime = 0.0f;
        float m_lastBubblingTime = 0.0f;
        string m_curBubblingStr = string.Empty;

        float m_randomBubblingTime = 0.0f;
        float m_randomIntervalTime = 0.0f;
        float m_lastIntervalTime = 0.0f;
        List<string> m_randomBubblingList = new List<string>();

        /// <summary>
        /// 重置对象，恢复所有设置
        /// </summary>
        public void ResetObject()
        {
            m_curBubblingTime = 0.0f;
            m_lastBubblingTime = 0.0f;
            m_curBubblingStr = string.Empty;

            m_randomBubblingTime = 0.0f;
            m_randomIntervalTime = 0.0f;
            m_lastIntervalTime = 0.0f;
            m_randomBubblingList.Clear();

            m_bubblingText.text = string.Empty;
            m_bubblingTextWithTask.text = string.Empty;

            SetTaskStatusIcon(0);
            m_thisState.SetCurrentState(0, false);
        }

        /// <summary>
        /// 设置角色的冒泡名字，一般只设置一次
        /// </summary>
        /// <param name="name">角色名</param>
        /// <param name="type">颜色类型，1:黄色Y1，其他默认绿色G2,</param>
        public void SetNickName(string name, int type)
        {
            // 根据类型设置颜色
            if (type == 1)
            {
                m_nameText.text = string.Format("<color=#{0}>{1}</color>", Config.ColorConfig.GetIndexByName("Y1"), name);
            }
            else
            {
                m_nameText.text = string.Format("<color=#{0}>{1}</color>", Config.ColorConfig.GetIndexByName("G2"), name);
            }
            m_nameTextWithTask.text = m_nameText.text;
        }

        /// <summary>
        /// 设置显示任务状态
        /// </summary>
        /// <param name="status"></param>
        public void SetTaskStatusIcon(int status)
        {
            TaskStatus = status;
            if (HasBubble)
            {
                if (status == 0)
                {
                    m_thisState.SetCurrentState(1, false);
                    m_bubblingText.text = m_curBubblingStr;
                }
                else
                {
                    m_thisState.SetCurrentState(2, false);
                    m_taskIconState.SetCurrentState(status - 1, false);
                    m_bubblingTextWithTask.text = m_curBubblingStr;
                }
            }
            else
            {
                m_thisState.SetCurrentState(0, false);
            }
            
        }

        /// <summary>
        /// 随机冒泡
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="bubblingTime"></param>
        public void SetRandomBubbling(string[] contents, float bubblingTime, float intervalTime, bool immediate = true)
        {
            m_randomBubblingTime = 0.0f;
            m_randomIntervalTime = 0.0f;
            m_lastIntervalTime = 0.0f;
            m_randomBubblingList.Clear();

            if (contents == null || contents.Length == 0)
                return;

            for (int i = 0; i < contents.Length; ++i)
            {
                if (string.IsNullOrEmpty(contents[i]))
                    continue;

                m_randomBubblingList.Add(contents[i]);
            }

            if (m_randomBubblingList.Count > 0)
            {
                m_randomBubblingTime = bubblingTime;
                m_randomIntervalTime = intervalTime;

                if (immediate && !HasBubble)
                    RandomBubbling();
            }
        }

        void RandomBubbling()
        {
            if (m_randomBubblingList.Count > 0)
            {
                if (m_randomBubblingList.Count == 1)
                {
                    ShowBubbling(m_randomBubblingList[0], m_randomBubblingTime);
                }
                else
                {
                    int index = Random.Range(0, m_randomBubblingList.Count);
                    ShowBubbling(m_randomBubblingList[index], m_randomBubblingTime);
                }
            }
        }

        /// <summary>
        /// 冒泡
        /// </summary>
        /// <param name="content"></param>
        /// <param name="bubblingTime"></param>
        public void ShowBubbling(string content, float bubblingTime)
        {
            if (string.IsNullOrEmpty(content))
            {
                HideBubbling();
                return;
            }

            m_curBubblingStr = content;
            m_curBubblingTime = bubblingTime;
            m_lastBubblingTime = Time.timeSinceLevelLoad;

            SetTaskStatusIcon(TaskStatus);
            m_titleObject.SetActive(false);
        }

        public void ShowBubbling(string content, float bubblingTime, int taskSign)
        {
            if (string.IsNullOrEmpty(content))
            {
                HideBubbling();
                return;
            }

            m_curBubblingStr = content;
            m_curBubblingTime = bubblingTime;
            m_lastBubblingTime = Time.timeSinceLevelLoad;

            SetTaskStatusIcon(taskSign);
            m_titleObject.SetActive(false);
        }

        /// <summary>
        /// 关闭冒泡
        /// </summary>
        public void HideBubbling()
        {
            m_thisState.SetCurrentState(0, false);
            m_titleObject.SetActive(true);

            m_bubblingText.text = string.Empty;
            m_bubblingTextWithTask.text = string.Empty;

            m_curBubblingStr = string.Empty;
            m_curBubblingTime = 0.0f;
            m_lastBubblingTime = 0.0f;
            m_lastIntervalTime = 0.0f;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            // 处理冒泡逻辑
            if (HasBubble)
            {
                if (Time.timeSinceLevelLoad >= (m_lastBubblingTime + m_curBubblingTime))
                {
                    HideBubbling();

                    if (m_randomBubblingList.Count > 0)
                    {
                        // 等待随机下一个冒泡
                        m_lastIntervalTime = Time.timeSinceLevelLoad;
                    }
                }
            }
            else
            {
                if (m_randomBubblingList.Count > 0)
                {
                    // 等待随机下一个冒泡
                    if (Time.timeSinceLevelLoad >= (m_lastIntervalTime + m_randomIntervalTime))
                    {
                        RandomBubbling();
                    }
                }
            }
        }
    }

}

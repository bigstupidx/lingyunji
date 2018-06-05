#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using xys.UI;
using xys.UI.State;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Config;
using NetProto.Hot;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIActivityPushPanel
    {
        class UIActivityPushItem
        {
            #region 列表子对象
            public Transform m_ContentTras;

            private StateRoot m_Toggle;

            private ActivityData m_ActivityData;

            public void OnInit()
            {
                m_Toggle = m_ContentTras.Find("content/toggle").GetComponent<StateRoot>();

                Button remindButton = m_Toggle.GetComponent<Button>();
                if (remindButton != null)
                {
                    remindButton.onClick.RemoveAllListeners();
                    remindButton.onClick.AddListener(this.OnClickBtn);
                }
            }

            public void SetData(ActivityData data)
            {
                ActivityDefine activityConf = ActivityDefine.Get(data.activityId);
                m_ActivityData = data;

                m_ContentTras.Find("content/Activity").GetComponent<Text>().text = activityConf.name;
                m_ContentTras.Find("content/Weeks").GetComponent<Text>().text = activityConf.openDateDesc;
                m_ContentTras.Find("content/Time").GetComponent<Text>().text = ActivityMgr.GetTimeStr(data, activityConf);
                m_ContentTras.Find("content/Number").GetComponent<Text>().text = activityConf.pepoleLimit == 1 ? "单人" : "组队";

                m_Toggle.CurrentState = data.activityOpen;
            }

            private void OnClickBtn()
            {
                // 向后端发送开启协议
                ActivityData changeData = m_ActivityData;
                changeData.activityOpen = (m_ActivityData.activityOpen == 0 ? 1 : 0);

                hotApp.my.eventSet.FireEvent(xys.EventID.Activity_OpenPush, changeData);
            }
            #endregion
        }

        [SerializeField]
        private StateToggle m_StateToggle;
        [SerializeField]
        private StateRoot m_OnlineState;

        [SerializeField]
        private Text m_TipText;

        [SerializeField]
        private GameObject m_ItemObj;

        [SerializeField]
        private Button m_CloseBtn;
        [SerializeField]
        private Button m_CloseOtherBtn;

        [SerializeField]
        private Transform m_Transform;
        [SerializeField]
        private Transform m_Grid;

        [SerializeField]
        private Animator m_Animator;

        private List<ActivityData> m_ActivityList = new List<ActivityData>();

        void Awake()
        {
            m_StateToggle.OnSelectChange = this.SelectPushType;
            m_CloseBtn.onClick.AddListener(() =>
            {
                CloseEvent(null);
            });
            m_CloseOtherBtn.onClick.AddListener(() =>
            {
                CloseEvent(null);
            });
        }

        public void SetData(Dictionary<int, ActivityData> data)
        {
            RefreshData(data);

            this.m_Transform.gameObject.SetActive(true);
            PlayAnimation(true);
        }

        public void RefreshData(Dictionary<int, ActivityData> data)
        {
            GetAllLimitAcitivies(data);

            CreateActivityList();
        }

        private void SelectPushType(StateRoot state, int index)
        {
            m_OnlineState.CurrentState = (index == 0 ? 1 : 0);

            m_TipText.text = (index == 0 ? "开启提醒，将在活动开始时收到游戏中的活动推送" : "开启提醒，将在活动开始时收到线下通知");
        }

        private void GetAllLimitAcitivies(Dictionary<int, ActivityData> activityData)
        {
            m_ActivityList.Clear();

            foreach (ActivityData data in activityData.Values)
            {
                ActivityDefine activityConf = ActivityDefine.Get(data.activityId);
                TimesDefine timeConf = ActivityMgr.GetActivityCurrentTime(data);

                if (activityConf.activityType == 2 || ((activityConf.activityType == 1 || activityConf.activityType == 3) &&
                    !ActivityMgr.IsShowTimeStr(timeConf))) // 有时间段活动
                    m_ActivityList.Add(data);
            }
        }

        private void CreateActivityList()
        {
            int count = 0;
            int gridCount = m_Grid.childCount;
            for (int i = 0; i < m_ActivityList.Count; i++)
            {
                GameObject go = null;
                if (i < gridCount)
                    go = m_Grid.GetChild(i).gameObject;
                else
                {
                    go = GameObject.Instantiate(m_ItemObj);
                    go.transform.SetParent(m_Grid);
                    go.transform.localScale = Vector3.one;
                }
                go.SetActive(true);

                UIActivityPushItem activityItem = new UIActivityPushItem();
                activityItem.m_ContentTras = go.transform;
                activityItem.OnInit();
                activityItem.SetData(m_ActivityList[i]);
                count++;
            }

            for (int i = count; i < gridCount; i++)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void PlayAnimation(bool isOpen)
        {
            if (isOpen)
                AnimationHelp.PlayAnimation(m_Animator, "open", "ui_Tankuang_Activity", null);
            else
                AnimationHelp.PlayAnimation(m_Animator, "close", "ui_Tankuang_Activity_Close", this.CloseEvent);
        }

        private void CloseEvent(object obj)
        {
            this.m_Transform.gameObject.SetActive(false);
        }
    }
}
#endif
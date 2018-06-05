#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using NetProto.Hot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;


namespace xys.hot.UI
{
    [AutoILMono]
    class UIActivityItemScrollView
    {
        class UIActivityItem
        {
            #region 活动子对象
            private Text m_NameText;
            private Text m_NameLightText;
            private Text m_TimeText;
            private Text m_TimeLightText;

            public Transform m_Transform;

            private ActivityData m_ActivityData;


            public void OnInit()
            {
                m_NameText = m_Transform.Find("Name").GetComponent<Text>();
                m_NameLightText = m_Transform.Find("Name1").GetComponent<Text>();
                m_TimeText = m_Transform.Find("Time").GetComponent<Text>();
                m_TimeLightText = m_Transform.Find("Time1").GetComponent<Text>();

                UnityEngine.UI.Button selfButton = m_Transform.GetComponentInChildren<UnityEngine.UI.Button>();
                if (selfButton != null)
                {
                    selfButton.onClick.RemoveAllListeners();
                    selfButton.onClick.AddListener(this.OnClickBtn);
                }
            }

            private void OnClickBtn()
            {
                App.my.uiSystem.ShowPanel(PanelType.UIActivityIntroductionPanel, m_ActivityData);
            }

            public void SetData(ActivityData activityData, int curDaySate)
            {
                Config.ActivityDefine activityConf = Config.ActivityDefine.Get(activityData.activityId);
                m_ActivityData = activityData;

                m_Transform.GetComponent<StateRoot>().CurrentState = curDaySate;

                m_NameText.text = m_NameLightText.text = activityConf.name;

                m_TimeText.text = m_TimeLightText.text = ActivityMgr.GetTimeStr(activityData, activityConf, true);
            }
            #endregion
        }

        [SerializeField]
        private GameObject m_ActivityItem;

        [SerializeField]
        private Transform m_Grid;


        public void SetData(List<ActivityData> data, int curDaySate)
        {
            int count = 0;
            int gridCount = m_Grid.childCount;
            foreach (ActivityData itemData in data)
            {
                GameObject go = null;
                if (count < gridCount)
                    go = m_Grid.GetChild(count).gameObject;
                else
                {
                    go = GameObject.Instantiate(m_ActivityItem);
                    go.transform.SetParent(m_Grid);
                    go.transform.localScale = Vector3.one;
                }
                go.SetActive(true);

                UIActivityItem activityItem = new UIActivityItem();
                activityItem.m_Transform = go.transform;
                activityItem.OnInit();
                activityItem.SetData(itemData, curDaySate);

                count++;
            }

            for (int i = count; i < gridCount; i++)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
#endif
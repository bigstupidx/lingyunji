#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/12


using NetProto.Hot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;


namespace xys.hot.UI
{
    [System.Serializable]
    class UIActivityWeekScrollView
    {
        class UIActivityWeekItem
        {
            #region 周历子对象
            public Transform m_Transform;

            private StateRoot m_Tag;

            private Text m_TitleText;
            private Text m_LightText;

            private UIActivityItemScrollView m_Item;

            private readonly string[] weekTitles = new string[8] { "", "周一", "周二", "周三", "周四", "周五", "周六", "周日" };


            public void OnInit()
            {
                m_Tag = m_Transform.Find("Day").GetComponent<StateRoot>();
                m_TitleText = m_Transform.Find("Day/Text").GetComponent<Text>();
                m_LightText = m_Transform.Find("Day/Text1").GetComponent<Text>();

                m_Item = (UIActivityItemScrollView)m_Transform.Find("ScrollRect").GetComponent<ILMonoBehaviour>().GetObject();
            }

            // 设置title是否当天
            public void SetDayTitleTag(int state)
            {
                m_Tag.CurrentState = state;
            }

            public void SetTitleText(int today)
            {
                m_TitleText.text = m_LightText.text = weekTitles[today];
            }

            public void SetListData(List<ActivityData> data, int curDaySate)
            {
                m_Item.SetData(data, curDaySate);
            }
            #endregion
        }


        [SerializeField]
        private GameObject m_WeekItem;

        [SerializeField]
        private Transform m_Grid;


        public void SetData(Dictionary<int, List<ActivityData>> activityData, int today)
        {
            int count = 0;
            int gridCount = m_Grid.childCount;

            Dictionary<int, List<ActivityData>>.Enumerator itr = activityData.GetEnumerator();
            while (itr.MoveNext())
            {
                GameObject go = null;
                if (count < gridCount)
                    go = m_Grid.GetChild(count).gameObject;
                else
                {
                    go = GameObject.Instantiate(m_WeekItem);
                    go.transform.SetParent(m_Grid);
                    go.transform.localScale = Vector3.one;
                }
                go.SetActive(true);

                UIActivityWeekItem item = new UIActivityWeekItem();
                item.m_Transform = go.transform;
                item.OnInit();
                item.SetDayTitleTag(itr.Current.Key == today ? 1 : 0);
                item.SetTitleText(itr.Current.Key);
                item.SetListData(itr.Current.Value, itr.Current.Key == today ? 1 : 0);

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
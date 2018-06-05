#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace xys.hot.UI
{
    [AutoILMono]
    public class TeamPlatformHotInfoScrollView
    {
        [SerializeField]
        public GameObject m_itemPrefab;
        [SerializeField]
        public GameObject m_content;
        [SerializeField]
        private TeamPlatformHotInfoItem m_selectedItem;
        [SerializeField]
        GameObject m_emptyView;

        private List<TeamPlatformHotInfoItem> m_itemList = new List<TeamPlatformHotInfoItem>();
        private void Awake()
        {
            
        }

        public void AddItems(TeamQueryTeamsResult ret)
        {
            foreach (TeamAllTeamInfo info in ret.teamsInfo)
            {
                bool isIgnore = false;
                foreach (var existItem in m_itemList)
                {
                    if (info.teamId == existItem.Info.teamId)
                    {
                        isIgnore = true;
                        break;
                    }
                }
                if (isIgnore)
                    continue;

                GameObject obj = GameObject.Instantiate(m_itemPrefab);
                obj.transform.SetParent(m_content.transform, false);
                obj.SetActive(true);
                obj.transform.localScale = Vector3.one;

                ILMonoBehaviour ilMn = obj.GetComponent<ILMonoBehaviour>();
                TeamPlatformHotInfoItem item = ilMn.GetObject() as TeamPlatformHotInfoItem;
                item.Set(info, this.OnClickItem);
                m_itemList.Add(item);
            }

            m_emptyView.SetActive(m_itemList.Count == 0);
        }

        public void ClearItems()
        {
            m_selectedItem = null;

            for (int i = 0; i < m_itemList.Count; ++ i)
            {
                TeamPlatformHotInfoItem item = m_itemList[i];
                GameObject.Destroy(item.m_root.gameObject);
            }

            m_itemList.Clear();
        }

        public void OnClickItem(TeamPlatformHotInfoItem item)
        {
            if (null != m_selectedItem)
            {
                // 选中相关
            }

            m_selectedItem = item;
        }

        public int SelectedTeamId()
        {
            if (null != m_selectedItem)
                return m_selectedItem.GetTeamId();
            return 0;
        }
    }
}

#endif
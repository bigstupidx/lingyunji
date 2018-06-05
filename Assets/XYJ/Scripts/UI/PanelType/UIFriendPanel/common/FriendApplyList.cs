#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
using xys.UI;
using xys.hot.UI.Friend;
using xys.UI.State;
using NetProto;
using NetProto.Hot;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class FriendApplyList
    {
        [SerializeField]
        protected Transform m_ItemGrid;
        [SerializeField]
        public GameObject m_ItemPrefab;

        protected List<FriendListItem> m_FriendApplyListItems = new List<FriendListItem>();

        private List<FriendItemInfo> m_applyList = new List<FriendItemInfo>();

        private Dictionary<long, FriendItemInfo> m_friendApplyInfo = null;

        protected FriendListItem m_SelectItem = null;

        protected System.Action m_SelectCallBack = null;

        private FriendPage m_Parent = null;
        public FriendApplyList() {

           
        }

        public void SetParent(FriendPage parent)
        {
            if (m_Parent == null)
                m_Parent = parent;
        }

        public void SetEvent(Event.HotObjectEventAgent eventAgent)
        {
            eventAgent.Subscribe<Dictionary<long, FriendItemInfo>>(EventID.Friend_ApplyDataChange, this.ApplyDataChange);
            eventAgent.Subscribe<FriendItemInfo>(EventID.Friend_FriendItemInfoUpdata, this.RecvFriendItemInfo);
        }

        void OnEnable()
        {
            m_ItemGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        public IEnumerator Show(System.Action callback = null)
        {
            App.my.eventSet.fireEvent(EventID.Friend_GetApplyData);
            yield return 0;
        }

        public List<FriendItemInfo> getApplyList(Dictionary<long, FriendItemInfo> data)
        {
            m_friendApplyInfo = null;
            m_friendApplyInfo = data;
            m_applyList.Clear();
           foreach (KeyValuePair<long, FriendItemInfo> item in data)
           {
                 m_applyList.Add(item.Value);
           }
            return m_applyList;
        }

        public void GetReadPoint()
        {
            if (m_friendApplyInfo != null)
            {
                int redCnt = 0;
                foreach (var item in m_friendApplyInfo)
                {
                    if (item.Value.isRead == 0)
                    {
                       redCnt++;
                    }
                }
                m_Parent.ShowRedPoint(FriendPage.FrindPage.RequestAdd, redCnt);
            }

        }
        public int SortApply(FriendItemInfo info1, FriendItemInfo info2)
        {
            if (info1.applyTime > info2.applyTime)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        public void SetApplyList()
        {
            if (m_applyList.Count <= 0)
            {
                return;
            }
            m_applyList.Sort(SortApply);
            int idex = 0;
            foreach (var data in m_applyList)
            {
                if (idex > m_ItemGrid.childCount - 1)
                {
                    FriendListItem listItem = NewItem(m_ItemGrid);
                    if (listItem != null)
                    {
                        if (idex > m_FriendApplyListItems.Count - 1)
                        {
                            m_FriendApplyListItems.Add(listItem);
                        }
                    }
                }

                if (idex < m_FriendApplyListItems.Count)
                {
                    UpDataItem(data,idex);
                }
                idex++;
            }

        }

        public bool UpDataItem(FriendItemInfo info, int index)
        {
           if (index < m_FriendApplyListItems.Count && m_ItemGrid.GetChild(index).gameObject != null && m_FriendApplyListItems[index] != null)
           {
                m_ItemGrid.GetChild(index).gameObject.SetActive(true);
                m_FriendApplyListItems[index].m_Type = FriendListItem.FriendItemType.Apply;
                m_FriendApplyListItems[index].SetData(info);
               return true;
           }
            return true;
        }

        public void ResetApplyList(bool IsRemove = false)
        {
            if (m_ItemGrid != null)
            {
                if (IsRemove)
                {
                    for (int i = 0; i < m_ItemGrid.childCount; i++)
                    {
                        GameObject.DestroyObject(m_ItemGrid.GetChild(i).gameObject);
                    }
                    m_FriendApplyListItems.Clear();

                }
                else
                {
                    for (int i = 0; i < m_ItemGrid.childCount; i++)
                    {
                        m_ItemGrid.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }

        public void Hide()
        {
            
            m_SelectItem = null;
            ResetApplyList(true);


        }

        public FriendListItem NewItem(Transform Gid)
        {
            if (Gid == null )
            {
                return null;
            }

            GameObject obj = null;
            obj = GameObject.Instantiate(m_ItemPrefab);

            if (obj != null)
            {

                obj.SetActive(true);
                obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                ILMonoBehaviour ilm = obj.GetComponent<ILMonoBehaviour>();
                FriendListItem item = (FriendListItem)ilm.GetObject();
                if (item == null)
                    return null;

                obj.transform.SetParent(Gid, false);

                item.Set(this.OnSelectItem);

                return item;

            }

            return null;
        }

        public void ReSetSelect()
        {
            foreach (var item in m_FriendApplyListItems)
            {
                item.SetSelect(0);
            }

        }

        protected void OnSelectItem(FriendListItem item)
        {

            ReSetSelect();
            m_SelectItem = item;
            //item.SetSelect(1);

            if (m_SelectItem != null)
            {
                if (m_SelectCallBack != null && item != null)
                {
                    m_SelectCallBack();
                }
            }

        }

        void ApplyDataChange(Dictionary<long, FriendItemInfo> data)
        {
            getApplyList(data);
            GetReadPoint();           
            ResetApplyList();
            ReSetSelect();
            SetApplyList();
        }

        public int GetListItemIndex(List<FriendListItem> list, long charId)
        {
            if (charId <= 0 || list == null || list.Count <= 0)
            {
                return -1;

            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    FriendItemInfo info = list[i].GetItemData();
                    if (info != null && info.charid == charId)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
        void RecvFriendItemInfo(FriendItemInfo info)
        {
            if (m_applyList != null)
            {
                int index = GetListItemIndex(m_FriendApplyListItems, info.charid);
                if (index != -1 && index < m_FriendApplyListItems.Count)
                {
                    if (m_FriendApplyListItems[index] != null)
                    {
                        if (m_friendApplyInfo != null && m_friendApplyInfo.ContainsKey(info.charid))
                        {
                            m_friendApplyInfo[info.charid] = info;
                            m_FriendApplyListItems[index].SetData(info);
                            this.GetReadPoint();
                        }
                    }
                }
            }
        }
    }

}
#endif
#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
using xys.UI;
using xys.hot;
using NetProto;
using xys.UI.State;
using NetProto.Hot;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{

    namespace Friend
    {
        [AutoILMono]
        class FriendSearchList
        {
            [SerializeField]
            protected Transform m_ItemGrid;
            [SerializeField]
            public GameObject m_ItemPrefab;

            [SerializeField]
            Transform m_ParentState;

            protected List<FriendListItem> m_FriendSearchListItems = new List<FriendListItem>();

            private List<FriendItemInfo> m_searchList = new List<FriendItemInfo>();

            protected FriendListItem m_SelectItem = null;

            protected System.Action m_SelectCallBack = null;

            private int m_Type  = 0;     // 0 为推荐列表 1 为搜索列表

            public bool isShow = false;

            public FriendSearchList()
            {

            }

            public void SetEvent(Event.HotObjectEventAgent eventAgent)
            {
                eventAgent.Subscribe<FriendSearchInfo>(EventID.Friend_SearchDataChange, this.SearchFriendChange);
            }

            public void SetType(int Type)
            {
                m_Type = Type;

                ResetSearchList();
                SetSearchList();
            }


            public void SetSearchList()
            {
                if (m_searchList.Count <= 0)
                {
                    return;
                }

                int idex = 0;
                foreach (var data in m_searchList)
                {
                    if (idex > m_ItemGrid.childCount - 1)
                    {
                        FriendListItem listItem = NewItem(m_ItemGrid);
                        if (listItem != null)
                        {
                            if (idex > m_FriendSearchListItems.Count - 1)
                            {
                                m_FriendSearchListItems.Add(listItem);
                            }
                        }
                    }

                    if (idex < m_FriendSearchListItems.Count)
                    {
                        UpDataItem(data, idex);
                    }
                    idex++;
                }
            }


            public bool UpDataItem(FriendItemInfo info, int index)
            {
                if (index < m_FriendSearchListItems.Count && m_ItemGrid.GetChild(index).gameObject != null && m_FriendSearchListItems[index] != null)
                {
                    m_ItemGrid.GetChild(index).gameObject.SetActive(true);
                    m_FriendSearchListItems[index].m_Type = FriendListItem.FriendItemType.recommend;
                    m_FriendSearchListItems[index].SetData(info);
                    return true;
                }
                return true;
            }
            public List<FriendItemInfo> getSearchList(FriendSearchInfo data)
            {
                m_searchList.Clear();

             
                if (data != null)
                {
                    foreach (KeyValuePair<long, FriendItemInfo> item in data.searchMap)
                    {
                        m_searchList.Add(item.Value);
                    }
                }
                return m_searchList;
            }

            public void ResetSearchList(bool IsRemove = false)
            {
                if (IsRemove)
                {
                    for (int i = 0; i < m_ItemGrid.childCount; i++)
                    {
                        GameObject.DestroyObject(m_ItemGrid.GetChild(i).gameObject);
                    }

                    m_FriendSearchListItems.Clear();

                }
                else
                {
                    for (int i = 0; i < m_ItemGrid.childCount; i++)
                    {
                        m_ItemGrid.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }


            void OnEnable()
            {
                m_ItemGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            public IEnumerator Show(System.Action callback = null)
            {
                isShow = true;
                m_searchList.Clear();
                ResetSearchList();
                yield return 0;
            }

            public void Hide()
            {
                isShow = false;
                m_SelectItem = null;
                ResetSearchList(true);

            }

            public FriendListItem NewItem(Transform Gid)
            {
                if (Gid == null)
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
                foreach (var item in m_FriendSearchListItems)
                {
                    item.SetSelect(0);
                }

            }

            public void SearchFriendChange(FriendSearchInfo data)
            {
                getSearchList(data);
                if (isShow)
                {
                    if (m_searchList.Count == 0)
                    {
                        TipsContent tipsConfig = TipsContent.Get(2241);
                        if (tipsConfig != null)
                        {
                            SystemHintMgr.ShowHint(tipsConfig.des);
                        }
                        ReSetSelect();
                        ResetSearchList();
                        m_ParentState.GetComponent<StateRoot>().CurrentState = 1;
                        return;
                    }
                    else
                    {
                        m_ParentState.GetComponent<StateRoot>().CurrentState = 0;
                        ResetSearchList();
                        SetSearchList();
                    }
                   
                }
            }

            protected void OnSelectItem(FriendListItem item)
            {

                ReSetSelect();
                m_SelectItem = item;
                item.SetSelect(1);

                if (m_SelectItem != null)
                {
                    if (m_SelectCallBack != null && item != null)
                    {
                        m_SelectCallBack();
                    }
                }

            }

        }
    }

}
#endif
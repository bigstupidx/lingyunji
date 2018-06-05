#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Config;
using xys.UI;
using xys.hot.UI.Friend;
using UnityEngine.UI;
using xys.UI.State;
using NetProto.Hot;
using NetProto;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class FriendRecentlyList
    {
        public enum ListType
        {
            Chat = 0,
            Team = 1,
            None = 3,
        }


        [SerializeField]
        protected Transform m_ParentGrid;
        [SerializeField]
        protected Transform m_Gid1;
        [SerializeField]
        protected Transform m_Gid2;

        [SerializeField]
        protected Button m_ChatBtn;

        [SerializeField]
        protected Button m_TeamBtn;


        [SerializeField]
        public GameObject m_ItemPrefab;


        private FriendPage m_Parent = null;

        protected List<FriendListItem> m_ChatListItems = new List<FriendListItem>();

        protected List<FriendListItem> m_TeamListItems = new List<FriendListItem>();

        private Dictionary<long, FriendItemInfo> m_RecentlyChat = null;

        private Dictionary<long, FriendItemInfo> m_RecentlyTeam = null;

        protected FriendListItem m_SelectItem = null;


        protected System.Action m_SelectCallBack = null;

        FriendListItem m_SystemChat = null;

        public FriendRecentlyList()
        {
           
        }
        
        public void SetEvent(Event.HotObjectEventAgent eventAgent)
        {
            eventAgent.Subscribe(EventID.Friend_RecentlyChatUpdata, this.RecvRecentlyInfo);
            eventAgent.Subscribe<FriendItemInfo>(EventID.Friend_FriendItemInfoUpdata, this.RecvFriendItemInfo);
                     
        }

        public void RecvSelectLeader(long charId)
        {
            ReSetSelect();

            foreach (var item in m_ChatListItems)
            {
                if (item.GetItemData().charid == charId)
                {
                    item.SetSelect(1);
                }
            }
        }
        public void Start()
        {
            AddSystemChat();

            m_ChatBtn.onClick.RemoveAllListeners();
            m_TeamBtn.onClick.RemoveAllListeners();
            m_ChatBtn.onClick.AddListener(() => { this.OnClickChat(); });
            m_TeamBtn.onClick.AddListener(() => { this.OnClickTeam(); });
        }

        public void SetParent(FriendPage parent)
        {
            if (m_Parent == null)
                m_Parent = parent;
        }

        void OnClickChat(object args = null)
        {

            if (m_ChatBtn.GetComponent<StateRoot>().CurrentState == 0)
            {
                m_ChatBtn.GetComponent<StateRoot>().CurrentState = 1;
                //SetChatListItem();
            }
            else
            {
                m_ChatBtn.GetComponent<StateRoot>().CurrentState = 0;
                //ResetChatListItem();
            }

            RefreshUI();

        }

        void OnClickTeam(object args = null)
        {
          
            if (m_TeamBtn.GetComponent<StateRoot>().CurrentState == 0)
            {
                m_TeamBtn.GetComponent<StateRoot>().CurrentState = 1;           
                //SetTeamListItem();
            }
            else
            {
                m_TeamBtn.GetComponent<StateRoot>().CurrentState = 0;
                //ResetTeamListItem();
            }

            RefreshUI();
        }

        public int SortByChatTime(FriendItemInfo info1, FriendItemInfo info2)
        {
            if (info1.lastChatTime > info2.lastChatTime)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public int SortByTeamTime(FriendItemInfo info1, FriendItemInfo info2)
        {
            if (info1.lastTeamTime > info2.lastTeamTime)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        void OnEnable()
        {
            m_ParentGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        public IEnumerator Show(System.Action callback = null)
        {
            App.my.eventSet.fireEvent(EventID.Friend_GetRecentlyInfo);
            m_ChatBtn.GetComponent<StateRoot>().CurrentState = 1;
            m_TeamBtn.GetComponent<StateRoot>().CurrentState = 1;
            RefreshUI();
            ReSetSelect();
            yield return 0;
        }

        public void Hide()
        {
            m_ChatBtn.onClick.RemoveAllListeners();
            m_TeamBtn.onClick.RemoveAllListeners();
            m_SelectItem = null;
            ResetAllListItem(true);
        }
        public bool DestroyListItem(Transform Gid, int index)
        {
            if (index < 0 || Gid == null)
            {
                return false;
            }

            if (Gid.GetChild(index) == null)
            {
                return false;
            }
            else
            {
                GameObject.DestroyObject(Gid.GetChild(index).gameObject);
            }
            return true;
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

        public bool UpDataItem(FriendItemInfo info, ListType type, int index)
        {
            if (type == ListType.Chat)
            {
                if (index != 0)
                {
                    if (index < m_ChatListItems.Count+1 && m_Gid1.GetChild(index).gameObject != null && m_ChatListItems[index-1] != null)
                    {
                        m_Gid1.GetChild(index).gameObject.SetActive(true);
                        m_ChatListItems[index - 1].m_Type = FriendListItem.FriendItemType.Recently;
                        m_ChatListItems[index-1].SetData(info);                     
                    }
                    return true;
                }

                if (index == 0 && m_SystemChat != null)
                {
                    m_Gid1.GetChild(index).gameObject.SetActive(true);
                    m_SystemChat.m_Type = FriendListItem.FriendItemType.System;
                    m_SystemChat.SetData(info);                 
                }

            }
            else if (type == ListType.Team)
            {
                if (index < m_TeamListItems.Count && m_Gid2.GetChild(index).gameObject != null && m_TeamListItems[index] != null)
                {
                    m_Gid2.GetChild(index).gameObject.SetActive(true);
                    m_TeamListItems[index].m_Type = FriendListItem.FriendItemType.Team;
                    m_TeamListItems[index].SetData(info);                   
                    return true;
                }

            }

            return true;
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

            return null;// obj.transform;

        }

        void SetChatListItem()
        {
            if (m_RecentlyChat != null)
            {
                if (m_RecentlyChat != null && m_RecentlyChat.Count>0)
                {
                    List<FriendItemInfo> chatList = new List<FriendItemInfo>();
                    chatList.Clear();
                    foreach (var item in m_RecentlyChat)
                    {
                        chatList.Add(item.Value);
                    }

                    chatList.Sort(SortByChatTime);

                    int idex = 0;    
                    foreach (var data in chatList)
                    {
                        if (idex > m_Gid1.childCount-2)
                        {
                            FriendListItem listItem = NewItem(m_Gid1);
                            if (listItem != null)
                            {
                                if (idex > m_ChatListItems.Count-1)
                                {
                                    m_ChatListItems.Add(listItem);
                                }
                            }
                        }

                        if (idex < m_ChatListItems.Count)
                        {
                            UpDataItem(data, ListType.Chat, idex+1);
                        }
                        idex++;
                    }
                }

            }
            
        }

        public void AddSystemChat()
        {

            if (m_SystemChat == null)
            {
                m_SystemChat = NewItem(m_Gid1);
            }
            
            if (m_SystemChat != null)
            {
                FriendItemInfo systemInfo = new FriendItemInfo();
                systemInfo.charid = -1;
                systemInfo.name = "系统消息";
                systemInfo.level = -1;
                systemInfo.job = 0;
                systemInfo.lastChatTime = 0;
                systemInfo.lastTeamTime = 0;
                systemInfo.itemType = 0;
                systemInfo.job = 0;
                systemInfo.isOnline = true;
                systemInfo.sex = 0;

                UpDataItem(systemInfo, ListType.Chat, 0);             
            }
            
        }

        void ResetChatListItem(bool isRemove = false)
        {
            if (m_Gid1 != null)
            {
                if (isRemove)
                {
                    for (int i = 0; i < m_Gid1.childCount; i++)
                    {
                        GameObject.DestroyObject(m_Gid1.GetChild(i).gameObject);
                    }

                    m_ChatListItems.Clear();
                }
                else
                {
                    for (int i = 0; i < m_Gid1.childCount; i++)
                    {
                        m_Gid1.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }


        void SetTeamListItem()
        {
            if (m_RecentlyTeam != null && m_RecentlyTeam.Count >0)
            {
                List<FriendItemInfo> teamList = new List<FriendItemInfo>();
                teamList.Clear();
                foreach (var item in m_RecentlyTeam)
                {
                    teamList.Add(item.Value);
                }

                teamList.Sort(SortByTeamTime);

                int idex = 0;    
                foreach (var data in teamList)
                {
                    if (idex > m_Gid2.childCount - 1)
                    {
                        FriendListItem listItem = NewItem(m_Gid2);
                        if (listItem != null)
                        {
                            if (idex > m_TeamListItems.Count - 1)
                            {
                                m_TeamListItems.Add(listItem);
                            }
                        }
                    }

                    if (idex < m_TeamListItems.Count)
                    {
                        UpDataItem(data, ListType.Team, idex);
                    }
                    idex++;
                }
            }
            
        }

        void ResetTeamListItem(bool isRemove = false)
        {
            if (m_Gid2 != null)
            {
                if (isRemove)
                {
                    for (int i = 0; i < m_Gid2.childCount; i++)
                    {
                        GameObject.DestroyObject(m_Gid2.GetChild(i).gameObject);
                    }

                    m_TeamListItems.Clear();
                }
                else
                {
                    for (int i = 0; i < m_Gid2.childCount; i++)
                    {
                        m_Gid2.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }


        void ResetAllListItem(bool isRemove = false)
        {
            ResetChatListItem(isRemove);
            ResetTeamListItem(isRemove);
            //m_Parent.ChangeMessageRoot(0);
        }

        public void RefreshUI()
        {
            ResetAllListItem();
           
            if (m_ChatBtn.GetComponent<StateRoot>().CurrentState == 1)
            {
                AddSystemChat();
                if (m_SystemChat != null)
                {                   
                    m_Gid1.GetChild(0).gameObject.SetActive(true);
                }
                SetChatListItem();
            }

            if (m_TeamBtn.GetComponent<StateRoot>().CurrentState == 1)
            {
                SetTeamListItem();
            }


                FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
                if (friendModule != null && friendModule.friendMgr != null)
                {
                    //显示分组数量
                    if (m_ParentGrid.Find("TabBtn1/CiShuText") != null)
                    {
                        if (m_RecentlyChat != null)
                        {
                            int chatOnlineCount = ((FriendMgr)friendModule.friendMgr).GetFriendOnlineCount(m_RecentlyChat);
                            m_ParentGrid.Find("TabBtn1/CiShuText").GetComponent<Text>().text = string.Format("{0}/{1}", chatOnlineCount, m_RecentlyChat.Count);
                        }
                    }
                    if (m_ParentGrid.Find("TabBtn2/CiShuText") != null)
                    {
                        if (m_RecentlyTeam != null)
                        {
                            int teamOnlineCount = ((FriendMgr)friendModule.friendMgr).GetFriendOnlineCount(m_RecentlyTeam);
                            m_ParentGrid.Find("TabBtn2/CiShuText").GetComponent<Text>().text = string.Format("{0}/{1}", teamOnlineCount, m_RecentlyTeam.Count);
                        }
                    }
                }
        }


        public void ReSetSelect()
        {
            
            foreach (var item in m_ChatListItems)
            {
                item.SetSelect(0);
            }

            foreach (var item in m_TeamListItems)
            {
                item.SetSelect(0);
            }

            if (m_SystemChat != null)
            {
                m_SystemChat.SetSelect(0);
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

                if (m_Parent != null)
                {
                    FriendItemInfo info = item.GetItemData();
                    if (item.m_Type == FriendListItem.FriendItemType.System)
                    {
                        m_Parent.ChangeMessageRoot(1);
                        m_Parent.OnChatItemClick(info,1);
                        //info.itemType = 1;
                    }
                    else
                    {                    
                        m_Parent.ChangeMessageRoot(2);
                        m_Parent.OnChatItemClick(info, 2);
                        //info.itemType = 2;                     
                    }
                    
                }
            }
        }
        public void GetReadPoint()
        {
            int redCnt = 0;
            if (m_RecentlyChat != null)
            {
                    
               foreach (var item in m_RecentlyChat)
               {
                  if (item.Value.isRead == 0)
                  {
                            redCnt++;
                  }
               }
              
            }
            /*
            if (m_RecentlyTeam != null)
            {

                foreach (var item in m_RecentlyTeam)
                {
                    if (item.Value.isRead == 0)
                    {
                        redCnt++;
                    }
                }

            }
            */
            m_Parent.ShowRedPoint(FriendPage.FrindPage.Recently, redCnt);

        }
        public void RecvRecentlyInfo()
        {
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                m_RecentlyChat = ((FriendMgr)friendModule.friendMgr).m_RecentlyChat;
                m_RecentlyTeam = ((FriendMgr)friendModule.friendMgr).m_RecentlyTeam;              
            }
            GetReadPoint();
            RefreshUI();
        }
        /*
        public void RecvRecentlyChat(FriendsRecentContactInfos info)
        {
            ChatDataList.Clear();

            foreach (var friend in info.chats)
            {
                
                if (friend.Value != null)
                {
                    ChatDataList.Add(friend.Value);
                }
                
            }

        }

        public void RecvRecentlyTeam(FriendsRecentContactInfos info)
        {
            TeamDataList.Clear();

            foreach (var friend in info.teams)
            {
                
                if (friend.Value != null)
                {
                    TeamDataList.Add(friend.Value);
                }
                
            }

        }
        */
        public void RecvFriendItemInfo(FriendItemInfo info)
        {
            if (info != null)
            {
                
                    if (m_RecentlyChat != null)
                    {
                        int index = GetListItemIndex(m_ChatListItems, info.charid);
                        if (index != -1 && index < m_ChatListItems.Count)
                        {
                            if (m_ChatListItems[index] != null)
                            {
                                if (m_RecentlyChat.ContainsKey(info.charid))
                                {
                                    m_RecentlyChat[info.charid] = info;
                                    this.GetReadPoint();
                                }
                                m_ChatListItems[index].SetData(info);
                            }
                        }
                    }
                    if (m_RecentlyTeam != null)
                    {
                        int index = GetListItemIndex(m_TeamListItems, info.charid);
                        if (index != -1 && index < m_TeamListItems.Count)
                        {
                            if (m_TeamListItems[index] != null)
                            {
                            if (m_RecentlyTeam.ContainsKey(info.charid))
                            {
                                m_RecentlyTeam[info.charid] = info;
                            }
                            m_TeamListItems[index].SetData(info);
                        }
                    }
                 }               
            }
        }
    }
}
#endif
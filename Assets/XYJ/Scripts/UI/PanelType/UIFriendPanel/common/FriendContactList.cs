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
using NetProto;
using System.Text;
using NetProto.Hot;
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.hot.UI
{
    [AutoILMono]
    class FriendContactList
    {
        public enum ListType
        {
            Friend,
            Enemy,
            Black,
        }

        
        [SerializeField]
        protected Transform m_ParentGrid;
        [SerializeField]
        protected Transform m_Gid1;
        [SerializeField]
        protected Transform m_Gid2;
        [SerializeField]
        protected Transform m_Gid3;

        [SerializeField]
        protected Button m_friendBtn;

        [SerializeField]
        protected Button m_EnemyBtn;

        [SerializeField]
        protected Button m_BlackBtn;

        [SerializeField]
        public GameObject m_ItemPrefab;

        [SerializeField]
        StateToggle m_ParentToggle;

        protected List<FriendListItem> m_FriendListItems = new List<FriendListItem>();

        protected List<FriendListItem> m_EnemyListItems = new List<FriendListItem>();

        protected List<FriendListItem> m_BlackListItems = new List<FriendListItem> ();

        private Dictionary<long, FriendItemInfo> m_friendsMap = null;

        private Dictionary<long, FriendItemInfo> m_enemyMap = null;

        private Dictionary<long, FriendItemInfo> m_blackMap = null;

        protected FriendListItem m_SelectItem = null;


        protected System.Action m_SelectCallBack = null;


        private FriendPage m_Parent = null;

        public FriendContactList()
        {

        }

        public void Start()
        {
            m_friendBtn.onClick.RemoveAllListeners();
            m_EnemyBtn.onClick.RemoveAllListeners();
            m_BlackBtn.onClick.RemoveAllListeners();
            m_friendBtn.onClick.AddListener(() => { this.OnClickFriend(); });
            m_EnemyBtn.onClick.AddListener(() => { this.OnClickEnemy(); });
            m_BlackBtn.onClick.AddListener(() => { this.OnClickBlack(); });
        }

        public void SetEvent(Event.HotObjectEventAgent eventAgent)
        {
            eventAgent.Subscribe(EventID.Friend_PushFriendData, this.RecvFriendData);
            eventAgent.Subscribe<FriendItemInfo>(EventID.Friend_FriendItemInfoUpdata, this.RecvFriendItemInfo);
        }

        void OnClickFriend(object args = null)
        {
        
            if (m_friendBtn.GetComponent<StateRoot>().CurrentState == 0)
            {
                m_friendBtn.GetComponent<StateRoot>().CurrentState = 1;
                m_EnemyBtn.GetComponent<StateRoot>().CurrentState = 0;
                m_BlackBtn.GetComponent<StateRoot>().CurrentState = 0;
                //SetFriendListItem();
            }
            else
            {
                m_friendBtn.GetComponent<StateRoot>().CurrentState = 0;

                //ResetFriendListItem();
            }
            RefreshUI();
        }

       
        void OnClickEnemy(object args = null)
        {

            if (m_EnemyBtn.GetComponent<StateRoot>().CurrentState == 0)
            {
                m_friendBtn.GetComponent<StateRoot>().CurrentState = 0;
                m_EnemyBtn.GetComponent<StateRoot>().CurrentState = 1;
                m_BlackBtn.GetComponent<StateRoot>().CurrentState = 0;
                //SetEnemyListItem();
            }
            else
            {
                m_EnemyBtn.GetComponent<StateRoot>().CurrentState = 0;          
                //ResetEnemyListItem();
            }
            RefreshUI();

        }

        public void SetParent(FriendPage parent)
        {
            if (m_Parent == null)
                m_Parent = parent;
        }

        

        void OnClickBlack(object args = null)
        {
            if (m_BlackBtn.GetComponent<StateRoot>().CurrentState == 0)
            {
                m_friendBtn.GetComponent<StateRoot>().CurrentState = 0;
                m_EnemyBtn.GetComponent<StateRoot>().CurrentState = 0;
                m_BlackBtn.GetComponent<StateRoot>().CurrentState = 1;

                //SetBlackListItem();
            }
            else
            {
                m_BlackBtn.GetComponent<StateRoot>().CurrentState = 0;
            }

            RefreshUI();
        }

        void OnEnable()
        {
            m_ParentGrid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        }

        public IEnumerator Show(System.Action callback = null)
        {
            m_friendBtn.GetComponent<StateRoot>().CurrentState = 1;
            App.my.eventSet.fireEvent(EventID.Friend_GetFriendData);
            ReSetSelect();
            yield return 0;
        }

        public void Hide()
        {
            m_SelectItem = null;
            ResetAllListItem(true);
        }

        public bool DestroyListItem(Transform Gid)
        {
            if ( Gid == null )
            {
                return false;
            }
            else
            {
                int GidCount = Gid.childCount - 1;
                Gid.GetChild(GidCount).gameObject.SetActive(false);
            }          
            return true;
        }

        public int GetListItemIndex(List<FriendListItem> list ,long charId)
        {
            if (charId <= 0 || list == null || list.Count <= 0)
            {
                return -1;

            }

            for (int i = 0;i< list.Count;i++)
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

        public bool UpDataItem(FriendItemInfo info, ListType type,int index)
        {
            if (type == ListType.Friend)
            {
                if (index < m_FriendListItems.Count && m_Gid1.GetChild(index).gameObject != null && m_FriendListItems[index] != null)
                {
                    m_Gid1.GetChild(index).gameObject.SetActive(true);
                    m_FriendListItems[index].m_Type = FriendListItem.FriendItemType.Friend;
                    m_FriendListItems[index].SetData(info);               
                    return true;
                }

            }
            else if (type == ListType.Enemy)
            {
                if (index < m_EnemyListItems.Count && m_Gid2.GetChild(index).gameObject != null && m_EnemyListItems[index] != null)
                {
                    m_Gid2.GetChild(index).gameObject.SetActive(true);
                    m_EnemyListItems[index].m_Type = FriendListItem.FriendItemType.Enemy;
                    m_EnemyListItems[index].SetData(info);               
                    return true;
                }

            }
            else if (type == ListType.Black)
            {
                if (index < m_BlackListItems.Count && m_Gid3.GetChild(index).gameObject != null && m_BlackListItems[index] != null)
                {
                    m_Gid3.GetChild(index).gameObject.SetActive(true);
                    m_BlackListItems[index].m_Type = FriendListItem.FriendItemType.Black;
                    m_BlackListItems[index].SetData(info);                 
                    return true;
                }
            }

            return true;
        }

        public FriendListItem NewItem(Transform Gid,int index)
        {
            if (Gid == null || index < 0 )
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

        public bool IsNumber(char tempChar)
        {
            switch (tempChar)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
                default:
                    return false;
            }
        }

        public bool IsEnglish(char tempChar)
        {
            switch (tempChar)
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    return true;
                default:
                    return false;
            }
        }

        public bool IsSymbol(char tempChar)
        {
            switch (tempChar)
            {
                case '~':
                case '!':
                case '@':
                case '#':
                case '$':
                case '%':
                case '^':
                case '&':
                case '*':
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '<':
                case '>':
                case '_':
                case '+':
                case '−':
                case '=':
                case '.':
                case ',':
                case ':':
                case ';':
                case '?':
                case '-':
                case '–':
                case '/':
                case '|':
                case '"':
                    return true;
                default:
                    return false;
            }
        }

        public int SortEnemy(FriendItemInfo info1,FriendItemInfo info2)
        {
            if (info1.lastKillTime > info2.lastKillTime)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

    
        public void SortFriendList(List<FriendItemInfo> friendList)
        {
            List<FriendItemInfo> OnlineList = new List<FriendItemInfo>();
            OnlineList.Clear();

            List<FriendItemInfo> OutlineList = new List<FriendItemInfo>();
            OutlineList.Clear();

            List<FriendItemInfo> OnlineChatList = new List<FriendItemInfo>();
            OnlineChatList.Clear();

            List<FriendItemInfo> OnlineNotChatList = new List<FriendItemInfo>();
            OnlineNotChatList.Clear();

            List<FriendItemInfo> OutlineChatList = new List<FriendItemInfo>();
            OutlineChatList.Clear();

            List<FriendItemInfo> OutlineNotChatList = new List<FriendItemInfo>();
            OutlineNotChatList.Clear();

            foreach (var item in friendList)
            {
                if (item.isOnline)
                {
                    OnlineList.Add(item);
                }
                else
                {
                    OutlineList.Add(item);
                }
            }

            foreach (var item in OnlineList)
            {
                if (item.lastChatTime > 0)
                {
                    OnlineChatList.Add(item);
                }
                else
                {
                    OnlineNotChatList.Add(item);
                }
            }

            foreach (var item in OutlineList)
            {
                if (item.lastChatTime > 0)
                {
                    OutlineChatList.Add(item);
                }
                else
                {
                    OutlineNotChatList.Add(item);
                }
            }
            SortFriendListChat(OnlineChatList);
            SortFriendListChat(OnlineNotChatList);
            SortFriendListChat(OutlineChatList);
            SortFriendListChat(OutlineNotChatList);

            friendList.Clear();

            foreach (var item in OnlineChatList)
            {
                friendList.Add(item);
            }

            foreach (var item in OnlineNotChatList)
            {
                friendList.Add(item);
            }

            foreach (var item in OutlineChatList)
            {
                friendList.Add(item);
            }

            foreach (var item in OutlineNotChatList)
            {
                friendList.Add(item);
            }

        }

        public int SortByChinese(FriendItemInfo info1, FriendItemInfo info2)
        {
            string str1 = getSpell(info1.name[0].ToString());
            string str2 = getSpell(info2.name[0].ToString());
            return string.Compare(str1, str2);
        }

        public void SortFriendListChat(List<FriendItemInfo> list)
        {
            List<FriendItemInfo> symbolList = new List<FriendItemInfo>();
            symbolList.Clear();

            List<FriendItemInfo> numberList = new List<FriendItemInfo>();
            numberList.Clear();

            List<FriendItemInfo> EnglishList = new List<FriendItemInfo>();
            EnglishList.Clear();

            List<FriendItemInfo> ChineseList = new List<FriendItemInfo>();
            ChineseList.Clear();

            foreach (var item in list)
            {
                if (IsSymbol(item.name[0]))
                {
                    symbolList.Add(item);
                }
                else if (IsNumber(item.name[0]))
                {
                    numberList.Add(item);
                }
                else if (IsEnglish(item.name[0]))
                {
                    EnglishList.Add(item);
                }
                else
                {
                    ChineseList.Add(item);
                }
     
                
              
            }

            symbolList.Sort((FriendItemInfo info1, FriendItemInfo info2) => { return string.Compare(info1.name[0].ToString(), info2.name[0].ToString()); });
            numberList.Sort((FriendItemInfo info1, FriendItemInfo info2) => { return string.Compare(info1.name[0].ToString(), info2.name[0].ToString()); });
            EnglishList.Sort((FriendItemInfo info1, FriendItemInfo info2) => { return string.Compare(info1.name[0].ToString(), info2.name[0].ToString()); });
            ChineseList.Sort(SortByChinese);

            list.Clear();
            foreach (var item in symbolList)
            {
                list.Add(item);
            }

            foreach (var item in numberList)
            {
                list.Add(item);
            }

            foreach (var item in EnglishList)
            {
                list.Add(item);
            }

            foreach (var item in ChineseList)
            {
                list.Add(item);
            }
        }

        void SetFriendListItem()
        {
            if (m_friendsMap != null)
            {
                if (m_friendsMap != null && m_friendsMap.Count >0)
                {
                    List<FriendItemInfo> friendList = new List<FriendItemInfo>();
                    friendList.Clear();
                    foreach (var item in m_friendsMap)
                    {
                        friendList.Add(item.Value);                     
                    }

                  

                    SortFriendList(friendList);
                    
                    int idex = 0;
                    foreach (var data in friendList)
                    {
                        if (idex > m_Gid1.childCount - 1)
                        {
                            FriendListItem listItem = NewItem(m_Gid1, idex);
                            if (listItem != null)
                            {
                                if (idex > m_FriendListItems.Count - 1)
                                {
                                    m_FriendListItems.Add(listItem);
                                }
                            }
                        }

                        if (idex < m_FriendListItems.Count)
                        {
                            UpDataItem(data, ListType.Friend, idex);
                        }
                        idex++;
                    }
                 
                }
            }
            
        }

        void ResetFriendListItem(bool isRemove = false)
        {
            if (m_Gid1 != null)
            {
                if (isRemove)
                {
                    for (int i = 0; i < m_Gid1.childCount; i++)
                    {
                        GameObject.DestroyObject(m_Gid1.GetChild(i).gameObject);
                    }

                    m_FriendListItems.Clear();
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


        void SetEnemyListItem()
        {
            if (m_enemyMap != null)
            {
                if (m_enemyMap != null && m_enemyMap.Count >0)
                {
                    List<FriendItemInfo> enemyList = new List<FriendItemInfo>();
                    enemyList.Clear();              
                    foreach (var item in m_enemyMap)
                    {
                        enemyList.Add(item.Value);
                    }
                
                    enemyList.Sort(SortEnemy);
                    int idex = 0;
                    foreach (var data in enemyList)
                    {
                        if (idex > m_Gid2.childCount - 1)
                        {
                            FriendListItem listItem = NewItem(m_Gid2, idex);
                            if (listItem != null)
                            {
                                if (idex > m_EnemyListItems.Count-1)
                                {
                                    m_EnemyListItems.Add(listItem);
                                }                            
                            }
                        }

                        if (idex < m_EnemyListItems.Count)
                        {
                            UpDataItem(data, ListType.Enemy, idex);
                        }
                        idex++;
                    }
                }
            }
        }

        void ResetEnemyListItem(bool isRemove = false)
        {
            if (m_Gid2 != null)
            {
                if (isRemove)
                {
                    for (int i = 0; i < m_Gid2.childCount; i++)
                    {
                        GameObject.DestroyObject(m_Gid2.GetChild(i).gameObject);
                    }

                    m_EnemyListItems.Clear();
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

        public void GetReadPoint()
        {
            int redCnt = 0;
            if (m_friendsMap != null)
            {
               
               foreach (var item in m_friendsMap)
               {
                  if (item.Value.isRead == 0)
                  {
                      redCnt++;
                  }
               }            
            }
            /*
            if (m_enemyMap != null)
            {

                foreach (var item in m_enemyMap)
                {
                    if (item.Value.isRead == 0)
                    {
                        redCnt++;
                    }
                }
            }
            if (m_blackMap != null)
            {

                foreach (var item in m_blackMap)
                {
                    if (item.Value.isRead == 0)
                    {
                        redCnt++;
                    }
                }
            }
            */
            m_Parent.ShowRedPoint(FriendPage.FrindPage.Contact, redCnt);
        }
        void SetBlackListItem()
        {
            if (m_blackMap != null)
            {
                if (m_blackMap != null)
                {
                    List<FriendItemInfo> blackList = new List<FriendItemInfo>();
                    blackList.Clear();
                    foreach (var item in m_blackMap)
                    {
                        blackList.Add(item.Value);
                    }

                    SortFriendListChat(blackList);

                    int idex = 0;
                    foreach (var data in blackList)
                    {
                        if (idex > m_Gid3.childCount- 1)
                        {
                            FriendListItem listItem = NewItem(m_Gid3, idex);
                            if (listItem != null)
                            {
                                if (idex > m_BlackListItems.Count -1)
                                {
                                    m_BlackListItems.Add(listItem);
                                }
                            }
                        }
                       
                        if (idex < m_BlackListItems.Count)
                        {
                            UpDataItem(data, ListType.Black, idex);
                        }
                        idex++;
                    }

                }
            }
            
        }

        void ResetBlackListItem(bool isRemove = false)
        {
            if (m_Gid3 != null)
            {
                if (isRemove)
                {
                    for (int i = 0; i < m_Gid3.childCount; i++)
                    {
                        GameObject.DestroyObject(m_Gid3.GetChild(i).gameObject);
                    }

                    m_BlackListItems.Clear();
                }
                else
                {
                    for (int i = 0; i < m_Gid3.childCount; i++)
                    {
                        m_Gid3.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
        }

        void ResetAllListItem(bool isRemove = false)
        {
            ResetFriendListItem(isRemove);
            ResetEnemyListItem(isRemove);
            ResetBlackListItem(isRemove);
        }

        void RefreshUI()
        {
            ResetAllListItem();
            if (m_friendBtn.GetComponent<StateRoot>().CurrentState == 1)
            {
                SetFriendListItem();
            }
            if (m_EnemyBtn.GetComponent<StateRoot>().CurrentState == 1)
            {
                SetEnemyListItem();
            }
            if (m_BlackBtn.GetComponent<StateRoot>().CurrentState == 1)
            {
                SetBlackListItem();
            }


            if (m_friendsMap !=null && m_enemyMap !=null && m_blackMap != null)
            {

                FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
                if (friendModule != null && friendModule.friendMgr != null)
                {

                    //显示分组数量
                    if (m_ParentGrid.Find("TabBtn1/CiShuText") != null)
                    {
                        if (m_friendsMap != null)
                        {
                            int friendOnlineCount = ((FriendMgr)friendModule.friendMgr).GetFriendOnlineCount(m_friendsMap);

                            m_ParentGrid.Find("TabBtn1/CiShuText").GetComponent<Text>().text = string.Format("{0}/{1}", friendOnlineCount, m_friendsMap.Count);
                        }
                    }

                    if (m_ParentGrid.Find("TabBtn2/CiShuText") != null)
                    {

                        if (m_enemyMap != null)
                        {
                            int enemyOnlineCount = ((FriendMgr)friendModule.friendMgr).GetFriendOnlineCount(m_enemyMap);
                            m_ParentGrid.Find("TabBtn2/CiShuText").GetComponent<Text>().text = string.Format("{0}/{1}", enemyOnlineCount, m_enemyMap.Count);
                        }
                    }

                    if (m_ParentGrid.Find("TabBtn3/CiShuText") != null)
                    {

                        if (m_blackMap != null)
                        {
                            int blackOnlineCount = ((FriendMgr)friendModule.friendMgr).GetFriendOnlineCount(m_blackMap);
                            m_ParentGrid.Find("TabBtn3/CiShuText").GetComponent<Text>().text = string.Format("{0}/{1}", blackOnlineCount, m_blackMap.Count);
                        }
                    }
                }
            }
        }

        public void ReSetSelect()
        {
            foreach (var item in m_FriendListItems)
            {
                item.SetSelect(0);
            }
            
            foreach (var item in m_EnemyListItems)
            {
                item.SetSelect(0);
            }

            foreach (var item in m_BlackListItems)
            {
                item.SetSelect(0);
            }

        }

        protected void OnSelectItem(FriendListItem item)
        {
            ReSetSelect();
            item.SetSelect(1);

            m_SelectItem = item;

            //Debuger.DebugLog(item.GetItemData().name);

            if (m_SelectCallBack != null && item != null)
                m_SelectCallBack();

            if (m_Parent != null)
            {
                FriendItemInfo info = item.GetItemData();
                if (item.m_Type == FriendListItem.FriendItemType.Black)
                {
                    return;
                }
                else
                {
                    m_Parent.ChangeMessageRoot(2);
                    m_Parent.OnChatItemClick(info,2);
                    //info.itemType = 2;
                }
               
            }

        }

        protected void RecvFriendData()
        {
            m_friendsMap = null;
            m_enemyMap = null;
            m_blackMap = null;
            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
            if (friendModule != null && friendModule.friendMgr != null)
            {
                m_friendsMap=((FriendMgr)friendModule.friendMgr).m_friendsMap;
                m_enemyMap = ((FriendMgr)friendModule.friendMgr).m_enemysMap;
                m_blackMap = ((FriendMgr)friendModule.friendMgr).m_blacksMap;
            }

            GetReadPoint();
            RefreshUI();
            
          

            //if (m_ParentToggle.Select == 3)
            //{
                //更新searchData的状态       
            if (friendModule != null && friendModule.friendMgr != null)
            {
                if (((FriendMgr)friendModule.friendMgr).m_searchList.searchMap.Count > 0)
                {
                    ((FriendMgr)friendModule.friendMgr).UpdataSearchState();
                }

            }
           // }
        }


        protected void RecvFriendItemInfo(FriendItemInfo info)
        {

                if (m_friendsMap != null)
                {
   
                    int index = GetListItemIndex(m_FriendListItems, info.charid);
                    if (index != -1 && index < m_FriendListItems.Count)
                    {
                        if (m_FriendListItems[index] != null)
                        {
                            if (m_friendsMap.ContainsKey(info.charid))
                            {
                            m_friendsMap[info.charid] = info;
                                this.GetReadPoint();
                            }
                            m_FriendListItems[index].SetData(info);
                        }
                    }

                }
                if (m_enemyMap != null)
                {
                    int index = GetListItemIndex(m_EnemyListItems, info.charid);
                    if (index != -1 && index < m_EnemyListItems.Count)
                    {
                        if (m_EnemyListItems[index] != null)
                        {
                            m_EnemyListItems[index].SetData(info);
                        }
                    }

                }

                if (m_blackMap != null)
                {

                    int index = GetListItemIndex(m_BlackListItems, info.charid);
                    if (index != -1 && index < m_BlackListItems.Count)
                    {
                        if (m_BlackListItems[index] != null)
                        {
                            m_BlackListItems[index].SetData(info);
                        }
                    }

                }

        }

        static public string getSpell(string cn)
        {
            byte[] arrCN = System.Text.Encoding.Default.GetBytes(cn);
            if (arrCN.Length > 1)
            {
                int area = (short)arrCN[0];
                int pos = (short)arrCN[1];
                int code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return System.Text.Encoding.Default.GetString(new byte[] { (byte)(65 + i) });
                    }
                }
                return "?";
            }
            else return cn;

        }

    }


   

}
#endif
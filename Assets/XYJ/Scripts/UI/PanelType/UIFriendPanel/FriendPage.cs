#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    namespace Friend
    {
        class FriendPage : HotTablePageBase
        {
            public enum FrindPage
            {
                Recently = 0,
                Contact  = 1,
                RequestAdd = 2,
                Space = 3,
            }

            FriendPage() : base(null) { }
            public FriendPage(HotTablePage page) : base(page)
            {

            }

            [SerializeField]
            Transform m_transform;
            [SerializeField]
            Button m_RecentlyBtn;
            [SerializeField]
            Button m_ContactBtn;
            [SerializeField]
            Button m_RequestAddBtn;
            [SerializeField]
            Button m_SpaceBtn;
            [SerializeField]
            ILMonoBehaviour m_ILFriendApplyList;
            FriendApplyList m_FriendApplyList;
            [SerializeField]
            ILMonoBehaviour m_ILFriendSearchList;
            FriendSearchList m_FriendSearchList;
            [SerializeField]
            ILMonoBehaviour m_ILFriendContactList;
            FriendContactList m_FriendContactList;
            [SerializeField]
            ILMonoBehaviour m_ILFriendRecentlyList;
            FriendRecentlyList m_FriendRecentlyList;
            [SerializeField]
            InputField m_SearchField;
            [SerializeField]
            Text m_SearchFieldText;
            [SerializeField]
            Button m_BtnSearch;
            [SerializeField]
            Text SearchTiltle;


            [SerializeField]
            StateRoot m_PageState;

            [SerializeField]
            ILMonoBehaviour m_ILMessage;
            FriendMessage m_Message;

            [SerializeField]
            Button m_Search;
            [SerializeField]
            StateRoot m_MessageStateRoot;
            [SerializeField]
            Button m_clearApplyBtn;

            [SerializeField]
            Transform m_BtnToggleForm;
            private FrindPage m_CurPage;

            int m_chatRedCnt = 0;
            int m_friendRedCnt = 0;
            int m_applyRedCnt = 0;
            int m_spaceReadCnt = 0;

            protected override void OnInit()
            {
                m_CurPage = FrindPage.Recently;

                m_RecentlyBtn.onClick.AddListenerIfNoExist(() => { this.OnRecentlyBtnClick(); });
                m_ContactBtn.onClick.AddListenerIfNoExist(() => { this.OnContactBtnClick(); });
                m_RequestAddBtn.onClick.AddListenerIfNoExist(() => { this.OnRequestAddClick(); });
                m_SpaceBtn.onClick.AddListenerIfNoExist(() => { this.OnSpaceBtnClick(); });

                m_BtnSearch.onClick.AddListenerIfNoExist(() => { this.OnSearchClick(); });

                m_Search.onClick.AddListenerIfNoExist(() => { this.OnClickSearch(); });

                m_clearApplyBtn.onClick.AddListenerIfNoExist(()=> { this.OnClickClearApply(); });

                
            }

            void InitMessage()
            {
                if (m_ILMessage != null)
                {
                    object obj = m_ILMessage.GetObject();
                    m_Message = (FriendMessage)obj;
                    if (m_Message == null)
                        return;
                }
            }

            void SetHeadBtnSelect(int index) //0最近 1联系人 2申请 3空间
            {
                
                m_RecentlyBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
                m_ContactBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
                m_RequestAddBtn.GetComponent<StateRoot>().SetCurrentState(0, true);
                m_SpaceBtn.GetComponent<StateRoot>().SetCurrentState(0, true);

                switch (index)
                {
                    case 0:
                        m_RecentlyBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
                        break;
                    case 1:
                        m_ContactBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
                        break;
                    case 2:
                        m_RequestAddBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
                        break;
                    case 3:
                        m_SpaceBtn.GetComponent<StateRoot>().SetCurrentState(1, true);
                        break;
                    default:
                        break;
                }
            }

            void InitSearchList()
            {
                if (m_ILFriendSearchList != null)
                {
                    object obj = m_ILFriendSearchList.GetObject();
                    m_FriendSearchList = (FriendSearchList)obj;
                    if (m_FriendSearchList == null)
                        return;
                    m_FriendSearchList.SetEvent(Event);
                    //m_FriendApplyList.selectedCallback = this.ResetPage;
                }
            }

            public void ChangeMessageRoot(int idx) //0 没有 1 系统 2 好友
            {
                if (idx >=0 && idx <3)
                {
                    m_MessageStateRoot.CurrentState = idx;
                 }
            }

            void InitApplyList()
            {
                if (m_ILFriendApplyList != null)
                {
                    object obj = m_ILFriendApplyList.GetObject();
                    m_FriendApplyList = (FriendApplyList)obj;
                    if (m_FriendApplyList == null)
                        return;
                    m_FriendApplyList.SetEvent(Event);
                    m_FriendApplyList.SetParent(this);
                    //m_FriendApplyList.selectedCallback = this.ResetPage;
                }
            }

            void InitContactList()
            {
                if (m_ILFriendContactList != null)
                {
                    object obj = m_ILFriendContactList.GetObject();
                    m_FriendContactList = (FriendContactList)obj;
                    if (m_FriendContactList == null)
                        return;
                    m_FriendContactList.SetEvent(Event);
                    m_FriendContactList.SetParent(this);
                    //m_FriendApplyList.selectedCallback = this.ResetPage;
                }
            }

            void InitRecentlyList()
            {
                if (m_ILFriendContactList != null)
                {
                    object obj = m_ILFriendRecentlyList.GetObject();
                    m_FriendRecentlyList = (FriendRecentlyList)obj;
                    if (m_FriendRecentlyList == null)
                        return;
                    m_FriendRecentlyList.SetEvent(Event);
                    m_FriendRecentlyList.SetParent(this);
                    //m_FriendApplyList.selectedCallback = this.ResetPage;
                }
            }

            public void OnChatItemClick(FriendItemInfo data,int idx) //0 没有选中 1系统 2好友
            {
                if (m_Message != null)
                {
                    m_Message.SetData(data,idx);
                }

            }

            public bool OnClickClearApply(object args = null)
            {
                Event.FireEvent(EventID.Friend_ClearAllApply, App.my.localPlayer.charid);
                return true;
            }

            private bool ChangeByte(string str, int i)
            {
                byte[] b = Encoding.Default.GetBytes(str);
                int m = b.Length;
                if (m < i)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public void OnClickSearch(object args = null)
            {
                if (!ChangeByte(m_SearchFieldText.text,4))
                {
                    SystemHintMgr.ShowHint("搜索关键词长度太短");
                    m_FriendSearchList.ResetSearchList();
                    return;
                }

                Event.FireEvent(EventID.Friend_Search, m_SearchFieldText.text);

            }


            void OnRecentlyBtnClick(object args = null)
            {
                m_CurPage = FrindPage.Recently;
                m_Message.SetMessgeType(0);
                RefreshFrame();
            }

            void OnContactBtnClick(object args = null)
            {
                m_CurPage = FrindPage.Contact;
                m_Message.SetMessgeType(0);
                RefreshFrame();
            }

            void OnRequestAddClick(object args = null)
            {
                m_CurPage = FrindPage.RequestAdd;
                RefreshFrame();
            }

            void OnSpaceBtnClick(object args = null)
            {
                //m_CurPage = FrindPage.Space;
                SystemHintMgr.ShowHint("功能尚未开放");
                RefreshFrame();
            }

            void OnSearchClick(object args = null)
            {
                if (m_SearchFieldText.text != "")
                {
                    m_FriendSearchList.SetType(1);
                    SearchTiltle.text = "搜索结果";
                }
                else
                {
                    m_FriendSearchList.SetType(0);
                    SearchTiltle.text = "推荐好友";
                    //m_FriendSearchList.ResetSearchList();
                }
            }
            
            protected override void OnShow(object args)
            {
                InitApplyList();
                InitSearchList();
                InitContactList();
                InitRecentlyList();
                InitMessage();

                Event.fireEvent(EventID.Friend_GetFriendData);
                Event.fireEvent(EventID.Friend_GetRecentlyInfo);
                Event.fireEvent(EventID.Friend_GetApplyData);

                m_CurPage = FrindPage.Recently;
                m_BtnToggleForm.gameObject.SetActive(true);
                if (m_BtnToggleForm != null)
                {
                    m_BtnToggleForm.GetComponent<StateToggle>().Select = 0;
                }
                RefreshFrame();
            }

            public void ShowRedPoint(FrindPage pageIdx,int count = 0)
            {
                switch (pageIdx)
                {
                    case FrindPage.Recently:
                        m_BtnToggleForm.Find("RecentlyBtn").Find("Point").gameObject.SetActive(count >0 ? true :false);
                        m_chatRedCnt = count ;
                        break;
                    case FrindPage.Contact:
                        m_friendRedCnt = count ;
                        m_BtnToggleForm.Find("ContactBtn").Find("Point").gameObject.SetActive(count > 0 ? true : false);                      
                        break;
                    case FrindPage.RequestAdd:
                        m_BtnToggleForm.Find("RequestAddBtn").Find("Point").gameObject.SetActive(count > 0 ? true : false);
                        m_applyRedCnt = count ;
                        break;
                    case FrindPage.Space:
                        m_BtnToggleForm.Find("SpaceBtn").Find("Point").gameObject.SetActive(count > 0 ? true : false);
                        m_spaceReadCnt = count ;
                        break;
                    default:
                        break;
                }

                App.my.eventSet.FireEvent(EventID.Friend_ShowRed_Point, m_chatRedCnt + m_friendRedCnt + m_applyRedCnt + m_spaceReadCnt);
            }

            protected override void OnHide()
            {
                m_BtnToggleForm.gameObject.SetActive(false);
                ResetAllList();
            }

            void ResetAllList()
            {          
                m_FriendContactList.Hide();
                m_FriendApplyList.Hide();
                m_FriendSearchList.Hide();
            }
                       

            void RefreshFrame()
            {
                if (m_FriendRecentlyList != null)
                {
                    m_FriendRecentlyList.GetReadPoint();
                }
                if (m_FriendApplyList != null)
                {
                    m_FriendApplyList.GetReadPoint();
                }
                if (m_FriendContactList != null)
                {
                    m_FriendContactList.GetReadPoint();
                }
                m_FriendSearchList.isShow = false;
                App.my.uiSystem.HidePanel(PanelType.UIRoleOperationPanel);
                if (m_CurPage == FrindPage.Recently)
                {
                    m_PageState.SetCurrentState(0,true);

                    if (m_FriendRecentlyList != null)
                    {
                        parent.StartCoroutine(m_FriendRecentlyList.Show());
                    }
                }
                else if (m_CurPage == FrindPage.Contact)
                {
                    m_PageState.SetCurrentState(1, true);

                    if (m_FriendContactList != null)
                    {
                        parent.StartCoroutine(m_FriendContactList.Show());
                    }
                }
                else if (m_CurPage == FrindPage.RequestAdd)
                {
                    m_PageState.SetCurrentState(2, true);
                    if (m_FriendApplyList != null)
                    {
                        parent.StartCoroutine(m_FriendApplyList.Show());
                    }
                    if (m_FriendSearchList != null)
                    {
                        m_FriendSearchList.isShow = true;
                        m_SearchField.text = "";
                        m_SearchFieldText.text = "";
                        m_FriendSearchList.SetType(0);
                        SearchTiltle.text = "推荐好友";
                        parent.StartCoroutine(m_FriendSearchList.Show());                    
                    }
                                                                      
                }

                

                /*
                else if (m_CurPage == FrindPage.Space)
                {
                    m_PageState.SetCurrentState(3, true);
                }
                */
            }

        }

    }

}
#endif
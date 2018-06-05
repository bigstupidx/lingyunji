#if !USE_HOT
#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using xys.hot.UI.Friend;
using xys.UI.State;
using NetProto;
using NetProto.Hot;
using Config;

namespace xys.hot.UI
{
    namespace Friend
    {
        [AutoILMono]
        public class FriendListItem
        {
            public enum FriendItemType
            {
                System = 0,          //系统消息
                Friend = 1,          //好友
                Enemy = 2,          //敌人
                Black = 3,          //黑名单
                Team = 4,          //最近组队
                Apply = 5,           //申请
                Recently = 6,        //最近聊天
                recommend = 7,       //推荐或搜索
            }


            [SerializeField]
            Transform m_Transform;

            [SerializeField]
            Button m_AddBtn;
            [SerializeField]
            Button m_HeadIcon;

            private FriendItemInfo m_Data = new FriendItemInfo();

            string[] Job = { "icon_feiyu_Color", "icon_guigu_Color", "icon_mojia_Color", "icon_qiyao_Color", "icon_tianjian_Color", "icon_tuoba_Color" };

            //string[] iconNan = { "TianJian_Nan" }

            //string[] iconNv = { "TianJian_Nan" }

            protected System.Action<FriendListItem> m_ClickEvent = null;

            public FriendItemType m_Type { get; set; }

            public Transform transform { get { return m_Transform; } }

            public FriendListItem() { }

            public FriendItemInfo GetItemData()
            {
                return m_Data;
            }

            EventAgent m_EventAgent = null;


            public void SetEvent(EventAgent eventAgent = null)
            {
                m_EventAgent = eventAgent;
                if (m_EventAgent != null)
                {
                    //m_EventAgent.Subscribe<FriendBaseData>(EventID.Friend_FriendItemInfoUpdata, this.RecvFriendItemInfo);
                }

            }


            void Awake()
            {
                UnityEngine.UI.Button selfButton = this.m_Transform.GetComponent<UnityEngine.UI.Button>();
                if (selfButton != null) selfButton.onClick.AddListenerIfNoExist(this.OnClick);

                m_AddBtn.onClick.AddListenerIfNoExist(() => { this.OnAddItemClick(); });
                m_HeadIcon.onClick.AddListenerIfNoExist(() => { this.OnClickHeadIcon(); });
            }

            public void SetData(FriendItemInfo data)
            {
                if (data.charid == 0 || m_AddBtn == null || m_Transform == null)
                {
                    return;
                }
                if (data.itemType >=0)
                {
                    m_Data = data;
                    if (m_AddBtn != null)
                    {
                        m_AddBtn.GetComponent<StateRoot>().CurrentState = 0;
                        m_AddBtn.gameObject.SetActive(true);
                    }
                    m_Transform.Find("Job").gameObject.SetActive(true);
                    m_Transform.GetComponent<StateRoot>().CurrentState = 0;

                    if (m_Type == FriendItemType.System)
                    {

                        m_Transform.Find("Job").gameObject.SetActive(false);

                        m_Transform.GetComponent<StateRoot>().CurrentState = 1;
                    }
                    else if (m_Type == FriendItemType.recommend)
                    {

                       
                        //更新打钩状态

                            FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
                            if (friendModule != null && friendModule.friendMgr != null)
                            {
                                bool isfriend = ((FriendMgr)friendModule.friendMgr).IsFriend(m_Data.charid);
                                if (isfriend)
                                {
                                    m_AddBtn.GetComponent<StateRoot>().CurrentState = 2;
                                }
                                else
                                {
                                     m_AddBtn.GetComponent<StateRoot>().CurrentState = 0;
                                }
                             }

                    }

                    if (m_Type != FriendItemType.Apply && m_Type != FriendItemType.recommend && m_Type != FriendItemType.System)
                    {
                        if (m_AddBtn)
                        {
                            m_AddBtn.GetComponent<StateRoot>().CurrentState = 1;
                        }
                    }
                }


                if (data.name != "")
                {
                    m_Transform.Find("Name").GetComponent<Text>().text = data.name;
                }



                if (data.job != 0)
                {
                    Image jobicon = m_Transform.Find("Job").Find("Icon").GetComponent<Image>();
                    jobicon.gameObject.SetActive(true);
               

                    Image headIcon = m_Transform.Find("Head").Find("HeadIcon").GetComponent<Image>();
                    Image headBg = m_Transform.Find("Head").Find("Bg2").GetComponent<Image>();
                   // headBg.gameObject.SetActive(true);


                    RoleJob roleJob = RoleJob.Get((int)data.job);

                    xys.UI.Helper.SetSprite(jobicon, roleJob.colorIcon);

                    //1为男性角色
                    xys.UI.Helper.SetSprite(headIcon, data.sex == 1 ? roleJob.maleIcon : roleJob.femalIcon);
                    xys.UI.Helper.SetSprite(headBg, data.sex == 1 ? roleJob.maleHeadBack : roleJob.femalHeadBank);

                }
                else
                {
                    //系统消息
                    Image jobicon = m_Transform.Find("Job").Find("Icon").GetComponent<Image>();
                    jobicon.gameObject.SetActive(false);

                    Image headIcon = m_Transform.Find("Head").Find("HeadIcon").GetComponent<Image>();
                    Image headBg = m_Transform.Find("Head").Find("Bg2").GetComponent<Image>();

                    //headBg.gameObject.SetActive(false);
                    xys.UI.Helper.SetSprite(headIcon, "FriendSettingIcon");

                }

                if (data.isOnline)
                {
                    m_Transform.Find("Head").Find("HeadIcon").GetComponent<StateRoot>().CurrentState = 1;

                }
                else
                {
                    m_Transform.Find("Head").Find("HeadIcon").GetComponent<StateRoot>().CurrentState = 0;
                }


                if (data.level >= 0)
                {
                    m_Transform.Find("Level").gameObject.SetActive(true);
                    m_Transform.Find("Level").GetComponent<Text>().text = string.Format("{0}级", data.level);
                }
                else
                {
                    m_Transform.Find("Level").GetComponent<Text>().text = "";
                    m_Transform.Find("Level").gameObject.SetActive(false);
                }

                if (data.isRead == 0)
                {
                    if (m_Type == FriendItemType.Apply || m_Type == FriendItemType.Recently || m_Type == FriendItemType.Friend)
                    {
                        m_Transform.Find("Head").Find("Point").gameObject.SetActive(true);
                    }
                    else
                    {
                        m_Transform.Find("Head").Find("Point").gameObject.SetActive(false);
                    }
                }
                else
                {
                    m_Transform.Find("Head").Find("Point").gameObject.SetActive(false);
                }
                
            }

            public void Set(System.Action<FriendListItem> click_event = null)
            {
                //Image image = this.m_Transform.Find("Icon").GetComponent<Image>();
               // Helper.SetSprite(image, item.icon);
                m_ClickEvent = click_event;
            }

            public void SetSelect(int state = 0)    //0非选中 1选中
            {
                m_Transform.Find("Bg").GetComponent<StateRoot>().CurrentState = state;
            }

            void OnClick()
            {

                if (m_ClickEvent != null)
                {
                    //点击的时候更新状态
                    FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
                    if (friendModule != null && friendModule.friendMgr != null)
                    {
                        switch (m_Type)
                        {
                            case FriendItemType.System:
                                break;                            
                            case FriendItemType.Enemy:
                            case FriendItemType.Black:
                            case FriendItemType.Team:
                            case FriendItemType.recommend:
                                ((FriendMgr)friendModule.friendMgr).GetFriendDetail(m_Data.charid, FriendListType.FD_None);
                                break;
                            case FriendItemType.Apply:
                                ((FriendMgr)friendModule.friendMgr).GetFriendDetail(m_Data.charid, FriendListType.FD_Apply);
                                break;
                            case FriendItemType.Recently:
                                ((FriendMgr)friendModule.friendMgr).GetFriendDetail(m_Data.charid, FriendListType.FD_Chat);
                                break;
                            case FriendItemType.Friend:
                                ((FriendMgr)friendModule.friendMgr).GetFriendDetail(m_Data.charid, FriendListType.FD_Friend);
                                break;
                           
                            default:
                                break;
                        }
                        
                        /*FriendItemInfo info = ((FriendMgr)friendModule.friendMgr).GetFriendDetail(m_Data.charid);
                        if (info != null)
                        {
                            this.SetData(info);
                        }
                        */
                    }


                    m_ClickEvent(this);
                }
            }

            void OnClickHeadIcon(object arg = null)
            {
                if (m_Type == FriendItemType.Apply || m_Type == FriendItemType.recommend)
                {
                    xys.UI.UIRoleOperationData obj = new xys.UI.UIRoleOperationData(m_Data.charid, xys.UI.RoleOperShowType.Custom);

                    if (m_Type == FriendItemType.Apply)
                    {
                        obj.panelPos = new Vector3(0, 105, 0);
                    }
                    else
                    {
                        obj.panelPos = new Vector3(100, 105, 0);
                    }
                    App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIRoleOperationPanel, obj);
                }
            }

            void OnAddItemClick(object arg = null)
            {

                switch (m_Type)
                {
                case FriendItemType.System:
                    break;
                case FriendItemType.Enemy:
                    xys.UI.UIRoleOperationData enemy = new xys.UI.UIRoleOperationData(m_Data.charid, xys.UI.RoleOperShowType.FriendEnemyPanel);
                    enemy.panelPos = new Vector3(0, 105, 0);

                    App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIRoleOperationPanel, enemy);
                    break;
                case FriendItemType.Recently:
                case FriendItemType.Team:
                    xys.UI.UIRoleOperationData recently = new xys.UI.UIRoleOperationData(m_Data.charid, xys.UI.RoleOperShowType.RecentlyListPanel);
                    recently.panelPos = new Vector3(0, 105, 0);

                    FriendModule friendModule = App.my.localPlayer.GetModule(ModuleType.MT_Friend) as FriendModule;
                    if (friendModule != null && friendModule.friendMgr != null)
                    {
                        ((FriendMgr)friendModule.friendMgr).m_RecentlyType = m_Type == FriendItemType.Recently ? 1 : 2;
                    }

                    App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIRoleOperationPanel, recently);
                    break;
                case FriendItemType.Friend:
                case FriendItemType.Black:
                    xys.UI.UIRoleOperationData obj = new xys.UI.UIRoleOperationData(m_Data.charid, xys.UI.RoleOperShowType.Custom);
                    obj.panelPos = new Vector3(0, 105, 0);

                    App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIRoleOperationPanel, obj);
                    break;
                case FriendItemType.Apply:
                    App.my.eventSet.FireEvent<long>(EventID.Friend_AddFriend, m_Data.charid);
                    break;
                case FriendItemType.recommend:
                    if (m_AddBtn.GetComponent<StateRoot>().CurrentState == 2)
                    {
                        break;
                    }
                    App.my.eventSet.FireEvent<long>(EventID.Friend_Apply, m_Data.charid);
                    break;
                default:
                    break;
                }

            }

            public void RecvFriendItemInfo(FriendItemInfo data)
            {
                if (data != null)
                {
                    if (this.m_Data.charid != 0 && data.charid == this.m_Data.charid)
                    {
                        this.SetData(data);
                    }
                }


            }

        }
    }
}
#endif
#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using xys.UI;
namespace xys.hot.UI
{
    class MainPanel : HotPanelBase
    {
        [SerializeField]
        Button activityStateBtn; // 活动状态按钮

        [SerializeField]
        ILMonoBehaviour m_ILActivityItem;
        FunctionItem activityItem; // 活动项

        [SerializeField]
        ILMonoBehaviour m_ILIntergralItem;
        FunctionItem integralItem; // 积分项

        [SerializeField]
        ILMonoBehaviour m_ILEasyCityItem;
        FunctionItem easyCityItem; // 易市

        [SerializeField]
        ILMonoBehaviour m_ILWelfareItem;
        FunctionItem welfareItem; // 福利

        [SerializeField]
        ILMonoBehaviour m_ILStrongerItem;
        FunctionItem strongerItem; // 变强

        [SerializeField]
        ILMonoBehaviour m_ILStoreItem;
        FunctionItem storeItem; // 商店

        [SerializeField]
        ILMonoBehaviour m_ILFirstChargeItem;
        FunctionItem firstChargeItem; // 首充

        [SerializeField]
        Button tempPackageItem; // 临时背包

        [SerializeField]
        ILMonoBehaviour m_ILFriendItem;
        FunctionItem friendItem; //好友

        [SerializeField]
        ILMonoBehaviour m_ILImproveItem;
        FunctionItem improveItem; //提升
        [SerializeField]
        ILMonoBehaviour m_ILMoneyTreeItem;
        FunctionItem moneyTreeItem; //摇钱树
        [SerializeField]
        ILMonoBehaviour m_ILFoodItem;
        FunctionItem foodItem; //抢食
        [SerializeField]
        ILMonoBehaviour m_ILAdventureItem;
        FunctionItem adventureItem; //奇遇
        [SerializeField]
        ILMonoBehaviour m_ILQuestionItem;
        FunctionItem questionItem; //答题
        [SerializeField]
        ILMonoBehaviour m_ILDungeonScoreItem;
        FunctionItem dungeonScoreItem; //战绩
        [SerializeField]
        ILMonoBehaviour m_ILExitDungeonItem;
        FunctionItem exitDungeonItem; //退出副本

        [SerializeField]
        UIMainMiniMap m_miniMap;

        [SerializeField]
        Button PetBtn;

        [SerializeField]
        Transform FriendTransform;

        // 进度条
        ProgressBase m_ProgessBar;

        [SerializeField]
        private UIMainChatPanel m_uiMainChatPanel;
        [SerializeField]
        UIMainBloodPoolPanel m_uiMainBloodPoolPanel;

        JoystickView joystick;
        UISkillManager skillMgr;
        UIRoleInfoMgr roleInfoMgr;
        TaskTeamPanel taskTeamPanel;
        static Dictionary<int, FunctionItem> m_ItemDic = new Dictionary<int, FunctionItem>();
        static Dictionary<int, Button> m_CommonBtnDic = new Dictionary<int, Button>();
        static Dictionary<int, MainPanelItemListener> m_ListenerDic = new Dictionary<int, MainPanelItemListener>();

        public MainPanel() : base(null)
        {
        }

        public MainPanel(xys.UI.UIMainPanel parent) : base(parent)
        {
            joystick = new JoystickView(parent.gameObject);
            skillMgr = new UISkillManager(parent.gameObject.transform);
            roleInfoMgr = new UIRoleInfoMgr(parent.gameObject.transform);

            taskTeamPanel = new TaskTeamPanel(parent.gameObject);
            m_ProgessBar = null;
        }

        protected override void OnInit()
        {
            InitItemDic();
            var itemItr = m_ItemDic.GetEnumerator();
            while (itemItr.MoveNext())
            {
                PanelType type = (PanelType)itemItr.Current.Key ;
                int index = itemItr.Current.Key;
                itemItr.Current.Value.AddOnClickListener(() =>
                {
                    if (m_ListenerDic.ContainsKey(index))
                    {
                        if (m_ListenerDic[index].onClickCallBack != null)
                            m_ListenerDic[index].onClickCallBack(m_ListenerDic[index].arg);
                        else
                            App.my.uiSystem.ShowPanel(type, m_ListenerDic[index].arg);
                    }
                        
                });
            }
            var btnItr = m_CommonBtnDic.GetEnumerator();
            while (btnItr.MoveNext())
            {
                PanelType type = (PanelType)itemItr.Current.Key;
                int index = itemItr.Current.Key;
                btnItr.Current.Value.onClick.AddListener(() =>
                {
                    if (m_ListenerDic.ContainsKey(index))
                        App.my.uiSystem.ShowPanel(type, m_ListenerDic[index].arg);
                });
            }
            activityStateBtn.onClick.AddListener(OnActivityStateBtnClick);

            PetBtn.onClick.AddListener(() => {
                App.my.uiSystem.ShowPanel(xys.UI.PanelType.UIPetsPanel, null/*showArgDic[PanelType.UIPetsPanel]*/);
            });
            m_uiMainChatPanel.OnInit();
            m_uiMainBloodPoolPanel.OnInit(parent);
        }

        protected override void OnShow(object args)
        {
            Event.Subscribe<int>(EventID.Friend_ShowRed_Point, ShowRedPoint);
            Event.Subscribe(NetProto.AttType.AT_Level, RefreshUI);

            RefreshUI();
            skillMgr.OnShow( Event);
            roleInfoMgr.OnShow(Event);
            m_uiMainChatPanel.OnShow();
            m_uiMainBloodPoolPanel.OnShow();
            Event.fireEvent(EventID.Friend_GetFriendData);
            Event.fireEvent(EventID.Friend_GetRecentlyInfo);
            Event.fireEvent(EventID.Friend_GetApplyData);

            Event.Subscribe<ProgressData>(xys.EventID.Package_ProgessBar, ShowProgessbar);

            m_miniMap.OnInit(Event);

            moneyTreeItem.SetText("");
        }

        protected override void OnHide()
        {
            roleInfoMgr.OnHide();
            m_uiMainChatPanel.OnHide();
            m_uiMainBloodPoolPanel.OnHide();
        }

        void OnActivityStateBtnClick()
        {
            bool v = ((UIMainPanel)parent).isActivtyHideState;
            ((UIMainPanel)parent).isActivtyHideState = !v;
        }

        void InitItemDic()
        {
            m_ItemDic.Clear();

            m_CommonBtnDic.Clear();
            if(m_ILActivityItem!=null)
                activityItem =(FunctionItem) m_ILActivityItem.GetObject();
            if (m_ILIntergralItem != null)
                integralItem = (FunctionItem)m_ILIntergralItem.GetObject();
            if (m_ILEasyCityItem != null)
                easyCityItem = (FunctionItem)m_ILEasyCityItem.GetObject();
            if (m_ILWelfareItem != null)
                welfareItem = (FunctionItem)m_ILWelfareItem.GetObject();
            if (m_ILStrongerItem != null)
                strongerItem = (FunctionItem)m_ILStrongerItem.GetObject();
            if (m_ILStoreItem != null)
                storeItem = (FunctionItem)m_ILStoreItem.GetObject();
            if (m_ILFirstChargeItem != null)
                firstChargeItem = (FunctionItem)m_ILFirstChargeItem.GetObject();
            if (m_ILFriendItem != null)
                friendItem = (FunctionItem)m_ILFriendItem.GetObject();
            if (m_ILImproveItem != null)
                improveItem = (FunctionItem)m_ILImproveItem.GetObject();
            if (m_ILMoneyTreeItem != null)
                moneyTreeItem = (FunctionItem)m_ILMoneyTreeItem.GetObject();
            if (m_ILFoodItem != null)
                foodItem = (FunctionItem)m_ILFoodItem.GetObject();
            if (m_ILAdventureItem != null)
                adventureItem = (FunctionItem)m_ILAdventureItem.GetObject();
            if (m_ILQuestionItem != null)
                questionItem = (FunctionItem)m_ILQuestionItem.GetObject();
            if (m_ILDungeonScoreItem != null)
                dungeonScoreItem = (FunctionItem)m_ILDungeonScoreItem.GetObject();
            if (m_ILExitDungeonItem != null)
                exitDungeonItem = (FunctionItem)m_ILExitDungeonItem.GetObject();

            m_ItemDic.Add((int)PanelType.UIWelfarePanel, welfareItem);
            m_ItemDic.Add((int)PanelType.UIExchangeStorePanel, integralItem);
            m_ItemDic.Add((int)PanelType.UITradingMarketTablePanel, easyCityItem);
            m_ItemDic.Add((int)PanelType.UIStorePanel, storeItem);
            m_ItemDic.Add((int)PanelType.UIActivityPanel, activityItem);
            m_ItemDic.Add((int)PanelType.UIStrongerPanel, strongerItem);
            m_ItemDic.Add((int)PanelType.UIFirstChargePanel, firstChargeItem);
            m_ItemDic.Add((int)PanelType.UIFriendPanel, friendItem);
            m_ItemDic.Add((int)PanelType.UIImprovePanel, improveItem);
            m_ItemDic.Add((int)PanelType.UIMoneyTreePanel, moneyTreeItem);
            m_ItemDic.Add((int)PanelType.UIFoodPanel, foodItem);
            m_ItemDic.Add((int)PanelType.UIAdventurePanel, adventureItem);
            m_ItemDic.Add((int)PanelType.UIQuestionPanel, questionItem);
            m_ItemDic.Add((int)PanelType.UIDungeonScorePanel, dungeonScoreItem);
            m_ItemDic.Add((int)PanelType.UIDungeonExitPanel, exitDungeonItem);
            m_CommonBtnDic.Add((int)PanelType.UITempPackagePanel, tempPackageItem);
        }

        /// <summary>
        /// 设定主界面按钮listener
        /// </summary>
        /// <param name="type">对应面板类型</param>
        /// <param name="effective">初始是否生效</param>
        /// <param name="showFunc">显示回调</param>
        /// <param name="itemReadyFunc">item就绪回调-默认null</param>
        /// <param name="args">显示参数默认null</param>
        /// <returns></returns>
        public static void SetItemListener(int type, MainPanelItemListener listener)
        {
            if (!m_ListenerDic.ContainsKey(type))
                m_ListenerDic.Add(type, listener);
            else
                m_ListenerDic[type] = listener;
        }
        void RefreshUI()
        {
            var itr = m_ItemDic.GetEnumerator();
            while(itr.MoveNext())
            {
                if (m_ListenerDic.ContainsKey(itr.Current.Key))
                {
                    if (m_ListenerDic[itr.Current.Key].itemShowFunc != null)
                        itr.Current.Value.SetActive(m_ListenerDic[itr.Current.Key].itemShowFunc());
                    if (m_ListenerDic[itr.Current.Key].itemReadyFunc != null)
                        SetItemReadyActive(itr.Current.Key, m_ListenerDic[itr.Current.Key].itemReadyFunc());
                }  
            }
            var itrCom = m_CommonBtnDic.GetEnumerator();
            while (itrCom.MoveNext())
            {
                if (m_ListenerDic.ContainsKey(itrCom.Current.Key))
                {
                    if (m_ListenerDic[itrCom.Current.Key].itemShowFunc != null)
                        itrCom.Current.Value.gameObject.SetActive(m_ListenerDic[itrCom.Current.Key].itemShowFunc());
                    if (m_ListenerDic[itrCom.Current.Key].itemReadyFunc != null)
                        SetItemReadyActive(itrCom.Current.Key, m_ListenerDic[itrCom.Current.Key].itemReadyFunc());
                }
            }
        }
        public static void SetItemReadyActive(int type,bool active)
        {
            Debug.Log("SetDot:"+(PanelType)type);
            if (m_ItemDic.ContainsKey(type))
            {
                if(active)
                    m_ItemDic[type].state = FunctionItem.ItemState.Red;
                else
                    m_ItemDic[type].state = FunctionItem.ItemState.Normal;
            }
        }

        public void ShowRedPoint(int count)
        {
            FriendTransform.Find("RedPoint").gameObject.SetActive(count >0 ? true :false);
        }

        public static void SetItemActive(int type,bool active)
        {
            if (m_ItemDic.ContainsKey(type))
            {
                if (m_ItemDic[type] != null)
                    m_ItemDic[type].SetActive(active);
                return; 
            }
            if (m_CommonBtnDic.ContainsKey(type))
            {
                if (m_CommonBtnDic[type] != null)
                    m_CommonBtnDic[type].gameObject.SetActive(active);
            }
        }

        public static void SetItemText(int type, string str)
        {
            if (m_ItemDic.ContainsKey(type))
            {
                if (m_ItemDic[type] != null)
                    m_ItemDic[type].SetText(str);
            }
        }
        void ShowProgessbar(ProgressData obj)
        {
            if (m_ProgessBar != null)
                m_ProgessBar.Stop();
            m_ProgessBar = App.my.uiSystem.progressMgr.PlayItemCasting(obj);
        }
    }
}
#endif
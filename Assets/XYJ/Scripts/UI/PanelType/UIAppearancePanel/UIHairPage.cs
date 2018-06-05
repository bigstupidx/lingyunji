#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;
using Config;
using NetProto;
namespace xys.hot.UI
{
   
    class UIHairPage:HotTablePageBase
    {
        public UIHairPage():base(null) { }
        public UIHairPage(HotTablePage _page) : base(_page)
        {
        }

        [SerializeField]
        GameObject m_contentGO;
        [SerializeField]
        Transform m_itemRoot;
        [SerializeField]
        Transform m_dirtyItemRoot;


        [SerializeField]
        ILMonoBehaviour m_ilCompositeOper;
        UICompositeOper m_compositeOper;

        [SerializeField]
        ILMonoBehaviour m_ilText;
        UITexts m_texts;


        hot.Event.HotObjectEventSet m_hotEventagent;
       
        HairHandle m_hairHandle;
        HairConfig m_hairConfig;
        RoleDisguiseHandle m_disguiseHandle;
        RoleTempApp m_roleTempData;
        //UI控件表
        private List<UIAprItem> m_UIItemList = new List<UIAprItem>();

        private int m_curItemIndex = -1;
        bool m_isChangedRole = false;
        protected override void OnInit()
        {
            if (m_ilCompositeOper != null)
            {
                m_compositeOper = m_ilCompositeOper.GetObject() as UICompositeOper;
            }
            if(m_ilText!=null)
            {
                m_texts = m_ilText.GetObject() as UITexts;
            }

            HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
            m_hotEventagent = module.Event;

            m_hairHandle = module.GetMgr().GetHairHandle();
            m_hairConfig = m_hairHandle.m_hairConfig;
            m_disguiseHandle= module.GetMgr().GetDisguiHandle();
        }

        protected override void OnShow(object p)
        {
            if (p == null) return;
            m_roleTempData = p as RoleTempApp;
            if (m_contentGO != null)
            {
                m_contentGO.SetActive(true);
            }
            m_roleTempData.m_rttHandler.SetRenderActive(true);
            Event.Subscribe(EventID.AP_RefreshUI, RefreshUI);
            InitContent();
        }

        void InitContent()
        {
            int count = m_hairConfig.GetHairList().Count;
            UITextureItem.AddNullItem(count, m_itemRoot, m_dirtyItemRoot);
            SetContentData();
            OnEndSetContent();
        }
        void SetContentData()
        {
            m_UIItemList.Clear();
            m_curItemIndex = -1;
            //物品排序
            //解锁-按ID排序
            //未解锁-按ID排序
            m_hairConfig.GetHairList().Sort(
                (first, second) =>
                {
                    int weight = first.m_id + second.m_id;
                    int lockWeightFirst = ((int)first.m_state > 1 ? -1 : 0) * weight;
                    int lockWeightSecond = ((int)second.m_state > 1 ? -1 : 0) * weight;

                    int firstWeight = first.m_id + lockWeightFirst;
                    int secondWeight = second.m_id + lockWeightSecond;
                    return firstWeight.CompareTo(secondWeight);
                });

            int count = m_hairConfig.GetHairList().Count;
            for (int i = 0; i < count; i++)
            {
                HairItem temp = m_hairConfig.GetHairList()[i];

                GameObject tempObj = m_itemRoot.GetChild(i).gameObject;
                ILMonoBehaviour tempIL = tempObj.GetComponent<ILMonoBehaviour>();
                if (tempIL == null)
                {
                    Debug.Log("没有ILMono组件");
                    return;
                }
                UIAprItem item = tempIL.GetObject() as UIAprItem;
                UIAprItem.State uiAprState = UIAprItem.State.Lock_Unselected;
                switch (temp.m_state)
                {
                    case AprItemState.Lock:
                        uiAprState = UIAprItem.State.Lock_Unselected;
                        break;
                    case AprItemState.Unlock:
                        uiAprState = UIAprItem.State.Unlock_Unselected;
                        break;
                    case AprItemState.OutTime:
                        uiAprState = UIAprItem.State.OutTime_Unselected;
                        break;
                    default:
                        break;
                }

                item.Set(i, uiAprState, temp.GetIconName(), OnClickItem, OnItemStateChange);
                m_UIItemList.Add(item);
            }
        }
        void OnEndSetContent()
        {
            //没有佩戴的头饰

            int count = m_hairConfig.GetHairList().Count;
            for (int i = 0; i < count; i++)
            {
                HairItem temp = m_hairConfig.GetHairList()[i];
                if (temp.m_id == m_roleTempData.m_hairDressId)
                {
                    m_curItemIndex = i;
                    m_UIItemList[m_curItemIndex].m_stateRoot.NextState();
                    return;
                }
            }
        }

        protected override void OnHide()
        {
            UITextureItem.RecycleItem(m_itemRoot, m_dirtyItemRoot);
            CloseAllComponent();

            if (m_isChangedRole)
            {
                WearRequest request = new WearRequest();
                request.itemId = m_hairHandle.m_roleHairData.m_hairDressId;
                request.aprType = AprType.Hair;
                m_hotEventagent.FireEvent<WearRequest>(EventID.Ap_WearItem, request);
                m_isChangedRole = false;
              
            }

        }
        void OnClickItem(UIAprItem item)
        {
            if (m_curItemIndex == item.m_index)
            {
                return;
            }
            else
            {
                if (m_curItemIndex > -1)
                {
                    m_UIItemList[m_curItemIndex].m_stateRoot.FrontState();
                }
                m_curItemIndex = item.m_index;
                m_roleTempData.m_hairDressId= m_hairConfig.GetHairList()[m_curItemIndex].m_id;
                item.m_stateRoot.NextState();
                m_disguiseHandle.SetHairStyleById(m_roleTempData.m_hairDressId);
            }
        }
        void OnItemStateChange(UIAprItem item)
        {
            switch ((UIAprItem.State)item.m_stateRoot.CurrentState)
            {
                case UIAprItem.State.Lock_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.Lock_Selected:
                    OpenOperation();
                    OpenTips();
                    break;
                case UIAprItem.State.Unlock_Unselected:
                    CloseAllComponent();
                    OpenTips();
                    break;
                case UIAprItem.State.Unlock_Selected:
                    OpenOperation();
                    OpenTips();
                    break;
                case UIAprItem.State.OutTime_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.OutTime_Selected:
                    OpenOperation();
                    OpenTips();
                    break;
                default:
                    break;
            }
        }

        void CloseAllComponent()
        {
            m_ilText.gameObject.SetActive(false);
            m_compositeOper.Close();
            m_ilCompositeOper.gameObject.SetActive(false);
        }
        void OpenTips()
        {
            m_ilText.gameObject.SetActive(true);

            HairItem temp = m_hairConfig.GetHairList()[m_curItemIndex];
            string name = temp.GetName();
            int type = temp.GetFashionType();
            string des = temp.GetDes();
            string time = null;
            if(temp.m_validTime==-1)
            {
                time = "永久";
            }
            else
            {
                time = temp.m_validTime.ToString() + " 天";
            }
            m_texts.Set(name, type, des, time);
               
        }
        void OpenOperation()
        {
            m_ilCompositeOper.gameObject.SetActive(true);
            HairItem temp = m_hairConfig.GetHairList()[m_curItemIndex];
            m_compositeOper.Set(temp, OnClickUnlock);                 
        }
        //解锁,装备,卸下或续期的回调
        void OnClickUnlock()
        {
            int state = m_compositeOper.m_multiBtn.CurrentState;
            int id = m_hairConfig.GetHairList()[m_curItemIndex].m_id;
            HairItem temp = m_hairConfig.Get(id);

            switch (temp.m_state)
            {
                case AprItemState.Lock:
                    UnlockHair();
                    break;
                case AprItemState.Unlock:
                    if(temp.m_id==m_hairHandle.m_roleHairData.m_hairDressId)
                    {
                        RemoveHair();
                    }
                    else
                    {
                        WearHair();
                    }
                    break;
                case AprItemState.OutTime:
                    RenewalHair();
                    break;
                default:
                    break;
            }
        }
        void UnlockHair()
        {
            int id = 0;
            int num = 0;
            HairItem temp = m_hairConfig.GetHairList()[m_curItemIndex];

            temp.GetUnlockInfo(out id, out num);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);

            if(curOwn<num)
            {
                ItemTipsPanel.Param param = new ItemTipsPanel.Param();
                param.itemId = id;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
            }
            else
            {
                App.my.localPlayer.GetModule<PackageModule>().UseItemById(id, num);
                Item unlockItem = Item.Get(id);
                if (unlockItem == null) return;
                Debug.Log("解锁");
                UnlockRequest request = new UnlockRequest();
                request.itemId = temp.m_id;
                request.aprType = AprType.Hair;

                request.day = unlockItem.addFashTimer/ AppearanceModule.TimeRatio;
                m_hotEventagent.FireEvent<UnlockRequest>(EventID.Ap_UnlockItem, request);
            }

        }
        void RenewalHair()
        {
            int id = 0;
            int num = 0;
            HairItem temp = m_hairConfig.GetHairList()[m_curItemIndex];

            temp.GetUnlockInfo(out id, out num);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(id);

            if (curOwn < num)
            {
                ItemTipsPanel.Param param = new ItemTipsPanel.Param();
                param.itemId = id;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
            }
            else
            {
                App.my.localPlayer.GetModule<PackageModule>().UseItemById(id, num);
                Item unlockItem = Item.Get(id);
                if (unlockItem == null) return;
                RenewalRequest request = new RenewalRequest();
                request.aprType = AprType.Hair;
                request.itemId = temp.m_id;
                request.day = unlockItem.addFashTimer/ AppearanceModule.TimeRatio;//根据解锁道具而定
                m_hotEventagent.FireEvent<RenewalRequest>(EventID.Ap_RenewalItem, request);
                Debug.Log("续期");
            }
        }
        void WearHair()
        {
            int id = m_hairConfig.GetHairList()[m_curItemIndex].m_id;
            m_hairHandle.m_roleHairData.m_hairDressId = id;
            m_isChangedRole = true;
            m_compositeOper.SetState(UICompositeOper.State.Unload);
            Debug.Log("装备");
        }
        void RemoveHair()
        {
            //置为默认
            m_disguiseHandle.SetDefaultHairId();
            m_isChangedRole = true;
            m_compositeOper.SetState(UICompositeOper.State.Load);
            Debug.Log("卸下");
        }
        void RefreshUI()
        {
            SetContentData();
            OnEndSetContent();
            m_compositeOper.RefreshUI();
        }

        
    }
}
#endif
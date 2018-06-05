#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using Config;
using NetProto;
namespace xys.hot.UI
{
   
    class UIRidePage:HotTablePageBase
    {
        public UIRidePage() :base(null){ }
        public UIRidePage(HotTablePage _page) : base(_page)
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

        [SerializeField]
        StateRoot m_prompt;
        [SerializeField]
        Text m_loadRideNum;


        hot.Event.HotObjectEventSet m_hotEventagent;
        HotAppearanceModule module;  
        RideHandle m_rideHandle;
        RideConfig m_rideConfig;
        RoleTempApp m_roleTempData;


        //UI控件表
        private List<UIAprItem> m_UIItemList = new List<UIAprItem>();

        private int m_curItemIndex=-1;
        bool m_isChangedRole = false;
        protected override void OnInit()
        {
            if (m_ilText != null)
            {
                m_texts = m_ilText.GetObject() as UITexts;
            }
            if (m_ilCompositeOper != null)
            {
                m_compositeOper = m_ilCompositeOper.GetObject() as UICompositeOper;
            }
            if (m_prompt != null)
            {
                m_prompt.onStateChange.AddListener(SetPrompt);
            }

            HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
            m_hotEventagent = module.Event;

            m_rideHandle = module.GetMgr().GetRideHandle();
            m_rideConfig = m_rideHandle.m_rideConfig;

        }

        protected override void OnShow(object p)
        {
            if (m_contentGO != null)
            {
                m_contentGO.SetActive(true);
            }
            if (p == null) return;
            m_roleTempData = p as RoleTempApp;
            m_roleTempData.m_rttHandler.SetRenderActive(false);
            m_rideHandle.SetModelManager(m_roleTempData.m_rttRideHandler.GetRTT().GetModelManger());
            Event.Subscribe(EventID.AP_RefreshUI, RefreshUI);
            InitContent();
        }
        void InitContent()
        {
            int count = m_rideConfig.GetRideItemList().Count;
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
            m_rideConfig.GetRideItemList().Sort(
                (first, second) =>
                {
                    int weight = first.m_id + second.m_id;
                    int lockWeightFirst = ((int)first.m_state > 1 ? -1 : 0) * weight;
                    int lockWeightSecond = ((int)second.m_state > 1 ? -1 : 0) * weight;

                    int firstWeight = first.m_id + lockWeightFirst;
                    int secondWeight = second.m_id + lockWeightSecond;
                    return firstWeight.CompareTo(secondWeight);
                });

            int count = m_rideConfig.GetRideItemList().Count;
            for (int i = 0; i < count; i++)
            {
                RideItem temp = m_rideConfig.GetRideItemList()[i];

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
                        if(m_rideHandle.m_roleRideData.IsInLoadRideList(temp.m_id))
                        {
                            uiAprState = UIAprItem.State.Load_Unselected;
                        }
                        else
                        {
                            uiAprState = UIAprItem.State.Unlock_Unselected;
                        }
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
            if (m_roleTempData.m_rideId==-1)
            {
                return;
            }
            else
            {
                int count = m_rideConfig.GetRideItemList().Count;
                for (int i = 0; i < count; i++)
                {
                    RideItem temp = m_rideConfig.GetRideItemList()[i];
                    if (temp.m_id == m_roleTempData.m_rideId)
                    {
                        m_curItemIndex = i;
                        m_UIItemList[m_curItemIndex].m_stateRoot.NextState();
                        m_roleTempData.m_rttRideHandler.SetRenderActive(true);
                        
                        m_roleTempData.m_rttRideHandler.ReplaceModel(temp.GetMod(),(obj) =>
                        {
                            m_roleTempData.m_rttRideHandler.SetCameraState(temp.GetCameraView(), temp.GetCamarePos());
                            m_roleTempData.m_rttRideHandler.SetCamareClipPlane(100f, 0.1f);
                        });
                        
                        break;
                    }
                }
            }
        }

        protected override void OnHide()
        {
            UITextureItem.RecycleItem(m_itemRoot, m_dirtyItemRoot);
            CloseAllComponent();
            m_compositeOper.Close();
            m_roleTempData.m_rttRideHandler.SetRenderActive(false);
            if (m_isChangedRole||m_compositeOper.m_isChangedRole)
            {
                WearRequest request = new WearRequest();
                request.aprType = AprType.Ride;
                request.itemId = m_rideHandle.m_roleRideData.m_curRide;
                request.colorIndex = m_rideHandle.m_roleRideData.m_curColor;
                m_hotEventagent.FireEvent<WearRequest>(EventID.Ap_WearItem, request);

                LoadRideReq loadRequest = new LoadRideReq();
                loadRequest.loadRideList.AddRange(m_rideHandle.m_roleRideData.GetLoadedRideList());
                m_hotEventagent.FireEvent(EventID.Ap_LoadRide, loadRequest);
                m_isChangedRole = false;
                m_compositeOper.m_isChangedRole = false;

       
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
                RideItem temp = m_rideConfig.GetRideItemList()[m_curItemIndex];
                m_roleTempData.m_rideId = temp.m_id;
                item.m_stateRoot.NextState();
                m_roleTempData.m_rttRideHandler.SetRenderActive(true);
                
                m_roleTempData.m_rttRideHandler.ReplaceModel(temp.GetMod(),(obj) =>
                {
                    m_roleTempData.m_rttRideHandler.SetCameraState(temp.GetCameraView(), temp.GetCamarePos());
                    m_roleTempData.m_rttRideHandler.SetCamareClipPlane(100f, 0.1f);
                });
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
                    OpenTips();
                    OpenOperation();
                    break;
                case UIAprItem.State.Unlock_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.Unlock_Selected:
                    OpenTips();
                    OpenOperation();
                    break;
                case UIAprItem.State.OutTime_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.OutTime_Selected:
                    OpenTips();
                    OpenOperation();
                    break;
                case UIAprItem.State.Load_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.Load_Selected:
                    OpenTips();
                    OpenOperation();
                    break;
                default:
                    break;
            }
        }

        void CloseAllComponent()
        {
            m_ilText.gameObject.SetActive(false);
            m_ilCompositeOper.gameObject.SetActive(false);
        }
        void SetPrompt()
        {
            int curLoadRide = m_rideHandle.m_roleRideData.GetLoadedRideList().Count;
            int maxLoad = 4;
            m_loadRideNum.text = curLoadRide.ToString() + "/" + maxLoad.ToString();
        }
        void OpenTips()
        {
            m_ilText.gameObject.SetActive(true);
            RideItem temp = m_rideConfig.GetRideItemList()[m_curItemIndex];
            string name = temp.GetName();
            string des = temp.GetDes();
            string time = null;
            if (temp.m_validTime == -1)
            {
                time = "永久";
            }
            else
            {
                time = temp.m_validTime.ToString() + " 天";
            }
            m_texts.Set(name, -1, des, time);
        }
        void OpenOperation()
        {
            m_ilCompositeOper.gameObject.SetActive(true);
            RideItem temp = m_rideConfig.GetRideItemList()[m_curItemIndex];
            m_compositeOper.Set(temp, OnClickMultiBtn);
        }

        void OnClickMultiBtn()
        {
            RideItem temp = m_rideConfig.GetRideItemList()[m_curItemIndex];

            switch (temp.m_state)
            {
                case AprItemState.Lock:
                    Unlock(temp);
                    break;
                case AprItemState.Unlock:                   
                    if(!temp.m_unlockedColorList.Contains(temp.m_curColor))//颜色未解锁
                    {
                        UnlockRideColor(temp);
                    }
                    else if (m_rideHandle.m_roleRideData.m_curRide == -1)//当前未骑乘坐骑，骑乘
                    {
                        Wear(temp);
                    }
                    else if (m_rideHandle.m_roleRideData.IsInLoadRideList(m_roleTempData.m_rideId))//已骑乘的坐骑，显示卸下
                    {
                        Remove();
                    }
                    else//点击非骑乘坐骑，显示装备
                    {
                        RandomRide(temp);
                    }
                    break;
                case AprItemState.OutTime:
                    Renewal(temp);
                    break;
                default:
                    break;
            }
        }
        void Unlock(RideItem item)
        {        
            if(item.m_state==AprItemState.Lock&& item.m_curColor==0)//解锁坐骑
            {
                int id = 0;
                int num = 0;
                item.GetUnlockInfo(0, out id, out num);
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
                    request.itemId = item.m_id;
                    request.aprType = AprType.Ride;
                    request.day =unlockItem.addFashTimer/ AppearanceModule.TimeRatio;//待定
                    m_hotEventagent.FireEvent<UnlockRequest>(EventID.Ap_UnlockItem, request);
                }
            }
            else
            {
                SystemHintMgr.ShowTipsHint(7604);
            }          
        }

        void UnlockRideColor(RideItem item)
        {
            int id = 0;
            int num = 0;
            item.GetUnlockInfo(item.m_curColor, out id, out num);
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

                UnlockRideColorReq request = new UnlockRideColorReq();
                request.itemId = item.m_id;
                request.colorIndex = item.m_curColor;
                m_hotEventagent.FireEvent<UnlockRideColorReq>(EventID.Ap_UnlockRideColor, request);
            }
        }
        void Renewal(RideItem item)
        {
            int id = 0;
            int num = 0;
            item.GetUnlockInfo(0, out id, out num);
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

                RenewalRequest request = new RenewalRequest();
                request.aprType = AprType.Ride;
                request.itemId = item.m_id;
                request.day = unlockItem.addFashTimer/ AppearanceModule.TimeRatio;//根据解锁道具而定
                m_hotEventagent.FireEvent<RenewalRequest>(EventID.Ap_RenewalItem, request);
                Debug.Log("续期");
            }
        }
        //骑乘
        void Wear(RideItem item)
        {
            m_rideHandle.m_roleRideData.m_curRide = m_roleTempData.m_rideId;
            m_rideHandle.m_roleRideData.m_curColor = item.m_curColor;
            m_isChangedRole = true;
           
            if(!m_rideHandle.m_roleRideData.IsInLoadRideList(m_roleTempData.m_rideId))
            {
                m_rideHandle.m_roleRideData.AddLoadRide(m_roleTempData.m_rideId, item.m_curColor);
            }     
            m_UIItemList[m_curItemIndex].SetState(UIAprItem.State.Load_Selected, false);
            m_compositeOper.SetState(UICompositeOper.State.Unload);
        }
        //卸下
        void Remove()
        {
            m_rideHandle.m_roleRideData.RemoveLoadRideById(m_roleTempData.m_rideId);
            if (m_roleTempData.m_rideId == m_rideHandle.m_roleRideData.m_curRide)
            {
                int count = m_rideHandle.m_roleRideData.GetLoadedRideList().Count;
                if (count > 0)
                {
                    //骑乘最后一个装备的坐骑
                    m_rideHandle.m_roleRideData.m_curRide = m_rideHandle.m_roleRideData.GetLoadedRideList()[count - 1].rideStyleId;
                    m_compositeOper.SetState(UICompositeOper.State.Load);
                }
                else
                {
                    //不骑乘
                    m_rideHandle.m_roleRideData.m_curRide = -1;
                    m_compositeOper.SetState(UICompositeOper.State.Ride);
                }
            }
            m_UIItemList[m_curItemIndex].SetState(UIAprItem.State.Unlock_Selected, false);
            m_isChangedRole = true;
        }
        //装备
        void RandomRide(RideItem temp)
        {
            if(m_rideHandle.m_roleRideData.GetLoadedRideList().Count<4)
            {
                m_rideHandle.m_roleRideData.AddLoadRide(m_roleTempData.m_rideId, temp.m_curColor);
                m_isChangedRole = true;
                m_compositeOper.SetState(UICompositeOper.State.Unload);
                m_UIItemList[m_curItemIndex].SetState(UIAprItem.State.Load_Selected, false);
            }   
            else
            {
                SystemHintMgr.ShowTipsHint(7605);
            }        
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
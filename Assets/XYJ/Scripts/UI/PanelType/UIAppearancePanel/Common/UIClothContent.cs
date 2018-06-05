#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;
using Config;
using NetProto;
using System;
namespace xys.hot.UI
{
    [AutoILMono]
    class UIClothContent
    {
        [SerializeField]
        GameObject m_contentGO;
        [SerializeField]
        Transform m_itemRoot;
        [SerializeField]
        Transform m_dirtyItemRoot;

        [SerializeField]
        ILMonoBehaviour m_ilUnlock;
        UIUnlock m_unlock;

        [SerializeField]
        ILMonoBehaviour m_ilOperation;
        UIOperation m_operation;

        [SerializeField]
        ILMonoBehaviour m_ilText;
        UITexts m_texts;

        //分页事件注册工具
         public hot.Event.HotObjectEventAgent m_eventAgent;
        //模块事件注册工具
        Event.HotObjectEventSet m_hotEventAgent;

        HotAppearanceModule module;
        ClothHandle m_clothHandle;
        ClothConfig m_clothConfig;
        RoleDisguiseHandle m_disguiseHandle;

        //UI控件表
        private List<UIAprItem> m_UIItemList = new List<UIAprItem>();
        //当前选中的服装，默认为当前角色穿着的服装
        public int m_curItemIndex = -1;
        public RoleTempApp m_roleTempData = null;
   
        void Awake()
        {
            if (m_ilUnlock != null)
            {
                m_unlock = m_ilUnlock.GetObject() as UIUnlock;
            }
            if (m_ilOperation != null)
            {
                m_operation = m_ilOperation.GetObject() as UIOperation;
            }
            if (m_ilText != null)
            {
                m_texts = m_ilText.GetObject() as UITexts;
            }
            module = hotApp.my.GetModule<HotAppearanceModule>();
            m_hotEventAgent = module.Event;
            m_clothHandle = module.GetMgr().GetClothHandle();
            m_clothConfig = m_clothHandle.m_clothConfig;
            m_disguiseHandle= module.GetMgr().GetDisguiHandle();

        }

        public void OpenContent()
        {
            if (m_contentGO != null)
            {
                m_contentGO.SetActive(true);
            }
            if(m_eventAgent!=null)
            {
                m_eventAgent.Subscribe(EventID.AP_RefreshUI, RefreshUI);
            }
            InitContent();
        }
        void InitContent()
        {
            int count = m_clothHandle.m_clothConfig.GetClothList().Count;
            UITextureItem.AddNullItem(count, m_itemRoot, m_dirtyItemRoot);
            SetContentData();
            OnEndSetContent();
        }

        void SetContentData()
        {
            CloseAllComponent();
            m_UIItemList.Clear();
            //物品排序
            //解锁-按ID排序
            //未解锁-按ID排序
            m_clothConfig.GetClothList().Sort(
                (first, second) =>
                {
                    int weight = first.m_id + second.m_id;
                    int lockWeightFirst = ((int)first.m_state > 1 ? -1 : 0) * weight;
                    int lockWeightSecond = ((int)second.m_state > 1 ? -1 : 0) * weight;

                    int firstWeight = first.m_id + lockWeightFirst;
                    int secondWeight = second.m_id + lockWeightSecond;
                    return firstWeight.CompareTo(secondWeight);
                });

            int count = m_clothConfig.GetClothList().Count;
            for (int i = 0; i < count; i++)
            {
                ClothItem temp = m_clothConfig.GetClothList()[i];

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
                //设置内容
                item.Set(i, uiAprState, temp.GetIconName(), OnClickItem, OnItemStateChange);
                m_UIItemList.Add(item);
            }
        }

        void OnEndSetContent()
        {
     
            //首次打开
            if(m_roleTempData.m_clothId==-1)
            {
                return;
            }
            else
            {
                int count = m_clothConfig.GetClothList().Count;
                for (int i = 0; i < count; i++)
                {
                    if (m_clothConfig.GetClothList()[i].m_id == m_roleTempData.m_clothId)
                    {
                        m_curItemIndex = i;
                        m_UIItemList[m_curItemIndex].m_stateRoot.NextState() ;

                        ClothItem tempCloth = m_clothConfig.Get(m_roleTempData.m_clothId);
                        m_disguiseHandle.SetClothById(tempCloth.m_id, tempCloth.m_curColor);
                        return;
                    }
                }
            }    
        }

        public void CloseContent()
        {
            if(m_contentGO!=null)
            {
                m_contentGO.SetActive(false);
            }
            CloseAllComponent();
            UITextureItem.RecycleItem(m_itemRoot, m_dirtyItemRoot);
            m_UIItemList.Clear();
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
                m_roleTempData.m_clothId = m_clothConfig.GetClothList()[m_curItemIndex].m_id;
                item.m_stateRoot.NextState();
                ClothItem tempCloth = m_clothConfig.Get(m_roleTempData.m_clothId);
                m_disguiseHandle.SetClothById(tempCloth.m_id, tempCloth.m_curColor);
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
                    InitUnlock();
                    OpenTexts();
                    break;
                case UIAprItem.State.Unlock_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.Unlock_Selected:
                    OpenOperation();
                    OpenTexts();
                    break;
                case UIAprItem.State.OutTime_Unselected:
                    CloseAllComponent();
                    break;
                case UIAprItem.State.OutTime_Selected:
                    InitUnlock();
                    OpenTexts();
                    break;
                default:
                    break;
            }
        }
        void CloseAllComponent()
        {
            m_ilOperation.gameObject.SetActive(false);
            m_ilText.gameObject.SetActive(false);
            m_ilUnlock.gameObject.SetActive(false);
        }
        void RefreshUI()
        {
            if (m_UIItemList.Count == 0) return;
            SetContentData();
            OnEndSetContent();
        }
        void OpenTexts()
        {
            ClothItem temp = m_clothConfig.GetClothList()[m_curItemIndex];
            string name = temp.GetName();
            int type = temp.GetFashionType();
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

            m_texts.Set(name, type, des, time);
            m_ilText.gameObject.SetActive(true);
        }

        void OpenOperation()
        {
            m_ilOperation.gameObject.SetActive(true);
            m_operation.m_clothItem = m_clothConfig.Get(m_roleTempData.m_clothId);   
            m_operation.Set();
        }

        void InitUnlock()
        {
            ClothItem temp = m_clothConfig.GetClothList()[m_curItemIndex];
            int unlockItem;
            int unlockItemNum;
            temp.GetUnlockInfo(out unlockItem, out unlockItemNum);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(unlockItem);
            switch (temp.m_state)
            {
                case AprItemState.Lock:
                    m_unlock.Set(0, unlockItem, curOwn, unlockItemNum, OnClickUnlock);
                    break;
                case AprItemState.OutTime:
                    m_unlock.Set(1, unlockItem, curOwn, unlockItemNum, OnClickUnlock);
                    break;
                default:
                    break;
            }

            m_ilUnlock.gameObject.SetActive(true);
        }
        //解锁或续期的回调
        void OnClickUnlock()
        {
            ClothItem temp = m_clothConfig.GetClothList()[m_curItemIndex];
            switch (temp.m_state)
            {
                case AprItemState.Lock:
                    UnlockItem(temp);
                    break;
                case AprItemState.OutTime:
                    RenewalItem(temp);
                    break;
                default:
                    break;
            }
        }
        void UnlockItem(ClothItem temp)
        {  
            int needItem = 0;
            int needNum = 0;
            temp.GetUnlockInfo(out needItem, out needNum);

            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(needItem);//
            //根据背包材料数量确定是否解锁
            if (curOwn<needNum)
            {
                //弹出获取途径面板
                ItemTipsPanel.Param param = new ItemTipsPanel.Param();
                param.itemId = needItem;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
                Debug.Log("材料数量不够");
            }
            else
            {
                App.my.localPlayer.GetModule<PackageModule>().UseItemById(needItem, needNum);
                Item unlockItem = Item.Get(needItem);
                if (unlockItem == null) return;
                UnlockRequest request = new UnlockRequest();
                request.itemId = temp.m_id;
                request.day = unlockItem.addFashTimer/AppearanceModule.TimeRatio;
                m_hotEventAgent.FireEvent<UnlockRequest>(EventID.Ap_UnlockItem, request);
                SystemHintMgr.ShowTipsHint(7600);
            }            
        }

        void RenewalItem(ClothItem temp)
        { 
            int needItem = 0;
            int needNum = 0;
            temp.GetUnlockInfo(out needItem, out needNum);
            int curOwn = App.my.localPlayer.GetModule<PackageModule>().GetItemCount(needItem);//
            //根据背包材料数量确定是否解锁
            if (curOwn < needNum)
            {
                //弹出获取途径面板
                ItemTipsPanel.Param param = new ItemTipsPanel.Param();
                param.itemId = needItem;
                App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
            }
            else
            {  
                App.my.localPlayer.GetModule<PackageModule>().UseItemById(needItem, needNum);
                Item unlockItem = Item.Get(needItem);
                if (unlockItem == null) return;
                RenewalRequest request = new RenewalRequest();
                request.itemId = temp.m_id;
                request.day = unlockItem.addFashTimer/ AppearanceModule.TimeRatio;
                m_hotEventAgent.FireEvent<RenewalRequest>(EventID.Ap_RenewalItem, request);
            }
        }
    }

}

#endif
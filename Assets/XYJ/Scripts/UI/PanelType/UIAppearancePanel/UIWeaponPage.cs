#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using Config;
using xys.UI.State;
using NetProto;
namespace xys.hot.UI
{
   
    class UIWeaponPage : HotTablePageBase
    {
        public UIWeaponPage():base(null) { }
        public UIWeaponPage(HotTablePage _page) : base(_page)
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
        AppearanceMgr m_appearanceMgr;
        WeaponHandle m_weaponHandle;
        WeaponConfig m_weaponConfig;
        RoleDisguiseHandle m_disguiseHandle;
      
        //UI控件表
        private List<UIAprItem> m_UIItemList = new List<UIAprItem>();

        bool m_isChangedRole = false;
        private int m_curItemIndex=-1;
        RoleTempApp m_roleTempData = null;
        protected override void OnInit()
        {
            if (m_ilCompositeOper != null)
            {
                m_compositeOper = m_ilCompositeOper.GetObject() as UICompositeOper;
            }
            if (m_ilText != null)
            {
                m_texts = m_ilText.GetObject() as UITexts;
            }

            HotAppearanceModule module = hotApp.my.GetModule<HotAppearanceModule>();
            m_hotEventagent = module.Event;
            m_weaponHandle = module.GetMgr().GetWeaponHandle() ;
            m_weaponConfig = m_weaponHandle.m_weaponConfig;
            m_disguiseHandle= module.GetMgr().GetDisguiHandle();
        }

        protected override void OnShow(object p)
        {
            if (m_contentGO != null)
            {
                m_contentGO.SetActive(true);
            }
            if (p == null) return;
            m_roleTempData = p as RoleTempApp;
            m_roleTempData.m_rttHandler.SetRenderActive(true);
            Event.Subscribe(EventID.AP_RefreshUI, RefreshUI);
            InitContent();
        }

        void InitContent()
        {
            int count = m_weaponConfig.GetWeaponList().Count;
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
            m_weaponConfig.GetWeaponList().Sort(
                (first, second) =>
                {
                    int weight = first.m_id + second.m_id;
                    int lockWeightFirst = ((int)first.m_state > 1 ? -1 : 0) * weight;
                    int lockWeightSecond = ((int)second.m_state > 1 ? -1 : 0) * weight;

                    int firstWeight = first.m_id + lockWeightFirst;
                    int secondWeight = second.m_id + lockWeightSecond;
                    return firstWeight.CompareTo(secondWeight);
                });

            int count = m_weaponConfig.GetWeaponList().Count;
            for (int i = 0; i < count; i++)
            {
                WeaponItem temp = m_weaponConfig.GetWeaponList()[i];

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
                item.Set(i, uiAprState, temp.GetIconName(1), OnClickItem, OnItemStateChange);
                m_UIItemList.Add(item);
            }
        }
        void OnEndSetContent()
        {
            
            if(m_roleTempData.m_weaponId==-1)
            {
                m_curItemIndex = 0;
            }
            else
            {
                int count = m_weaponConfig.GetWeaponList().Count;
                for (int i = 0; i < count; i++)
                {
                    WeaponItem temp = m_weaponConfig.GetWeaponList()[i];
                    if (temp.m_id == m_roleTempData.m_weaponId)
                    {
                        m_curItemIndex = i;
                        m_UIItemList[m_curItemIndex].m_stateRoot.NextState(); 
                        break;
                    }
                }
            }      
        }
        protected override void OnHide()
        {
            UITextureItem.RecycleItem(m_itemRoot, m_dirtyItemRoot);
            CloseAllComponent();
            m_UIItemList.Clear();
            m_compositeOper.Close();
            if(m_isChangedRole)
            {
                WearRequest request = new WearRequest();
                request.aprType = AprType.Weapon;
                request.itemId = m_weaponHandle.m_roleWeaponData.m_weaponId;
                request.colorIndex = m_weaponHandle.m_roleWeaponData.m_curEffect;
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
                m_roleTempData.m_weaponId = m_weaponConfig.GetWeaponList()[m_curItemIndex].m_id;
                item.m_stateRoot.NextState();

                m_disguiseHandle.SetWeaponById(m_roleTempData.m_weaponId);
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
            m_ilCompositeOper.gameObject.SetActive(false);
            m_compositeOper.Close();
            m_ilText.gameObject.SetActive(false);
        }
        void OpenTips()
        {
            m_ilText.gameObject.SetActive(true);
            WeaponItem temp = m_weaponConfig.GetWeaponList()[m_curItemIndex];
            string name = temp.GetName(1);
            string des = temp.GetDes(1);
            string time = null;
            if(temp.m_validTime==-1)
            {
                time = "永久";
            }
            else
            {
                time = temp.m_validTime.ToString() + " 天";
            }
            m_texts.Set(name,-1,des,time);
        }
        void OpenOperation()
        {
            m_ilCompositeOper.gameObject.SetActive(true);
            WeaponItem temp = m_weaponConfig.GetWeaponList()[m_curItemIndex];
            m_compositeOper.Set(temp, OnClickMultiBtn);           
        }

        void OnClickMultiBtn()
        {
            WeaponItem temp = m_weaponConfig.GetWeaponList()[m_curItemIndex];
            switch (temp.m_state)
            {
                case AprItemState.Lock:
                    Unlock(temp);
                    break;
                case AprItemState.Unlock:
                    if(m_weaponConfig.Get(m_roleTempData.m_weaponId).m_curEffect >= temp.m_maxEffect)
                    {
                        SystemHintMgr.ShowTipsHint(7604);
                    }
                    else if(m_roleTempData.m_weaponId==m_weaponHandle.m_roleWeaponData.m_weaponId)
                    {
                        Remove();
                    }
                    else
                    {
                        Wear();
                    }
                    break;
                case AprItemState.OutTime:
                    Renewal(temp);
                    break;
                default:
                    break;
            }
        }

        void Unlock(WeaponItem item)
        {
            if(item.m_state==AprItemState.Lock&&m_weaponConfig.Get(m_roleTempData.m_weaponId).m_curEffect==0)
            {
                int id = 0;
                int num = 0;
                item.GetUnlockInfo(1, out id, out num);
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
                    request.aprType = AprType.Weapon;
                    request.day = unlockItem.addFashTimer/AppearanceModule.TimeRatio;//待定
                    m_hotEventagent.FireEvent<UnlockRequest>(EventID.Ap_UnlockItem, request);
                }
            }
            else
            {
                //解锁条件未达成
                SystemHintMgr.ShowTipsHint(7604);
            } 
        }
        void Renewal(WeaponItem item)
        {
            int id = 0;
            int num = 0;
            item.GetUnlockInfo(1, out id, out num);
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
                request.aprType = AprType.Weapon;
                request.itemId = item.m_id;
                request.day = unlockItem.addFashTimer/ AppearanceModule.TimeRatio;//根据解锁道具而定
                m_hotEventagent.FireEvent<RenewalRequest>(EventID.Ap_RenewalItem, request);
                Debug.Log("续期");
            }   
        }
        void Wear()
        {
            m_weaponHandle.m_roleWeaponData.m_curEffect = m_weaponConfig.Get(m_roleTempData.m_weaponId).m_curEffect;
            m_weaponHandle.m_roleWeaponData.m_weaponId = m_roleTempData.m_weaponId;
            m_isChangedRole = true;
            m_compositeOper.SetState(UICompositeOper.State.Unload);
        }
        void Remove()
        {
            m_disguiseHandle.SetDefaultWeapon();
            m_isChangedRole = true;
            m_compositeOper.SetState(UICompositeOper.State.Load);
        }

        void RefreshUI()
        {
            if (m_UIItemList.Count == 0) return;
            SetContentData();
            OnEndSetContent();
            m_compositeOper.RefreshUI();
        }

    }
}
#endif
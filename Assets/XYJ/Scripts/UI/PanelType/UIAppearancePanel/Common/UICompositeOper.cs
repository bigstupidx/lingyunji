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
    class UICompositeOper
    {
        public enum State
        {
            Load,//穿戴，骑乘等
            Unload,//卸下，恢复初始值
            Ride,//坐骑装备，用于随机骑乘
            UnLock,//解锁
            Renewal,//续期
        }
        [SerializeField]
        GameObject m_colorRoot;
        [SerializeField]
        GameObject m_conditionRoot;
        [SerializeField]
        GameObject m_level;
        [SerializeField]
        GameObject m_material;

        [SerializeField]
        public  StateRoot m_multiBtn;

        [SerializeField]
        Text m_levelInfo;
        [SerializeField]
        Text m_materialName;
        [SerializeField]
        Text m_materialNum;

        public List<UIColorBtn> m_colorBtnList = new List<UIColorBtn>();
        private System.Action m_clickMultiBtnEvent = null;
        private object m_appItem;

        PackageModule pakageModule ;

        HotAppearanceModule module ;
        AppearanceMgr m_appearanceMgr;
        HairHandle m_hairHandle;
        WeaponHandle m_weaponHandle;
        RideHandle m_rideHandle;

        public bool m_isChangedRole = false;
        void Awake()
        {
            if (m_multiBtn != null)
            {
                m_multiBtn.onClick.AddListener(OnClick);
            }
            if (m_colorRoot != null)
            {
                Transform temp = m_colorRoot.transform;
                int count = temp.childCount;
                for (int i = 0; i < count; i++)
                {
                    Transform tempTran = temp.GetChild(i);
                    if (tempTran == null) return;
                    ILMonoBehaviour tempIL = tempTran.GetComponent<ILMonoBehaviour>();
                    tempIL.gameObject.SetActive(false);
                    if (tempIL == null) return;
                    UIColorBtn tempItem = tempIL.GetObject() as UIColorBtn;
                    m_colorBtnList.Add(tempItem);
                }
            }
            module = hotApp.my.GetModule<HotAppearanceModule>();
            pakageModule = App.my.localPlayer.GetModule<PackageModule>();
            m_appearanceMgr = module.GetMgr();
            m_hairHandle = m_appearanceMgr.GetHairHandle();
            m_weaponHandle = m_appearanceMgr.GetWeaponHandle();
            m_rideHandle = m_appearanceMgr.GetRideHandle();
        }
        public void Set(object arg, System.Action clickMultiBtnEvent)
        {
            if (arg == null) return;
            m_appItem = arg;
            m_clickMultiBtnEvent = null;
            m_clickMultiBtnEvent = clickMultiBtnEvent;
            OpenOper();
        }

        public void OpenOper()
        {
            if (m_appItem is HairItem)
            {
                HairItem temp = m_appItem as HairItem;
                Set(temp);
            }
            else if (m_appItem is WeaponItem)
            {
                WeaponItem temp = m_appItem as WeaponItem;
                Set(temp);
            }
            else if (m_appItem is RideItem)
            {
                RideItem temp = m_appItem as RideItem;
                Set(temp);
            }
        }


        public void OpenLevelInfo(int level)
        {
            if (level == 0) return;
            m_conditionRoot.SetActive(true);
            m_level.SetActive(true);
            m_levelInfo.text = "装备强化至" + "<color=#ff0000ff>" + level.ToString() + "级" + "</color>" + "后解锁";
        }
        public void OpenMaterialInfo(int id, int own, int need)
        {
            m_conditionRoot.SetActive(true);
            m_material.SetActive(true);
            Item tempItem = Item.Get(id);
            if (tempItem != null)
            {
                m_materialName.text = "消耗：" + tempItem.name;
            }   
            if (own < need)
            {
                m_materialNum.color = Color.red;
            }
            else
            {
                m_materialNum.color = Color.green;
            }
            m_materialNum.text = own.ToString() + "/" + need.ToString();
        }
        public void OpenPreCondition(string name)
        {
            m_conditionRoot.SetActive(true);
            m_material.SetActive(true);
            m_materialName.text="解锁外观：" + name;
        }
        public void CloseComponent()
        {
            m_conditionRoot.SetActive(false);
            m_level.SetActive(false);
            m_material.SetActive(false);
        }
        public void Close()
        {
            for(int i=0;i<m_colorRoot.transform.childCount;i++)
            {
                m_colorRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
            m_colorRoot.SetActive(false);
            m_conditionRoot.SetActive(false);
            m_level.SetActive(false);
            m_material.SetActive(false);

        }
        void Set(HairItem item)
        {
            m_colorRoot.SetActive(false);
            m_level.SetActive(false);
            switch (item.m_state)
            {
                case AprItemState.Lock:
                    int id = 0;
                    int need = 0;
                    item.GetUnlockInfo(out id, out need);
                    int curOwn = pakageModule.GetItemCount(id);
                    OpenMaterialInfo(id, curOwn, need);
                    m_multiBtn.SetCurrentState((int)State.UnLock, false);
                    break;
                case AprItemState.Unlock:                
                    if(item.m_id!=m_hairHandle.m_roleHairData.m_hairDressId)
                    {
                        m_multiBtn.SetCurrentState((int)State.Load, false);
                    }
                    else
                    {
                        m_multiBtn.SetCurrentState((int)State.Unload, false);
                    }                 
                    break;
                case AprItemState.OutTime:   
                    int renewalId = 0;
                    int renewalNeed = 0;
                    item.GetUnlockInfo(out renewalId, out renewalNeed);
                    int curOwnRenewal = pakageModule.GetItemCount(renewalId);
                    OpenMaterialInfo(renewalId, curOwnRenewal, renewalNeed);
                    m_multiBtn.SetCurrentState((int)State.Renewal, false);
                    break;
                default:
                    break;
            }
        }
        void Set(WeaponItem item)
        {
            m_colorRoot.SetActive(true);
            switch (item.m_state)
            {
                case AprItemState.Lock:                   
                    for (int i = 0; i < item.m_keyCount; i++)
                    {
                        m_colorBtnList[i].Set(i, UIColorBtn.State.Lock_Unselected, item.GetEffectColor(i+1), OnClickItem, OnStateChange);
                        m_colorBtnList[i].m_stateRoot.gameObject.SetActive(true);
                    }
                   
                    m_colorBtnList[item.m_curEffect].m_stateRoot.NextState();

                    int id = 0;
                    int need = 0;
                    item.GetUnlockInfo(1,out id, out need);
                    int curOwn = pakageModule.GetItemCount(id);
                    OpenMaterialInfo(id, curOwn, need);
                    m_multiBtn.SetCurrentState((int)State.UnLock, false);
                    break;
                case AprItemState.Unlock:
                    for (int i = 0; i < item.m_keyCount; i++)
                    {
                        if (i < item.m_maxEffect)
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Unlock_Unselected, item.GetEffectColor( i + 1), OnClickItem, OnStateChange);
                        }
                        else
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Lock_Unselected, item.GetEffectColor(i + 1), OnClickItem, OnStateChange);
                        }
                        m_colorBtnList[i].m_stateRoot.gameObject.SetActive(true);
                    }
                    if(item.m_id==m_weaponHandle.m_roleWeaponData.m_weaponId&&
                       item.m_curEffect== m_weaponHandle.m_roleWeaponData.m_curEffect )
                    {
                        m_multiBtn.SetCurrentState((int)State.Unload, false);
                    }
                    else
                    {
                        m_multiBtn.SetCurrentState((int)State.Load, false);
                    }

                    m_colorBtnList[item.m_curEffect].m_stateRoot.NextState();
                    break;
                case AprItemState.OutTime:
                    for (int i = 0; i < item.m_keyCount; i++)
                    {
                        if (i < item.m_maxEffect)
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Unlock_Unselected, item.GetEffectColor(i + 1), OnClickItem, OnStateChange);
                        }
                        else
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Lock_Unselected, item.GetEffectColor(i + 1), OnClickItem, OnStateChange);
                        }
                        m_colorBtnList[i].m_stateRoot.gameObject.SetActive(true);
                    }

                    m_colorBtnList[item.m_curEffect].m_stateRoot.NextState();

                    int id_1 = 0;
                    int need_1 = 0;
                    item.GetUnlockInfo(1, out id_1, out need_1);
                    int curOwn_1 = pakageModule.GetItemCount(id_1);
                    OpenMaterialInfo(id_1, curOwn_1, need_1);

                    m_multiBtn.SetCurrentState((int)State.Renewal, false);
                    break;
                default:
                    break;
            }
        }
        void Set(RideItem item)
        {
            m_colorRoot.SetActive(true);
            switch (item.m_state)
            {
                case AprItemState.Lock:  
                    for (int i = 0; i < item.m_keyCount; i++)
                    {
                        m_colorBtnList[i].Set(i, UIColorBtn.State.Lock_Unselected, item.GetColor(i), OnClickItem, OnStateChange);
                        m_colorBtnList[i].m_stateRoot.gameObject.SetActive(true);
                    }
                  
                    m_colorBtnList[item.m_curColor].m_stateRoot.NextState();

                    int id = 0;
                    int need = 0;
                    item.GetUnlockInfo(0, out id, out need);
                    int curOwn = pakageModule.GetItemCount(id);
                    OpenMaterialInfo(id, curOwn, need);
                    m_multiBtn.SetCurrentState((int)State.UnLock, false);
                    break;
                case AprItemState.Unlock:
                    for (int i = 0; i < item.m_keyCount; i++)
                    {
                        if (item.m_unlockedColorList.Contains(i))
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Unlock_Unselected, item.GetColor(i), OnClickItem, OnStateChange);
                        }
                        else
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Lock_Unselected, item.GetColor(i), OnClickItem, OnStateChange);
                        }
                        m_colorBtnList[i].m_stateRoot.gameObject.SetActive(true);
                    }

                    if (m_rideHandle.m_roleRideData.m_curRide == -1)//当前未骑乘坐骑，显示骑乘
                    {
                        m_multiBtn.SetCurrentState((int)State.Ride, false);
                    }
                    else if (m_rideHandle.m_roleRideData.IsInLoadRideList(item.m_id))//已装备的坐骑，显示卸下
                    {
                        m_multiBtn.SetCurrentState((int)State.Unload, false);
                    }
                    else//非装备坐骑，显示装备
                    {
                        m_multiBtn.SetCurrentState((int)State.Load, false);
                    }

                    m_colorBtnList[item.m_curColor].m_stateRoot.NextState();
                    break;
                case AprItemState.OutTime:
                    for (int i = 0; i < item.m_keyCount; i++)
                    {
                        if (item.m_unlockedColorList.Contains(i))
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Unlock_Unselected, item.GetColor(i), OnClickItem, OnStateChange);
                        }
                        else
                        {
                            m_colorBtnList[i].Set(i, UIColorBtn.State.Lock_Unselected, item.GetColor(i), OnClickItem, OnStateChange);
                        }
                        m_colorBtnList[i].m_stateRoot.gameObject.SetActive(true);
                    }
                    m_colorBtnList[item.m_curColor].m_stateRoot.NextState();

                    int id_1 = 0;
                    int need_1 = 0;
                    item.GetUnlockInfo(0, out id_1, out need_1);
                    int curOwn_1 = pakageModule.GetItemCount(id_1);
                    OpenMaterialInfo(id_1, curOwn_1, need_1);

                    m_multiBtn.SetCurrentState((int)State.Renewal, false);
                    break;
                default:
                    break;
            }
        }
        void OnClick()
        {
            if (m_clickMultiBtnEvent != null)
            {
                m_clickMultiBtnEvent();
            }
        }

        void OnClickItem(UIColorBtn item)
        {
            if(m_appItem is WeaponItem)
            {
                WeaponItem weaponItem = m_appItem as WeaponItem;
                if (weaponItem.m_curEffect == item.m_index)
                {
                    return;
                }
                else
                {
                    m_colorBtnList[weaponItem.m_curEffect].m_stateRoot.FrontState();
                    weaponItem.m_curEffect = item.m_index;
                    item.m_stateRoot.NextState();
                }
            }
            else if(m_appItem is RideItem)
            {
                RideItem rideItem = m_appItem as RideItem;
                if (rideItem.m_curColor == item.m_index)
                {
                    return;
                }
                else
                {
                    m_colorBtnList[rideItem.m_curColor].m_stateRoot.FrontState();
                    rideItem.m_curColor = item.m_index;
                    LoadedRide temp = m_rideHandle.m_roleRideData.GetLoadRideById(rideItem.m_id);
                    if(temp !=null)
                    {
                        temp.curColor = rideItem.m_curColor;
                        m_isChangedRole = true;
                    }
                    item.m_stateRoot.NextState();
                    string newMatName = rideItem.GetMatNameByIndex(rideItem.m_curColor);
                    m_rideHandle.ChangeRideMaterial(newMatName);
                }
            }           
        }
        void OnStateChange(UIColorBtn item)
        {
            SetCommonTab(item);
        }

        void SetCommonTab(UIColorBtn item)
        {
            if (m_appItem is WeaponItem)
            {
                SetTab((WeaponItem)m_appItem, item);
            }
            else if (m_appItem is RideItem)
            {
                SetTab((RideItem)m_appItem, item);
            }
        }
        void SetTab(WeaponItem temp, UIColorBtn item)
        {
            switch ((UIColorBtn.State)item.m_stateRoot.CurrentState)
            {
                case UIColorBtn.State.Null_Color:
                    break;
                case UIColorBtn.State.Unlock_Unselected:
                    CloseComponent();
                    break;
                case UIColorBtn.State.Unlock_Selected:
                    if (temp.m_state == AprItemState.OutTime)
                    {
                        int id = 0;
                        int need = 0;
                        temp.GetUnlockInfo(1, out id, out need);
                        int curOwn = pakageModule.GetItemCount(id);
                        OpenMaterialInfo(id, curOwn, need);
                        SetState(State.Renewal);
                    }
                    else
                    {
                        if (item.m_index != m_weaponHandle.m_roleWeaponData.m_curEffect ||
                        temp.m_id != m_weaponHandle.m_roleWeaponData.m_weaponId)
                        {
                            SetState(State.Load);
                        }
                        else
                        {
                            SetState(State.Unload);
                        }
                    }
                    break;
                case UIColorBtn.State.Lock_Unselected:
                    CloseComponent();
                    break;
                case UIColorBtn.State.Lock_Selected:
                    if(temp.m_state==AprItemState.OutTime)
                    {
                        int id = 0;
                        int need = 0;
                        temp.GetUnlockInfo(1, out id, out need);
                        int curOwn = pakageModule.GetItemCount(id);
                        OpenMaterialInfo(id, curOwn, need);
                        SetState(State.Renewal);                      
                    }
                    else if (temp.m_state == AprItemState.Lock)
                    {
                        if(temp .m_curEffect== 0)
                        {
                            int needItem = 0;
                            int needNum = 0;
                            temp.GetUnlockInfo(1, out needItem, out needNum);
                            int curOwn = pakageModule.GetItemCount(needItem);
                            OpenMaterialInfo(needItem, curOwn, needNum);
                        }
                        else
                        {
                            OpenPreCondition(temp.GetName(1));
                        }
                        OpenLevelInfo(temp.GetUnlockLevel(temp.m_curEffect + 1));
                        SetState(State.UnLock);
                    }
                    else
                    {
                        OpenLevelInfo(temp.GetUnlockLevel(temp.m_curEffect + 1));
                        SetState(State.UnLock);
                    }
                   
                    break;
                default:
                    break;
            }
        }
        void SetTab(RideItem temp, UIColorBtn item)
        {
            switch ((UIColorBtn.State)item.m_stateRoot.CurrentState)
            {
                case UIColorBtn.State.Null_Color:
                    break;
                case UIColorBtn.State.Unlock_Unselected:
                    CloseComponent();
                    break;
                case UIColorBtn.State.Unlock_Selected:
                    if (temp.m_state == AprItemState.OutTime)
                    {
                        int id = 0;
                        int need = 0;
                        temp.GetUnlockInfo(0, out id, out need);
                        int curOwnNum = pakageModule.GetItemCount(id);
                        OpenMaterialInfo(id, curOwnNum, need);
                        SetState(State.Renewal);
                    }
                    else
                    {
                        if (m_rideHandle.m_roleRideData.m_curRide == -1)
                        {
                            SetState(State.Ride);
                        }
                        else if (m_rideHandle.m_roleRideData.IsInLoadRideList(temp.m_id))
                        {
                            SetState(State.Unload);
                        }
                        else
                        {
                            SetState(State.Load);
                        }
                    }
                    break;
                case UIColorBtn.State.Lock_Unselected:
                    CloseComponent();
                    break;
                case UIColorBtn.State.Lock_Selected:
                    if(temp.m_state==AprItemState.OutTime)
                    {
                        int id = 0;
                        int need = 0;
                        temp.GetUnlockInfo(0, out id, out need);
                        int curOwnNum = pakageModule.GetItemCount(id);
                        OpenMaterialInfo(id, curOwnNum, need);
                        SetState(State.Renewal);
                    }
                    else 
                    {
                        int needItem = 0;
                        int needNum = 0;
                        int index = temp.m_curColor;
                        temp.GetUnlockInfo(index, out needItem, out needNum);
                        int curOwn = pakageModule.GetItemCount(needItem);
                        OpenMaterialInfo(needItem, curOwn, needNum);
                        SetState(State.UnLock);
                    }

                    break;
                default:
                    break;
            }
        }
        public void SetState(State state)
        {
            m_multiBtn.SetCurrentState((int)state, false);
        }
        public void RefreshUI()
        {
            CloseComponent();
            OpenOper();
            if(m_colorRoot.activeInHierarchy)
            {
                if(m_appItem is WeaponItem)
                {
                    WeaponItem weaponItem = m_appItem as WeaponItem;
                    SetCommonTab(m_colorBtnList[weaponItem.m_curEffect]);
                }
                else if(m_appItem is RideItem)
                {
                    RideItem rideItem = m_appItem as RideItem;
                    SetCommonTab(m_colorBtnList[rideItem.m_curColor]);
                }
            }
        }
    }
}
#endif
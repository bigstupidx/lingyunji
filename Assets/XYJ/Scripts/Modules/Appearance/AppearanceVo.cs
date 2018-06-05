#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    //网络事件注册
    partial class HotAppearanceModule
    {
        public C2AAppearanceRequest request { get; protected set; }

        void Init()
        {
            request = new C2AAppearanceRequest(App.my.gameRPC.local);

            Event.Subscribe<UnlockRequest>(EventID.Ap_UnlockItem, UnlockItem);
            Event.Subscribe<RenewalRequest>(EventID.Ap_RenewalItem, RenewalItem);
            Event.Subscribe<PaintRequest>(EventID.Ap_PaintCloth, PaintCloth);
            Event.Subscribe<WearRequest>(EventID.Ap_WearItem, WearItem);
            Event.Subscribe<UnlockRideColorReq>(EventID.Ap_UnlockRideColor, UnlockRideColor);
            Event.Subscribe<UnlockWeaponEffectReq>(EventID.Ap_UnlockWeaponEffect, UnlockWeaponEfffect);
            Event.Subscribe<LoadRideReq>(EventID.Ap_LoadRide, LoadRide);
            Event.Subscribe<DeleteColorReq>(EventID.Ap_DeleteColor, DeleteColor);
            Event.Subscribe<PresettingSaveReq>(EventID.Ap_SaveNewFace, SaveNewFace);
            Event.Subscribe<DisguiseSetReq>(EventID.Ap_ApplyNewFace, ApplyNewFace);
            Event.Subscribe<PresettingDelReq>(EventID.Ap_DeletePreset, DeletePreset);
  

            hotApp.my.handler.Reg<OutTimeReqest>(Protoid.A2C_Item_Outtime, ExecuteOuttime);

        }

        void ExecuteOuttime(OutTimeReqest input)
        {
           
            RoleDisguiseHandle disguiseHandle = appearanceMgr.GetDisguiHandle();
            RideHandle rideHandle = disguiseHandle.m_rideHandle;
            RoleRideData roleRideData = rideHandle.m_roleRideData;
            switch (input.aprType)
            {
                case AprType.Cloth:
                    if (input.itemId == appearanceMgr.m_AppearanceData.clothStyleId)
                    {
                        disguiseHandle.SetDefaultCloth();
                        appearanceMgr.m_AppearanceData.clothStyleId = disguiseHandle.m_clothHandle.m_roleClothData.m_clothId;
                        appearanceMgr.m_AppearanceData.clothColorIdx = disguiseHandle.m_clothHandle.m_roleClothData.m_curColor;
                    }
                    break;
                case AprType.Hair:
                    if (input.itemId == appearanceMgr.m_AppearanceData.hairDressId)
                    {
                        disguiseHandle.SetDefaultHairId();
                        appearanceMgr.m_AppearanceData.hairDressId = -1;
                    }
                    break;
                case AprType.Weapon:
                    if (input.itemId == appearanceMgr.m_AppearanceData.weaponStyleId)
                    {
                        disguiseHandle.SetDefaultWeapon();
                        appearanceMgr.m_AppearanceData.weaponStyleId = disguiseHandle.m_weaponHandle.m_roleWeaponData.m_weaponId;
                        appearanceMgr.m_AppearanceData.weaponEffectIdx = disguiseHandle.m_weaponHandle.m_roleWeaponData.m_curEffect;
                    }
                    break;
                case AprType.Ride:
                    if (roleRideData.IsInLoadRideList(input.itemId))
                    {
                        roleRideData.RemoveLoadRideById(input.itemId);
                        if (input.itemId == appearanceMgr.m_AppearanceData.rideStyleId)
                        {
                            int count = roleRideData.GetLoadedRideList().Count;
                            if (count > 0)
                            {
                                roleRideData.m_curRide = roleRideData.GetLoadedRideList()[count - 1].rideStyleId;
                                roleRideData.m_curColor = roleRideData.GetLoadedRideList()[count - 1].curColor;
                            }
                            else
                            {
                                roleRideData.m_curRide = -1;
                                roleRideData.m_curColor = 0;
                            }
                            appearanceMgr.m_AppearanceData.rideStyleId = roleRideData.m_curRide;
                        }

                    }
                    break;
                default:
                    break;
            }

            RefreshData(input.itemId, input.aprType);
            Event.fireEvent(EventID.AP_RefreshUI);
        }
        void RefresModel()
        {
            appearanceMgr.RefreshDisguisehandle();
            App.my.localPlayer.battle.actor.m_partManager.m_disguiseHandle = appearanceMgr.GetDisguiHandle();
            App.my.localPlayer.battle.actor.m_partManager.RefreshSettingObject();
        }
        //刷新handle的配置数据
        void RefreshData(int id,AprType aprType)
        {
            switch (aprType)
            {
                case AprType.Cloth:
                    List<ClothItem> clothList = appearanceMgr.GetClothHandle().m_clothConfig.GetClothList();
                    int clothCount = clothList.Count;
                    for(int i = 0 ; i < clothCount; i++)
                    {
                        ClothItem temp = clothList[i];
                        if(temp.m_id == id)
                        {
                            temp.RefreshData(appearanceMgr.m_AppearanceData);
                            return;
                        }
                    }
                    break;
                case AprType.Hair:
                    List<HairItem> hairList = appearanceMgr.GetHairHandle().m_hairConfig.GetHairList();
                    int hairCount = hairList.Count;
                    for (int i = 0; i < hairCount; i++)
                    {
                        HairItem temp = hairList[i];
                        if (temp.m_id == id)
                        {
                            temp.RefreshData(appearanceMgr.m_AppearanceData);
                            return;
                        }
                    }
                    break;
                case AprType.Weapon:
                    List<WeaponItem> weaponList = appearanceMgr.GetWeaponHandle().m_weaponConfig.GetWeaponList();
                    int weaponCount = weaponList.Count;
                    for (int i = 0; i < weaponCount; i++)
                    {
                        WeaponItem temp = weaponList[i];
                        if (temp.m_id == id)
                        {
                            temp.RefreshData(appearanceMgr.m_AppearanceData);
                            return;
                        }
                    }
                    break;
                case AprType.Ride:
                    List<RideItem> rideList = appearanceMgr.GetRideHandle().m_rideConfig.GetRideItemList();
                    int rideCount = rideList.Count;
                    for (int i = 0; i < rideCount; i++)
                    {
                        RideItem temp = rideList[i];
                        if (temp.m_id == id)
                        {
                            temp.RefreshData(appearanceMgr.m_AppearanceData);
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        //解锁
        void UnlockItem(UnlockRequest input)
        {
            request.UnlockItem(input, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                {
                    return;
                }
                if (respone.code == ReturnCode.Ap_ItemNotExist)
                {
                    Debug.Log("物品不存在");
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    switch (input.aprType)
                    {
                        case AprType.Cloth:
                            AddClothItem(input.itemId, input.day);
                            break;
                        case AprType.Hair:
                            AddHairItem(input.itemId, input.day);
                            break;
                        case AprType.Weapon:
                            AddWeaponItem(input.itemId, input.day);
                            break;
                        case AprType.Ride:
                            AddRideItem(input.itemId, input.day);
                            break;
                        default:
                            break;
                    }
                  
                    RefreshData(input.itemId,input.aprType);
                    Event.fireEvent(EventID.AP_RefreshUI);
                    return;
                }
            });
        }
        //解锁一个服装
        void AddClothItem(int id, double day)
        {
            ClothStyleItem temp = new ClothStyleItem();
            temp.clothStyleId = id;
            if (day < 0)//永久
            {
                temp.clothCD = DateTime.MaxValue.Ticks;
            }
            else
            {
                temp.clothCD = DateTime.Now.AddDays(day).Ticks;
                Debug.Log("clothCd" + temp.clothCD);
            }
            appearanceMgr.m_AppearanceData.clothItems.Add(temp);
        }
        //解锁一个头饰
        void AddHairItem(int id, double day)
        {
            HairStyleItem temp = new HairStyleItem();
            temp.hairId = id;
            if (day < 0)//永久
            {
                temp.hairCD = DateTime.MaxValue.Ticks;
            }
            else
            {
                temp.hairCD = DateTime.Now.AddDays(day).Ticks;
                Debug.Log("hairCd" + temp.hairCD);
            }
            appearanceMgr.m_AppearanceData.hairItems.Add(temp);
        }
        //解锁一个武饰
        void AddWeaponItem(int id, double day)
        {
            WeaponStyleItem temp = new WeaponStyleItem();
            temp.weaponStyleId = id;
            temp.weaponMaxEffect = 1;
            if (day < 0)//永久
            {
                temp.weaponCD = DateTime.MaxValue.Ticks;
            }
            else
            {
                temp.weaponCD = DateTime.Now.AddDays(day).Ticks;
                Debug.Log("weaponCd" + temp.weaponCD);
            }
            appearanceMgr.m_AppearanceData.weapoinItems.Add(temp);
        }
        //解锁一个坐骑
        void AddRideItem(int id, double day)
        {
            RideStyleItem temp = new RideStyleItem();
            temp.rideStyleId = id;
            temp.unlockedColor.Add(0);
            if (day < 0)//永久
            {
                temp.rideCD = DateTime.MaxValue.Ticks;
            }
            else
            {
                temp.rideCD = DateTime.Now.AddDays(day).Ticks;
                Debug.Log("RideCd" + temp.rideCD);
            }
            appearanceMgr.m_AppearanceData.rideItems.Add(temp);
        }

        //续期
        void RenewalItem(RenewalRequest input)
        {
            request.RenewalItem(input, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                {
                    return;
                }
                if (respone.code == ReturnCode.Ap_ItemNotExist)
                {
                    Debug.Log("物品不存在");
                    return;
                }
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    switch (input.aprType)
                    {
                        case AprType.Cloth:
                            RenewalCloth(input.itemId, input.day);
                            break;
                        case AprType.Hair:
                            RenewalHair(input.itemId, input.day);
                            break;
                        case AprType.Weapon:
                            RenewalWeapon(input.itemId, input.day);
                            break;
                        case AprType.Ride:
                            RenewalRide(input.itemId, input.day);
                            break;
                        default:
                            break;
                    }


                    RefreshData(input.itemId, input.aprType);
                    Event.fireEvent(EventID.AP_RefreshUI);
                }
            });
        }
        void RenewalCloth(int id, double day)
        {
            int count = appearanceMgr.m_AppearanceData.clothItems.Count;
            for (int i = 0; i < count; i++)
            {
                ClothStyleItem temp = appearanceMgr.m_AppearanceData.clothItems[i];
                if (temp.clothStyleId == id)
                {
                    if (day < 0)
                    {
                        temp.clothCD = DateTime.MaxValue.Ticks;
                    }
                    else
                    {
                        temp.clothCD = DateTime.Now.AddDays(day).Ticks;
                        Debug.Log("clothCd" + temp.clothCD);
                    }
                }
            }
        }
        void RenewalHair(int id, double day)
        {
            int count = appearanceMgr.m_AppearanceData.hairItems.Count;
            for (int i = 0; i < count; i++)
            {
                HairStyleItem temp = appearanceMgr.m_AppearanceData.hairItems[i];
                if (temp.hairId == id)
                {
                    if (day < 0)
                    {
                        temp.hairCD = DateTime.MaxValue.Ticks;
                    }
                    else
                    {
                        temp.hairCD = DateTime.Now.AddDays(day).Ticks;
                        Debug.Log("clothCd" + temp.hairCD);
                    }
                }
            }
        }
        void RenewalWeapon(int id, double day)
        {
            int count = appearanceMgr.m_AppearanceData.weapoinItems.Count;
            for (int i = 0; i < count; i++)
            {
                WeaponStyleItem temp = appearanceMgr.m_AppearanceData.weapoinItems[i];
                if (temp.weaponStyleId == id)
                {
                    if (day < 0)
                    {
                        temp.weaponCD = DateTime.MaxValue.Ticks;
                    }
                    else
                    {
                        temp.weaponCD = DateTime.Now.AddDays(day).Ticks;
                        Debug.Log("clothCd" + temp.weaponCD);
                    }
                }
            }
        }
        void RenewalRide(int id, double day)
        {
            int count = appearanceMgr.m_AppearanceData.rideItems.Count;
            for (int i = 0; i < count; i++)
            {
                RideStyleItem temp = appearanceMgr.m_AppearanceData.rideItems[i];
                if (temp.rideStyleId == id)
                {
                    if (day < 0)
                    {
                        temp.rideCD = DateTime.MaxValue.Ticks;
                    }
                    else
                    {
                        temp.rideCD = DateTime.Now.AddDays(day).Ticks;
                        Debug.Log("clothCd" + temp.rideCD);
                    }
                }
            }
        }

        //添加服饰染色方案
        void PaintCloth(PaintRequest input)
        {
            if (input != null)
            {
                request.PaintCloth(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.Ap_ColorFull)
                    {
                        //颜色已满，请先删除
                        return;
                    }
                    if (respone.code == ReturnCode.ReturnCode_OK)
                    {
                        int count = appearanceMgr.m_AppearanceData.clothItems.Count;
                        for (int i = 0; i < count; i++)
                        {
                            ClothStyleItem temp = appearanceMgr.m_AppearanceData.clothItems[i];
                            if (temp.clothStyleId == input.itemId)
                            {
                                temp.hsv.Add(input.color);



                                RefreshData(input.itemId, AprType.Cloth);
                                Event.fireEvent(EventID.AP_RefreshUI);
                                
                                return;
                            }
                        }
                    }
                });
            }
        }
        //装备，骑乘等
        void WearItem(WearRequest input)
        {
            if (input != null)
            {
                request.WearItem(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.ReturnCode_OK)
                    {
                        switch (input.aprType)
                        {
                            case AprType.Cloth:
                                WearCloth(input.itemId, input.colorIndex);
                                break;
                            case AprType.Hair:
                                WearHair(input.itemId);
                                break;
                            case AprType.Weapon:
                                WearWeapon(input.itemId, input.colorIndex);
                                break;
                            case AprType.Ride:
                                WearRide(input.itemId, input.colorIndex);
                                break;
                            default:
                                break;
                        }
                        RefresModel();
                        Event.fireEvent(EventID.AP_RefreshUI);
                    }
                });
            }
        }

        void WearCloth(int id, int colorIndex)
        {
            appearanceMgr.m_AppearanceData.clothStyleId = id;
            appearanceMgr.m_AppearanceData.clothColorIdx = colorIndex;
  
        }

        void WearHair(int id)
        {
            appearanceMgr.m_AppearanceData.hairDressId = id;
      
        }
        void WearWeapon(int id, int curEffect)
        {
            appearanceMgr.m_AppearanceData.weaponStyleId = id;
            appearanceMgr.m_AppearanceData.weaponEffectIdx = curEffect;
       
        }
        void WearRide(int id, int colorIndex)
        {
            appearanceMgr.m_AppearanceData.rideStyleId = id;
            appearanceMgr.m_AppearanceData.rideColorIdx = colorIndex;
          
        }

        //解锁坐骑颜色
        void UnlockRideColor(UnlockRideColorReq input)
        {
            if (input != null)
            {
                request.UnlockRideColor(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.ReturnCode_OK)
                    {
                        int count = appearanceMgr.m_AppearanceData.rideItems.Count;
                        for (int i = 0; i < count; i++)
                        {
                            RideStyleItem temp = appearanceMgr.m_AppearanceData.rideItems[i];
                            if (temp.rideStyleId == input.itemId)
                            {
                                temp.unlockedColor.Add(input.colorIndex);

                                RefreshData(input.itemId, AprType.Ride);
                                Event.fireEvent(EventID.AP_RefreshUI);
                                return;
                            }
                        }
                    }

                });
            }
        }
        //解锁武器特效
        void UnlockWeaponEfffect(UnlockWeaponEffectReq input)
        {
            if (input != null)
            {
                request.UnlockWeaponEffect(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.ReturnCode_OK)
                    {
                        int count = appearanceMgr.m_AppearanceData.weapoinItems.Count;
                        for (int i = 0; i < count; i++)
                        {
                            WeaponStyleItem temp = appearanceMgr.m_AppearanceData.weapoinItems[i];
                            if (temp.weaponStyleId == input.itemId)
                            {
                                temp.weaponMaxEffect = input.level;

                                RefreshData(input.itemId, AprType.Weapon);
                                Event.fireEvent(EventID.AP_RefreshUI);
                                return;
                            }
                        }
                    }

                });
            }
        }
        void LoadRide(LoadRideReq input)
        {
            if (input != null)
            {
                request.LoadRide(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.ReturnCode_OK)
                    {
                        appearanceMgr.m_AppearanceData.loadedRideList.Clear();
                        appearanceMgr.m_AppearanceData.loadedRideList.AddRange(input.loadRideList);
                        Event.fireEvent(EventID.AP_RefreshUI);
                        return;
                    }
                });
            }
        }
        //删除服饰颜色方案
        void DeleteColor(DeleteColorReq input)
        {
            if (input != null)
            {
                request.DeleteColor(input, (error, respone) =>
                 {
                     if(wProtobuf.RPC.Error.Success!=error)
                     {
                         return;
                     }
                     if(respone.code==ReturnCode.ReturnCode_OK)
                     {
                         int count = appearanceMgr.m_AppearanceData.clothItems.Count;
                         for (int i = 0; i < count; i++)
                         {
                             ClothStyleItem temp = appearanceMgr.m_AppearanceData.clothItems[i];
                             if (temp.clothStyleId == input.itemId)
                             {
                                 temp.hsv.RemoveAt(input.colorIndex);

                                 if(input.itemId== appearanceMgr.m_AppearanceData.clothStyleId)
                                 {
                                     if(input.colorIndex== appearanceMgr.m_AppearanceData.clothColorIdx)
                                     {
                                         appearanceMgr.m_AppearanceData.clothColorIdx = 0;                                         
                                     }
                                     else if(input.colorIndex < appearanceMgr.m_AppearanceData.clothColorIdx)
                                     {
                                         appearanceMgr.m_AppearanceData.clothColorIdx -= 1;
                                     }
                                 }
                                 RefreshData(input.itemId, AprType.Cloth);
                                 Event.fireEvent(EventID.AP_RefreshUI);
                                 return;
                             }
                         }

                     }
                 });
            }
        }

        void SaveNewFace(PresettingSaveReq input)
        {
            if(input!=null)
            {
                request.PresettingSave(input, (error, respone) =>
                 {
                     if(error!=wProtobuf.RPC.Error.Success)
                     {
                         return;
                     }
                     if(respone.code==ReturnCode.Ap_PresettingFull)
                     {
                         return;
                     }
                     if(respone.code==ReturnCode.ReturnCode_OK)
                     {
                         if(input.faceType==0)
                         {
                             appearanceMgr.m_AppearanceData.presettings_0.Add(input.persetting);
                         }
                         else
                         {
                             appearanceMgr.m_AppearanceData.presettings_1.Add(input.persetting);
                         }
                         Event.fireEvent(EventID.AP_RefreshUI);
                         return;
                     }
                 });
            }
        }
        void ApplyNewFace(DisguiseSetReq input)
        {
            if (input != null)
            {
                request.PresettingApply(input, (error, respone) =>
                {
                    if (error != wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.Ap_PresettingFull)
                    {
                        return;
                    }
                    if (respone.code == ReturnCode.ReturnCode_OK)
                    {
                        appearanceMgr.m_AppearanceData.faceType = input.faceType;
                        appearanceMgr.m_AppearanceData.faceJson = input.faceJson;
                        appearanceMgr.m_AppearanceData.hairStyleId = input.hairStyleId;
                        appearanceMgr.m_AppearanceData.hairColorId = input.hairColorId;
                        appearanceMgr.m_AppearanceData.skinColorId = input.skinColorId;
                        return;
                    }
                });
            }
        }
        void DeletePreset(PresettingDelReq input)
        {
            if(input!=null)
            {
                request.DeletePresetting(input, (error, respone) =>
                {
                    if(error!=wProtobuf.RPC.Error.Success)
                    {
                        return;
                    }
                    if(respone.code==ReturnCode.ReturnCode_OK)
                    {
                        if(input.faceType==0)
                        {
                            appearanceMgr.m_AppearanceData.presettings_0.RemoveAt(input.index);
                        }
                        else
                        {
                            appearanceMgr.m_AppearanceData.presettings_1.RemoveAt(input.index);
                        }
                        Event.fireEvent(EventID.AP_RefreshUI);
                    }
                });
            }
        }
    }
}
#endif
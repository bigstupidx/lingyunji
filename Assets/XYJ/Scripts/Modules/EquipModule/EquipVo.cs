#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using NetProto;
using NetProto.Hot;
using UnityEngine;
using wProtobuf;
using Config;
using xys.hot.UI;
using xys.UI;

namespace xys.hot
{

    partial class HotEquipModule
    {
        C2AEquipRequest m_C2AEquipRequest;
        int m_CurrentEquipSubType = 0;
        int m_CurrentGridIndex = 0;
        void Init()
        {
            //注册协议
            this.m_C2AEquipRequest = new C2AEquipRequest(App.my.socket.game.local);
            //this.m_C2AEquipRequest = new C2AEquipRequest(hotApp.my.gameRPC);
            Event.Subscribe<EquipRequest>(EventID.Equip_Inforce, this.EquipInforce);
            Event.Subscribe<RefineryEquipMsg>(EventID.Equip_Refine, this.EquipRefine);
            Event.Subscribe<EquipRequest>(EventID.Equip_ReplaceRefine, this.ReplaceRefine);
            Event.Subscribe<EquipRequest>(EventID.Equip_ReplaceRecast, this.ReplaceRecast); 
            Event.Subscribe<EquipRequest>(EventID.Equip_ReplaceConcise, this.ReplaceConcise);
            Event.Subscribe<EquipRequest>(EventID.Equip_Recast, this.EquipRecast);
            Event.Subscribe<EquipRequest>(EventID.Equip_Concise, this.EquipConcise);
            Event.Subscribe<NetProto.Int32>(EventID.Equip_Create, this.EquipCreate);
            Event.Subscribe<ForgeEquipMsg>(EventID.Equip_Forge, this.EquipForge);
            Event.Subscribe(EventID.Equip_ResetTimes,this.OnResetTimes);
            Event.Subscribe<bool>(EventID.Equip_SetOpTimesActive, this.OnSetOpTimesActive);
            hotApp.my.handler.Reg(Protoid.A2C_Equip_ResetDay, ResetTimes);
        }

        void EquipInforce(EquipRequest request)
        {
            this.m_C2AEquipRequest.EnforceEquip(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (response.code == ReturnCode.Equip_EnforceVal_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6001).des);
                    return;
                }
                if (response.code == ReturnCode.Material_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_Config_Error)
                {
                    Debug.Log(string.Format("Config error,subtype:{0},isPackageEquip:{1},gridIndex:{2}",request.subType, request.isPackageEquip, request.gridIndex));
                    return;
                }
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    equipMgr.IncreaseEnforceLv(data);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                }
            });
        }

        void EquipRefine(RefineryEquipMsg request)
        {
            this.m_C2AEquipRequest.EquipRefine(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (response.code == ReturnCode.Material_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                    return;
                }
                if (response.code == ReturnCode.Slider_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_Times_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6007).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_Config_Error)
                {
                    Debug.Log(string.Format("Config error,subtype:{0},isPackageEquip:{1},gridIndex:{2},refType:{3}", request.subType, request.isPackageEquip, request.gridIndex, request.refIndex));
                    return;
                }
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    data.equipdata.equipBasicData.refTimes++;
                    equipMgr.ReplaceTempRefinePropety(response, data);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                } 
            });
        }

        void ReplaceRefine(EquipRequest request)
        {
            this.m_C2AEquipRequest.ReplaceRefProperty(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<EquipResponse>(response))
                    return;
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    equipMgr.RelpaceRefAttrs(data);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                }
            });
        }
        void EquipRecast(EquipRequest request)
        {
            this.m_C2AEquipRequest.EquipRecast(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (response.code == ReturnCode.Material_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                    return;
                }
                if (response.code == ReturnCode.Slider_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_Times_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6006).des);
                    return;
                }
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    equipMgr.ReplaceTempRecastPropety(response, data);
                    data.equipdata.equipBasicData.recastTimes++;
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                }
            });
        }
        void ReplaceRecast(EquipRequest request)
        {
            this.m_C2AEquipRequest.ReplaceRecastProperty(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<EquipResponse>(response))
                    return;
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    equipMgr.RelpaceRecastAttrs(data);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                }
                    
            });
        }
        void EquipConcise(EquipRequest request)
        {
            this.m_C2AEquipRequest.EquipConcise(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (response.code == ReturnCode.Material_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_Times_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6008).des);
                    return;
                }
                if (response.code == ReturnCode.Slider_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                    return;
                }
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    equipMgr.ReplaceTempConsicePropety(response, data);
                    data.equipdata.equipBasicData.consiceTimes++;
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                }
            });
        }
        void ReplaceConcise(EquipRequest request)
        {
            this.m_C2AEquipRequest.ReplaceConciseProperty(request, (error, response) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<EquipResponse>(response))
                    return;
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    ItemData data;
                    if (request.isPackageEquip)
                        data = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(request.gridIndex).data;
                    else
                        data = equipMgr.GetEquipData(request.subType);
                    equipMgr.RelpaceConciseAttrs(data);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                }

            });
        }
        void EquipCreate(NetProto.Int32 equipId)
        {
            this.m_C2AEquipRequest.EquipCreate(equipId, (error, response) =>
            {
                
                if(response.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    Debug.Log("Created equip " + equipId);
                    //equipMgr.WearEquip(respone.data);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                }  
            });
        }
        xys.UI.Dialog.TwoBtn m_twoBtn;
        public void LoadEquip(int gridIndex)
        {
            var msg = new NetProto.EquipLoadRequest();
            msg.gridIndex = gridIndex;
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            ItemData data = new ItemData();
            data = packageMgr.GetItemInfo(gridIndex).data;
            if (data == null)
            {
                Debug.Log("GridIndex error");
                return;
            }
            var cfg = Config.EquipPrototype.Get(data.id);
            if (cfg == null)
            {
                Debug.Log("data id doesnot exit");
                return;
            }
            if (cfg.job.Has(hotApp.my.localPlayer.job))
            {
                if (cfg.leve <= hotApp.my.localPlayer.levelValue)
                {
                    if (equipMgr.GetEquipData(data.equipdata.equipBasicData.nSubType) != null)
                    {
                        if (equipMgr.CanAutoChangeEnforceVal(equipMgr.GetEquipData(data.equipdata.equipBasicData.nSubType), data))
                        {
                            if (null == m_twoBtn)
                            {
                                m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                                    "", TipsContent.Get(6004).des,
                                    "取消", () =>
                                    {
                                        msg.isReplaceAttrs = false;
                                        LoadEquipCore(msg, data);
                                        return false;
                                    },
                                    "确定", () =>
                                    {
                                        msg.isReplaceAttrs = true;
                                        LoadEquipCore(msg, data);
                                        return false;
                                    }, true, true, () => { m_twoBtn = null; });
                                return;
                            }
                        }
                    }
                    LoadEquipCore(msg, data);
                }
                else
                    SystemHintMgr.ShowHint(TipsContent.Get(3113).des);
            }
            else
                SystemHintMgr.ShowHint(TipsContent.Get(3112).des);
        }
        void LoadEquipCore(EquipLoadRequest msg,ItemData data)
        {
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            this.m_C2AEquipRequest.LoadEquip(msg, (error, response) =>
            {
                if (response.code == ReturnCode.PlayerLevel_Error)
                {
                    return;
                }
                if (response.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6010).des);
                    return;
                }
                if (response.code == ReturnCode.PlayerLevel_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3113).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_PlayerJob_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3112).des);
                    return;
                }
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    int subType = data.equipdata.equipBasicData.nSubType;
                    if (!equipMgr.GetAllEquips().ContainsKey(subType))
                        equipMgr.WearEquip(data);
                    else
                        equipMgr.ExChangeItemData(subType, data, msg.isReplaceAttrs);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                    Event.FireEvent(EventID.Equip_LoadFinish, subType);
                }

            });
        }
        public void UnLoadEquip(int subType)
        {
            var msg = new NetProto.Int32();
            msg.value = subType;
            this.m_C2AEquipRequest.UnLoadEquip(msg, (error, response) =>
            {
                if (response.code==ReturnCode.Equip_NoneEquip_Error)
                {
                    Debug.Log("角色未装备此件物品");
                    return;
                }
                if (response.code == ReturnCode.Equip_ID_Error)
                {
                    Debug.Log("装备ID错误");
                    return;
                }
                if (response.code == ReturnCode.Backage_Full_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(3110).des);
                    return;
                }
                //通知UI刷新
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    equipMgr.TakeOffEquip(subType);
                    Event.fireEvent(EventID.Equip_RefreshEquipList);
                    Event.fireEvent(EventID.Equip_UpdateData);
                    Event.FireEvent(EventID.Equip_UnLoadFinish, subType);
                }
                
            });
        }
        void OnResetTimes()
        {
            var msg = new NetProto.Int32();
            msg.value = 0;
            this.m_C2AEquipRequest.ResetInUseEquipOperationTimes(msg, (error, respone) =>
            {
                if (respone.code == ReturnCode.ReturnCode_OK)
                {
                    Debug.Log("ResetEquipTimes Return success");
                }
            });
        }
        void OnSetOpTimesActive(bool value)
        {
            var msg = new NetProto.Bool();
            msg.value = value;
            this.m_C2AEquipRequest.SetOperationTimeActive(msg, (error, response) =>
            {
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    equipMgr.SetOperationTimesActive(value);
                    Debug.Log("Set OperationTimes Active:"+value);
                }
            });
        }
        void ResetTimes()
        {
            equipMgr.ResetInUseEquipOpTimes();
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            int gridCount = packageMgr.package.Count;
            for (int i = 0; i<gridCount;i++)
            {
                NetProto.ItemGrid item = packageMgr.GetItemInfo(i);
                if (item!=null&&item.data.equipdata!=null)
                    equipMgr.ResetEquipOpTimes(item.data);
            }
            Event.fireEvent(EventID.Equip_RefreshEquipList);
        }
        void EquipForge(ForgeEquipMsg msg)
        {
            this.m_C2AEquipRequest.EquipForge(msg, (error,response) => 
            {
                if (response.code  == ReturnCode.Material_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                    return;
                }
                if (response.code  == ReturnCode.Equip_ID_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6011).des);
                    return;
                }
                if (response.code == ReturnCode.Slider_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                    return;
                }
                if (response.code == ReturnCode.Equip_MaterialID_Error)
                {
                    SystemHintMgr.ShowHint(TipsContent.Get(6012).des);
                    return;
                }
                if (response.code == ReturnCode.ReturnCode_OK)
                {
                    Debug.Log("Create success");
                }
            });
        }
    }
}
#endif
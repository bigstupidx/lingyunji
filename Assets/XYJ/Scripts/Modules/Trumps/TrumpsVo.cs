#if !USE_HOT
namespace xys.hot
{
    using Config;
    using NetProto;
    using NetProto.Hot;
    using System.Collections.Generic;

    partial class HotTrumpsModule 
    {
        TrumpsModuleRequest m_TrumpsRequest;
        void Init()
        {
            m_TrumpsRequest = new TrumpsModuleRequest(App.my.game.local);
            Event.Subscribe<int>(EventID.Trumps_Create, this.CreateTrumps);
            Event.Subscribe<TrumpsEquipRequest>(EventID.Trumps_Equip, this.EquipTrumps);
            Event.Subscribe<TrumpsExpRequest>(EventID.Trumps_AddExp, this.OnAddExp);
            Event.Subscribe<TrumpsSkillRequest>(EventID.Trumps_SkillUpgrade, this.OnSkillUpgrade);
            Event.Subscribe<int>(EventID.Trumps_TasteUp, this.OnTasteUp);
            Event.Subscribe<TrumpsInfusedRequest>(EventID.Trumps_InfusedUp, this.OnInfusedUp);
            Event.Subscribe<TrumpStrengthenRequest>(EventID.Trumps_Strengthen, this.OnStrengthen);

            hotApp.my.handler.Reg<TrumpAttribute>(Protoid.A2C_TrumpCreate, OnItem2Trumps);
        }

        void OnItem2Trumps(TrumpAttribute attribute)
        {
            if (attribute == null)
                return;
            this.trumpMgr.table.attributes.Add(attribute.id, attribute);
            //Event.fireEvent(EventID.Trumps_RefleashUI);
            App.my.uiSystem.HidePanel(xys.UI.PanelType.UIPackagePanel, false);
            App.my.uiSystem.ShowPanel(xys.UI.PanelType.UITrumpsPanel, attribute.id);
        }

        void CreateTrumps(int trumpid)
        {
            Int32 request = new Int32();
            request.value = trumpid;
            m_TrumpsRequest.Create(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
                 if (NetReturnCode.isError<TrumpsAttributeRespone>(respone))
                     return;
                 if (respone.code == ReturnCode.Trumps_Config_Error || respone.code == ReturnCode.Trumps_Create_Error)
                     return;
                 this.trumpsMgr.table.attributes.Add(respone.attribute.id,respone.attribute);

                 Event.fireEvent(EventID.Trumps_RefleashUI);
             });
        }

        void EquipTrumps(TrumpsEquipRequest request )
        {
            m_TrumpsRequest.Equip(request, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<TrumpsRespone>(respone))
                    return;
                if (respone.code == ReturnCode.Trumps_Create_Error || respone.code == ReturnCode.Trumps_EquipPos_Error)
                    return;
                //旧法宝对象
                xys.battle.BattleAttri oldBattleAttri = new battle.BattleAttri();
                this.trumpMgr.CalculateTrumps(ref oldBattleAttri);
                //旧连携数据
                List<TrumpJoining> oldJoining = trumpsMgr.GetActiveJoining();
                //
                int oldEquipPos = this.trumpMgr.GetEquipPos(request.trumpid);
                int newEquipPos = request.equippos;
                foreach (int pos in this.trumpsMgr.table.equiptrumps.Keys)
                {
                    if (this.trumpsMgr.table.equiptrumps[pos] == request.trumpid)
                    {
                        this.trumpsMgr.table.equiptrumps.Remove(pos);
                        xys.UI.SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_UnEquip").des);
                        break;
                    }
                }
                //记录装备法宝
                if(oldEquipPos != newEquipPos)
                {
                    if (!this.trumpsMgr.table.equiptrumps.ContainsKey(request.equippos))
                        this.trumpsMgr.table.equiptrumps.Add(request.equippos, request.trumpid);
                    else
                        this.trumpsMgr.table.equiptrumps[request.equippos] = request.trumpid;
                    xys.UI.SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Equip_Success").des);
                }

                //新连携数据
                List<TrumpJoining> newJoining = trumpsMgr.GetActiveJoining();
                //计算连携数据
                if (trumpsMgr.CompareJoiningList(oldJoining, newJoining))
                    xys.UI.SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Equip_Joining").des);
                //新法宝对象
                xys.battle.BattleAttri newBattleAttri = new battle.BattleAttri();
                this.trumpMgr.CalculateTrumps(ref newBattleAttri);
                //显示tips
                UI.UICommon.ShowAttributeTips(oldBattleAttri, newBattleAttri);
                //
                Event.fireEvent(EventID.Trumps_RefleashUI);
            });
        }

        void OnAddExp(TrumpsExpRequest request)
        {
            m_TrumpsRequest.AddExp(request, (error,respone) => 
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<TrumpsAttributeRespone>(respone))
                    return;
                if (respone.code == ReturnCode.Trumps_Create_Error)
                    return;
                if(respone.code == ReturnCode.Trumps_Item_Error)
                {
                    //道具不足
                    return;
                }
                if(respone.code == ReturnCode.Trumps_Item_Error)
                {
                    //不能升级
                    return;
                }
                this.trumpsMgr.table.attributes[request.trumpid] = respone.attribute;

                Event.fireEvent(EventID.Trumps_RefleashUI);
                Event.fireEvent(EventID.Package_UpdatePackage);
            });
        }

        void OnSkillUpgrade(TrumpsSkillRequest request)
        {
            m_TrumpsRequest.SkillUpgrade(request, (error,respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;
                if (NetReturnCode.isError<TrumpsAttributeRespone>(respone))
                    return;
                if (respone.code == ReturnCode.Trumps_Create_Error)
                    return;
                if(respone.code == ReturnCode.Trumps_Lv_Error)
                {
                    //法宝技能等级数据异常
                    return;
                }
                if(respone.code == ReturnCode.Trumps_Item_Error)
                {
                    //道具数量不足
                    return;
                }
                if(respone.code == ReturnCode.Trumps_TasteLv_Error)
                {
                    //境界不满足条件
                    return; 
                }
                this.trumpsMgr.table.attributes[request.trumpid] = respone.attribute;
                Event.fireEvent(EventID.Trumps_RefleashUI);
            });
        }

        void OnTasteUp(int trumpid)
        {
            Int32 request = new Int32();
            request.value = trumpid;
            m_TrumpsRequest.TasteUp(request, (error,respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
//                  if (NetReturnCode.isError<TrumpsAttributeRespone>(respone))
//                      return;
                 if (respone.code == ReturnCode.Trumps_Create_Error)
                     return;
                 if(respone.code == ReturnCode.Trumps_TasteLv_Error)
                 {
                     //境界数据异常
                     return;
                 }
                 if(respone.code == ReturnCode.Trumps_InfusedLv_Error)
                 {
                     //注灵数据异常
                     return;
                 }
                 if (respone.code == ReturnCode.Trumps_Item_Error)
                 {
                     //金钱不足
                     TrumpAttribute trumpAttribute = this.trumpsMgr.table.attributes[request.value];
                     TrumpSoul trumpInfused = TrumpSoul.GetGroupBytrumpid(trumpAttribute.id)[trumpAttribute.tastelv];
                     if (trumpInfused.cost > App.my.localPlayer.GetMoney(AttType.AT_SilverShell))
                     {
                         xys.UI.SystemHintMgr.ShowHint(string.Format("银币低于{0}", trumpInfused.cost));
                     }
                     return;
                 }
                 this.trumpsMgr.table.attributes[request.value] = respone.attribute;
                 Event.fireEvent(EventID.Trumps_RefleashUI);
             });
        }
        
        void OnInfusedUp(TrumpsInfusedRequest request)
        {
            m_TrumpsRequest.InfusedUp(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
//                  if (NetReturnCode.isError<TrumpsAttributeRespone>(respone))
//                      return;
                 if (respone.code == ReturnCode.Trumps_Create_Error)
                     return;
                 if (respone.code == ReturnCode.Trumps_InfusedLv_Error)
                 {
                     //注灵数据异常
                     TrumpInfused infusedData = TrumpInfused.Get(request.infusedid);
                     if(App.my.localPlayer.levelValue < infusedData.playlv)
                        xys.UI.SystemHintMgr.ShowHint(string.Format("角色等级低于{0}", infusedData.playlv));
                     return;
                 }
                 if(respone.code == ReturnCode.Trumps_Item_Error)
                 {
                     return;
                 }
                 //旧法宝对象
                 TrumpObj oldTrump = new TrumpObj();
                 oldTrump.Init(this.trumpMgr.table.attributes[request.trumpid]);
                 //
                 this.trumpsMgr.table.attributes[request.trumpid] = respone.attribute;
                 //新法宝对象
                 TrumpObj newTrump = new TrumpObj();
                 newTrump.Init(this.trumpMgr.table.attributes[request.trumpid]);

                 //显示tips
                 UI.UICommon.ShowAttributeTips(oldTrump.battleAttri, newTrump.battleAttri);
                 //
                 xys.UI.SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Infused_Success").des);
                 Event.fireEvent(EventID.Trumps_RefleashUI);
             });
        }

        void OnStrengthen(TrumpStrengthenRequest request)
        {
            m_TrumpsRequest.Strengthen(request, (error, respone) =>
             {
                 if (error != wProtobuf.RPC.Error.Success)
                     return;
                 if (NetReturnCode.isError<TrumpsAttributeRespone>(respone))
                     return;
                 if (respone.code == ReturnCode.Trumps_Create_Error)
                     return;
                 if (respone.code == ReturnCode.Trumps_Strengthen_Error)
                     return;

                 this.trumpsMgr.table.attributes[request.trumpId] = respone.attribute;
                 Event.fireEvent(EventID.Trumps_RefleashUI);
                 //刷新装备法宝数据
                 if(this.trumpsMgr.GetEquipPos(request.trumpId) != -1)
                 {

                 }
             });
        }

        TrumpsTable GetTrumpsTable()
        {
            return this.trumpsMgr.table;
        }

    }
}
#endif
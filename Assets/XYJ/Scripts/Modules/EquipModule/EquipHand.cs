//using System;
//using System.Collections;
//using System.Collections.Generic;
//using NetProto;
//using UnityEngine;
//using wProtobuf;
//using Config;
//using xys.hot.UI;

//namespace xys
//{
//    using xys.hot;
//    partial class EquipModule
//    {
//        C2WEquipRequest m_C2WEquipRequest;
//        public void Init()
//        {
//            //注册协议
//            this.m_C2WEquipRequest = new C2WEquipRequest(App.my.game.local);           
//            App.my.eventSet.Subscribe<InforceEquipMsg>(EventID.Equip_Inforce, this.EquipInforce);
//            App.my.eventSet.Subscribe<RefineryEquipMsg> (EventID.Equip_Refine, this.EquipRefine);
//            App.my.eventSet.Subscribe<NetProto.Int32>(EventID.Euqip_ReplaceRefine, this.ReplaceRefine);
//            App.my.eventSet.Subscribe<NetProto.Int32>(EventID.Equip_Create, this.EquipCreate);
//            App.my.eventSet.Subscribe<NetProto.Int32>(EventID.Equip_OnLoad, this.LoadEquip);
//            App.my.eventSet.Subscribe<NetProto.Int32>(EventID.Equip_UnLoad, this.UnLoadEquip);
//            App.my.eventSet.Subscribe<NetProto.Int32>(EventID.Equip_SyncData, this.SyncEquipList);
            
            
//        }

//        static ArrayList ParsingStr(string strData, string strIndex1, string strIndex2)
//        {
//            ArrayList strArray1 = new ArrayList();
//            if (strIndex1 == "" && strIndex2 == "" && strData != "")
//            {
//                strArray1.Add(strData);
//                return strArray1;
//            }

//            if (strIndex1 == "")
//            {
//                return strArray1;
//            }

//            if (strIndex1 != "" && strIndex2 != "")
//            {
//                string[] str1 = strData.Split((strIndex1.ToCharArray())[0]);
//                foreach (var data1 in str1)
//                {
//                    ArrayList strArray2 = new ArrayList();
//                    string[] str2 = data1.Split((strIndex2.ToCharArray())[0]);
//                    foreach (var data2 in str2)
//                    {
//                        strArray2.Add(data2);
//                    }
//                    strArray1.Add(strArray2);
//                }
//                return strArray1;
//            }
//            else if (strIndex1 != "" && strIndex2 == "")
//            {
//                string[] str1 = strData.Split((strIndex1.ToCharArray())[0]);
//                foreach (var data in str1)
//                {
//                    strArray1.Add(data);
//                }
//                return strArray1;
//            }
//            return strArray1;
//        }

//        public bool GetPropInfo(int equipId, ref List<xys.hot.UI.UIArrayInfo.PropData> propList)
//        {
//            EqupMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr;
//            if (equipMgr == null)
//                return false;
//            EquipGrids m_Table = equipMgr.m_Table;
//            if (m_Table == null)
//            {
//                return false;
//            }

//            ItemData m_ItemData = new ItemData();

//            foreach (var value in m_Table.euqips)
//            {
//                 if (value.Value.id == equipId)
//                {
//                    m_ItemData = value.Value;
//                    break;
//                }
//            }

//            if (m_ItemData != null)
//            {
//                //foreach (var value in m_ItemData.equipdata.baseAtts)
//                //{
//                //    xys.hot.UI.UIArrayInfo.PropData m_PropData = new xys.hot.UI.UIArrayInfo.PropData();
//                //    m_PropData.name = AttributeDefine.Get(value.Key).attrName;
//                //    m_PropData.localVal = (int)(value.Value);
//                //    m_PropData.propType = xys.hot.UI.UIArrayInfo.Type.enBase;   //基础属性
//                //    propList.Add(m_PropData);
//                //}

//                //foreach (var value in m_ItemData.equipdata.randAtts)
//                //{
//                //    xys.hot.UI.UIArrayInfo.PropData m_PropData = new xys.hot.UI.UIArrayInfo.PropData();
//                //    m_PropData.name = AttributeDefine.Get(value.Key).attrName;
//                //    m_PropData.localVal = (int)(value.Value);
//                //    m_PropData.propType = xys.hot.UI.UIArrayInfo.Type.enRand;  //随机属性
//                //    propList.Add(m_PropData);
//                //}

//                int propIndex = 1;
//                foreach (var value in m_ItemData.equipdata.inforceAtts)
//                {
//                    xys.hot.UI.UIArrayInfo.PropData m_PropData = new xys.hot.UI.UIArrayInfo.PropData();
//                    if (propIndex <= m_ItemData.equipdata.baseAtts.Count)
//                    {
//                        m_PropData.propType = xys.hot.UI.UIArrayInfo.Type.enBase;  //随机属性
//                    }
//                    else
//                    {
//                        m_PropData.propType = xys.hot.UI.UIArrayInfo.Type.enRand;  //随机属性
//                    }
//                    m_PropData.key = value.key;
//                    m_PropData.name = AttributeDefine.Get(value.key).attrName;
//                    m_PropData.localVal = (int)(value.value);
                    
//                    propList.Add(m_PropData);
//                }
//                return true;
//            }

//            return false;
//        }

//        public bool GetRefPropInfo(int equipId, ref List<xys.hot.UI.UIRefArrayInfo.PropData> propList, int type)
//        {
//            EqupMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr;
//            if (equipMgr == null)
//                return false;
//            EquipGrids m_Table = equipMgr.m_Table;
//            if (m_Table == null)
//            {
//                return false;
//            }

//            ItemData m_ItemData = new ItemData();

//            foreach (var value in m_Table.euqips)
//            {
//                if (value.Value.id == equipId)
//                {
//                    m_ItemData = value.Value;
//                    break;
//                }
//            }

//            if (m_ItemData != null)
//            {
//                foreach (var value in m_ItemData.equipdata.optionalAtts)
//                {
//                    xys.hot.UI.UIRefArrayInfo.PropData m_PropData = new xys.hot.UI.UIRefArrayInfo.PropData();
//                    m_PropData.key = value.Key;
//                    m_PropData.name = AttributeDefine.Get(value.Key).attrName;
//                    m_PropData.localVal = (int)(value.Value);
//                    //获取当前可炼化等级最大值
//                    if (type == (int)UIRefArrayInfo.propType.enRefProp)
//                    {
//                        m_PropData.reftype = m_ItemData.equipdata.refType;
//                    }
//                    else if (type == (int)UIRefArrayInfo.propType.enRefBackProp)
//                    {
//                        m_PropData.reftype = m_ItemData.equipdata.tempRefType;
//                    }

//                    RefineQualityPrototype data = new RefineQualityPrototype();
//                    data = RefineQualityPrototype.Get(m_PropData.reftype);
//                    if (data.qualityId > 4 || data.qualityId < 1)
//                        return false;
//                    int maxRefLv = data.propMaxLevel;
//                    int minRefLv = data.propMinLevel;
//                    RefineValuePrototype maxValue = new RefineValuePrototype();
//                    maxValue = RefineValuePrototype.Get(maxRefLv);
//                    RefineValuePrototype minValue = new RefineValuePrototype();
//                    minValue = RefineValuePrototype.Get(minRefLv);
//                    if (data.qualityId > 4 || data.qualityId < 1)
//                        return false;
//                    int max = 0;
//                    int min = 0;
//                    max = (int)maxValue.battleAttri.Get(value.Key);
//                    min = (int)minValue.battleAttri.Get(value.Key);
//                    if (max <= min)
//                    {
//                        return false;
//                    }
//                    m_PropData.lineVal = (m_PropData.localVal - min) / (max - min);
//                    propList.Add(m_PropData);
//                }
//                return true;
//            }

//            return false;
//        }

//        //根据当前点击的装备Index找到对应的装备信息（装备id，装备名字，强化消耗材料信信息，强化等级）
//        public bool GetEquipInfo(int equipId, ref string name, ref int materNum, ref int infLv, ref int costNum,  ref int infType, ref int materId)
//        {
//            EqupMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr;
//            if (equipMgr == null)
//                return false;
//            EquipGrids m_Table = equipMgr.m_Table;
//            if (m_Table == null)
//            {
//                return false;
//            }

//            ItemData m_ItemData = new ItemData();

//            foreach (var value in m_Table.euqips)
//            {
//                if (value.Value.id == equipId)
//                {
//                    m_ItemData = value.Value;
//                    break;
//                }
//            }

//            if (m_ItemData == null)
//            {
//                return false;
//            }

//            ////获取已背包物品信息
//            PackageModule packageModule = App.my.localPlayer.GetModule<PackageModule>();
//            if (packageModule == null)
//                return false;
//            //查找背包中的物品数量
//            //costNum = packageModule.packageMgr.GetItemCount(1000);

//            //获取配置表信息
//            Config.EquipPrototype data = Config.EquipPrototype.Get(equipId);
//            if (data == null)
//                return false;

//            name = data.name;
            

//            if (m_ItemData.equipdata.awaken_status == 1)
//            {
//                infLv = m_ItemData.equipdata.awaken_enhance;
//                infType = (int)xys.hot.UI.EquipStrengthenPage.InfType.enAwakenInf;
//                Config.AwakeInforceTab infTbl = Config.AwakeInforceTab.Get(m_ItemData.equipdata.awaken_enhance + 1);
//                if (infTbl != null)
//                {
//                    materId = infTbl.materialCostId;
//                    materNum = packageModule.packageMgr.GetItemCount(infTbl.materialCostId);
//                    costNum = infTbl.materialCostCount;
//                }
//            }

//            if (m_ItemData.equipdata.awaken_status == 0 && m_ItemData.equipdata.enhance == 20)
//            {
//                infType = (int)xys.hot.UI.EquipStrengthenPage.InfType.enAwaken;
//            }


//            if (m_ItemData.equipdata.awaken_status == 0 && m_ItemData.equipdata.enhance + 1 < 20)
//            {
//                //检查装备类型
//                int inforType = 0;
//                inforType = data.sonType;
//                infLv = m_ItemData.equipdata.enhance;
//                if (inforType == 1)   //武器装备
//                {
//                    Config.EquipInforceTab infTbl = Config.EquipInforceTab.Get(m_ItemData.equipdata.enhance + 1);
//                    if (infTbl != null)
//                    {
//                        materId = infTbl.materialCostId;
//                        materNum = packageModule.packageMgr.GetItemCount(infTbl.materialCostId);
//                        costNum = infTbl.materialCostCount;
//                    }
//                }
//                else if (inforType >= 2 && inforType <= 6)   //防具装备
//                {
//                    Config.ArmorInforceTab infTbl = Config.ArmorInforceTab.Get(m_ItemData.equipdata.enhance + 1);
//                    if (infTbl != null)
//                    {
//                        materId = infTbl.materialCostId;
//                        materNum = packageModule.packageMgr.GetItemCount(infTbl.materialCostId);
//                        costNum = infTbl.materialCostCount;
//                    }
//                }
//                else if (inforType >= 7 && inforType <= 9)   //饰品装备
//                {
//                    Config.AccessoryInforceTab infTbl = Config.AccessoryInforceTab.Get(m_ItemData.equipdata.enhance + 1);
//                    if (infTbl != null)
//                    {
//                        materId = infTbl.materialCostId;
//                        materNum = packageModule.packageMgr.GetItemCount(infTbl.materialCostId);
//                        costNum = infTbl.materialCostCount;
//                    }
//                }
//                infType = (int)xys.hot.UI.EquipStrengthenPage.InfType.enNormalInf;
//            }
//            return true;
//        }

//        //点击强化按钮检查生成的条件
//        public bool CheckConditions(int equipId, ref int equipIndex)
//        {
//            //获取玩家身上的属性
//            //App.my.localPlayer.attributes.Get(AttType.AT_SilverShell);
//            ////获取已穿戴装备信息
//            EqupMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr;
//            if (equipMgr == null)
//                return false;
//            EquipGrids m_Table = equipMgr.m_Table;
//            if (m_Table == null)
//            {
//                return false;
//            }

//            ItemData m_ItemData = new ItemData();

//            foreach (var value in m_Table.euqips)
//            {
//                if (value.Value.id == equipId)
//                {
//                    m_ItemData = value.Value;
//                    equipIndex = value.Key;
//                    break;
//                }
//            }

//            if (m_ItemData == null)
//            {
//                return false;
//            }

//            ////获取已背包物品信息
//            PackageModule packageModule = App.my.localPlayer.GetModule<PackageModule>();
//            if (packageModule == null)
//                return false;
//            //查找背包中的物品数量
//            //int itemNum = packageModule.packageMgr.GetItemCount(equipId);

//            //获取配置表信息
//            Config.EquipPrototype data = Config.EquipPrototype.Get(equipId);
//            if (data == null)
//                return false;

//            //检查当前强化等级是否符合条件
//            if (m_ItemData.equipdata.awaken_status == 1)
//            {
//                //检查觉醒等级是否已经达到最大值
//                if (m_ItemData.equipdata.awaken_enhance >= 20 || m_ItemData.equipdata.awaken_enhance + 1 > data.awake)
//                {
//                    Debuger.Log("觉醒强化等级已经达到最大值或者觉醒强化等级超过最大限制");
//                    return false;
//                }

//                //检查觉醒强化材料是否足够
//                Config.AwakeInforceTab infTbl = Config.AwakeInforceTab.Get(m_ItemData.equipdata.awaken_enhance + 1);
//                if (infTbl == null)
//                {
//                    Debuger.LogError("infTbl == null");
//                    return false;
//                }
//                if (infTbl.materialCostCount > packageModule.packageMgr.GetItemCount(infTbl.materialCostId))
//                {
//                    Debuger.Log("觉醒强化材料不足");
//                    return false;
//                }
//            }

//            //检查强化等级是否符合配置
//            if (m_ItemData.equipdata.enhance + 1 > data.InforceValue && data.InforceValue != 0)
//            {
//                Debuger.Log("强化等级超过最大限制");
//                return false;
//            }

//            if (m_ItemData.equipdata.awaken_status == 0 && m_ItemData.equipdata.enhance + 1 == 20)
//            {
//                int materNum = 0;
//                int materId = 0;
//                ArrayList list = ParsingStr(data.awakeMater, "|", "");
//                if (list.Count == 0)
//                {
//                    //该装备不可以进行觉醒
//                    materId = 0;
//                    materNum = 0;
//                    Debuger.Log("该装备不可进行觉醒");
//                }
//                else if (list.Count == 2)
//                {
//                    materId = int.Parse((string)list[0]);
//                    materNum = int.Parse((string)list[1]);
//                }

//                if (materNum > packageModule.packageMgr.GetItemCount(materId))
//                {
//                    Debuger.Log("强化材料不足");
//                    return false;
//                }
//            }

//            if (m_ItemData.equipdata.awaken_status == 0 && m_ItemData.equipdata.enhance + 1 < 20)
//            {
//                //检查装备类型
//                int inforType = 0;
//                inforType = data.sonType;

//                if (inforType == 1)   //武器装备
//                {
//                    Config.EquipInforceTab infTbl = Config.EquipInforceTab.Get(m_ItemData.equipdata.awaken_enhance + 1);
//                    if (infTbl == null)
//                    {
//                        Debuger.LogError("infTbl == null");
//                        return false;
//                    }
//                    if (infTbl.materialCostCount > packageModule.packageMgr.GetItemCount(infTbl.materialCostId))
//                    {
//                        Debuger.Log("武器装备强化材料不足");
//                        return false;
//                    }
//                }
//                else if (inforType >= 2 && inforType <= 6)   //防具装备
//                {
//                    Config.ArmorInforceTab infTbl = Config.ArmorInforceTab.Get(m_ItemData.equipdata.awaken_enhance + 1);
//                    if (infTbl == null)
//                    {
//                        Debuger.LogError("infTbl == null");
//                        return false;
//                    }
//                    if (infTbl.materialCostCount > packageModule.packageMgr.GetItemCount(infTbl.materialCostId))
//                    {
//                        Debuger.Log("防具装备强化材料不足");
//                        return false;
//                    }
//                }
//                else if (inforType >= 7 && inforType <= 9)   //饰品装备
//                {
//                    Config.AccessoryInforceTab infTbl = Config.AccessoryInforceTab.Get(m_ItemData.equipdata.awaken_enhance + 1);
//                    if (infTbl == null)
//                    {
//                        Debuger.LogError("infTbl == null");
//                        return false;
//                    }
//                    if (infTbl.materialCostCount > packageModule.packageMgr.GetItemCount(infTbl.materialCostId))
//                    {
//                        Debuger.Log("饰品装备强化材料不足");
//                        return false;
//                    }
//                }
//                else
//                {
//                    Debuger.Log("装备子类型配置错误");
//                    return false;
//                }
//            }

//            return true;
//        }

//        //点击强化按钮检查生成的条件
//        public bool CheckRefConditions(int equipId, ref int equipIndex, int refType)
//        {
//            ////获取已穿戴装备信息
//            EqupMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr;
//            if (equipMgr == null)
//                return false;
//            EquipGrids m_Table = equipMgr.m_Table;
//            if (m_Table == null)
//            {
//                return false;
//            }

//            ItemData m_ItemData = new ItemData();

//            foreach (var value in m_Table.euqips)
//            {
//                if (value.Value.id == equipId)
//                {
//                    m_ItemData = value.Value;
//                    equipIndex = value.Key;
//                    break;
//                }
//            }

//            if (m_ItemData == null)
//            {
//                return false;
//            }

//            ////获取已背包物品信息
//            PackageModule packageModule = App.my.localPlayer.GetModule<PackageModule>();
//            if (packageModule == null)
//                return false;

//            //获取配置表信息
//            Config.EquipPrototype data = Config.EquipPrototype.Get(equipId);
//            if (data == null)
//                return false;

//            int materId = 1;
//            int costNum = packageModule.packageMgr.GetItemCount(materId);

//            RefinePrototype refdata = RefinePrototype.Get(refType);
//            if (refdata != null)
//            {
//                if (refdata.copperCostCount > costNum)
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        void EquipInforce(InforceEquipMsg msg)
//        {
//            this.m_C2WEquipRequest.InforceEquip(msg, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<Bool>(respone))
//                    return;

//                if (respone.value != true)
//                {
//                    //装备不能强化
//                    return;
//                }

//                NetProto.Int32 syncMsg = new NetProto.Int32();
//                syncMsg.value = 1;
//                App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Equip_SyncData, syncMsg);

//                //通知UI刷新(暂时不需要)
//                //App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//            });
//        }

//        void EquipRefine(RefineryEquipMsg request)
//        {
//            this.m_C2WEquipRequest.RefineryEquip(request, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<RefineryResult>(respone))
//                    return;
//                if (respone.result == (int)ReturnCode.enRefinerFaild)
//                {
//                    //装备不能炼化
//                    return;
//                }
//                //通知UI刷新
//                //App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//                NetProto.Int32 syncMsg = new NetProto.Int32();
//                syncMsg.value = 1;
//                App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Equip_SyncData, syncMsg);
//            });
//        }
        
//        void ReplaceRefine(NetProto.Int32 request)
//        {
//            this.m_C2WEquipRequest.ReplaceRefPry(request, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<NetProto.Bool>(respone))
//                    return;
//                if (respone.value == false)
//                {
//                    //装备不能炼化
//                    return;
//                }
//                //通知UI刷新
//                //App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//                NetProto.Int32 syncMsg = new NetProto.Int32();
//                syncMsg.value = 1;
//                App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Equip_SyncData, syncMsg);
//            });
//        }

//        void EquipCreate(NetProto.Int32 equipId)
//        {
//            this.m_C2WEquipRequest.CreateEquip(equipId, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<Bool>(respone))
//                    return;
//                //通知UI刷新
//                //App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//            });
//        }
        
//        void LoadEquip(NetProto.Int32 equipKey)
//        {
//            this.m_C2WEquipRequest.LoadEquip(equipKey, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<Bool>(respone))
//                    return;
//                NetProto.Int32 syncMsg = new NetProto.Int32();
//                syncMsg.value = 1;
//                App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Equip_SyncData, syncMsg);
//                //通知UI刷新
//                //App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//            });
//        }
        
//        void UnLoadEquip(NetProto.Int32 equipKey)
//        {
//            this.m_C2WEquipRequest.UnLoadEquip(equipKey, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<Bool>(respone))
//                    return;
//                NetProto.Int32 syncMsg = new NetProto.Int32();
//                syncMsg.value = 1;
//                App.my.eventSet.FireEvent<NetProto.Int32>(EventID.Equip_SyncData, syncMsg);
//                //通知UI刷新
//                //App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//            });
//        }

//        void SyncEquipList(NetProto.Int32 equipId)
//        {
//            this.m_C2WEquipRequest.SyncEquipData(equipId, (error, respone) =>
//            {
//                if (error != wProtobuf.RPC.Error.Success)
//                    return;
//                if (NetReturnCode.isError<EquipGrids>(respone))
//                    return;
//                //修改客户端部分的已装备列表数据
//                EqupMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr;
//                if (equipMgr == null)
//                    return;
//                equipMgr.RefreshData(respone);
//                //通知UI刷新
//                App.my.eventSet.fireEvent(EventID.Equip_RefreshUI);
//                App.my.eventSet.fireEvent(EventID.Package_UpdateEquip);
//            });
//        }
//    }
//}

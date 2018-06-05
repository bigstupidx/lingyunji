#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using System.Collections.Generic;
    using Config;
    using xys.UI;
    using NetProto;
    using System.Linq;

    class ItemTipsPanel : HotPanelBase
    {
        public class Param
        {
            public int itemId;
        }

        [SerializeField]
        EquipTipsItem equipGrid; // 已装备的项

        [SerializeField]
        EquipTipsItem grid; // 正常格子的项

        [SerializeField]
        FunTipsItem funTips; // 功能物品项

        [SerializeField]
        SoulTipsItem soulTips; // 魂魄物品项

        [SerializeField]
        PetTipsItem petTips; // 灵魂物品项

        [SerializeField]
        TrumpTipsItem trumpTips; // 灵魂物品项

        [SerializeField]
        PopupItems popupItems; // 功能按钮

        [SerializeField]
        ObtainTips obtainTips; // 获取途径

        ItemTipsPanel() : base(null) { }

        ItemTipsPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            equipGrid.OnInit(); 
            grid.OnInit();
            funTips.OnInit();
            soulTips.OnInit();
            petTips.OnInit();
            trumpTips.OnInit();
            popupItems.OnInit();
            obtainTips.OnInit();
        }

        protected override void OnShow(object args)
        {
            equipGrid.root.SetActive(false);
            grid.root.SetActive(false);
            funTips.root.SetActive(false);
            soulTips.root.SetActive(false);
            petTips.root.SetActive(false);
            trumpTips.root.SetActive(false);
            popupItems.root.SetActive(false);
            obtainTips.root.SetActive(false);

            if(args is InitItemTipsData)
            {
                InitItemTipsData tipsData = args as InitItemTipsData;
                if (tipsData != null)
                {
                    switch (tipsData.type)
                    {
                        case xys.UI.InitItemTipsData.Type.Package:
                        case xys.UI.InitItemTipsData.Type.Mail:
                            SetPackageTips(tipsData.itemData, tipsData.index, tipsData.m_BagType);
                            break;
                        case InitItemTipsData.Type.CommonEquipTips:
                            SetEquipTips(tipsData.itemData.data);
                            break;
                        case InitItemTipsData.Type.EquipPrototypeTips:
                            SetEquipPrototypeTips(tipsData.itemData.data.id);
                            break;
                        case InitItemTipsData.Type.Compound:
                        case InitItemTipsData.Type.CommonTips:
                            SetCommonTips(tipsData.itemData);
                            break;
                    }
                }
            }
            if(args is Param)
            {
                Param param = args as Param;
                if (param != null)
                {
                    int itemId = param.itemId;
                    this.OnItemTips(itemId);
                }
            }
        }

        #region zjh
        void OnItemTips(int itemId)
        {
            ItemBase itemData = ItemBaseAll.Get(itemId);
            if (itemData == null)
            {
                App.my.uiSystem.HidePanel(PanelType.UIItemTipsPanel, false);
                return;
            }
            this.SetItemTips(itemData);
        }

        void SetItemTips(ItemBase data)
        {
            Item itemConfig = Item.Get(data.id);
            if (itemConfig == null)
                return;
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfig.quality);
            if (qualitConfig == null)
                return;

            string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);

            FunTipsData ftd = new FunTipsData();
            ftd.name = GlobalSymbol.ToUT(nameStr);
            ftd.icon = itemConfig.icon;
            ftd.isBind = data.isBind;
            ftd.type = "消耗品";
            ftd.quality = qualitConfig.tips;
            ftd.funIntro = itemConfig.use;
            ftd.desc = itemConfig.desc;
            if (itemConfig.type == ItemType.consumables && itemConfig.sonType == 5) //秘籍
            {
                string addDesc = App.my.localPlayer.GetModule<SkillModule>().IsItemComprehend(itemConfig.id) ? "#[O]已领悟#n" : "#[G2]未领悟#n";
                ftd.desc = addDesc + "\n" + ftd.desc;
            }
            if (itemConfig.limitNum > 0)
                ftd.desc = ftd.desc + "\n" + "可用次数 " + itemConfig.limitNum.ToString();
            ftd.levelLimit = CheckLevel(itemConfig.useLevel, "");
            ftd.isShowGetType = false;
            funTips.Set(ftd);
        }
        #endregion

        // 包裹道具tips
        void SetPackageTips(NetProto.ItemGrid itemData, int index, BagType type)
        {
            ItemBase itemConfig = ItemBaseAll.Get(itemData.data.id);
            if (itemConfig == null)
                return;

            switch (itemConfig.type)
            {
            case ItemType.consumables:
                SetItemTips(itemData.data);
                SetItemFuncBtn(itemData, index, type);
                break;
            case ItemType.task:
                SetTaskItemTips(itemData.data);
                break;
            case ItemType.money:
//                SetMoneyTips(Item.Get(itemData.data.id));
                break;
            case ItemType.equip:
                if (type == BagType.item && itemConfig.job.Has((RoleJob.Job)App.my.localPlayer.carrerValue))
                    EquipCompared(itemData.data);
                SetEquipTips(itemData.data);
                SetEquipFuncBtn(itemData, index, type);
                break;
            }
        }

        // 通用道具tips
        void SetCommonTips(NetProto.ItemGrid itemData)
        {
            ItemBase itemConfig = ItemBaseAll.Get(itemData.data.id);
            if (itemConfig == null)
                return;

            switch (itemConfig.type)
            {
                case ItemType.consumables:
                case ItemType.task:
                    SetItemTips(itemConfig);
                    break;
                case ItemType.money:

                    break;
                case ItemType.equip:
                    SetEquipTips(itemData.data);
                    break;
            }
        }

        // 物品tips
        void SetItemTips(NetProto.ItemData itemData)
        {
            Item itemConfig = Item.Get(itemData.id);
            if (itemConfig == null)
                return;
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfig.quality);
            if (qualitConfig == null)
                return;

            string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);

            FunTipsData ftd = new FunTipsData();
            ftd.name = GlobalSymbol.ToUT(nameStr);
            ftd.icon = itemConfig.icon;
            ftd.isBind = itemData.GetFlag(NetProto.ItemData.Flag.isBind);
            ftd.type = "消耗品";
            ftd.quality = qualitConfig.tips;
            ftd.funIntro = itemConfig.use;
            ftd.desc = itemConfig.desc;
            if (itemConfig.type == ItemType.consumables && itemConfig.sonType == 5) //秘籍
            {
                string addDesc = App.my.localPlayer.GetModule<SkillModule>().IsItemComprehend(itemConfig.id) ? "#[O]已领悟#n" : "#[G2]未领悟#n";
                ftd.desc = addDesc + "\n" + ftd.desc;
            }
            if (itemConfig.limitNum > 0)
                ftd.desc = ftd.desc + "\n" + "可用次数 " + itemConfig.limitNum.ToString();
            int strLevel = itemConfig.useLevel;
            if (strLevel == 0)
                strLevel = 1;
            ftd.levelLimit = CheckLevel(itemConfig.useLevel, "");
            ftd.isShowGetType = false;
            funTips.Set(ftd);
        }

        // 货币tips
        void SetMoneyTips(Item itemConfig)
        {
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfig.quality);
            if (qualitConfig == null)
                return;

            string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);

            FunTipsData ftd = new FunTipsData();
            ftd.name = GlobalSymbol.ToUT(nameStr);
            ftd.icon = itemConfig.icon;
            ftd.type = "货币";
            ftd.quality = qualitConfig.tips;
            ftd.funIntro = itemConfig.use;
            ftd.desc = itemConfig.desc;
            if (itemConfig.limitNum > 0)
                ftd.desc = ftd.desc + "\n" + "可用次数 " + itemConfig.limitNum.ToString();
            int strLevel = itemConfig.useLevel;
            if (strLevel == 0)
                strLevel = 1;
            ftd.levelLimit = CheckLevel(itemConfig.useLevel, "");
            ftd.isShowGetType = false;
            funTips.Set(ftd);
        }

        // 任务物品tips
        void SetTaskItemTips(NetProto.ItemData itemData)
        {
            TaskItem itemConfig = TaskItem.Get(itemData.id);
            if (itemConfig == null)
                return;
            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(itemConfig.quality);
            if (qualitConfig == null)
                return;

            string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);

            FunTipsData ftd = new FunTipsData();
            ftd.name = GlobalSymbol.ToUT(nameStr);
            ftd.icon = itemConfig.icon;
            ftd.isBind = itemData.GetFlag(NetProto.ItemData.Flag.isBind);
            ftd.type = "任务道具";
            ftd.quality = qualitConfig.tips;
            ftd.funIntro = itemConfig.use;
            ftd.desc = itemConfig.desc;

            int strLevel = itemConfig.useLevel;
            if (strLevel == 0)
                strLevel = 1;
            ftd.levelLimit = CheckLevel(itemConfig.useLevel, "");
            ftd.isShowGetType = false;
            funTips.Set(ftd);
        }

        // 背包装备tips
        void SetEquipTips(NetProto.ItemData itemData)
        {
            EquipMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            if (equipMgr == null)
                return;

            EquipPrototype equipConfig = EquipPrototype.Get(itemData.id);
            if (equipConfig == null)
                return;
            ItemBase itemConfig = ItemBaseAll.Get(itemData.id);
            if (itemConfig == null)
                return;

            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(equipConfig.quality);
            EquipTipsData tipsData = new EquipTipsData();

            string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);
            tipsData.icon = equipConfig.icon;
            tipsData.quality = qualitConfig.tips;
            tipsData.name = GlobalSymbol.ToUT(nameStr);
            tipsData.type = string.Format("【{0}】", grid.GetEquipPartStr(equipConfig.sonType));
            tipsData.levelLimit = CheckLevel(equipConfig.leve, "等级: ");

            if (itemData.equipdata != null)
            {
                EquipBasicData equipBasicData = itemData.equipdata.equipBasicData;
                // 强化等级
                tipsData.strengthlevel = equipBasicData.enforceLv;
                tipsData.strengthMaxLevel = equipConfig.InforceValue;
                // 是否可重铸
                tipsData.isCanReset = equipConfig.isCanRecast;
                // 是否可炼化
                tipsData.isCanRefine = equipConfig.isCanRefine;
                // 耐久度(暂时采用配置)
                tipsData.currentDurable = equipConfig.enduLevel;
                tipsData.maxDurable = equipConfig.enduLevel;
                // 装备评分(暂时采用默认值)
                tipsData.score = 100.ToString();
                // 职业限制
                tipsData.jobLimit = SetEquipTipsJobStr(itemConfig);

                // 基础属性
                foreach (var attr in itemData.equipdata.equipBasicData.baseAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.Key);
                    if (attrConfig == null)
                        continue;
                    double value = EquipCfgMgr.GetPropertValInTotal(itemData.id, equipBasicData.nSubType, attr.Value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    string attrStr = attrConfig.attrNameByOtherUI + " +" + value.ToString();
                    tipsData.baseAtts.Add(attrStr);
                }
                //定制属性
                foreach (var attr in itemData.equipdata.equipBasicData.customAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.Key);
                    if (attrConfig == null)
                        continue;
                    double value = EquipCfgMgr.GetPropertValInTotal(itemData.id, equipBasicData.nSubType, attr.Value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    string attrStr = attrConfig.attrNameByOtherUI + " +" + value.ToString();
                    tipsData.randomAtts.Add(attrStr);
                }
                // 随机属性
                foreach (var attr in itemData.equipdata.equipBasicData.randomAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.Key);
                    if (attrConfig == null)
                        continue;
                    double value = EquipCfgMgr.GetPropertValInTotal(itemData.id, equipBasicData.nSubType, attr.Value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    string attrStr = attrConfig.attrNameByOtherUI + " +" + value.ToString();
                    tipsData.randomAtts.Add(attrStr);
                }
                // 基础属性特效
                foreach (var attr in equipBasicData.effectValues)
                {
                    var cfg = Config.EquipEffectPrototype.Get(attr.id);
                    if (cfg == null)
                    {
                        continue;
                    }
                    tipsData.effectAtts.Add(string.Format("特效: {0} +{1}", cfg.effectName, attr.level));
                }
                // 炼化属性
                foreach (var attr in equipBasicData.refAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.key);
                    if (attrConfig == null)
                        continue;
                    double value = attr.value;
                    string attrStr = attrConfig.attrNameByOtherUI + " +" + value.ToString();
                    tipsData.refineAtts.Add(attrStr);
                }
                // 附魂属性
                // todo:
                //
            }

            var equipList = equipMgr.GetAllEquips();
            if (equipList.ContainsKey(equipConfig.sonType))
            {
                if (itemData == equipList[equipConfig.sonType])
                    grid.SetWearFlagImage(true);
                else
                    grid.SetWearFlagImage(false);
            }

            grid.Set(tipsData);

        }
        //装备原型Tips
        void SetEquipPrototypeTips(int equipID)
        {
            EquipPrototype equipConfig = EquipPrototype.Get(equipID);
            if (equipConfig != null)
            {
                ItemBase itemConfig = ItemBaseAll.Get(equipID);
                if (itemConfig == null)
                    return;

                QualitySourceConfig qualitConfig = QualitySourceConfig.Get(equipConfig.quality);
                EquipTipsData tipsData = new EquipTipsData();

                string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);
                tipsData.icon = equipConfig.icon;
                tipsData.quality = qualitConfig.tips;
                tipsData.name = GlobalSymbol.ToUT(nameStr);
                tipsData.type = string.Format("【{0}】", grid.GetEquipPartStr(equipConfig.sonType));
                tipsData.levelLimit = CheckLevel(equipConfig.leve, "等级: ");
 
                // 强化等级
                tipsData.strengthlevel = 0;
                tipsData.strengthMaxLevel = equipConfig.InforceValue;
                // 是否可重铸
                tipsData.isCanReset = equipConfig.isCanRecast;
                // 是否可炼化
                tipsData.isCanRefine = equipConfig.isCanRefine;
                // 耐久度(暂时采用配置)
                tipsData.currentDurable = equipConfig.enduLevel;
                tipsData.maxDurable = equipConfig.enduLevel;
                // 装备评分(暂时采用默认值)
                tipsData.score = 100.ToString();
                // 职业限制
                tipsData.jobLimit = SetEquipTipsJobStr(itemConfig);

                // 基础属性
                foreach (var key in equipConfig.battleAttri.GetKeys())
                {
                    double min, max;
                    EquipCfgMgr.GetBaseAttrRange(equipID, equipConfig.sonType, key, 0, 0, false,out min,out max);
                    string attr = string.Format("{0} {1}～{2}",Config.AttributeDefine.Get(key).attrNameByOtherUI, min, max);
                    tipsData.baseAtts.Add(attr);
                }
                // 随机属性数量
                int randomAttrMaxNum = EquipCfgMgr.GetRandomAttrMaxNum(equipConfig.randomAttributeNum);
                // 定制属性数量
                if (equipConfig.customAttributeId != 0)
                    randomAttrMaxNum += Config.CustomPrototypeLibrary.Get(equipConfig.customAttributeId).battleAttri.GetKeys().Count;
                if (randomAttrMaxNum > 0)
                    tipsData.randomAtts.Add(string.Format("随机属性 {0}～{1}条", 0, randomAttrMaxNum));
                // 基础属性特效
                int effectNum = 0;
                if (equipConfig.effecPro1 != 0)
                    effectNum++;
                if (equipConfig.effecPro2 != 0)
                    effectNum++;
                if (effectNum > 0)
                    tipsData.effectAtts.Add(string.Format("特效条目 {0}～{1}条", 0, effectNum));
                // 炼化属性

                // 附魂属性
                // todo:
                //
                grid.SetWearFlagImage(false);
                grid.Set(tipsData);
            }
        }

        // 装备对比
        void EquipCompared(NetProto.ItemData itemData)
        {
            EquipMgr equipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            if (equipMgr == null)
                return;

            EquipPrototype equipConfig = EquipPrototype.Get(itemData.id);
            if (equipConfig == null)
                return;

            var equipList = equipMgr.GetAllEquips();
            if (!equipList.ContainsKey(equipConfig.sonType))
                return;

            NetProto.ItemData inUseItemData = equipList[equipConfig.sonType];

            EquipPrototype equipCompared = EquipPrototype.Get(inUseItemData.id);
            if (equipCompared == null)
                return;

            ItemBase itemConfig = ItemBaseAll.Get(inUseItemData.id);
            if (itemConfig == null)
                return;

            QualitySourceConfig qualitConfig = QualitySourceConfig.Get(equipCompared.quality);
            EquipTipsData tipsData = new EquipTipsData();

            string nameStr = string.Format("#[{0}]{1}#n", qualitConfig.colorname, itemConfig.name);
            tipsData.icon = equipCompared.icon;
            tipsData.quality = qualitConfig.tips;
            tipsData.name = GlobalSymbol.ToUT(nameStr);
            tipsData.type = string.Format("【{0}】", grid.GetEquipPartStr(equipConfig.sonType));
            tipsData.levelLimit = CheckLevel(equipCompared.leve, "等级: ");

            if (inUseItemData.equipdata != null)
            {
                EquipBasicData equipBasicData = inUseItemData.equipdata.equipBasicData;
                // 强化等级
                tipsData.strengthlevel = equipBasicData.enforceLv;
                tipsData.strengthMaxLevel = equipCompared.InforceValue;
                // 是否可重铸
                tipsData.isCanReset = equipCompared.isCanRecast;
                // 是否可炼化
                tipsData.isCanRefine = equipCompared.isCanRefine;
                // 耐久度(暂时采用配置)
                tipsData.currentDurable = equipCompared.enduLevel;
                tipsData.maxDurable = equipCompared.enduLevel;
                // 装备评分(暂时采用默认值)
                tipsData.score = 100.ToString();

                // 职业限制
                tipsData.jobLimit = SetEquipTipsJobStr(itemConfig);

                // 基础属性
                foreach (var attr in equipBasicData.baseAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.Key);
                    if (attrConfig == null)
                        continue;
                    if (itemData.equipdata == null)
                        continue;
                    double value = EquipCfgMgr.GetPropertValInTotal(itemData.id, equipBasicData.nSubType, attr.Value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    string attrStr = attrConfig.name + " +" + value.ToString();
                    tipsData.baseAtts.Add(attrStr);
                }
                // 随机属性
                foreach (var attr in itemData.equipdata.equipBasicData.randomAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.Key);
                    if (attrConfig == null)
                        continue;
                    double value = EquipCfgMgr.GetPropertValInTotal(itemData.id, equipBasicData.nSubType, attr.Value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    string attrStr = attrConfig.attrNameByOtherUI + " +" + value.ToString();
                    tipsData.randomAtts.Add(attrStr);
                }
                // 基础属性特效
                foreach (var attr in equipBasicData.effectValues)
                {
                    var cfg = Config.EquipEffectPrototype.Get(attr.id);
                    if (cfg == null)
                    {
                        continue;
                    }
                    tipsData.effectAtts.Add(string.Format("特效: {0} +{1}", cfg.effectName, attr.level));
                }
                // 炼化属性
                foreach (var attr in equipBasicData.refAtts)
                {
                    AttributeDefine attrConfig = AttributeDefine.Get(attr.key);
                    if (attrConfig == null)
                        continue;
                    double value = EquipCfgMgr.GetPropertValInTotal(itemData.id, equipBasicData.nSubType, attr.value, equipBasicData.enforceLv, equipBasicData.awakenEnforceLV, equipBasicData.awakenStatus);
                    string attrStr = attrConfig.name + " +" + value.ToString();
                    tipsData.refineAtts.Add(attrStr);
                }
                // 附魂属性
                // todo:
                //
            }

            equipGrid.SetWearFlagImage(true);

            equipGrid.Set(tipsData);
        }

        // 道具tips功能按钮
        void SetItemFuncBtn(NetProto.ItemGrid itemData, int index, BagType type)
        {
            if (BagType.mail == type)
                return;

            Item itemConfig = Item.Get(itemData.data.id);
            if (itemConfig == null)
                return;

            ItemFunction itemFunc = new ItemFunction();
            List<PopupData> btnList = new List<PopupData>();

            if (type == BagType.item && itemConfig.isCanUse == true)
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "使 用";
                ItemFuncObject reject = new ItemFuncObject();
                reject.GridIndex = index;
                reject.itemId = itemData.data.id;
                reject.itemCount = 1;
                useBtn.action = () =>
                {
                    itemFunc.ItemUseFuncBtn(reject);
                };
                btnList.Add(useBtn);
            }

            if (type == BagType.item && itemConfig.allUse == true && itemData.count > 1)
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "全部使用";
                ItemFuncObject reject = new ItemFuncObject();
                reject.GridIndex = index;
                useBtn.action = () =>
                {
                    itemFunc.ItemUseAll(reject);
                };
                btnList.Add(useBtn);
            }

            // 装备治疗药品
            if (type == BagType.item && itemConfig.sonType == (int)ItemChildType.cure)
            {
                PopupData useBtn = new PopupData();
                if (App.my.localPlayer.bloodBottleValue == itemData.data.id)
                    useBtn.name = "卸 下";
                else
                    useBtn.name = "装 备";
                ItemFuncObject obj = new ItemFuncObject();
                if (App.my.localPlayer.bloodBottleValue == itemData.data.id)
                    obj.itemId = 0;
                else
                    obj.itemId = itemData.data.id;
                useBtn.action = () =>
                {
                    itemFunc.EquipCureItem(obj);
                };
                btnList.Add(useBtn);
            }

            if (type == BagType.item && itemConfig.decProduc != 0)
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "分 解";
                ItemFuncObject obj = new ItemFuncObject();
                obj.GridIndex = index;
                useBtn.action = () =>
                {
                    itemFunc.DecomposeItem(obj);
                };
                btnList.Add(useBtn);
            }

            if (type == BagType.item && itemConfig.comId != 0)
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "合 成";
                ItemFuncObject obj = new ItemFuncObject();
                obj.itemId = itemData.data.id;
                useBtn.action = () =>
                {
                    itemFunc.CompoundItem(obj);
                };
                btnList.Add(useBtn);
            }

            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (type == BagType.item && packageMgr.CanCombineItem(itemData.data.id))
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "合 并";
                ItemFuncObject reject = new ItemFuncObject();
                if (!Item.Get(itemData.data.id).isBind)
                    reject.itemId = itemData.data.id;
                else if (Item.Get(itemData.data.id).unbindId != 0)
                    reject.itemId = Item.Get(itemData.data.id).unbindId;
                useBtn.action = () =>
                {
                    itemFunc.RelationItem(reject);
                };
                btnList.Add(useBtn);
            }

            if (type == BagType.item && itemConfig.isConsign)
            {
                PopupData sellBtn = new PopupData();
                sellBtn.name = "寄 售";
                ItemFuncObject sellFunc = new ItemFuncObject();
                sellFunc.GridIndex = index;
                sellBtn.action = () =>
                {
                    itemFunc.SellToStore(sellFunc);
                };
                btnList.Add(sellBtn);
            }

            if (type == BagType.item && !itemConfig.IsCanSell)
            {
                PopupData sellBtn = new PopupData();
                sellBtn.name = "出 售";
                ItemFuncObject sellFunc = new ItemFuncObject();
                sellFunc.GridIndex = index;
                sellFunc.itemId = itemData.data.id;
                sellBtn.action = () =>
                {
                    itemFunc.SellItem(sellFunc);
                };
                btnList.Add(sellBtn);
            }

            if (type == BagType.temp)
            {
                PopupData rejectBtn = new PopupData();
                rejectBtn.name = "取 回";
                ItemFuncObject reject = new ItemFuncObject();
                reject.GridIndex = index;
                rejectBtn.action = () =>
                {
                    itemFunc.GetItemFromTempPackage(reject);
                };
                btnList.Add(rejectBtn);
            }

            if (!itemConfig.IsCanLost)
            {
                PopupData rejectBtn = new PopupData();
                rejectBtn.name = "丢 弃";
                ItemFuncObject reject = new ItemFuncObject();
                reject.GridIndex = index;
                reject.PackageType = type;
                rejectBtn.action = () =>
                {
                    itemFunc.ItemRejectFuncBtn(reject);
                };
                btnList.Add(rejectBtn);
            }

            if (btnList.Count == 0)
                return;

            popupItems.Set(btnList);
        }

        // 装备tips功能按钮
        void SetEquipFuncBtn(NetProto.ItemGrid itemData, int index, BagType type)
        {
            EquipPrototype equipConfig = EquipPrototype.Get(itemData.data.id);
            if (equipConfig == null)
                return;

            ItemFunction itemFunc = new ItemFunction();
            List<PopupData> btnList = new List<PopupData>();

            if (type == BagType.item)
            {
                PopupData equipBtn = new PopupData();
                equipBtn.name = "装 备";
                ItemFuncObject obj = new ItemFuncObject();
                obj.GridIndex = index;
                equipBtn.action = () =>
                {
                    itemFunc.EquipEquipment(obj);
                };
                btnList.Add(equipBtn);
            }
            else if (type != BagType.temp)
            {
                PopupData equipBtn = new PopupData();
                equipBtn.name = "卸 下";
                ItemFuncObject obj = new ItemFuncObject();
                obj.itemId = itemData.data.id;
                equipBtn.action = () =>
                {
                    itemFunc.RemoveEquioment(obj);
                };
                btnList.Add(equipBtn);
            }

            if (type != BagType.temp && 
                (equipConfig.isCanRefine || equipConfig.isCanRecast || equipConfig.isCanRefine || equipConfig.InforceValue > 0))
            {
                PopupData sellBtn = new PopupData();
                sellBtn.name = "炼 器";
                ItemFuncObject sellFunc = new ItemFuncObject();
                sellFunc.GridIndex = index;
                sellBtn.action = () =>
                {
                    itemFunc.RefinEquiment(sellFunc);
                };
                btnList.Add(sellBtn);
            }

            if (type == BagType.item && equipConfig.decProduc != 0 && equipConfig.quality > ItemQuality.purple)
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "分 解";
                ItemFuncObject obj = new ItemFuncObject();
                obj.GridIndex = index;
                useBtn.action = () =>
                {
                    itemFunc.DecomposeItem(obj);
                };
                btnList.Add(useBtn);
            }

            if (type == BagType.item && equipConfig.decProduc != 0 && equipConfig.quality < ItemQuality.Orange)
            {
                PopupData useBtn = new PopupData();
                useBtn.name = "一键分解";
                ItemFuncObject obj = new ItemFuncObject();
                obj.GridIndex = index;
                useBtn.action = () =>
                {
                    itemFunc.DecomposeEquips(obj);
                };
                btnList.Add(useBtn);
            }

            if (type == BagType.item && !equipConfig.IsCanSell)
            {
                PopupData sellBtn = new PopupData();
                sellBtn.name = "出 售";
                ItemFuncObject sellFunc = new ItemFuncObject();
                sellFunc.GridIndex = index;
                sellBtn.action = () =>
                {
                    itemFunc.SellEquiment(sellFunc);
                };
                btnList.Add(sellBtn);
            }

            if (type == BagType.temp)
            {
                PopupData rejectBtn = new PopupData();
                rejectBtn.name = "取 回";
                ItemFuncObject reject = new ItemFuncObject();
                reject.GridIndex = index;
                rejectBtn.action = () =>
                {
                    itemFunc.GetItemFromTempPackage(reject);
                };
                btnList.Add(rejectBtn);
            }

            popupItems.Set(btnList);
        }

        string CheckLevel(int level, string str)
        {
            string strLevel = "";
            if (level == 0)
                level = 1;
            if (App.my.localPlayer.levelValue < level)
                strLevel = string.Format("#[R]{0}{1}#n", str, level);
            else
                strLevel = str + level.ToString();
            return GlobalSymbol.ToUT(strLevel);
        }

        string SetEquipTipsJobStr(ItemBase config)
        {
            string str = "门派: ";
            if (config.job.Has(RoleJob.Job.All))
                return str + "全门派";
            else if (config.job.Has((RoleJob.Job)App.my.localPlayer.carrerValue))
                return str + config.job.info;
            else
                return GlobalSymbol.ToUT(string.Format("#[R]{0}#n", str + config.job.info));
        }
    }
}
#endif
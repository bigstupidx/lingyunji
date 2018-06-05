#if !USE_HOT
namespace xys.hot
{
    using Config;
    using NetProto;
    using NetProto.Hot;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    class EquipMgr
    {
        public EquipGrids equipTable = new EquipGrids();  //装备中的数据以subtype为key
        public bool isOperationTimeActive
        {
            get;private set;
        }
        public Dictionary<int, ItemData> GetAllEquips()
        {
            return equipTable.equipDic;
        }

        public void WearEquip(ItemData data)
        {
            int subType = data.equipdata.equipBasicData.nSubType;
            if (equipTable.equipDic.ContainsKey(subType))
            {
                equipTable.equipDic[subType] = data;
            }
            else
            {
                equipTable.equipDic.Add(subType, data);
                //equipTable.equipDic.Sort();
            }
        }

        public void TakeOffEquip(int subType)
        {
            if(equipTable.equipDic.ContainsKey(subType))
                equipTable.equipDic.Remove(subType);
        }

        public bool IsEquipedEmpty()
        {
            if (equipTable.equipDic.Count!=0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanAutoChangeEnforceVal(ItemData inUseItem, ItemData unUseItem)
        {
            if ((inUseItem.equipdata.equipBasicData.enforceLv > 0) && (0 == unUseItem.equipdata.equipBasicData.enforceLv))
            {
                int unUseItemEnforceLimit = Config.EquipPrototype.Get(unUseItem.id).InforceValue;
                if (inUseItem.equipdata.equipBasicData.enforceLv <= unUseItemEnforceLimit)
                {
                    if ((inUseItem.equipdata.equipBasicData.awakenStatus == false) && (unUseItem.equipdata.equipBasicData.awakenStatus == false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public ItemData GetEquipData(int subType)
        {
            if (equipTable.equipDic.ContainsKey(subType))
                return equipTable.equipDic[subType];
            else
                return null;
        }

        public bool isWearingEquip(int equipID)
        {
            var cfg = Config.EquipPrototype.Get(equipID);
            if (cfg != null)
            {
                var itemData = GetEquipData(cfg.sonType);
                if (itemData != null)
                {
                    return itemData.id == equipID;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public void ExChangeItemData(int subType,ItemData packageData, bool isAutoTransfer)
        {
            int enforceLv = equipTable.equipDic[subType].equipdata.equipBasicData.enforceLv;
            equipTable.equipDic.Remove(subType);
            Log.Debug(string.Format("Remove subType[{0}]", subType));
            if (isAutoTransfer)
                packageData.equipdata.equipBasicData.enforceLv = enforceLv;
            equipTable.equipDic.Add(subType, packageData);
            Log.Debug(string.Format("Add subType[{0}] to equip", packageData.id));
        }

        public void RelpaceRefAttrs(ItemData data)
        {
            data.equipdata.equipBasicData.refAtts = data.equipdata.equipBasicData.tempRefAtts;
            data.equipdata.equipBasicData.tempRefAtts = new List<RefAttr>();
        }
        public void RelpaceRecastAttrs(ItemData data)
        {
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            equipBasicData.baseAtts = equipBasicData.tempBaseAttsByRecast;
            equipBasicData.tempBaseAttsByRecast = new Dictionary<int, double>();
            equipBasicData.customAtts = equipBasicData.tempCustomAtts;
            equipBasicData.tempCustomAtts = new Dictionary<int, double>();
            equipBasicData.randomAtts = equipBasicData.tempRandomAtts;
            equipBasicData.tempRandomAtts = new Dictionary<int, double>();
            equipBasicData.effectValues = equipBasicData.tempEffectValues;
            equipBasicData.tempEffectValues = new List<EffectValue>();
        }
        public void RelpaceConciseAttrs(ItemData data)
        {
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            equipBasicData.baseAtts = equipBasicData.tempBaseAttsByConcise;
            equipBasicData.tempBaseAttsByConcise = new Dictionary<int, double>();
        }

        public void ReplaceTempRecastPropety(RecastingResult ret, ItemData data)
        {
            EquipBasicData equipBasicData = data.equipdata.equipBasicData;
            equipBasicData.tempBaseAttsByRecast = ret.baseAttrs;
            equipBasicData.tempCustomAtts = ret.customAttrs;
            equipBasicData.tempRandomAtts = ret.randomAttrs;
            equipBasicData.tempEffectValues = ret.effectValues;
        }
        public void ReplaceTempConsicePropety(RecastingResult ret, ItemData data)
        {
            data.equipdata.equipBasicData.tempBaseAttsByConcise = ret.baseAttrs;
        }
        public void ReplaceTempRefinePropety(RefineryResult ret, ItemData data)
        {
            data.equipdata.equipBasicData.tempRefAtts = ret.propertyData;
        }
        public void IncreaseEnforceLv(ItemData data)
        {
            if (data.equipdata.equipBasicData.awakenStatus)
            {
                data.equipdata.equipBasicData.awakenEnforceLV++;
            }
            else
                data.equipdata.equipBasicData.enforceLv++;
        }
        public void ResetInUseEquipOpTimes()
        {
            var itr = equipTable.equipDic.GetEnumerator();
            while (itr.MoveNext())
            {
                ResetEquipOpTimes(itr.Current.Value);
            }
        }
        public void SetOperationTimesActive(bool active)
        {
            isOperationTimeActive = active;
        }

        public void ResetEquipOpTimes(ItemData itemData)
        {
            EquipBasicData equipBasicData = itemData.equipdata.equipBasicData;
            Log.Debug(string.Format("reset equip:id:{0}|sonType:{1}| operation times:recastTimes:{2},refTimes:{3},consiceTimes:{4},",
                itemData.id, equipBasicData.nSubType, equipBasicData.recastTimes, equipBasicData.refTimes, equipBasicData.consiceTimes));
            equipBasicData.recastTimes = equipBasicData.refTimes = equipBasicData.consiceTimes = 0;
        }
    }
}
#endif
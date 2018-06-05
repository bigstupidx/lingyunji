using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
    public class EquipCfgMgr
    {
        /// <summary>
        /// 获取当前等级的强化材料信息
        /// </summary>
        /// <param name="subType"></param>
        /// <param name="enforceLv"></param>
        /// <param name="isAwake"></param>
        /// <returns></returns>
        public static void GetEquipEnforceMatData(int subType, int enforceLv, int awakeEnfoceLv, bool isAwake, out int id, out int amount)
        {
            if (!isAwake)
            {
                var cfg = Config.EquipInforceTab.Get(enforceLv + 1, subType);
                if (cfg != null)
                {
                    id = cfg.materialCostId;
                    amount = cfg.materialCostCount;
                    return;
                }
                else
                    XYJLogger.LogError(string.Format("enforceLv & subType undefined:{0},{1}", enforceLv + 1, subType));
            }
            else
            {
                var cfg = Config.AwakeInforceTab.Get(awakeEnfoceLv + 1);
                if (cfg != null)
                {
                    id = cfg.materialCostId;
                    amount = cfg.materialCostCount;
                    return;
                }
                else
                    XYJLogger.LogError("AwakeInforceTab awakeEnfoceLv undefined:" + (awakeEnfoceLv + 1));
            }
            id = amount = -1;
        }
        public static float GetEquipEnforceIncPercentage(int subType, int enforceLv, bool isAwake)
        {
            float ret = 0.0f;
            if (!isAwake)
            {
                var cfg = Config.EquipInforceTab.Get(enforceLv, subType);
                if (cfg != null)
                {
                    ret = cfg.increasePercent;
                    return ret;
                }
                else
                    XYJLogger.LogError(string.Format("enforceLv & subType undefined:{0},{1}", enforceLv, subType));
            }
            else
            {
                var cfg = Config.AwakeInforceTab.Get(enforceLv);
                if (cfg != null)
                {
                    ret = cfg.increasePercent;
                }
                else
                    XYJLogger.LogError("AwakeInforceTab enforceLv undefined:" + enforceLv);
            }
            return ret;
        }

        /// <summary>
        /// 根据ID，装备类型，属性值，强化值，是否觉醒，计算当前强化等级下的属性值
        /// </summary>
        /// <param name="equipID">ID</param>
        /// <param name="subType">装备类型</param>
        /// <param name="value">属性值</param>
        /// <param name="enforceLv">强化值</param>
        /// <param name="isAwake">是否觉醒</param>
        /// <returns>装备属性最终值</returns>
        public static double GetPropertValInTotal(int equipID,int subType, double value, int enforceLv, int awakenEnforceLV, bool isAwake)
        {
            double ret = value;
            
            for (int i = 1; i <= enforceLv; i++)
            {
                float percentage = GetEquipEnforceIncPercentage(subType, i, false);
                double addValue = GetPropertyAddVal(value, percentage);
                ret += addValue;
            }
            if (isAwake)
            {
                for (int i = 1; i <= awakenEnforceLV; i++)
                {
                    float percentage = GetEquipEnforceIncPercentage(subType, i, isAwake);
                    double addValue = GetPropertyAddVal(value, percentage);
                    ret += addValue;
                }
            }
            return ret;
        }

        public static double GetPropertyAddVal(double originVal, float percentage)
        {
            return Math.Ceiling(originVal * (double)(percentage / 100));
        }

        public static void GetBaseAttrRange(int equipID, int subType, int propertyID, int enforceLv, int awakenEnforceLV, bool isAwake,out double min,out double max)
        {

            //基础属性上线： 装备表-基础属性&&随机范围
            min = max = 0;

            var equipCfg = Config.EquipPrototype.Get(equipID);
            if (equipCfg ==null)
            {
                return;
            }
            double tempMin = Math.Ceiling(equipCfg.battleAttri.Get(propertyID) * equipCfg.basicAttributeRandomRange.m_Min);
            double tempMax = Math.Ceiling(equipCfg.battleAttri.Get(propertyID) * equipCfg.basicAttributeRandomRange.m_Max);
            min = GetPropertValInTotal(equipID,subType, tempMin, enforceLv, awakenEnforceLV, isAwake);
            max = GetPropertValInTotal(equipID,subType, tempMax, enforceLv, awakenEnforceLV, isAwake);
        }

        public static void GetCustomAttrRange(int equipID, int subType,  int propertyID, int enforceLv, int awakenEnforceLV, bool isAwake, out double min, out double max)
        {
            //定制属性上限： 定制属性表-属性值  装备表-随机范围
            min = max = 0;
            var equipCfg = Config.EquipPrototype.Get(equipID);
            if (equipCfg == null)
            {
                return;
            }
            var cusAttrCfg = Config.CustomPrototypeLibrary.Get(equipCfg.customAttributeId);
            if (cusAttrCfg == null)
            {
                return;
            }
            double tempMin = Math.Ceiling(equipCfg.customAttributeRandomRange.m_Min * cusAttrCfg.battleAttri.Get(propertyID));
            double tempMax = Math.Ceiling(equipCfg.customAttributeRandomRange.m_Max * cusAttrCfg.battleAttri.Get(propertyID));
            min = GetPropertValInTotal(equipID, subType, tempMin, enforceLv, awakenEnforceLV, isAwake);
            max = GetPropertValInTotal(equipID, subType, tempMax, enforceLv, awakenEnforceLV, isAwake);
        }

        public static void GetRandomAttrRange(int equipID, int subType, int propertyID, int enforceLv, int awakenEnforceLV, bool isAwake, out double min, out double max)
        {
            //随机属性上线：装备表-随机属性等级&随机范围 随机属性值表-属性值 
            min = max = 0;
            var equipCfg = Config.EquipPrototype.Get(equipID);
            if (equipCfg == null)
            {
                return;
            }
            var rdAttrCfg = Config.RandomValuePrototype.Get(equipCfg.randAttrLv);
            if (rdAttrCfg == null)
            {
                return;
            }
            double tempMin = Math.Ceiling(equipCfg.randAttriRange.m_Min * rdAttrCfg.battleAttri.Get(propertyID));
            double tempMax = Math.Ceiling(equipCfg.randAttriRange.m_Max * rdAttrCfg.battleAttri.Get(propertyID));
            min = GetPropertValInTotal(equipID, subType, tempMin, enforceLv, awakenEnforceLV, isAwake);
            max = GetPropertValInTotal(equipID, subType, tempMax, enforceLv, awakenEnforceLV, isAwake);
        }
        public static int GetRandomAttrMaxNum(ProbabilityTable proTable)
        {
            int ret = 0;
            var itr = proTable.m_ProbabilityDic.GetEnumerator();
            while (itr.MoveNext())
                ret = itr.Current.Key;
            return ret;
        }
        public static bool isEquipRecastable(int id)
        {
            var cfg = Config.EquipPrototype.Get(id);
            if (cfg != null)
            {
                if (cfg.isCanRecast)
                    return true;
            }
            return false;
        }
        public static bool isEquipRefinable(int id)
        {
            var cfg = Config.EquipPrototype.Get(id);
            if (cfg != null)
            {
                if (cfg.isCanRefine)
                    return true;
            }
            return false;
        }

        public static long CaculateEquipValue(int subType)
        {
            long ret = subType + 1;
            return ret;
        }

        public static bool IsEquipID(int id)
        {
            return Config.EquipPrototype.GetAll().Keys.Contains(id);
        }
        public static string GetEquipTypeName(EquipPartsType type)
        {
            string ret = "";
            switch (type)
            {
                case EquipPartsType.weapons:
                    ret = "武器";
                    break;
                case EquipPartsType.helmet:
                    ret = "头盔";
                    break;
                case EquipPartsType.chestArmor:
                    ret = "胸甲";
                    break;
                case EquipPartsType.gloves:
                    ret = "手套";
                    break;
                case EquipPartsType.leggings:
                    ret = "护腿";
                    break;
                case EquipPartsType.shoes:
                    ret = "鞋子";
                    break;
                case EquipPartsType.necklace:
                    ret = "项链";
                    break;
                case EquipPartsType.ring:
                    ret = "戒指";
                    break;
                case EquipPartsType.jewelry:
                    ret = "饰品";
                    break;
                case EquipPartsType.totalNum:
                    break;
                default:
                    break;
            }
            return ret;
        }
    }
}

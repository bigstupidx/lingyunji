using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Config
{
    public partial class AttributeDefine
    {
        #region
        public enum uAttributeDefine : uint
        {
            uNone = 0,
            uStrength = 1 << AttributeDefine.iStrength,
            uIntelligence = 1 << AttributeDefine.iIntelligence,
            uBone = 1 << AttributeDefine.iBone,
            uPhysique = 1 << AttributeDefine.iPhysique,
            uAgility = 1 << AttributeDefine.iAgility,
            uBodyway = 1 << AttributeDefine.iBodyway,
            uAll = 0xffffffff,
        }

        public static bool CheckBaseAttribute(uAttributeDefine flag)
        {
            return (flag & uAttributeDefine.uStrength) == 0 &&
                (flag & uAttributeDefine.uIntelligence) == 0 &&
                (flag & uAttributeDefine.uBone) == 0 &&
                (flag & uAttributeDefine.uPhysique) == 0 &&
                (flag & uAttributeDefine.uAgility) == 0 &&
                (flag & uAttributeDefine.uBodyway) == 0;
        }
        #endregion

        #region 已使用的属性,顺序也已经按新表排列好的
        //力量
        public const int iStrength = 1;
        //智慧
        public const int iIntelligence = 2;
        //根骨
        public const int iBone = 3;
        //体魄
        public const int iPhysique = 4;
        //敏捷
        public const int iAgility = 5;
        //身法
        public const int iBodyway = 6;

        //生命值
        public const int iHp = 7;
        //物理攻击
        public const int iPhysicalAttack = 8;
        //法术攻击
        public const int iMagicAttack = 9;
        //治疗强度
        public const int iCureRate = 10;
        //物理防御
        public const int iPhysicalDefense = 11;
        //物理防御率
        public const int fPhysicalDefenseRate = 12;
        //法术防御
        public const int iMagicDefense = 13;
        //法术防御率
        public const int fMagicDefenseRate = 14;
        //真气
        public const int iMp = 15;
        //护体
        public const int iHuti = 16;
        //护体恢复
        public const int iHutiRecover = 17;
        //灵力
        public const int iMana = 18;
        //移动速度
        public const int fMoveSpeed = 19;
        //移动速度加成
        public const int fSpeedAddition = 20;
        //生命恢复
        public const int iHpRecover = 21;

        //暴击等级
        public const int iCritLevel = 22;
        //暴击率
        public const int fCritRate = 23;
        //暴击防御等级
        public const int iCritDefenseLevel = 24;
        //暴击防御率
        public const int fCritDefenseRate = 25;
        //暴击伤害等级
        public const int iCritDamageLevel = 26;
        //暴击伤害率
        public const int fCritDamageRate = 27;   
        //暴击伤害减免等级
        public const int iCritDamageReduceLevel = 28;
        //暴击伤害减免率
        public const int fCritDamageReduceRate = 29;

        //命中等级
        public const int iHitLevel = 30;
        //命中率
        public const int fHitRate = 31;
        //回避等级
        public const int iAvoidLevel = 32;
        //回避率
        public const int fAvoidRate = 33;
        //招架等级
        public const int iParryLevel = 34;
        //招架率
        public const int fParryRate = 35;

        //物理穿透等级
        public const int iPhysicalPenetrateLevel = 36;
        //物理穿透
        public const int fPhysicalPenetrate = 37;
        //法术穿透等级
        public const int iMagicPenetrateLevel = 38;
        //法术穿透
        public const int fMagicPenetrate = 39;
        //额外伤害
        public const int iExtraDamage = 40;
        //伤害减免
        public const int iDamageReduce = 41;

        //金属性
        public const int iMetal = 42;
        //木属性
        public const int iWood = 43;
        //水属性
        public const int iWater = 44;
        //火属性
        public const int iFire = 45;
        //土属性
        public const int iEarth = 46;

        //金抗性
        public const int iMetalResistance = 47;
        //金抗性率
        public const int fMetalResistanceRate = 48;
        //木抗性
        public const int iWoodResistance = 49;
        //木抗性率
        public const int fWoodResistanceRate = 50;
        //水抗性
        public const int iWaterResistance = 51;
        //水抗性率
        public const int fWaterResistanceRate = 52;
        //火抗性
        public const int iFireResistance = 53;
        //火抗性率
        public const int fFireResistanceRate = 54;
        //土抗性
        public const int iEarthResistance = 55;
        //土抗性率
        public const int fEarthResistanceRate = 56;
        //全属性抗性
        public const int fResistance = 57;
        //全属性抗性率
        public const int fResistanceRate = 58;

        //金伤害
        public const int iMetalDamage = 59;
        //金伤害率
        public const int fMetalDamageRate = 60;
        //木伤害
        public const int iWoodDamage = 61;
        //木伤害率
        public const int fWoodDamageRate = 62;
        //水伤害
        public const int iWaterDamage = 63;
        //水伤害率
        public const int fWaterDamageRate = 64;
        //火伤害
        public const int iFireDamage = 65;
        //火伤害率
        public const int fFireDamageRate = 66;
        //土伤害
        public const int iEarthDamage = 67;
        //土伤害率
        public const int fEarthDamageRate = 68;
        

        //力量提高
        public const int fStrengthEnhance = 69;
        //智慧提高
        public const int fIntelligenceEnhance = 70;
        //根骨提高
        public const int fBoneEnhance = 71;
        //体魄提高
        public const int fPhysiqueEnhance = 72;
        //敏捷提高
        public const int fAgilityEnhance = 73;
        //身法提高
        public const int fBodyWayEnhance = 74;

        //生命值加成
        public const int fHpAddition = 75;
        //物理攻击加成
        public const int fPhysicalAttackAddition = 76;
        //法术攻击加成
        public const int fMagicAttackAddition = 77;
        //物理防御加成
        public const int fPhysicalDefenseAddition = 78;
        //法术防御加成
        public const int fMagicDefenseAddition = 79;

        //物理伤害提高
        public const int fPhysicalDamageEnhance = 80;
        //法术伤害提高
        public const int fMagicDamageEnhance = 81;
        //物理伤害降低
        public const int fPhysicalDamageReduce = 82;
        //法术伤害降低
        public const int fMagicDamageReduce = 83;
        //治疗效果提高
        public const int fCureAdd = 84;
        //受治疗效果提高
        public const int fCureAddFromOther = 85;

        //对怪物伤害提高
        public const int fDamageAddToMonster = 86;
        //对怪物物理伤害提高
        public const int fPhysicalDamageAddToMonster = 87;
        //对怪物法术伤害提高
        public const int fMagicDamageAddToMonster = 88;
        //受怪物伤害降低
        public const int fDamageReduceFromMonster = 89;
        //对玩家伤害提高
        public const int fDamageAddToPlayer = 90;
        //受玩家伤害降低
        public const int fDamageReduceFromPlayer = 91;
        //对灵兽伤害提高
        public const int fDamageAddToPet = 92;
        //受灵兽伤害降低
        public const int fDamageReduceFromPet = 93;

        #endregion

        public const int All = 94;
    }
}
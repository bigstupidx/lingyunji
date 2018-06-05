using System;
using System.Collections.Generic;
using Config;
using NetProto;
using System.Diagnostics;

namespace xys.battle
{
    public class AttackFlg
    {
        //标识,使用位操作
        public enum Flg
        {
            Miss = 1 << 0,       //是否击中（没有击中则是miss）
            Baoji = 1 << 1,     //是否暴击
            GeDange = 1 << 2,   //是否格挡
            IsRestore = 1 << 3, //是恢复还是伤害
            ImmuneState = 1 << 4, //免疫控制状态,霸体或者护体状态
            Poti = 1 << 5,        //是否破体
            ZhaoJia = 1 << 6,     //是否招架
        };

        public static bool IsFlg(int value, Flg flg)
        {
            return (value & (int)flg) != 0;
        }
        public static void SetFlg(ref int value, Flg flg)
        {
            value = value | (int)flg;
        }
    }


    //攻击属性处理
    public class BattleAttriAttack
    {

        public class TestAttack
        {
            public string text = "";
            void Debug(string tips)
            {
                text = text + tips + "\r\n \r\n";
            }

            [Conditional("DEBUG")]
            public static void Debug(TestAttack ret, string tips)
            {
                if (ret != null)
                    ret.Debug(tips);
            }
        }

        //属性封装,方便其他地方修改
        public struct AttackData
        {
            public float iHitLevel;
            public float fHitRate;

            public float t_iAvoidLevel;
            public float t_fAvoidRate;

            public float t_iParryLevel; //招架
            public float t_fParryRate;

            public float iCritLevel; //暴击
            public float fCritRate;
            public float t_iCritDefenseLevel;
            public float t_fCritDefenseRate;

            public float iCritDamageLevel;  //暴击伤害等级
            public float fCritDamageRate; //暴击伤害率
            public float t_iCritDamageReduceLevel; //暴击伤害减免等级
            public float t_fCritDamageReduceRate;

            public float attack;
            public float attackMul;
            public float t_defense;
            public float t_defenseMul;
            public float penetrateRate;//穿透

            public float iExtraDamage;
            public float t_iDamageReduce;
        }

        static public void Attack(IAction.ActionInfo info, AttackActionConfig cfg, TestAttack ret = null)
        {
            BattleAttriMgr s = info.source.battle.m_attrMgr;
            BattleAttriMgr t = info.target.battle.m_attrMgr;
            #region 属性封装,方便其他地方修改
            AttackData p;
            p.iHitLevel = s.GetFloat(AttributeDefine.iHitLevel);
            p.fHitRate = s.GetFloat(AttributeDefine.fHitRate);
            p.t_iAvoidLevel = t.GetFloat(AttributeDefine.iAvoidLevel);
            p.t_fAvoidRate = t.GetFloat(AttributeDefine.fAvoidRate);
            p.t_iParryLevel = t.GetFloat(AttributeDefine.iParryLevel);
            p.t_fParryRate = t.GetFloat(AttributeDefine.fParryRate);
            p.iCritLevel = s.GetFloat(AttributeDefine.iCritLevel);
            p.fCritRate = s.GetFloat(AttributeDefine.fCritRate);
            p.t_iCritDefenseLevel = t.GetFloat(AttributeDefine.iCritDefenseLevel);
            p.t_fCritDefenseRate = t.GetFloat(AttributeDefine.fCritDefenseRate);
            p.iCritDamageLevel = s.GetFloat(AttributeDefine.iCritDamageLevel);
            p.fCritDamageRate = s.GetFloat(AttributeDefine.fCritDamageRate);
            p.t_iCritDamageReduceLevel = t.GetFloat(AttributeDefine.iCritDamageReduceLevel);
            p.t_fCritDamageReduceRate = t.GetFloat(AttributeDefine.fCritDamageReduceRate);
            p.iExtraDamage = s.GetFloat(AttributeDefine.iExtraDamage);
            p.t_iDamageReduce = t.GetFloat(AttributeDefine.iDamageReduce);
            if (cfg.isMagicDamage)
            {
                p.attack = s.GetFloat(AttributeDefine.iMagicAttack);
                p.attackMul = s.GetFloat(AttributeDefine.fPhysicalAttackAddition);
                p.t_defense = t.GetFloat(AttributeDefine.iMagicDefense);
                p.t_defenseMul = t.GetFloat(AttributeDefine.fPhysicalDefenseAddition);
                p.penetrateRate = s.GetFloat(AttributeDefine.fMagicPenetrate);
            }
            else
            {
                p.attack = s.GetFloat(AttributeDefine.iPhysicalAttack);
                p.attackMul = s.GetFloat(AttributeDefine.fMagicAttackAddition);
                p.t_defense = t.GetFloat(AttributeDefine.iPhysicalDefense);
                p.t_defenseMul = t.GetFloat(AttributeDefine.fMagicDefenseAddition);
                p.penetrateRate = s.GetFloat(AttributeDefine.fPhysicalPenetrate);
            }
            #endregion

            //伤害结算流程
            float zhaojiaDamageMul = 1.0f;
            float baojiDamageMul = 1.0f;

            //等级函数，后面改为缓存起来
            float s_levelFix = (float)RoleAttriFx.GetLevelFx((int)info.source.job, info.source.levelValue);
            float t_levelFix = (float)RoleAttriFx.GetLevelFx((int)info.target.job, info.target.levelValue);
            float t_defenseFix = (float)RoleAttriFx.GetDefenseFx((int)info.target.job, info.target.levelValue);

            //=====命中判定
            //0.85+总命中等级*目标等级函数+命中率-目标总回避等级*等级函数-目标回避率
            float hitRate = 0.85f +
                p.iHitLevel * t_levelFix + p.fHitRate
                + cfg.mingzhongExtra
                - p.t_iAvoidLevel * s_levelFix - p.t_fAvoidRate;

            TestAttack.Debug(ret, string.Format("等级函数={0} 目标等级函数={1}", s_levelFix, t_levelFix));

            TestAttack.Debug(ret, string.Format("命中率{0} = 0.85+总命中等级{1}*目标等级函数{2}+命中率{3}+附加命中率{4}-目标总回避等级{5}*等级函数{6}-目标回避率{7}"
                , hitRate, p.iHitLevel, t_levelFix, p.fHitRate,
                cfg.mingzhongExtra, p.t_iAvoidLevel, s_levelFix, p.t_fAvoidRate));

            if (hitRate < 0.05f)
                hitRate = 0.05f;

            //miss
            if (!BattleHelp.RandPercent(hitRate))
            {
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.Miss);
                return;
            }


            //=====招架判定 
            //招架率=0.25 +目标招架等级*等级函数+目标招架率
            float zhaojiaRate = 0.25f + p.t_iParryLevel * s_levelFix + p.t_fParryRate;

            TestAttack.Debug(ret, string.Format("招架率{0}=0.25+目标招架等级{1}*等级函数{2}+目标招架率{3}",
                zhaojiaRate, p.t_iParryLevel, s_levelFix, p.t_fParryRate));

            //招架成功，伤害修正
            if (BattleHelp.RandPercent(zhaojiaRate))
            {
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.ZhaoJia);
                zhaojiaDamageMul = 0.5f;
            }



            //=====暴击判定
            //总暴击等级*目标等级函数+暴击率+附加暴击率-目标总暴击防御等级*等级修正-目标暴击防御率
            float baojiRate = p.iCritLevel * s_levelFix + p.fCritRate + cfg.baojiRateExtra
                - p.t_iCritDefenseLevel * t_levelFix - p.t_fCritDefenseRate;

            TestAttack.Debug(ret, string.Format("暴击率{0}=总暴击等级{1}*目标等级函数{2}+暴击率{3}+附加暴击率{4}-目标总暴击防御等级{5}*等级修正{6}-目标暴击防御率{7}",
        baojiRate, p.iCritLevel, s_levelFix, p.fCritRate, cfg.baojiRateExtra,
        p.t_iCritDefenseLevel, t_levelFix, p.t_fCritDefenseRate
        ));


            if (BattleHelp.RandPercent(baojiRate))
            {
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.Baoji);
                //=====暴击伤害
                baojiDamageMul = 1.5f + p.iCritDamageLevel * s_levelFix + p.fCritDamageRate
                - p.t_iCritDamageReduceLevel * t_levelFix - p.t_fCritDamageReduceRate;
                if (baojiDamageMul < 1)
                    baojiDamageMul = 1.0f;
            }

            //计算防御率
            float defenseRate = p.t_defense * (1 + p.t_defenseMul) / t_defenseFix;

            //理论攻击 = 技能伤害系数*(总攻击+攻击提高效果)+技能附加伤害系数*标准伤害值+技能附加伤害
            float attackValue = cfg.damageFix * (p.attack * (1 + p.attackMul));
            int skillStandardDamage;
            //技能标准伤害值
            if (info.skill != null)
                skillStandardDamage = info.skill.skillDamageCfg.damage;
            else
                skillStandardDamage = SkillDamageConfig.Get(1).damage;
            attackValue += cfg.extraDamageFix * skillStandardDamage;
            TestAttack.Debug(ret, string.Format("理论攻击{0} = 技能伤害系数{1}*总攻击{2}+技能附加伤害系数{3}*标准伤害值{4}",
                attackValue, cfg.damageFix, p.attack * (1 + p.attackMul), cfg.extraDamageFix, skillStandardDamage
));

            //理论防御=目标防御率-总穿透率-穿透提高效果
            float defenseValue = defenseRate - p.penetrateRate;

            TestAttack.Debug(ret, string.Format("理论防御{0}=目标防御率{1}-总穿透率{2}",
    defenseValue, defenseRate, p.penetrateRate
));

            BattleHelp.CheckValue(ref defenseValue, 0, 0.8f);
            //理论伤害=(理论攻击*(1-目标理论防御)+额外伤害-目标伤害减免)*暴击伤害系数*招架系数
            float damageExtra = p.iExtraDamage - p.t_iDamageReduce;
            if (damageExtra < 0)
                damageExtra = 0;
            float damageValue = (attackValue * (1 - defenseValue) + damageExtra) * baojiDamageMul * zhaojiaDamageMul;

            TestAttack.Debug(ret, string.Format("理论伤害{0}=(理论攻击{1}*(1-目标理论防御{2})+额外伤害{3}-目标伤害减免{4})*暴击伤害系数{5}*招架系数{6}",
                damageValue, attackValue, defenseValue, p.iExtraDamage,
                p.t_iDamageReduce, baojiDamageMul, zhaojiaDamageMul
));

            //====目标类型修正(暂时跳过)

            //====五行修正(暂时跳过)

            //====最终伤害(暂时跳过)
            //最终伤害=int(五行修正伤害*(1+伤害提高-受伤害降低)*random(0.8,1.2))

            int damageFinal = (int)damageValue;
            if (damageFinal < 1)
                damageFinal = 1;

            //填充伤害数据
            if (info.targetAction != null)
            {
                info.targetAction.attack.damageValue = damageFinal;
            }

            //被击处理
            info.target.battle.m_skillMgr.OnHandleAttackData(info, cfg);

            //修改血量
            info.target.battle.m_attrLogic.AddHp(-damageFinal);

            //死亡了，不能往下走
            if (!info.target.isAlive)
                return;

            //被击会切换姿态
            if (info.target.battle.m_attrMgr.postureCfg.logic.behitEnd)
                info.target.battle.m_attrLogic.ChangeToNextPosture();

            //霸体免疫控制
            if (info.target.battle.m_buffMgr.IsFlag(BuffManager.Flag.Bati))
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.ImmuneState);

            //护体伤害
            if (cfg.hutiDamage > 0 && info.target.battle.m_attrLogic.Huti_Damage(cfg.hutiDamage))
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.Poti);

            //护体状态可以免疫部分控制
            if (cfg.isStateImmuneByHuti && info.target.huTiStateValue == (short)HutiState.Huti)
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.ImmuneState);

            //进入战斗状态
            info.target.battle.m_attrLogic.SetBattleState(true);

            //仇恨值
            info.target.battle.m_targetMgr.AddValue(info.source, (int)(damageFinal * cfg.haterdMul));
        }
    }
}



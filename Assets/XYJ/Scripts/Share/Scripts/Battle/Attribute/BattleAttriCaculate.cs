using Config;
using System;
using System.Collections.Generic;


namespace xys.battle
{
    /// <summary>
    /// 属性计算
    /// </summary>
    public class BattleAttriCaculate
    {
        //获取ui显示属性
        public static void GetUIShowAttribute(BattleAttri baseAttri, BattleAttri uiAttri, RoleJob.Job job, int level)
        {
            //等级函数
            double levelFix = RoleAttriFx.GetLevelFx((int)job, level);
            //防御函数
            double defenseFix = RoleAttriFx.GetDefenseFx((int)job, level);

            uiAttri.Copy(baseAttri);

            uiAttri.Mul(AttributeDefine.iHp, 1 + uiAttri.Get(AttributeDefine.fHpAddition));
            uiAttri.Mul(AttributeDefine.iPhysicalAttack, 1 + uiAttri.Get(AttributeDefine.fPhysicalAttackAddition));
            uiAttri.Mul(AttributeDefine.iMagicAttack, 1 + uiAttri.Get(AttributeDefine.fMagicAttackAddition));
            uiAttri.Mul(AttributeDefine.iPhysicalDefense, 1 + uiAttri.Get(AttributeDefine.fPhysicalDefenseAddition));
            uiAttri.Set(AttributeDefine.fPhysicalDefenseRate, uiAttri.Get(AttributeDefine.iPhysicalDefense) / defenseFix);
            uiAttri.Mul(AttributeDefine.iMagicDefense, 1 + uiAttri.Get(AttributeDefine.fMagicDefenseAddition));
            uiAttri.Set(AttributeDefine.fMagicDefenseRate, uiAttri.Get(AttributeDefine.iMagicDefense) / defenseFix);
            uiAttri.Mul(AttributeDefine.fMoveSpeed, 1 + uiAttri.Get(AttributeDefine.fSpeedAddition));
            //

            uiAttri.Add(AttributeDefine.fCritRate, uiAttri.Get(AttributeDefine.iCritLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fCritDefenseRate, uiAttri.Get(AttributeDefine.iCritDefenseLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fCritDamageRate, 1.5f + uiAttri.Get(AttributeDefine.iCritDamageLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fCritDamageReduceRate, uiAttri.Get(AttributeDefine.iCritDamageReduceLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fHitRate, 0.85f + uiAttri.Get(AttributeDefine.iHitLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fAvoidRate, uiAttri.Get(AttributeDefine.iAvoidLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fParryRate, 0.25 + uiAttri.Get(AttributeDefine.iParryLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fPhysicalPenetrate, uiAttri.Get(AttributeDefine.iPhysicalPenetrateLevel) * levelFix);
            uiAttri.Add(AttributeDefine.fMagicPenetrate, uiAttri.Get(AttributeDefine.iMagicPenetrateLevel) * levelFix);
            //
            for (int i = AttributeDefine.fMetalResistanceRate; i != AttributeDefine.fEarthDamageRate; i += 2)
                uiAttri.Add(i, uiAttri.Get(i - 1) * levelFix);

            //向上取整
            uiAttri.Round();
        }

        //测试浮点取整精度问题
        public static void Test_Round()
        {
            Test_RoundFloat((double)1.4f, "1.4000");
            Test_RoundFloat((double)0.0571f, "0.0571");
            Test_RoundFloat(1.4, "1.4000");
            Test_RoundFloat(0.0571, "0.0571");
        }

        static void Test_RoundFloat(double v,string text)
        {
            double t = BattleAttri.RoundFloat(v);
            if ( t.ToString("f4") != text)
                XYJLogger.LogError("取整出错 " + v);
        }

        //由基础属性获取最终属性
        public static BattleAttri GetPlayerAttributeFix( BattleAttri extraAttri,BattleAttri buffAttri, RoleJob.Job job,int level )
        {
            BattleAttri finalAttri = new BattleAttri();
            //角色成长属性加上额外属性
            finalAttri.Copy(CareerAttribute.GetLevelAttribute(job, level));
            finalAttri.Add(extraAttri);
            finalAttri.Add(buffAttri);
            //一级属性加成
            MulLevel1(finalAttri);
            //一级属性转化值
            BattleAttri changeAttri = RoleChangeAttr.ChangeAttribute(finalAttri, job);
            finalAttri.Add(changeAttri);
            return finalAttri;
        }

        //获得怪物属性
        //extraAttri内部可能会修改
        static public BattleAttri GetMonsterAttributeFix(BattleAttri extraAttri,BattleAttri buffAttri, RoleDefine roleCfg, int level)
        {
            //额外的需要加上角色表的配置
            extraAttri.Add(roleCfg.battleAttri);

            MonsterAttributeFix fixCfg = MonsterAttributeFix.Get(level);
            BattleAttri finalAttri = new BattleAttri();
            //角色成长属性
            finalAttri.Copy(MonsterAttributeDefine.Get(level).battleAttri);

            //资质修正
            finalAttri.Mul(AttributeDefine.iStrength, roleCfg.liliang_zz);
            finalAttri.Mul(AttributeDefine.iIntelligence, roleCfg.zihui_zz);
            finalAttri.Mul(AttributeDefine.iBone, roleCfg.gengu_zz);
            finalAttri.Mul(AttributeDefine.iPhysique, roleCfg.tipo_zz);
            finalAttri.Mul(AttributeDefine.iAgility, roleCfg.minjie_zz);
            finalAttri.Mul(AttributeDefine.iBodyway, roleCfg.shenfa_zz);
            finalAttri.Mul(AttributeDefine.iHp, roleCfg.shengming_zz);
            finalAttri.Mul(AttributeDefine.iPhysicalAttack, roleCfg.wuligongji_zz);
            finalAttri.Mul(AttributeDefine.iMagicAttack, roleCfg.fashugongji_zz);
            finalAttri.Mul(AttributeDefine.iPhysicalDefense, roleCfg.wulifangyu_zz);
            finalAttri.Mul(AttributeDefine.iMagicDefense, roleCfg.fashufangyu_zz);

            //额外的一级属性不受资质影响,额外的高级属性也不参与修正
            finalAttri.AddByLevel(extraAttri, true);
            finalAttri.AddByLevel(buffAttri,true);

            //一级属性加成
            MulLevel1(finalAttri);
            //一级属性转化值
            BattleAttri changeAttri = RoleChangeAttr.ChangeAttribute(finalAttri, RoleJob.Job.Monster);
            finalAttri.Add(changeAttri);

            //修正高级属性
            finalAttri.Mul(AttributeDefine.iHp, roleCfg.hpfix * fixCfg.hpfix);
            finalAttri.Mul(AttributeDefine.iPhysicalAttack, roleCfg.attackfix * fixCfg.attackfix);
            finalAttri.Mul(AttributeDefine.iMagicAttack, roleCfg.attackfix * fixCfg.attackfix);
            finalAttri.Mul(AttributeDefine.iPhysicalDefense, roleCfg.defensefix * fixCfg.defensefix);
            finalAttri.Mul(AttributeDefine.iMagicDefense, roleCfg.defensefix * fixCfg.defensefix);
            //额外属性添加高级属性
            finalAttri.AddByLevel(extraAttri,false);
            finalAttri.AddByLevel(buffAttri, false);
            return finalAttri;
        }

        //宠物由基础属性获取最终属性
        //baseAttri只是一级属性
        public static BattleAttri GetPetsAttributeFix(BattleAttri baseAttri, int level, int[] qualifications, BattleAttri extraAttri=null)
        {
            BattleAttri finalAttri = new BattleAttri();
            finalAttri.Copy(baseAttri);

            //额外的一级属性不受资质影响,额外的高级属性也不参与修正
            if(extraAttri!=null)
                finalAttri.AddByLevel(extraAttri, true);
            //一级属性加成
            MulLevel1(finalAttri);
            //一级属性转化值
            BattleAttri changeAttri = RoleChangeAttr.ChangeAttribute(finalAttri, RoleJob.Job.Pet);
            #region 宠物一级属性转化值修正
            Action<int, int> fun = (id, zizhi) =>
               {
                   changeAttri.Mul(id, qualifications[zizhi - 1] * 0.001f - 0.1f);
               };

            fun(AttributeDefine.iHp, AttributeDefine.iBone);
            fun(AttributeDefine.iPhysicalAttack, AttributeDefine.iStrength);   
            fun(AttributeDefine.iMagicAttack, AttributeDefine.iIntelligence);
            fun(AttributeDefine.iCureRate, AttributeDefine.iPhysique);
            fun(AttributeDefine.iPhysicalDefense, AttributeDefine.iBone);
            fun(AttributeDefine.iMagicDefense, AttributeDefine.iPhysique);
            fun(AttributeDefine.iCritLevel, AttributeDefine.iAgility);
            fun(AttributeDefine.iCritDefenseLevel, AttributeDefine.iBodyway);
            fun(AttributeDefine.iHitLevel, AttributeDefine.iAgility);
            fun(AttributeDefine.iAvoidLevel, AttributeDefine.iBodyway);
            fun(AttributeDefine.iParryLevel, AttributeDefine.iBone);
            #endregion
            finalAttri.Add(changeAttri);
            //额外属性添加高级属性
            if(extraAttri!=null)
                finalAttri.AddByLevel(extraAttri, false);
            return finalAttri;
        }

        //一级属性加成
        static void MulLevel1(BattleAttri baseAttri)
        {
            //一级属性
            baseAttri.Mul(AttributeDefine.iStrength, 1 + baseAttri.Get(AttributeDefine.fStrengthEnhance));
            baseAttri.Mul(AttributeDefine.iIntelligence, 1 + baseAttri.Get(AttributeDefine.fIntelligenceEnhance));
            baseAttri.Mul(AttributeDefine.iBone, 1 + baseAttri.Get(AttributeDefine.fBoneEnhance));
            baseAttri.Mul(AttributeDefine.iPhysique, 1 + baseAttri.Get(AttributeDefine.fPhysiqueEnhance));
            baseAttri.Mul(AttributeDefine.iAgility, 1 + baseAttri.Get(AttributeDefine.fAgilityEnhance));
            baseAttri.Mul(AttributeDefine.iBodyway, 1 + baseAttri.Get(AttributeDefine.fBodyWayEnhance));
        }

 
    }
}
#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using Config;
    using UnityEngine;
    using System.Collections.Generic;
    using xys.battle;
    using NetProto.Hot;

    partial class PetObj
    {
        struct AttributeType
        {
            public const int iHp = 0;
            public const int iPhysicalAttack = 1;
            public const int iMagicAttack = 2;
            public const int iPhysicalDef = 3;
            public const int iMagicDef = 4;
            public const int iCirLv = 5;
            public const int iCirDefLv = 6;
            public const int iHitLv = 7;
            public const int iAvoidLv = 8;
            public const int iParryLv = 9;
            public const int iTenacityLv = 10;
            public const int iCureRate = 11;

            public const string sPower = "力量";
            public const string sIntelligence = "智慧";
            public const string sRoot = "根骨";
            public const string sBodies = "体魄";
            public const string sAgile = "敏捷";
            public const string sBodyposition = "身法";
        }

        class SkillProbabilityData
        {
            public int id = 0;
            public int intervall = 0;
        }
        long m_CharId = 0;
        int m_uuid = -1;
        string pet_name;
        string nick_name;
        int m_Identity;
        int m_Type;
        int m_AiType;
        int m_Level;
        int m_Exp;

        int m_PropertyGrow;
        float m_PropertySavvy;
        int m_Personality;

        //资质
        int m_PowerQualification;
        int m_IntelligenceQualification;
        int m_RootBoneQualification;
        int m_BodiesQualification;
        int m_AgileQualification;
        int m_BodyPositionQualification;
        //总资质
        int m_AllPowerQualification;
        int m_AllIntelligenceQualification;
        int m_AllRootBoneQualification;
        int m_AllBodiesQualification;
        int m_AllAgileQualification;
        int m_AllBodyPositionQualification;


        //融合值
        int m_FusionPower;
        int m_FusionIntelligence;
        int m_FusionRootBone;
        int m_FusionBodies;
        int m_FusionAgile;
        int m_FusionBodyPosition;

        //潜能点
        int m_TotalPotentialPoint;
        int m_PowerPoint;
        int m_IntelligencePoint;
        int m_RootBonePoint;
        int m_BodiesPoint;
        int m_AgilePoint;
        int m_BodyPositionPoint;
        //预加点
        int m_PowerSlider;
        int m_IntelligentSilder;
        int m_RootSlider;
        int m_BodiesSlider;
        int m_AgileSlider;
        int m_BodypositionSlider;
        
        BattleAttri m_BattleAttri = new BattleAttri();
        BattleAttri m_BattleAllAttri = new BattleAttri();
        BattleAttri m_UIBattleAllAttri = new BattleAttri();

        PetSkillData m_TrickSkills;
        PetSkillData m_TalentSkills;
        List<PetSkillData> m_PassiveSkills = new List<PetSkillData>();

        Dictionary<int, PetUseItemData> m_UseItemList = new Dictionary<int, PetUseItemData>();

        //锁定被动技能列表
        Dictionary<int, PetSkillData> m_LockPassiveSkills = new Dictionary<int, PetSkillData>();

        //剩余重置加点次数
        int m_ResetTimes;
        //洗练重置
        int m_WashTimes;

        public PetsAttribute attribute { get; private set; }

        public bool Load(PetsAttribute item)
        {
            if (item == null)
                return false;
            attribute = item;
            m_TrickSkills = new PetSkillData();
            m_TalentSkills = new PetSkillData();
            m_PassiveSkills = new List<PetSkillData>();
            m_LockPassiveSkills.Clear();

            if (PetAttribute.Get(item.id) == null)
                return false;
            m_uuid = item.uuid;
            m_Identity = item.id;
            m_CharId = item.char_id;
            nick_name = item.nick_name;

            m_Level = item.lv;
            m_Exp = item.exp;
            m_CharId = item.char_id;
            m_PropertyGrow = item.property_grow;
            m_PropertySavvy = item.property_savvy;
            m_ResetTimes = item.reset_times;
            m_WashTimes = item.wash_times;
            m_Personality = item.personality;
            m_Type = item.ai_type;
            //资质
            m_PowerQualification = item.baseAtt[(int)PetsDefines.power];
            m_IntelligenceQualification = item.baseAtt[(int)PetsDefines.intelligence];
            m_RootBoneQualification = item.baseAtt[(int)PetsDefines.root];
            m_BodiesQualification = item.baseAtt[(int)PetsDefines.bodies];
            m_AgileQualification = item.baseAtt[(int)PetsDefines.agile];
            m_BodyPositionQualification = item.baseAtt[(int)PetsDefines.bodyposition];
            //融合值
            m_FusionPower = item.fusionAtt[(int)PetsDefines.power];
            m_FusionIntelligence = item.fusionAtt[(int)PetsDefines.intelligence];
            m_FusionRootBone = item.fusionAtt[(int)PetsDefines.root];
            m_FusionBodies = item.fusionAtt[(int)PetsDefines.bodies];
            m_FusionAgile = item.fusionAtt[(int)PetsDefines.agile];
            m_FusionBodyPosition = item.fusionAtt[(int)PetsDefines.bodyposition];
            //潜能点
            m_PowerPoint = item.pointAtt[(int)PetsDefines.power];
            m_IntelligencePoint = item.pointAtt[(int)PetsDefines.intelligence];
            m_RootBonePoint = item.pointAtt[(int)PetsDefines.root];
            m_BodiesPoint = item.pointAtt[(int)PetsDefines.bodies];
            m_AgilePoint = item.pointAtt[(int)PetsDefines.agile];
            m_BodyPositionPoint = item.pointAtt[(int)PetsDefines.bodyposition];
            //加点方案
            m_PowerSlider = item.sliderpointAtt[(int)PetsDefines.power];
            m_IntelligentSilder = item.sliderpointAtt[(int)PetsDefines.intelligence];
            m_RootSlider = item.sliderpointAtt[(int)PetsDefines.root];
            m_BodiesSlider = item.sliderpointAtt[(int)PetsDefines.bodies];
            m_AgileSlider = item.sliderpointAtt[(int)PetsDefines.agile];
            m_BodypositionSlider = item.sliderpointAtt[(int)PetsDefines.bodyposition];

            //计算总潜能点
            m_TotalPotentialPoint = 0;
            for (int i = 1; i <= m_Level; i++)
                m_TotalPotentialPoint += PetProperty.Get(i).potential_point;

            //skill
            m_TrickSkills = item.trick_skills;
            m_TalentSkills = item.talent_skills;
            m_PassiveSkills = item.passive_skills;
            //加载锁定列表
            for (int i = 0; i < m_PassiveSkills.Count; i++)
                if (m_PassiveSkills[i].islock == 1)
                    m_LockPassiveSkills.Add(m_PassiveSkills[i].id, m_PassiveSkills[i]);
            //计算基础属性
            this.m_BattleAttri.Clear();
            this.RecalAttribute(m_Level,ref m_BattleAttri);
            //总资质
            int[] qualifications = this.RecalQualification();
            //最终属性
            m_BattleAllAttri.Clear();
            m_BattleAllAttri = BattleAttriCaculate.GetPetsAttributeFix(m_BattleAttri, m_Level, qualifications);
            //ui显示属性
            BattleAttriCaculate.GetUIShowAttribute(m_BattleAllAttri, m_UIBattleAllAttri, RoleJob.Job.Pet, m_Level);
            return true;
        }

        #region 双端通用
        /// <summary>
        /// 刷新总资质
        /// </summary>
        int[] RecalQualification()
        {
            m_AllPowerQualification = (int)(m_PowerQualification * (1 + 0.2f * (m_PropertySavvy-1) * 0.01f));
            m_AllIntelligenceQualification = (int)(m_IntelligenceQualification * (1 + 0.2f * (m_PropertySavvy - 1) * 0.01f));
            m_AllRootBoneQualification = (int)(m_RootBoneQualification * (1 + 0.2f * (m_PropertySavvy - 1) * 0.01f));
            m_AllBodiesQualification = (int)(m_BodiesQualification * (1 + 0.2f * (m_PropertySavvy - 1) * 0.01f));
            m_AllAgileQualification = (int)(m_AgileQualification * (1 + 0.2f * (m_PropertySavvy - 1) * 0.01f));
            m_AllBodyPositionQualification = (int)(m_BodyPositionQualification * (1 + 0.2f * (m_PropertySavvy - 1) * 0.01f));

            int[] quas = new int[6];
            quas[0] = m_AllPowerQualification;
            quas[1] = m_AllIntelligenceQualification;
            quas[2] = m_AllRootBoneQualification;
            quas[3] = m_AllBodiesQualification;
            quas[4] = m_AllAgileQualification;
            quas[5] = m_AllBodyPositionQualification;
            return quas;
        }
        /// <summary>
        /// 刷新基础属性
        /// </summary>
        void RecalAttribute(int lv,ref BattleAttri tempBA)
        {
            PetAttribute attribute = PetAttribute.Get(m_Identity);
            PetProperty property = PetProperty.Get(lv);
            RoleDefine role = RoleDefine.Get(m_Identity);
            if (property == null || attribute == null || role == null || tempBA == null)
                return;

            //刷新技能属性
            if (m_TalentSkills != null)
                this.RecallPassiveAttribute(m_TalentSkills.id,ref tempBA);
            foreach (PetSkillData item in m_PassiveSkills)
                this.RecallPassiveAttribute(item.id, ref tempBA);

            float growRate = 0.001f * m_PropertyGrow, temp = (growRate * 5.0f + 2) / 12;
            //基础属性计算
            double res = 0.01f * attribute.init_power *property.base_power * temp + 0.01f * attribute.init_power + m_PowerPoint;
            tempBA.Add(AttributeDefine.iStrength, res);
            res = 0.01f * attribute.init_intelligence * property.base_intelligence * temp + 0.01f * attribute.init_intelligence + m_IntelligencePoint;
            tempBA.Add(AttributeDefine.iIntelligence, res);
            res = 0.01f * attribute.init_root_bone * property.base_root_bone * temp + 0.01f * attribute.init_root_bone + m_RootBonePoint;
            tempBA.Add(AttributeDefine.iBone, res);
            res = 0.01f * attribute.init_bodies * property.base_bodies * temp + 0.01f * attribute.init_bodies + m_BodiesPoint;
            tempBA.Add(AttributeDefine.iPhysique, res);
            res = 0.01f * attribute.init_agile * property.base_agile * temp + 0.01f * attribute.init_agile + m_AgilePoint;
            tempBA.Add(AttributeDefine.iAgility, res);
            res = 0.01f * attribute.init_body_position * property.base_body_position * temp + 0.01f * attribute.init_body_position + m_BodyPositionPoint;
            tempBA.Add(AttributeDefine.iBodyway, res);

            //二级基础属性计算
            res = (0.6f + 0.45f * growRate) * property.base_life;
            tempBA.Add(AttributeDefine.iHp, res);//基础生命值
            res = (0.82f + 0.23f * growRate) * property.base_physical_attack;
            tempBA.Add(AttributeDefine.iPhysicalAttack, res);//基础物理攻击
            res = (0.82f + 0.23f * growRate) * property.base_magic_attack;
            tempBA.Add(AttributeDefine.iMagicAttack, res);//基础法术攻击
            res = (0.82f + 0.23f * growRate) * property.base_treatment;
            tempBA.Add(AttributeDefine.iCureRate, res);//基础治疗强度
            res = (0.6f + 0.45f * growRate) * property.base_physical_defense;
            tempBA.Add(AttributeDefine.iPhysicalDefense, res);//基础物理防御
            res = (0.6f + 0.45f * growRate) * property.base_magic_defense;
            tempBA.Add(AttributeDefine.iPhysicalDefense, res);//基础法术防御
        }

        void RecallPassiveAttribute(int skillid,ref BattleAttri tempBA)
        {
            if (skillid == 0)
                return;
            if (!Config.PassiveSkills.GetAll().ContainsKey(skillid))
                return;
            PetProperty property = PetProperty.Get(m_Level);
            if (property == null)
                return;

            //技能属性
            tempBA.Add(PassiveSkills.Get(skillid).battleAttri);
        }

        double LevelFx()
        {
            return RoleAttriFx.GetLevelFx((int)RoleJob.Job.Pet, m_Level);
        }

        double DefenseFx()
        {
            return RoleAttriFx.GetDefenseFx((int)RoleJob.Job.Pet, m_Level);
        }
        #endregion
    }
}
#endif
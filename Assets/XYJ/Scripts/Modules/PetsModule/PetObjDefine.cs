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
        #region 属性
        public int id { get { return m_Identity; } }
        public int personality { get { return m_Personality; } }
        public float savvy { get { return m_PropertySavvy; } }
        public int lv { get { return m_Level; } }
        public int exp { get { return m_Exp; } }
        public string nickName { get { return nick_name; } }

        public PetSkillData trickSkills { get { return m_TrickSkills; } }
        public PetSkillData talentSkills { get { return m_TalentSkills; } }
        public List<PetSkillData> passiveSkills { get { return m_PassiveSkills; } }
        public int power_slider_point { get { return m_PowerSlider; } }
        public int intelligence_slider_point { get { return m_IntelligentSilder; } }
        public int root_bone_slider_point { get { return m_RootSlider; } }
        public int bodies_slider_point { get { return m_BodiesSlider; } }
        public int agile_slider_point { get { return m_AgileSlider; } }
        public int body_position_slider_point { get { return m_BodypositionSlider; } }

        public int total_potential_point { get { return m_TotalPotentialPoint; } }
        public int power_point { get { return m_PowerPoint; } }
        public int intelligence_point { get { return m_IntelligencePoint; } }
        public int root_bone_point { get { return m_RootBonePoint; } }
        public int bodies_point { get { return m_BodiesPoint; } }
        public int agile_point { get { return m_AgilePoint; } }
        public int body_position_point { get { return m_BodyPositionPoint; } }

        public int basepower_qualification { get { return m_PowerQualification; } }
        public int baseintelligence_qualification { get { return m_IntelligenceQualification; } }
        public int baseroot_bone_qualification { get { return m_RootBoneQualification; } }
        public int basebodies_qualification { get { return m_BodiesQualification; } }
        public int baseagile_qualification { get { return m_AgileQualification; } }
        public int basebody_position_qualification { get { return m_BodyPositionQualification; } }

        public int power_qualification { get { return m_AllPowerQualification; } }
        public int intelligence_qualification { get { return m_AllIntelligenceQualification; } }
        public int root_bone_qualification { get { return m_AllRootBoneQualification; } }
        public int bodies_qualification { get { return m_AllBodiesQualification; } }
        public int agile_qualification { get { return m_AllAgileQualification; } }
        public int body_position_qualification { get { return m_AllBodyPositionQualification; } }
        #endregion
        public float growRate { get { return m_PropertyGrow; } }
        public int lockNumSKills { get { return m_LockPassiveSkills.Count; } }

        public BattleAttri battleAttri{get{ return m_BattleAllAttri; }}
        public BattleAttri uibattleAttri { get { return m_UIBattleAllAttri ; } }
        string GetStr(int index)
        {
            return AttributeDefine.GetValueStr(index, m_BattleAttri.Get(index));
        }

        public void GetTipsDes(int id, ref string content)
        {
            int startIndex = content.LastIndexOf('{');
            int endIndex = content.LastIndexOf('}');
            string replaceStr = string.Empty;
            if (startIndex != -1 && endIndex != -1)
                replaceStr += content.Substring(startIndex, endIndex - startIndex + 1);
            else
                return;
            switch (id)
            {
                case AttributeDefine.iBone:
                    content = string.Format(content, this.GetStr(AttributeDefine.iHp), this.GetStr(AttributeDefine.iPhysicalDefense));
                    break;
                case AttributeDefine.iPhysique:
                    content = string.Format(content,
                        this.GetStr(AttributeDefine.iMagicDefense),
                        0,
                        this.GetStr(AttributeDefine.iCureRate));
                    break;
                case AttributeDefine.iAgility:
                    content = string.Format(content,
                          this.GetStr(AttributeDefine.iCritLevel),
                          this.GetStr(AttributeDefine.iHitLevel));
                    break;
                case AttributeDefine.iBodyway:
                    content = string.Format(content,
                        this.GetStr(AttributeDefine.iCritDefenseLevel),
                        this.GetStr(AttributeDefine.iAvoidLevel));
                    break;
                default:
                    content = string.Format(content, this.GetStr(id));
                    break;
            }
        }
     
#region 预加点逻辑

        public BattleAttri GetNextPotentialAttribute(int[] tempPotentialPoint)
        {
            int[] num = new int[AttributeDefine.fResistanceRate];
            if (this.attribute == null)
                return new BattleAttri();
            int lv = this.attribute.lv;
            if (!Config.RoleExp.GetAll().ContainsKey(lv))
                return new BattleAttri();

            BattleAttri tempBA = new BattleAttri();
            //复制下一级数据
            for(int i = 0; i < tempPotentialPoint.Length;i++)
            {
                if (tempPotentialPoint[i] == 0) continue;
                tempBA.Add(AttributeDefine.iStrength + i, tempPotentialPoint[i]);
            }

            this.RecalAttribute(lv,ref tempBA);
            int[] qualifications = this.RecalQualification();
            tempBA = BattleAttriCaculate.GetPetsAttributeFix(tempBA,lv, qualifications);
            return tempBA;
        }
#endregion
    }
}
#endif
#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;

    class DemonplotsMgr
    {
        public DemonplotsTable m_Tables;
        public DemonplotsMgr()
        {
            m_Tables = new DemonplotsTable();
        }

        public DemonplotSkillData NewSkillData(int skillTypes)
        {
            if(!Config.DemonplotSkill.GetAll().ContainsKey((Config.DemonplotSkillType)skillTypes))
            {
                Debuger.LogError("Demonplot skilltype error:" + skillTypes);
                return null;
            }
            DemonplotSkillData skillData = new DemonplotSkillData();
            skillData.lv = 1;
            skillData.exp = 0;
            return skillData;
        }
    }
}
#endif
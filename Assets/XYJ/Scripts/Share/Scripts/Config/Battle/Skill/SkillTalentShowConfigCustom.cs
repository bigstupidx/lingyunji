using System.Collections.Generic;

namespace Config
{
    public partial class SkillTalentShowConfig
    {
        static public SkillTalentShowConfig GetByKeyAndPointId(int key, int index)
        {
            List<SkillTalentShowConfig> list = SkillTalentShowConfig.GetGroupBykey(key);
            foreach (SkillTalentShowConfig item in list)
            {
                if (item.skillPointId == index)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
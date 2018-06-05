using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Config
{
    public partial class SkillTalentConfig
    {
        //根据技能天赋系号获取初级天赋Id
        static public int GetPrimaryBySkillSeries(int index)
        {
            for (int i = 0; i < DataList.Count; i++)
            {
                SkillTalentConfig skill = DataList[i];
                if (skill.skillSeries == index && skill.primary == 1)
                {
                    return skill.skillId;
                }
            }
            return 0;
        }

        //根据技能点号获取所属所有技能天赋系号
        static public Dictionary<int, List<SkillTalentConfig>> GetSkillSeriesGroupByKey(int index)
        {
            Dictionary<int, List<SkillTalentConfig>> dic = new Dictionary<int, List<SkillTalentConfig>>();

            List<SkillTalentConfig> list = GetGroupBykey(index);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (!dic.ContainsKey(list[i].skillSeries))
                    {
                        List<SkillTalentConfig> l = new List<SkillTalentConfig>();
                        l.Add(list[i]);
                        dic.Add(list[i].skillSeries, l);
                    }
                    else
                    {
                        dic[list[i].skillSeries].Add(list[i]);
                    }
                }
            }
            return dic;
        }

        //根据技能点和技能ID获取
        static public SkillTalentConfig GetByKeyAndId(int key, int skillId)
        {
            List<SkillTalentConfig> list = GetGroupBykey(key);
            if (list == null) return null;
            foreach (SkillTalentConfig item in list)
            {
                if (item.skillId == skillId)
                    return item;
            }
            return null;
        }

        //根据秘籍ID获取有进阶的技能
        static public List<SkillTalentConfig> GetSkillTalentByBookId(int bookId)
        {
            List<SkillTalentConfig> list = new List<SkillTalentConfig>();
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].bookId == bookId)
                    list.Add(DataList[i]);
            }
            return list;
        }

        //获取技能孔所有技能系默认天赋
        static public List<SkillTalentConfig> GetSeriesDefaultSkillsBykey(int key)
        {
            List<SkillTalentConfig> list = null;
            if (GetGroupBykey(key) != null)
                list = GetGroupBykey(key);
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].primary != 1)
                    list.Remove(list[i]);
            }
            return list;
        }
    }
}
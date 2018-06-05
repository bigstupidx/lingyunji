// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleTitle
    {
        public static SortedDictionary<int, List<int>> m_LevelMap = new SortedDictionary<int, List<int>>();
        public static SortedDictionary<int, List<int>> m_CombatMap = new SortedDictionary<int, List<int>>();

        public static void OnLoadEndLine(RoleTitle data, CsvCommon.ICsvLoad reader, int i)
        {
            if (data.level > 0)
            {
                if (!m_LevelMap.ContainsKey(data.level))
                {
                    List<int> list = new List<int>();
                    m_LevelMap[data.level] = list;
                }
                m_LevelMap[data.level].Add(data.id);
            }

            if (data.combatValue > 0)
            {
                if (!m_CombatMap.ContainsKey(data.combatValue))
                {
                    List<int> list = new List<int>();
                    m_CombatMap[data.combatValue] = list;
                }
                m_CombatMap[data.combatValue].Add(data.id);
            }
        }
    }

    public enum TitleTimeLimit
    {
        forever = 1,    // ”¿æ√
        limit = 2,      // œﬁ ±
    }
}



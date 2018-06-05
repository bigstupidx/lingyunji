// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class DemonplotProperty
    {
        public static Dictionary<DemonplotSkillType, List<DemonplotProperty>> m_ItemGroup = new Dictionary<DemonplotSkillType, List<DemonplotProperty>>();
        public static Dictionary<DemonplotSkillType, List<DemonplotProperty>> m_DemonGroup = new Dictionary<DemonplotSkillType, List<DemonplotProperty>>();
        static void OnLoadEnd()
        {
            #region 炼物
            if (DataList_type.ContainsKey(DemonplotType.Item))
            {
                foreach (DemonplotProperty item in DataList_type[DemonplotType.Item])
                {
                    if (m_ItemGroup.ContainsKey(item.skilltype))
                    {
                        if (m_ItemGroup[item.skilltype] == null)
                        {
                            List<DemonplotProperty> list = new List<DemonplotProperty>();
                            list.Add(item);
                            m_ItemGroup.Add(item.skilltype, list);
                        }
                        else
                        {
                            m_ItemGroup[item.skilltype].Add(item);
                        }
                    }
                    else
                    {
                        List<DemonplotProperty> list = new List<DemonplotProperty>();
                        list.Add(item);
                        m_ItemGroup.Add(item.skilltype, list);
                    }
                }
            }
            #endregion
            #region 炼妖
            if (DataList_type.ContainsKey(DemonplotType.Demon))
            {
                foreach (DemonplotProperty item in DataList_type[DemonplotType.Demon])
                {
                    if (m_DemonGroup.ContainsKey(item.skilltype))
                    {
                        if (m_DemonGroup[item.skilltype] == null)
                        {
                            List<DemonplotProperty> list = new List<DemonplotProperty>();
                            list.Add(item);
                            m_DemonGroup.Add(item.skilltype, list);
                        }
                        else
                        {
                            m_DemonGroup[item.skilltype].Add(item);
                        }
                    }
                    else
                    {
                        List<DemonplotProperty> list = new List<DemonplotProperty>();
                        list.Add(item);
                        m_DemonGroup.Add(item.skilltype, list);
                    }
                }
            }
           
            #endregion
        }
    }
    public enum DemonplotSkillType
    {
        Collect = 1,//采集
        Survey = 2,//勘察
        Medic = 3,//药剂
        Strange = 4,//奇物
        Talisman = 5,//护符
    }

    public enum DemonplotType
    {
        Item = 1,//炼物
        Demon = 2,//炼妖
    }

    public class ProduceData
    {
        public int id = 0;
        public int count = 0;
        public static ProduceData InitConfig(string text)
        {
            ProduceData produceData = new ProduceData();
            string[] value = text.Split(';');
            if (value.Length != 2)
                return produceData;
            int temp1, temp2;
            if (int.TryParse(value[0], out temp1) && int.TryParse(value[1], out temp2))
            {
                produceData.id = temp1;
                produceData.count = temp2;
            }
            return produceData;
        }
    }

    public class MatchinData
    {
        public struct ItemData
        {
            public int id;
            public int count;
        }

        public List<ItemData> list = new List<ItemData>();
        public static MatchinData InitConfig(string text)
        {
            MatchinData matchinData = new MatchinData();
            string[] value = text.Split(',');
            for (int i = 0; i < value.Length; i++)
            {
                string[] res = value[i].Split(';');
                int temp1, temp2;
                if (int.TryParse(res[0], out temp1) && int.TryParse(res[1], out temp2))
                {
                    ItemData itemData = new ItemData();
                    itemData.id = temp1;
                    itemData.count = temp2;
                    matchinData.list.Add(itemData);
                }
            }
            return matchinData;
        }
    }

    public partial class DemonplotSkillExp
    {
        static public void Recalculate(ref int lv,ref int exp,int playerLv)
        {
            while(true)
            { 
                //超出技能限制等级上限
                if (CheckLv(lv,playerLv))
                {
                    exp = 0;
                    break;
                }
                DemonplotSkillExp data = DemonplotSkillExp.Get(lv);
                if (data == null) break;
                if (exp < data.exp) break;
                exp -= data.exp;
                lv += 1;
            }
        }
        static public bool CheckLv(int skillLv,int playerLv)
        {
            return skillLv >= (int)((float)(playerLv - 40) /10) * 10 + 10;
        }
    }

}

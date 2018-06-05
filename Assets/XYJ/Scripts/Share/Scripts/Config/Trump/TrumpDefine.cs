// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class TrumpJoining
    {
        public static BattleAttri GetBattleAttri(int joiningid)
        {
            return new BattleAttri();
        }
    }
    public enum TrumpSkillType
    {
        Active = 1,//
        Passive = 2,//
    }
    public enum TrumpsInfusedType
    {
        Normal = 1,//灵巧
        Special = 2,//灵枢
    }
    public partial class TrumpInfused
    {
        //法宝ID，法宝境界等级，list
        public static Dictionary<int, Dictionary<int, List<TrumpInfused>>> infusedDic = new Dictionary<int, Dictionary<int, List<TrumpInfused>>>();

        static void OnLoadEnd()
        {
            for(int i = 0; i < TrumpInfused.DataList.Count;i++)
            {
                TrumpInfused data = TrumpInfused.DataList[i];
                if(!infusedDic.ContainsKey(data.trumpid))
                {
                    List<TrumpInfused> tempList = new List<TrumpInfused>();
                    tempList.Add(data);
                    Dictionary<int, List<TrumpInfused>> tempLvList = new Dictionary<int, List<TrumpInfused>>();
                    tempLvList.Add(data.tastelv, tempList);
                    infusedDic.Add(data.trumpid, tempLvList);
                }
                else if(!infusedDic[data.trumpid].ContainsKey(data.tastelv))
                {
                    List<TrumpInfused> tempList = new List<TrumpInfused>();
                    tempList.Add(data);
                    infusedDic[data.trumpid].Add(data.tastelv, tempList);
                }
                else if(infusedDic[data.trumpid].ContainsKey(data.tastelv))
                {
                    infusedDic[data.trumpid][data.tastelv].Add(data);
                }
            }
        }
    }

    public class InfuseMaterialData
    {
        public struct ItemData
        {
            public int id;
            public int count;
        }

        public ItemData[] list;

        public static InfuseMaterialData InitConfig(string text)
        {
            InfuseMaterialData data = new InfuseMaterialData();
            string[] value = text.Split(',');
            data.list = new ItemData[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                string[] res = value[i].Split(';');
                int temp1, temp2;
                if (int.TryParse(res[0], out temp1) && int.TryParse(res[1], out temp2))
                {
                    data.list[i].id = temp1;
                    data.list[i].count = temp2;
                }
            }
            return data;
        }
    }

    public partial class TrumpJoining
    {
        public static string GetJoiningDes(int joiningId)
        {
            if (!DataList.ContainsKey(joiningId))
                return string.Empty;
            TrumpJoining data = DataList[joiningId];
            string res = string.Empty;
            for(int i = 0; i < data.trump.Length;i++)
            {
                res += TrumpProperty.GetUIName(data.trump[i]);
                if (i != data.trump.Length - 1)
                    res += "+";
            }
            res = string.Format(data.joiningdes, res);
            return res;
        }

        public static string GetDefaultJoiningDes(int joiningId)
        {
            if (!DataList.ContainsKey(joiningId))
                return string.Empty;
            TrumpJoining data = DataList[joiningId];
            string res = string.Empty;
            for (int i = 0; i < data.trump.Length; i++)
            {
                res += TrumpProperty.GetColorName(data.trump[i]);
                if (i != data.trump.Length - 1)
                    res += "+";
            }
            res = string.Format(data.joiningdes, res);
            return res;
        }

        public static string GetUTJoiningDes(int joiningId)
        {

            if (!DataList.ContainsKey(joiningId))
                return string.Empty;
            TrumpJoining data = DataList[joiningId];
            string res = string.Empty;
            for (int i = 0; i < data.trump.Length; i++)
            {
                res += TrumpProperty.GetUTColorName(data.trump[i]);
                if (i != data.trump.Length - 1)
                    res += "+";
            }
            res = string.Format(data.joiningdes, res);
            return res;
        }
    }

    public partial class TrumpSoul
    {
        //法宝ID，境界等级 list
        public static Dictionary<int, Dictionary<int, TrumpSoul>> soulDic = new Dictionary<int, Dictionary<int, TrumpSoul>>();
        static void OnLoadEnd()
        {
            for (int i = 0; i < TrumpSoul.DataList.Count; i++)
            {
                TrumpSoul data = TrumpSoul.DataList[i];

                if (!soulDic.ContainsKey(data.trumpid))
                {
                    Dictionary<int, TrumpSoul> tempLvList = new Dictionary<int, TrumpSoul>();
                    tempLvList.Add(data.tastelv, data);
                    soulDic.Add(data.trumpid, tempLvList);
                }
                else if (!soulDic[data.trumpid].ContainsKey(data.tastelv))
                {
                    soulDic[data.trumpid].Add(data.tastelv, data);
                }
            }
        }
    }

    public partial class TrumpProperty
    {
        public static string GetColorName(int trumpId)
        {
            if (!GetAll().ContainsKey(trumpId))
                return trumpId.ToString();
            TrumpProperty property = Get(trumpId);
            return  string.Format("<color=#{0}>{1}</color>", QualitySourceConfig.Get(property.quality).color, property.name);
        }

        public static string GetUTColorName(int trumpId)
        {
            if (!GetAll().ContainsKey(trumpId))
                return trumpId.ToString();
            TrumpProperty property = Get(trumpId);
            return string.Format("#c{0}{1}#n", QualitySourceConfig.Get(property.quality).color, property.name);
        }

        public static string GetUIName(int trumpId)
        {
            if (!GetAll().ContainsKey(trumpId))
                return trumpId.ToString();
            TrumpProperty property = Get(trumpId);
            string res = string.Format("#h#c{0}[{1}]{{2}}#h", QualitySourceConfig.Get(property.quality).color, property.name);
            res = res.Replace("2", trumpId.ToString());
            return res;
        }
    }
}

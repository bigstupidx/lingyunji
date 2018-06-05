using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Config
{
    public partial class SkillIconConfig 
    {
        public static string GetIcon(int skillid)
        {
            SkillIconConfig cfg = Get(skillid);
            if (cfg == null)
                return "item_malei";
            else
                return cfg.icon;
        }
    }

    public partial class MoveRateConfig
    {
        public static float GetMoveRate(string model,string name)
        {
            List<MoveRateConfig> list = null;
            if (DataList_key.TryGetValue(model, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].name == name)
                        return list[i].aniSpeed;
                }
            }

            return 5;
        }
    }


    public static class ClassExtend
    {
        public static Vector3 InitConfig(this Vector3 v,string text)
        {
            float[] sList = ParsrFloats(text);
            return new Vector3(sList[0], sList[1], sList[2]);
        }

        static float[] ParsrFloats( string text )
        {
            string[] list = text.Split(':');
            float[] vList = new float[list.Length];
            for(int i=0;i<vList.Length;i++)
            {
                float.TryParse(list[i], out vList[i]);
            }
            return vList;
        }
    }

    public class PetSavvy
    {
        public struct SavvyData
        {
            public int value;
            public int weight;
        }
        public List<SavvyData> list = new List<SavvyData>();
        public static PetSavvy InitConfig(string text)
        {
            PetSavvy petSavvy = new PetSavvy();
            string[] value = text.Split(',');
            for(int i = 0; i < value.Length;i++)
            {
                string[] res = value[i].Split(':');
                if(res.Length == 2)
                {
                    int temp1, temp2;
                    if(int.TryParse(res[0],out temp1) && int.TryParse(res[1],out temp2))
                    {
                        SavvyData data = new SavvyData();
                        data.value = temp1;
                        data.weight = temp2;
                        petSavvy.list.Add(data);
                    }
                }
            }
            return petSavvy;
        }

    }
    public class RandomRange
    {
        public float m_Min = 0;
        public float m_Max = 0;
        public static RandomRange InitConfig(string str)
        {
            RandomRange ret = new RandomRange();
            if (str.Length>0)
            {
                string[] value = str.Split(',');
                ret.m_Min = float.Parse(value[0]);
                ret.m_Max = float.Parse(value[1]);
            }
            return ret;
        }
    }
    public class ProbabilityTable
    {
        public Dictionary<int, int> m_ProbabilityDic = new Dictionary<int, int>();
        public static ProbabilityTable InitConfig(string str)
        {
            ProbabilityTable ret = new ProbabilityTable();
            if (str.Length > 0)
            {
	            string[] value = str.Split(',');
	            for (int i = 0; i < value.Length; i++)
	            {
	                int[] singlePair = ParseSinglePair(value[i]);
	                ret.m_ProbabilityDic.Add(singlePair[0], singlePair[1]);
	            }
            }
            return ret;
        }
        public static int[] ParseSinglePair(string str)
        {
            int[] ret = new int[2];
            string[] value = str.Split(':');
            ret[0] = int.Parse(value[0]);
            ret[1] = int.Parse(value[1]);
            return ret;
        }
    }

    public class SubTypeTable
    {
        public int[] m_subTypes;
        public static SubTypeTable InitConfig(string str)
        {
            SubTypeTable subTypeTable = new SubTypeTable();
            List<int> data = new List<int>();
            int front = 0, back = 0;
            while (front <= str.Length)
            {
                back = str.IndexOf(",", front) == -1 ? str.Length + 1 : str.IndexOf(",", front);
                int temp = int.Parse(str.Substring(front, back - front));
                data.Add(temp);
                front = back + 1;
            }
            subTypeTable.m_subTypes = data.ToArray();

            return subTypeTable;
        }
    }

    public class EffectTable
    {
        public int effectMinLevel = 0;
        public int effectMaxLevel = 0;
        public int probability = 0;

        public static EffectTable InitConfig(string str)
        {
            EffectTable ret = new EffectTable();
            string[] value = str.Split(',');
            if (value.Length == 3)
            {
                ret.effectMinLevel = int.Parse(value[0]);
                ret.effectMaxLevel = int.Parse(value[1]);
                ret.probability = int.Parse(value[2]);
            }
            return ret;
        }
    }

    public class SoulAttr
    {
        public double value;
        public JobMask jobMask;

        public static SoulAttr InitConfig(string str)
        {
            SoulAttr ret = new SoulAttr();
            string[] dataArray = str.Split('|');
            if (dataArray.Length > 1)
            {
                ret.value = double.Parse(dataArray[0]);
                ret.jobMask = JobMask.InitConfig(dataArray[1]);
            }
            return ret;
        }
    }
}
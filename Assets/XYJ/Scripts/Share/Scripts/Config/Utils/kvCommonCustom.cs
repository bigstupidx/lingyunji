using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    /// <summary>
    /// 杂项里面的keyvalue对
    /// </summary>
    public partial class kvCommon
    {
        public static List<int[]> petLockInfo { get; private set; }

        //角色等级上限
        public static int levelMax { get; private set; }

        static void OnLoadEnd()
        {

            levelMax = GetInt("levelMax", 10);

            petLockInfo = new List<int[]>();
            string[] tempStr = kvCommon.Get("PetSkillLockNum").value.Split('|');
            for(int i = 0; i < tempStr.Length;i++)
            {
                string[] tempStr1 = tempStr[i].Split(',');
                if (tempStr1.Length != 2)
                    continue;
                int[] res = new int[2];
                res[0] = int.Parse(tempStr1[0]);
                res[1] = int.Parse(tempStr1[1]);
                petLockInfo.Add(res);
            }
        }

        static int GetInt(string key, int defaultV)
        {
            kvCommon p = Get(key);
            if (p != null)
                return int.Parse(p.value);
            else
                return defaultV;
        }
        static float GetFloat(string key, float defaultV)
        {
            kvCommon p = Get(key);
            if (p != null)
                return float.Parse(p.value);
            else
                return defaultV;
        }
    }
}

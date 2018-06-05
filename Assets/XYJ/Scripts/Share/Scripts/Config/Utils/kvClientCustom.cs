using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    /// <summary>
    /// 杂项里面的keyvalue对
    /// </summary>
    public partial class kvClient
    {
        public static List<float> petsLevelDefine { get; private set; }
        public static List<string> petsColorDefine { get; private set; }
        public static List<string> petsLevelTextDefine { get; private set; }
        static void OnLoadEnd()
        {

            petsLevelDefine = new List<float>(); 
            string[] temp = kvClient.Get("PetSkilDefine").value.Split('|');
            for(int i = 0; i < temp.Length; i++)
                petsLevelDefine.Add(int.Parse(temp[i]));

            petsColorDefine = new List<string>();
            temp = kvClient.Get("PetSkillColorDefine").value.Split('|');
            for (int i = 0; i < temp.Length; i++)
                petsColorDefine.Add(temp[i]);

            petsLevelTextDefine = new List<string>();
            temp = kvClient.Get("PetSkillTextDefine").value.Split('|');
            for (int i = 0; i < temp.Length; i++)
                petsLevelTextDefine.Add(temp[i]);
        }

        public static int GetInt(string key,int defaultV)
        {
            kvClient p = Get(key);
            if (p != null)
                return int.Parse(p.value);
            else
                return defaultV;
        }
        public static float GetFloat(string key, float defaultV)
        {
            kvClient p = Get(key);
            if (p != null)
                return float.Parse(p.value);
            else
                return defaultV;
        }
    }
}

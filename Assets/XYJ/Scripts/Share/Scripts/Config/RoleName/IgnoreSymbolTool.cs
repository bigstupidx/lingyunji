using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public partial class IgnoreSymbol
    {
        /// <summary>
        /// 获取过滤后的名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFilterName(string name)
        {
            Dictionary<int, IgnoreSymbol> datas = GetAll();
            List<char> symbolList = new List<char>();
            foreach(var itor in datas)
            {
                string specialName = itor.Value.specialName;
                char[] specialList = specialName.ToCharArray();
                for(int index = 0; index < specialList.Length; ++index)
                {
                    symbolList.Add(specialList[index]);
                }
            }

            string retName = "";
            char[] charList = name.ToCharArray();
            for(int i = charList.Length - 1; i >=0; --i)
            {
                if(!symbolList.Contains(charList[i]))
                {
                    retName = charList[i] + retName;
                }
            }

            return retName;
        }
    }
}

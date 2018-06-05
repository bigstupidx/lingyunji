using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class StringHashCode
    {
        static Dictionary<int, string> HashToString = new Dictionary<int, string>();

        public static void Release()
        {
            HashToString.Clear();
        }

        public static void Add(string s)
        {
            if (s.EndsWith(".x"))
                s = s.Substring(0, s.Length - 2);

            string hash;
            if (HashToString.TryGetValue(s.GetHashCode(), out hash))
            {
                if (s != hash)
                {
                    Debuger.ErrorLog("{0} {1} hashcode same!", s, hash);
                }
            }
            else
            {
                HashToString.Add(s.GetHashCode(), s);
            }
        }

        public static string Get(int hashcode)
        {
            string hash;
            if (HashToString.TryGetValue(hashcode, out hash))
                return hash;

            Debuger.ErrorLog("StringHashCode:{0}", hashcode);
            return string.Empty;
        }

#if UNITY_EDITOR
        public static Dictionary<int, string> PathToHash()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (string file in Directory.GetFiles("Assets", "*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".meta"))
                    continue;

                if (file.StartsWith("Assets/__copy__"))
                    continue;

                string key = file.Substring(7);
                key = key.Replace('\\', '/');
                dic.Add(key.GetHashCode(), key);
            }

            return dic;
        }
#endif
    }
}
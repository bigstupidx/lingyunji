using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

/// <summary>
/// 多语言字符串处理模块.key可以直接使用中文，开发阶段直接返回key，后面可以通过配表显示相应text
/// </summary>
public class StrUtil
{
    //代码里直接使用的中文，需要调用改接口来获取,不能给静态变量赋值
    public static string GetStr(string key)
    {
        return key;
    }

    public static string GetStrFormat(string key, params object[] ps)
    {
        if (ps == null || ps.Length == 0)
            return key;
        else
        {
            try
            {
                return string.Format(key, ps);
            }
            catch (System.Exception ex)
            {
                XYJLogger.LogException(ex);
                return key;
            }
        }
    }

    public static String intToZH(int i)
    {
        String[] zh = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        String[] unit = { "", "十", "百", "千", "万", "十", "百", "千", "亿", "十" };

        String str = "";
        char[] charArray = i.ToString().ToCharArray();
        Array.Reverse(charArray);
        string sb = new string(charArray);
 
        int r = 0;
        int l = 0;
        for (int j = 0; j < sb.Length; j++)
        {
            if (j + j + 1 > sb.Length)
                continue;
            r = Int32.Parse(sb.Substring(j, j + 1));

            if (j != 0)
                l = Int32.Parse(sb.Substring(j - 1, j));

            if (j == 0)
            {
                if (r != 0 || sb.Length == 1)
                    str = zh[r];
                continue;
            }

            if (j == 1 || j == 2 || j == 3 || j == 5 || j == 6 || j == 7 || j == 9)
            {
                if (r != 0)
                    str = zh[r] + unit[j] + str;
                else if (l != 0)
                    str = zh[r] + str;
                continue;
            }

            if (j == 4 || j == 8)
            {
                str = unit[j] + str;
                if ((l != 0 && r == 0) || r != 0)
                    str = zh[r] + str;
                continue;
            }
        }
        return GetStr(str);
    }

    public static string[] ToArr<T>(T[] a)
    {
        if (a == null || a.Length == 0)
            return null;
        return System.Array.ConvertAll<T, string>(a, obj => obj.ToString());
    }

        public static string Join<T>(T[] a, string split)
    {
        if (a == null || a.Length == 0)
            return "";
        return string.Join(split, ToArr<T>(a));
    }

    public static string Join<T>(List<T> a, string split = ",")
    {
        if (a == null || a.Count == 0)
            return "";
        return string.Join(split, ToArr<T>(a.ToArray()));
    }
    public static void Parse(string s, ref List<int> l, char split = ',')
    {
        l.Clear();
        if (string.IsNullOrEmpty(s))
            return;
        int i;
        string[] ss = s.Split(split);
        foreach (string item in ss)
        {
            if (int.TryParse(item, out i))
                l.Add(i);
        }
    }

    public static string RichColorText(string txt, Color c)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", (int)(c.r * 255), (int)(c.g * 255), (int)(c.b * 255), (int)(c.a * 255), txt);
    }
}

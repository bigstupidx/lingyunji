using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using xys.battle;

namespace Config
{
    //属性提供的接口
    public partial class AttributeDefine
    {
        //ui显示格式
        public enum UIShowType
        {
            Normal,
            Percent,
        };

        public enum DataType
        {
            Int,
            Float
        };


        //根据属性名字获取下标
        public static int GetIndexByName(string name)
        {
            int index;
            if (s_nameToIndexs.TryGetValue(name, out index))
                return index;
            else
                return -1;
        }

        //获取类型，后面考虑使用数组
        public static DataType GetDataType(int index)
        {
            AttributeDefine p;
            if (DataList.TryGetValue(index, out p))
                return p.dataType;
            else
                return DataType.Int;
        }

        static Dictionary<string, int> s_nameToIndexs;

        static void OnLoadEnd()
        {
            s_nameToIndexs = new Dictionary<string, int>();

            foreach (var p in DataList)
                s_nameToIndexs.Add(p.Value.name, p.Key);
        }

        //客户端接口
#if !COM_SERVER
        //获取属性描述Tips文本
        public static string GetAttriTips(int index, int jobId, BattleAttri battleAttri)
        {
            string text = "";
            AttributeDefine p;
            RoleChangeAttr r;

            if (!DataList.TryGetValue(index, out p)) return text;
            switch (index)
            {
                case iStrength:
                    if (RoleChangeAttr.GetChangeAttrByNameInGroup(jobId, p.name, out r))
                        text = string.Format(p.attrDescribe, r.physicalAttack);
                    break;

                case iIntelligence:
                    if (RoleChangeAttr.GetChangeAttrByNameInGroup(jobId, p.name, out r))
                        text = string.Format(p.attrDescribe, r.magicAttack);
                    break;

                case iBone:
                    if (RoleChangeAttr.GetChangeAttrByNameInGroup(jobId, p.name, out r))
                        text = string.Format(p.attrDescribe, r.iHp, r.physicalDefense);
                    break;

                case iPhysique:
                    if (RoleChangeAttr.GetChangeAttrByNameInGroup(jobId, p.name, out r))
                        text = string.Format(p.attrDescribe, r.cureRate, r.magicDefense, r.parryLevel);
                    break;

                case iAgility:
                    if (RoleChangeAttr.GetChangeAttrByNameInGroup(jobId, p.name, out r))
                        text = string.Format(p.attrDescribe, r.critLevel, r.hitLevel);
                    break;

                case iBodyway:
                    if (RoleChangeAttr.GetChangeAttrByNameInGroup(jobId, p.name, out r))
                        text = string.Format(p.attrDescribe, r.critDefenseLevel, r.avoidLevel);
                    break;

                default:
                    text = GetAttrTipsWithValue(p.attrDescribe, battleAttri);
                    break;
            }

            text = GlobalSymbol.ToUT(text);
            text += index == RoleChangeAttr.GetMainAttriId(jobId) ? "\n" + RoleChangeAttr.GetMainAttriDesc(jobId) : "";

            return text;
        }

        //获取对应数值替换[att_*]属性描述Tips文本
        public static string GetAttrTipsWithValue(string t, BattleAttri battleAttri)
        {
            Regex r = new Regex(@"\[att_(.+?)\]", RegexOptions.Singleline);
            foreach (Match item in r.Matches(t))
            {
                if (item.Groups.Count >= 2)
                {
                    int attrIndex = GetIndexByName(item.Groups[1].ToString());
                    t = t.Replace(item.Groups[0].ToString(), GetValueStr(attrIndex, battleAttri.Get(attrIndex)).ToString());
                }
            }
            return t;
        }

        //获得属性ui标题
        public static string GetTitleStr(int index)
        {
            AttributeDefine p;
            if (DataList.TryGetValue(index, out p))
                return p.attrName;
            else
                return "";
        }

        //获得属性值显示字符串
        public static string GetValueStr(int index, double value)
        {
            AttributeDefine p;
            if (DataList.TryGetValue(index, out p))
            {
                if (p.uiShowType == UIShowType.Percent)
                    return (Math.Ceiling(value * 1000) * 0.1) + "%";
                else
                    return Math.Ceiling(value).ToString();
            }
            return "";
        }
#endif
    }
}
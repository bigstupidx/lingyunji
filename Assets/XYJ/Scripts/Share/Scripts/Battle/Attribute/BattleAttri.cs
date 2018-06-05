using Config;
using System;
using System.Collections.Generic;


namespace xys.battle
{
    /// <summary>
    /// 战斗属性
    /// </summary>
    public class BattleAttri
    {
        Dictionary<int, double> m_values = new Dictionary<int, double>();

        public void Copy(BattleAttri attri)
        {
            if (this == attri)
                return;
            m_values.Clear();
            foreach (var index in attri.GetKeys())
            {
                Set(index, attri.Get(index));
            }
        }

        public double Get(int index)
        {
            double v;
            if (m_values.TryGetValue(index, out v))
                return v;
            else
                return 0;
        }

        public virtual void Set(int index, double value)
        {
            if (!m_values.ContainsKey(index))
                m_values.Add(index, value);
            else
                m_values[index] = value;
        }

        public virtual void Add(int index, double value)
        {
            if (!m_values.ContainsKey(index))
                m_values.Add(index, value);
            else
                m_values[index] += value;
        }

        public virtual void Mul(int index, double value)
        {
            if (m_values.ContainsKey(index))
                m_values[index] *= value;
        }

        public void Clear()
        {
            m_values.Clear();
        }

        public Dictionary<int, double>.KeyCollection GetKeys()
        {
            return m_values.Keys;
        }

        public void Add(BattleAttri attri)
        {
            foreach (var index in attri.GetKeys())
            {
                Set(index, Get(index) + attri.Get(index));
            }
        }

        public void Sub(BattleAttri attri)
        {
            foreach (var index in attri.GetKeys())
            {
                Set(index, Get(index) - attri.Get(index));
            }
        }


        public void Mul(BattleAttri attri)
        {
            foreach (var index in attri.GetKeys())
            {
                Set(index, Get(index) * attri.Get(index));
            }
        }

        //浮点数保留4位，整形和浮点数都是向上取整
        public void Round()
        {
            Dictionary<int, double> temMap = new Dictionary<int, double>();
            foreach (var p in m_values)
            {
                if (AttributeDefine.GetDataType(p.Key) == AttributeDefine.DataType.Int)
                    temMap.Add(p.Key, Math.Ceiling(p.Value));
                else
                {
                    temMap.Add(p.Key, RoundFloat(p.Value));
                }
            }
            m_values = temMap;
        }

        //一般计算过程都不需要取整，只有最终在界面显示才需要
        public static int RoundInt(double value)
        {
            return (int)Math.Ceiling(value);
        }

        public static double RoundFloat(double value)
        {
            double b = (float)(value * 10000);
            return Math.Ceiling(b) / 10000;
        }

        public string GetAllValueString()
        {
            List<int> ids = new List<int>();
            foreach (var p in AttributeDefine.GetAll())
            {
                ids.Add(p.Key);
            }
            ids.Sort();
            string text = "";
            foreach (var id in ids)
            {
                var p = AttributeDefine.Get(id);
                var value = Get(id);
                text = string.Format("{0} {1} {2} {3}\r\n", text, id, p.name, value.ToString());
            }

            text += "\r\n";
            return text;
        }

        //按属性类型添加
        public void AddByLevel(BattleAttri attri, bool isLevel1)
        {
            foreach (var index in attri.GetKeys())
            {
                switch (index)
                {
                    case AttributeDefine.iStrength:
                    case AttributeDefine.iIntelligence:
                    case AttributeDefine.iBone:
                    case AttributeDefine.iPhysique:
                    case AttributeDefine.iAgility:
                    case AttributeDefine.iBodyway:
                        if (!isLevel1)
                            continue;
                        break;
                    default:
                        if (isLevel1)
                            continue;
                        break;
                }
                Set(index, Get(index) + attri.Get(index));
            }
        }
        //是否包含一级属性
        public bool IsContainLevel1()
        {
            foreach (var index in m_values.Keys)
            {
                switch (index)
                {
                    case AttributeDefine.iStrength:
                    case AttributeDefine.iIntelligence:
                    case AttributeDefine.iBone:
                    case AttributeDefine.iPhysique:
                    case AttributeDefine.iAgility:
                    case AttributeDefine.iBodyway:
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }
    }
}

#if !USE_HOT
namespace xys.hot
{
    using battle;
    using UnityEngine;
    using Config;
    using UnityEngine.UI;
    using NetProto;
    using NetProto.Hot;
    using System.Collections.Generic;

    public partial class TrumpObj
    {
        BattleAttri m_BattleAttri = new BattleAttri();
        public BattleAttri battleAttri { get { return m_BattleAttri; } }

        int m_MaxSoulPoint;
        public int maxSoulPoint { get { return m_MaxSoulPoint; } }
        int m_UseSoulPoint;
        public int useSoulPoint { get { return m_UseSoulPoint; } }
        public bool Init(TrumpAttribute attribute)
        {
            this.m_BattleAttri.Clear();
            //检查数据是否异常
            if (!TrumpProperty.GetAll().ContainsKey(attribute.id))
                return false;

//             AttributeDefine.uAttributeDefine flags = AttributeDefine.uAttributeDefine.uNone;
//             foreach (int id in attribute.cultivatepoints.Keys)
//                 flags |= (AttributeDefine.uAttributeDefine)(1 << id);
//             if (!AttributeDefine.CheckBaseAttribute(flags))
//                 return false;
            //基础属性附加
            TrumpProperty property = TrumpProperty.Get(attribute.id);
            m_BattleAttri.Add(property.battleAttri);
            //注灵属性附加
            if (TrumpInfused.infusedDic.ContainsKey(attribute.id))
            {
                List<TrumpInfused> infusedList = null;
                //
                for (int i = 0; i < attribute.tastelv; i++)
                {
                    infusedList = TrumpInfused.infusedDic[attribute.id][i];
                    foreach (TrumpInfused infusedData in infusedList)
                    {
                        if (infusedData == null)
                            continue;
                        m_BattleAttri.Add(infusedData.battleAttri);
                    }
                }

                infusedList = TrumpInfused.infusedDic[attribute.id][attribute.tastelv];
                foreach (int id in attribute.infuseds)
                {
                    TrumpInfused infusedData = TrumpInfused.Get(id);
                    if (infusedData == null)
                        continue;
                    m_BattleAttri.Add(infusedData.battleAttri);
                }
            }

            //潜修属性附加
            foreach (int attributeIndex in attribute.cultivatepoints.Keys)
            {
                if (!TrumpCultivate.GetAll().ContainsKey(attribute.cultivatepoints[attributeIndex].value))
                    continue;
                int index = attribute.cultivatepoints[attributeIndex].value;
                double value = m_BattleAttri.Get(attributeIndex);
                for (int i = 1; i <= index; i++)
                {
                    TrumpCultivate cultivatesData = TrumpCultivate.Get(i);
                    value += Mathf.Ceil((float)value * (float)cultivatesData.propertyvalue * 0.01f);
                }
                if(value > 0)
                    m_BattleAttri.Set(attributeIndex,value);
            }

            //计算拥有多少潜修点
            foreach (TrumpCultivateExp data in TrumpCultivateExp.GetAll().Values)
            {
                if (data.id < attribute.souldata.lv)
                    m_MaxSoulPoint += data.point;
                else
                    break;
             }
            //已使用点数
            foreach(Int32 value in attribute.cultivatepoints.Values)
            {
                for (int i = 0; i <= value.value; i++)
                    m_UseSoulPoint += TrumpCultivate.Get(i).soulpoints;
            }
            return true;
        }
    }
}
#endif
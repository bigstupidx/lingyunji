using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{

    public class BuffAddAttribute : IBuffTypeLogic
    {
        BattleAttri attri;
        bool m_isLevel1;
        public void ParsePara(string[] para)
        {
            attri = AttributeEffectConfig.Get(para[0]).battleAttri;
            m_isLevel1 = attri.IsContainLevel1();
        }
        public void OnEnter(IObject source, IObject target)
        {
            if (attri == null)
                return;
            target.battle.m_attrMgr.RefreshAttributeByBuff(attri, true, m_isLevel1);
        }
        public void OnExit(IObject target)
        {
            if (attri == null)
                return;
            target.battle.m_attrMgr.RefreshAttributeByBuff(attri,false, m_isLevel1);
        }
    }
}

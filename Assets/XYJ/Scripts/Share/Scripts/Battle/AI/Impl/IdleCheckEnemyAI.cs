using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{
    class IdleCheckEnemyAI : ISimpleAI
    {
        public override void OnUpdate()
        {
            if(m_obj.battle.m_targetMgr.target!=null)
                m_obj.battle.m_ai.EnterBattleAI();
        }
    }
}

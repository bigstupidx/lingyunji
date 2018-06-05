using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using behaviac;
using GameServer;
using Config;

namespace xys.AI
{
    public class BT_PlayAnimation : BT_IAction
    {
        float m_time;
        public override bool OnStart(object[] para)
        {
            m_time = BattleHelp.timePass;
            XYJLogger.LogError("ani " + para[0]);
            return true;
        }

        public override EBTStatus OnUpdate()
        {
            if (BattleHelp.timePass - m_time > 3)
                return EBTStatus.BT_SUCCESS;
            else
                return EBTStatus.BT_RUNNING;
        }
    }
}
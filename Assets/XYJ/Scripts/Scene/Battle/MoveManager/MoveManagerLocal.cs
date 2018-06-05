using xys.battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class MoveManagerLocal : MoveManagerBase
    {
        //摇杆移动
        public void JoystickMove(Vector3 moveSpeed)
        {
            moveSpeed.y = -10;
            ChangeState(MoveState.Stop);
            Vector3 move = moveSpeed * Time.deltaTime;
            BattleHelp.SetLookAt(m_obj, m_obj.position + move);
            m_obj.battle.actor.CCMove(move);
        }
    }
}
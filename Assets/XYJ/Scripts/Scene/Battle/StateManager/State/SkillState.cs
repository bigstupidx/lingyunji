using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class SkillState : IState
    {
        public override void OnEnter(IObject obj, StateType lastState, object para) 
        { 

        }

        public override void OnExit(IObject obj, StateType nextState) 
        {
            //SwitchByOperation类型技能结束了，立马切换回合适技能
            if ((obj is LocalPlayer))
                ((SkillManagerLocal)obj.battle.m_skillMgr).CheckToDefaultSkill(nextState == StateType.Skill);
                
            obj.battle.m_skillMgr.Stop();
        }

        public override void OnUpdate(IObject obj) 
        {
            if (!obj.battle.m_skillMgr.IsPlaying())
                obj.battle.m_stateMgr.SetStateFinish();
        }

    }

}

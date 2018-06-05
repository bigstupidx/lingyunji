using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using behaviac;
using GameServer;

namespace xys.AI
{

    public class BT_ActionCreate
    {
        public static BT_IAction Create(string name)
        {
            switch (name)
            {
                case "Skill_FollowToPlay":
                    return new BT_SkillFollowToPlay();
                case "Animation_Play":
                    return new BT_PlayAnimation();
                default:
                    return null;
            }
        }
    }
}

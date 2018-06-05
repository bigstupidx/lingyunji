using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class ManagerCreate
    {
        public static SkillManagerBase CreateSkillManager(IObject obj)
        {
            if (obj is LocalPlayer)
                return new SkillManagerLocal();
            else
                return new SkillManagerBase();
        }

        public static TargetManager CreateTargetManager(IObject obj)
        {
            if (obj is LocalPlayer)
                return new TargetManagerLocal();
            else
                return new TargetManagerMonster();
        }

        public static BattleManagerBase CreateBattleManager(IObject obj)
        {
            if (obj is LocalPlayer)
                return new BattleManagerLocal(obj);
            else
                return new BattleManagerBase(obj);
        }

        public static MoveManagerBase CreateMoveManager(IObject obj)
        {
            if (obj is LocalPlayer)
                return new MoveManagerLocal();
            else
                return new MoveManagerBase();
        }

        public static JumpManagerBase CreateJumpManager(IObject obj)
        {
            if (obj is LocalPlayer)
                return new JumpManagerLocal();
            else
                return new JumpManagerRemote();
        }

        public static AIManager CreateAI(IObject obj)
        {
              return new AIManagerBase();
        }
    }
}
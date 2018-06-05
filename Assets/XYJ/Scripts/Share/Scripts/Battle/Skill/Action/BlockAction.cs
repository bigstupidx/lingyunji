using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class BlockAction : IAction<BlockActionConfig>
    {

        public override RunType GetRunType()
        {
            return RunType.Both;
        }


        public override bool OnExcute(ActionInfo info)
        {
            info.source.battle.m_skillMgr.SetBlockAction(cfg);
            return true;
        }
    }
}

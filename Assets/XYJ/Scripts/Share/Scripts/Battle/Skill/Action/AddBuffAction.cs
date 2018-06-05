using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace xys.battle
{
    class AddBuffAction : IAction<AddBuffActionConfig>
    {
        public override RunType GetRunType()
        {
            return RunType.ServerToClient;
        }



        public override bool OnExcute(ActionInfo info)
        {
            IObject addToTarget;
            //buff内部不需要广播，因为action前后端都会执行
            if (cfg.targetType == EffectTarget.Self)
                addToTarget = info.source;
            else
                addToTarget = info.target;

            foreach (int id in cfg.buffid)
            {
                addToTarget.battle.m_buffMgr.AddBuff(info.source, id);
            }
            return true;
        }
    }
}

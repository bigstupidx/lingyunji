using Config;
using GameServer;
using NetProto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class SummonAction : IAction<SummonActionConfig>
    {

        public override RunType GetRunType()
        {
            return RunType.ServerOnly;
        }


        public override bool OnExcute(ActionInfo info)
        {
            for(int i=0;i<cfg.objCnt;i++)
            {
                int roleid;
                if (cfg.objIds.Length == 1)
                    roleid = cfg.objIds[0];
                else
                    roleid = cfg.objIds[BattleHelp.Rand(0, cfg.objIds.Length)];

                Vector3 posoff = new Vector3(cfg.bornPos[i+1], 0, cfg.bornPos[i]);
                Vector3 toPos = Vector3.zero;
                if (!MoveAction.GetPos(cfg.posType, info, posoff, ref toPos))
                    break;
#if COM_SERVER
                info.source.zone.CreateObject(roleid, toPos, BattleHelp.Angle2Vector(info.source.rotateAngle));
#endif
            }
            return true;
        }
    }
}

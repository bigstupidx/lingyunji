using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class BulletAction : IAction<BulletActionConfig>
    {

        public override RunType GetRunType()
        {
            return RunType.ServerToClient;
        }

        public override bool OnExcute(ActionInfo info)
        {
#if !COM_SERVER
            BulletLogicClient bullet = BulletLogicCreate.Create(cfg);
#else
            BulletLogic bullet = new BulletLogic();
#endif
            if (bullet.Init(cfg, info))
                info.source.battle.m_skillMgr.AddBullet(bullet);
            return true;
        }
    }
}

using NetProto;
using CommonBase;
using System.Collections.Generic;
using xys.battle;
using UnityEngine;
using Config;

#if COM_SERVER
    using GameServer.Battle;
    namespace GameServer
#else
namespace xys
#endif
{
    abstract public partial class ObjectBase
    {
        //配置表的ID
        public RoleDefine cfgInfo { get; protected set; }
        public BattleCamp battleCamp { get; protected set; }
        // 战斗相关
        public BattleManagerBase battle { get; private set; }
        public RoleJob.Job job { get; protected set; }
        public int charSceneId { get; protected set; }
        public ObjectType type { get; private set; }
        //刷新点数据,组件需要在OnEnterScene之后才能使用
        public int m_refreshId { get; private set; }
        public LevelDesignConfig.LevelSpawnData m_refreshData { get; private set; }
        protected void InitCfg(int cfgid)
        {
            cfgInfo = RoleDefine.Get(cfgid);
            if (cfgInfo == null)
                cfgInfo = RoleDefine.Get(100);
            job = RoleJob.GetJobByRoleid(cfgid, type);

            if (type == ObjectType.Player)
                battleCamp = BattleCamp.PlayerCamp;
            else
                battleCamp = cfgInfo.battleCamp;
        }

        public void SetLevel(int level)
        {
            if (level <= 0)
            {
                Log.Error("SetLevel level:{0} < 0", level);
                return;
            }
            levelValue = (ushort)level;
        }

        public void SetRefreshId(int refreshId, LevelDesignConfig.LevelSpawnData spawnData)
        {
            m_refreshId = refreshId;
            m_refreshData = spawnData;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using NetProto;
using CommonBase;
using Config;

namespace xys.battle
{
    //前后端共享的对象
    public partial interface IShareObject
    {
        // 由场景分配的ID
        int charSceneId { get; }
        Vector3 position { get; }
        float rotateAngle { get; }
        ObjectType type { get; }
        //配置表信息(如果是法术场对象，该值为null)
        RoleDefine cfgInfo { get; }
        //是否存活
        bool isAlive { get; }
        //职业
        RoleJob.Job job { get; }
        //战斗阵营
        BattleCamp battleCamp { get; }
        //刷新点
        int m_refreshId { get;}
        LevelDesignConfig.LevelSpawnData m_refreshData { get; }

        #region 方法
        // 设置位置
        void SetPosition(Vector3 pos);
        void SetRotate(float angle);
        void SetDead();

        ////走到目的地,走到目的地回调，中断移动不会回调
        //bool MoveToPos(Vector3 pos,Action<object> onFinishMove = null);

        //施放技能
        //bool PlaySkill(int skillid);
        #endregion
    }
}

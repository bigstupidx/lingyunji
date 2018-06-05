using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using GameServer;
using NetProto;

namespace xys.battle
{
    public partial class SkillManager
    {
        //格挡
        public BlockActionConfig m_blockActionCfg { get; private set; }
        float m_blockActionFinishTime;

        public void SetBlockAction(BlockActionConfig cfg)
        {
            m_blockActionCfg = cfg;
            m_blockActionFinishTime = BattleHelp.timePass + cfg.time;
        }

        void UpdateSpecial()
        {
            if (m_blockActionCfg != null && BattleHelp.timePass >= m_blockActionFinishTime)
                m_blockActionCfg = null;
        }

        //技能结束需要清除
        void StopSpecial()
        {
            m_blockActionCfg = null;
        }


        public void OnHandleAttackData(IAction.ActionInfo info, AttackActionConfig cfg)
        {
            //处理格挡,正面攻击才算
            if (m_blockActionCfg != null && BattleHelp.IsForward(info.target.rotateAngle, info.source.position - info.target.position))
            {
                AttackFlg.SetFlg(ref info.targetAction.attack.damageFlg, AttackFlg.Flg.GeDange);
                if (m_blockActionCfg.actionList.Count > 0)
                    ActionManager.HandleActionList(info.skill,info.target,info.source,info.actionRecord, m_blockActionCfg.actionList,info.firePos,info.fireAngle);
            }
        }
    }
}
using Config;
using NetProto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class ConditionAction : IAction<ConditionActionConfig>
    {

        public override RunType GetRunType()
        {
            return RunType.ServerOnly;
        }


        public override bool OnExcute(ActionInfo info)
        {
            List<IAction> actionList;
            if (Check(info))
                actionList = cfg.suscessActions;
            else
                actionList = cfg.failActions;

            //只要没执行事件就返回false
            if(actionList == null || actionList.Count==0)
                return false;
            ActionManager.HandleActionList(info.skill, info.source, info.target, info.actionRecord, actionList, info.firePos, info.fireAngle);
            return true;
        }

        bool Check(ActionInfo info)
        {
            if (cfg.rate < 1 && !BattleHelp.RandPercent(cfg.rate))
                return false;
            if (cfg.m_checkTargetConditon.Count == 0)
                return true;

            IObject checkTarget;
            if (cfg.targetType == EffectTarget.Self)
                checkTarget = info.source;
            else
                checkTarget = info.target;
            if (checkTarget == null)
                return false;
            for (int i = 0; i < cfg.m_checkTargetConditon.Count; i++)
            {
                if (!cfg.m_checkTargetConditon[i](checkTarget, cfg, info))
                    return false;
            }
            return true;
        }


#region 作用单位条件 
        static public List<Func<IObject, ConditionActionConfig, ActionInfo, bool>> ParseCheckTarget(ConditionActionConfig cfg)
        {
            List<Func<IObject, ConditionActionConfig,ActionInfo, bool>> list = new List<Func<IObject, ConditionActionConfig, ActionInfo, bool>>();
            if (cfg.hpMul.Length == 2)
                list.Add(CheckHp);
            if (cfg.buff != 0)
            {
                if (BuffConfig.Get(cfg.buff).addType == BuffConfig.AddType.ObjectAdd)
                    XYJLogger.LogError("conditon不支持buff类型为 不同施法者独立叠加");
                else
                    list.Add(CheckBuff);
            }

            return list;
        }
        static bool CheckHp(IObject checkTarget, ConditionActionConfig cfg, ActionInfo info)
        {
            float mul = (float)checkTarget.hpValue / checkTarget.maxHpValue;
            return cfg.hpMul[0] <= mul && cfg.hpMul[1] >= mul;
        }
        static bool CheckBuff(IObject checkTarget, ConditionActionConfig cfg, ActionInfo info)
        {
            return checkTarget.battle.m_buffMgr.GetBuffAddCnt(cfg.buff) == cfg.buffcnt;
        }
        static bool CheckTargetDistance(IObject checkTarget, ConditionActionConfig cfg, ActionInfo info)
        {
            if (info.target == null)
                return false;
            float dis = BattleHelp.GetDistance(info.source, info.target);
            return cfg.targetDistance[0] <= dis && cfg.targetDistance[1] >= dis;
        }
        #endregion

    }
}

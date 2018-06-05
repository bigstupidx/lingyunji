using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    class AttackAction : IAction<AttackActionConfig>
    {
        public override RunType GetRunType()
        {
            return RunType.ServerToClient;
        }

        public override bool IsNeedSysPos()
        {
            return cfg.state == StateType.BeatBack;
        }


        public override bool OnExcute(ActionInfo info)
        {
            if (info.target == null)
                return false;

            //计算伤害
            if (BattleHelp.IsRunBattleLogic())
            {
                //客户端控制的时候需要内部创建这个
                if (info.targetAction == null)
                    info.SetTargetAction(new Battle_TargetAction() { actionid = cfg.idhash, targetid = info.target.charSceneId });
                BattleAttriAttack.Attack(info, cfg);

                //死亡了，不能往下走
                if (!info.target.isAlive || !info.source.isAlive)
                    return false;
            }

            //命中action
            if (!AttackFlg.IsFlg(info.targetAction.attack.damageFlg, AttackFlg.Flg.Miss))
            {
                if(cfg.hitActionList.Count>0)
                    ActionManager.HandleActionList(info.skill, info.source, info.target,info.actionRecord, cfg.hitActionList, info.firePos, info.fireAngle);

                if(cfg.hitActionListOnce.Count>0 &&  info.actionRecord.IsCanAction(cfg.idhash))
                    ActionManager.HandleActionList(info.skill, info.source, info.target, info.actionRecord, cfg.hitActionListOnce, info.firePos, info.fireAngle);
            }


            //格挡不进入状态
            if (!AttackFlg.IsFlg(info.targetAction.attack.damageFlg, AttackFlg.Flg.GeDange))
            {
                bool immuneState = AttackFlg.IsFlg(info.targetAction.attack.damageFlg, AttackFlg.Flg.ImmuneState);

                if (cfg.state == StateType.Empty)
                {

                }
                //击退的参数是 时间,距离
                else
                {
                    float time;
                    object para;
                    if (cfg.state == StateType.BeatBack)
                    {
                        //霸体移动距离修正
                        float beatBackLen = cfg.statePara[1];
                        if(immuneState)
                            beatBackLen *= kvBattle.batiBeatBackMoveMul;

                        Vector3 moveLen = BattleHelp.Normalize(info.source.position, info.target.position) * beatBackLen;
                        Vector3 toPos = info.target.position + moveLen;//目标点
                        time = cfg.statePara[0] + AniConst.BeatBackToIdleTime;
                        para = toPos;
#if !COM_SERVER
                        //前端击退处理
                        ActionClient.HandleBeatBack(info.targetAction, info.target, cfg.statePara[0], toPos);
#else
                        //服务器直接修改坐标
                        info.target.SetPosition(toPos);
#endif
                    }
                    else
                    {
                        para = null;
                        time = cfg.statePara[0];
                    }
                    //免疫控制
                    if (!immuneState)
                    {
                         info.target.battle.m_stateMgr.ChangeHitState(cfg.state, para, cfg.statePara[0]);
                    }
                }
            }
#if !COM_SERVER
            //前端表现
            ActionClient.AttackAction(info.source, info.target, cfg, info.targetAction);
#endif
            return true;
        }
    }
}

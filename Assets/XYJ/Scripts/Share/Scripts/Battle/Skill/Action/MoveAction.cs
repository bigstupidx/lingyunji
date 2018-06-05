using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class MoveAction : IAction<MoveActionConfig>
    {
        public override RunType GetRunType()
        {
            return RunType.Both;
        }

        public override bool IsNeedSysPos() { return true; }


        public static bool GetPos(PosType posType, ActionInfo info, Vector3 posOff,ref Vector3 pos)
        {
            switch (posType)
            {
                case PosType.SelfPos:
                    pos = info.firePos;
                    break;
                case PosType.ActionTargetPos:
                    if (info.target == null)
                        return false;
                    pos = info.target.position;
                    break;
                case PosType.SkillTargetPos:
                    if (info.skill == null)
                        return false;
                    pos = info.skill.m_skillPoint;
                    break;
                case PosType.WorldPos:
                    break;
            }

            posOff = BattleHelp.RotateAngle(posOff, info.source.rotateAngle);
            pos += posOff;
            return true;
        }


        public override bool OnExcute(ActionInfo info)
        {
            Vector3 toPos = Vector3.zero;
            Vector3 posOff = Vector3.zero;
 
            if (cfg.posOff.Length >= 1)
            {
                posOff.z = cfg.posOff[0];
                if (cfg.posOff.Length == 2)
                    posOff.x = cfg.posOff[1];
                else
                    posOff.x = 0;
                posOff.y = 0;

                //自身正朝向移动时，可以配置距离目标不要太近
                if (cfg.posType == PosType.SelfPos && cfg.posColliderRadiu > 0 && info.skill.m_target != null && posOff.z > 0)
                {
                    float dis = BattleHelp.GetAttackDistance(info.source.position, info.skill.m_target);
                    //向前移动后距离目标太近
                    if (dis - cfg.posColliderRadiu < posOff.z)
                        posOff.z = dis - cfg.posColliderRadiu;
                }
            }

            if (!GetPos(cfg.posType, info, posOff, ref toPos))
                return false;

            //瞬移
            if(cfg.teleport)
            {
                info.source.SetPosition(toPos);
                if (cfg.lookAtPos)
                    BattleHelp.SetLookAt(info.source, toPos);
            }
            //移动，冲锋
            else
            {
                if (info.skill != null)
                {
                    float timeLenght;

                    //冲锋,修改动画时间
                    if (cfg.moveSpeed > 0)
                    {
                        timeLenght = BattleHelp.GetDistance(info.source.position, toPos) / cfg.moveSpeed;
                        if (info.skill != null)
                            info.skill.SetAniFinishTime(timeLenght + BattleHelp.timePass);
                    }
                    else
                    {
                        if (cfg.moveFrame == 0)
                            timeLenght = info.skill.GetAniTimeFinish() - BattleHelp.timePass;
                        else
                            timeLenght = (float)cfg.moveFrame / AniConst.AnimationFrameRate;
                    }

                    info.source.battle.m_moveMgr.PlayForceMovePos(toPos, timeLenght);
                }
            }
            return true;
        }
    }

}

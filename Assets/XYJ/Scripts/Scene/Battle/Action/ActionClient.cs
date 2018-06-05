using xys.battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using NetProto;
namespace xys.battle
{
    //客户端action
    public class ActionClient
    {
        //攻击action
        public static void AttackAction(IObject source, IObject target, AttackActionConfig cfg, Battle_TargetAction targetAction)
        {
            //飘字
            FlyTextManager.Play(target.battle.actor.m_titile, targetAction.attack, BattleHelp.IsMe(target), BattleHelp.IsMe(source));

            //格挡动作
            if (AttackFlg.IsFlg(targetAction.attack.damageFlg, AttackFlg.Flg.GeDange))
            {
                //还在持续格挡
                if (target != null && target.isAlive && target.battle.m_skillMgr.m_blockActionCfg != null)
                {
                    string[] anis = target.battle.m_skillMgr.m_blockActionCfg.animations;
                    if (anis.Length > 0)
                    {
                        int aniid = Random.Range(0, anis.Length - 1);
                        target.battle.m_aniMgr.PlayBlockAni(anis[aniid]);
                    }

                    //格挡成功切换技能
                    target.battle.m_skillMgr.SwitchSkillByCondition(SkillConditionConfig.Condition.Block);
                }
            }
            else
            {
                //受击特效
                EffectManager.PlayHurtFx(source, target, cfg);
            }

            //暴击
            if (AttackFlg.IsFlg(targetAction.attack.damageFlg, AttackFlg.Flg.Baoji))
            {
                //如果技能已经结束了，则不算
                if (source.battle.m_skillMgr.IsPlaying())
                    source.battle.m_skillMgr.SwitchSkillByCondition(SkillConditionConfig.Condition.Baoji, source.battle.m_skillMgr.GetCurSkill());
            }
        }

        //击退坐标修正
        public static void HandleBeatBack(Battle_TargetAction clientData, IObject target,float time, Vector3 toPos)
        {
            //如果客户端击退后坐标和服务器差别较大，使用服务器的
            if (clientData != null)
            {
                Vector3 serverPos = clientData.topos.ToVector3();
                //if(BattleHelp.GetDistance(toPos,serverPos ) > 0.5f)
                toPos = serverPos;
            }


            //击退逻辑
            float moveTime = time;
            float moveLen = BattleHelp.GetDistance(toPos, target.position);
            float moveSpeed = moveLen / moveTime;
            target.battle.m_moveMgr.SetForceMove(target.position, toPos, moveLen, moveSpeed);
        }

        //无敌斩
        public static void OmnislashAction(OmnislashActionLogic logic, IObject source, IObject target, Vector3 topos)
        {
            if (logic.cfg.moveType == OmnislashActionConfig.MoveType.Move)
                source.battle.m_moveMgr.PlayForceMovePos(topos, logic.cfg.actionInterval);
            else
                source.root.position = topos;
            //给目标加特效
            EffectManager.PlayEffect(target, logic.cfg.effect, (go, para) =>
            {
                BattleHelp.CheckEffectDestroy(go);
                if (source != null && target != null)
                {
                    float angle = logic.cfg.angle;
                    //0度需要随机
                    if (angle == 0)
                        angle = Random.Range(0, 360.0f);
                    else
                        angle = (target.rotateAngle + angle + 360) % 360;
                    go.transform.position = go.transform.position + BattleHelp.Angle2Vector(angle) * logic.cfg.distance;

                    //特效角度需要朝向目标
                    if (go.transform.position != source.position)
                        go.transform.forward = BattleHelp.GetForward(source.position, go.transform.position);
                }
            });
        }

        public static void OmnislashActionBegin(OmnislashActionLogic logic, IObject source)
        {
            //隐藏模型
            source.battle.actor.SetHide(true);
            ////主角才处理镜头
            //if (m_source.m_isme)
            //{
            //    CameraManage.Instance.ForbidRotate(true);
            //}
        }
        public static void OmnislashActionStop(OmnislashActionLogic logic, IObject source)
        {
            //隐藏模型
            source.battle.actor.SetHide(false);
            ////主角才处理镜头
            //if (m_source.m_isme)
            //{
            //    CameraManage.Instance.ForbidRotate(true);
            //}
        }
    }
}
using UnityEngine;
using System.Collections;
using Config;
using System.Collections.Generic;


namespace xys.battle
{
    //技能释放流程
    public partial class SkillManagerLocal
    {
        //向目标使用技能,目标在射程外智能选取距离内，则会先移动再释放
        //如果目标在智能选取距离外，则释放失败
        bool MoveToTargetAndPlaySkill(SkillConfig cfg, IObject target)
        {
            float distance = BattleHelp.GetAttackDistance(m_obj.position, target);

            //目标在射程内，直接释放
            if (distance < cfg.range)
            {
                TrueRequestPlaySKill(cfg, target);
                return true;
            }
            //目标在射程外
            else
            {
                //距离在智能释放距离内，则需要移动到射程内再使用技能
                if (distance < cfg.autoTargetRange)
                {
                    App.my.input.ResetJoyInputCnt(InputManager.InputFlg.MoveToSkillFlg);
                    m_obj.battle.m_stateMgr.ChangeState(StateType.Move);
                    m_obj.battle.m_moveMgr.SetMoveToTarget(target,cfg.range-target.cfgInfo.behitRaidus, OnFinishMove,new object[]{cfg,target});
                    return true;
                }
                //可以空放,这时候目标设置为空
                else if (!cfg.isNeedTarget)
                {
                    TrueRequestPlaySKill(cfg, null);
                    return true;
                }
            }
            return false;
        }

        void OnFinishMove( object para)
        {
            object[] plist = (object[])para;
            IObject target = (IObject)plist[1];
            if (target == null || !target.isAlive)
                return;
            TrueRequestPlaySKill((SkillConfig)plist[0], target);
        }

        //真正开始请求施放技能
        bool TrueRequestPlaySKill(SkillConfig cfg, IObject target)
        {
            if ( RequestPlaySkill(cfg.id,target))
            {
                m_uiclickSlotid = cfg.slotid;
                //空目标时不需要移除当前的目标，不然ui又会再选一次目标
                if(target != null)
                    m_obj.battle.m_targetMgr.SetTarget(target);
                return true;
            }
            return false;
        }

        //根据距离角度获得合适目标
        IObject GetClosestAngleTarget(IObject source, float disMin, float disMax, float angleLimit, SkillConfig cfg)
        {
            IObject target = null;
            Vector3 sourcePos = source.position;
            float distance = float.MaxValue;
            float angle;
            foreach (KeyValuePair<int, IObject> p in BattleHelp.GetAOIObj(source))
            {
                IObject br = p.Value;
                if (!br.isAlive || !BattleHelp.IsEnemy(source, br) || br.battle.m_buffMgr.IsFlag(BuffManager.Flag.NotTarget))
                    continue;

                float dis = BattleHelp.GetAttackDistance(sourcePos, br);
                //距离合适
                if (dis < distance && dis < disMax && dis >= disMin)
                {
                    //目标不符合技能的条件
                    if (cfg.conditionCfg != null && cfg.conditionCfg.effectTarget == EffectTarget.Target && !IsConditionStateLegal(br, cfg))
                        continue;

                    //角度合适
                    angle = BattleHelp.GetForwardAngle(source.rotateAngle, br.position - sourcePos) * 2;
                    if (angle <= angleLimit)
                    {
                        target = br;
                        distance = dis;
                    }
                }
            }
            return target;
        }

        //释放技能流程
        bool HandelUIClickSkill(SkillInfo info)
        {
            SkillConfig cfg = info.cfg;            
            IObject target = null;

            if (IsCanSkill(cfg.id) != PlayResult.OK)
                return false;

            //轻功特殊处理
            if(info.cfg.type == SkillConfig.Type.QingGongSkill)
            {
                if (m_obj.battle.m_stateMgr.m_curStType == StateType.Jump)
                    ((JumpManagerLocal)m_obj.battle.m_jumpMgr).JumpCache(info.cfg);
                else
                    App.my.battleProtocol.Jump_Request(info.cfg, m_obj);
                return true;
            }

            //需要指定目标
            if (cfg.isNeedTarget)
            {
                target = m_obj.battle.GetTarget();
                //当前有目标
                if (target != null && target.isAlive)
                {
                    //目标不合法，不释放
                    if (!BattleHelp.IsEnemy(m_obj, target))
                        return false;
                }
            }
            //不需要指定目标,如果配置了自动朝向，则设置为目标
            else if(cfg.lookAtTarget==0)
            {
                target = m_obj.battle.GetTarget();
                if (target==null || !target.isAlive || !BattleHelp.IsEnemy(m_obj, target))
                    target = null;
            }

            //需要智能选取目标
            if (target == null && cfg.isAutoTarget)
                target = GetClosestAngleTarget(m_obj, 0, cfg.autoTargetRange, cfg.autoTargetAngle, cfg);

            //有目标，走向目标并施放技能
            if (target != null)
                return MoveToTargetAndPlaySkill(cfg, target);
            //可以空放
            else if (!cfg.isNeedTarget)
                return TrueRequestPlaySKill(cfg, target);

            return false;
        }
    }
}
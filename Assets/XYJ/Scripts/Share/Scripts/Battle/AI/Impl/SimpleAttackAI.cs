using Config;
using System;
using System.Collections.Generic;
using UnityEngine;
using xys.battle;

namespace xys.battle
{

    class SimpleAttackAI : ISimpleAI
    {
        SkillConfig m_skillcfg;
        int m_skillIndex;
        IObject m_target;
        //下次思考时间
        float m_nextThinkTime;

        public override void OnExit()
        {
            m_target = null;
        }
        //随机技能
        SkillConfig SkillRandom()
        {
            if (m_obj.cfgInfo.defaultSkills.Length > 0)
            {
                m_skillcfg = null;
                int id;
                //ai按顺序施放
                if (m_obj.cfgInfo.simpleAI == 1)
                    id = m_skillIndex++;
                else
                    id = BattleHelp.Rand(0, m_obj.cfgInfo.defaultSkills.Length);

                for (int i = 0; i < m_obj.cfgInfo.defaultSkills.Length; i++)
                {
                    int skillid = m_obj.cfgInfo.defaultSkills[(i + id) % m_obj.cfgInfo.defaultSkills.Length];
                    //自动加上技能
                    xys.battle.SkillManager.SkillInfo info = m_obj.battle.m_skillMgr.GetSkill(skillid, true);
                    if (m_obj.battle.m_skillMgr.IsCanSkill(skillid) == SkillManager.PlayResult.OK)
                        return info.cfg;
                }
            }
            return null;
        }

        //施放技能
        bool SkillCheckPlay( IObject target)
        {
            if (m_skillcfg != null && target != null && target.isAlive)
            {
                m_obj.battle.m_skillMgr.RequestPlaySkill(m_skillcfg.id, target);
                return true;
            }
            return false;
        }

        //设置下次思考时间
        void SetNextThinkTime()
        {
            m_nextThinkTime = BattleHelp.timePass + m_obj.cfgInfo.GetThinkInterval();
        }

        //由AI来决定下个状态
        public override void OnUpdate()
        {
            switch (m_obj.battle.m_stateMgr.m_curStType)
            {
                case StateType.Skill:
                    m_nextThinkTime = 0;
                    break;
                case StateType.Idle:
                    if (m_nextThinkTime == 0)
                        SetNextThinkTime();
                    //待机发现目标,向目标移动
                    if (BattleHelp.timePass> m_nextThinkTime)
                    {
                        SetNextThinkTime();
                        IObject target = m_target = m_obj.battle.m_targetMgr.target;
                        if (target == null)
                            return;

                        m_skillcfg = SkillRandom();

                        //技能都在cd不会移动
                        if (m_skillcfg == null)
                            return;

                        //移动结束距离
                        float finishDis = 0;
                        finishDis += target.cfgInfo.behitRaidus;
                        if (m_skillcfg != null)
                            finishDis += m_skillcfg.range;

                        //不要贴在一起
                        if (finishDis < 1.0f)
                            finishDis = 1.0f;
                        //距离近直接放技能
                        if (BattleHelp.GetDistance(m_obj.position, target.position) < finishDis)
                            SkillCheckPlay(target);
                        else
                        {
                            if (finishDis < 0.1f)
                                finishDis = 0.1f;
                            m_obj.battle.State_MoveToTarget(target, finishDis, OnFinishMove);
                        }
                    }
                    return;
                default:
                    break;
            }
        }

        void OnFinishMove( object para )
        {
            //同一个目标才释放技能
            if (m_obj.battle.m_targetMgr.target == m_target)
            {
                //释放了技能
                if (SkillCheckPlay(m_target))
                    return;
            }

            m_obj.battle.m_moveMgr.StopMove();
            //返回待机
            m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
        }
    }
}

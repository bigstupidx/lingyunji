using UnityEngine;
using System.Collections;
using Config;
using System.Collections.Generic;


namespace xys.battle
{
    public partial class SkillManagerLocal
    {
        void Switch_Update()
        {
            //技能持续时间到了，重新选择技能
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                if (m_skillSlots[i].m_curSkill == null || m_skillSlots[i].m_curSkill.cfg.changeSkillType == SkillConfig.SwitchType.Null)
                    continue;

                //时间到了，技能正在cd，都要判断切换
                if (BattleHelp.timePass > m_skillSlots[i].m_nextChangeTime || !m_skillSlots[i].m_curSkill.IsCanPlayByCD())
                {
                    //需要设置为null，避免当前是后续技能，会导致不会被切换
                    m_skillSlots[i].m_curSkill = null;

                    SkillInfo toSkill = Switch_GetBestSkill(i);
                    Slot_SetSkill(i, toSkill);
                }
            }             
        }

        //获得最合适的技能,后续技能不会被替换成默认技能
        SkillInfo Switch_GetBestSkill(int slot)
        {
            SkillInfo toSkill = m_skillSlots[slot].m_curSkill;
            //当前技能不合法了
            if (!Switch_IsSkillLegal(toSkill))
                toSkill = null;

            for (int i = 0; i < m_skillSlots[slot].m_skills.Count; i++)
            {
                SkillInfo p = m_skillSlots[slot].m_skills[i];

                //后续技能不能被选择
                if (p.cfg.switchAttribute== SkillConfig.SkillSwitchAttribute.NextSkill)
                    continue;

                //该技能满足施放
                if (!Switch_IsSkillLegal(p))
                    continue;

                //需要瞬发条件的技能也不行
                if (p.cfg.conditionCfg!=null && p.cfg.conditionCfg.condtion != SkillConditionConfig.Condition.Null)
                    continue;

                //获取优先级更高的技能
                toSkill = Switch_GetBetter(toSkill, p);
            }
            return toSkill;
        }

        //技能当前是否满足释放需求,瞬发条件不在这里判断
        bool Switch_IsSkillLegal(SkillInfo skill)
        {
            if (skill == null)
                return false;

            //姿态不对
            if (!skill.cfg.IsCastingPostureLegal(m_obj.postureValue))
                return false;

            //战斗状态不对
            if (skill.cfg.castingBattleState == SkillConfig.CastingBattleState.Battle && !m_obj.battle.m_attrMgr.battleState)
                return false;
            else if (skill.cfg.castingBattleState == SkillConfig.CastingBattleState.NoBattle && m_obj.battle.m_attrMgr.battleState)
                return false;

            //施放条件
            if (skill.cfg.conditionCfg != null)
            {
                //技能条件是否合法
                if (!IsConditionStateLegal(m_obj.battle.GetTarget(), skill.cfg))
                    return false;
            }

            return true;
        }

        //技能条件状态是否合法,常规技能都合法,这里合法不表示技能能施放，只是表示技能能被ui选中
        bool IsConditionStateLegal(IObject target,SkillConfig cfg )
        {
            if (cfg.switchAttribute == SkillConfig.SkillSwitchAttribute.Default)
                return true;

            IObject conditionTarget = cfg.conditionCfg.effectTarget == EffectTarget.Self ? m_obj :target;
            //状态不对
            if (cfg.conditionCfg.isNeedCheckState)
            {
                if (!cfg.conditionCfg.IsState(conditionTarget))
                    return false;
            }
            return true;
        }

        //有施放条件的优先级高,需要细化
        SkillInfo Switch_GetBetter(SkillInfo oldSkill, SkillInfo newSkill)
        {
            if (oldSkill == null)
                return newSkill;
            if (newSkill == null)
                return oldSkill;

            //使用优先级高的
            if (oldSkill.cfg.switchPriority > newSkill.cfg.switchPriority)
                return oldSkill;
            else if (oldSkill.cfg.switchPriority < newSkill.cfg.switchPriority)
                return newSkill;
            //优先级相等，还是使用旧的
            else
                return oldSkill;

            ////新技能有条件
            //if (newSkill.cfg.conditionCfg != null)
            //{
            //    //旧技能也有条件则继续用旧的,否者用新
            //    if (oldSkill.cfg.conditionCfg != null)
            //        return oldSkill;
            //    else
            //        return newSkill;
            //}
            ////新技能没有条件，则还是用回旧的
            //else
            //    return oldSkill;
        }

        //切换技能下个阶段
        void Switch_SetNextSkill(SkillConfig cfg)
        {
            int slot = cfg.slotid;
            SkillInfo nextSkill;

            //没有后续技能
            if (cfg.changeSkill == 0)
            {
                //该类型使用后如果没有后续技能则立马结束
                SkillInfo curSlotSkill = m_skillSlots[slot].m_curSkill;
                if (curSlotSkill != null && curSlotSkill.cfg.changeSkillType == SkillConfig.SwitchType.SwitchByOperation)
                    m_skillSlots[slot].m_curSkill = null;

                nextSkill = Switch_GetBestSkill(slot);
            }
            else
            {
                nextSkill = GetSkill(cfg.changeSkill);
                if (nextSkill == null)
                    Debug.LogError("找不到后续技能 " + cfg.changeSkill);
            }

            Slot_SetSkill(slot, nextSkill,true);
        }

        //当前技能是ui点击了那个slot
        int m_uiclickSlotid;
        //SwitchByOperation类型技能结束了，立马切换回合适技能
        public void CheckToDefaultSkill( bool playSkill )
        {
            for (int slotid = 0; slotid < (int)Slot.MaxCnt; slotid++)
            {
                //后续技能2切换后续3时，不要重置
                if (playSkill && m_uiclickSlotid == slotid)
                    continue;

                SkillInfo curSlotSkill = m_skillSlots[slotid].m_curSkill;
                if (curSlotSkill != null && curSlotSkill.cfg.changeSkillType == SkillConfig.SwitchType.SwitchByOperation)
                {
                    m_skillSlots[slotid].m_curSkill = null;
                    SkillInfo toSkill = Switch_GetBestSkill(slotid);
                    Slot_SetSkill(slotid, toSkill);
                }
            }
        }

        //重新选择所有技能
        public void Switch_CheckAllSkill()
        {
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                Slot_SetSkill(i, Switch_GetBestSkill(i));
            }
        }

        //根据瞬发条件切换技能
        void Switch_CheckSkillByCondition( SkillConditionConfig.Condition condition )
        {
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                 foreach (var p in m_skillSlots[i].m_skills)
                 {
                     if( p.cfg.conditionCfg==null )
                         continue;

                     //技能是否满足瞬发条件
                     if (condition == SkillConditionConfig.Condition.Block)
                     {
                         if (!p.cfg.conditionCfg.block)
                             continue;
                     }
                     else
                         continue;

                     //技能合法
                     if (Switch_IsSkillLegal(p))
                         Slot_SetSkill(i, p);
                 }
            }
        }
    }

}

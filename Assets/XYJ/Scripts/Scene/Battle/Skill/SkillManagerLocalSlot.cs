using UnityEngine;
using System.Collections;
using Config;
using System.Collections.Generic;


namespace xys.battle
{
    public partial class SkillManagerLocal
    {
        public enum UISkillState
        {
            Normal,
            Forbit,         //技能禁用
            ConditionFail,  //条件不够，例如真气不足
            Lock,           //技能未解锁
        }

        class SlotInfo
        {
            //当前技能
            public SkillInfo        m_curSkill;            
            //下次切换技能时间
            public float            m_nextChangeTime;    
            //当前槽位拥有的技能
            public List<SkillInfo>  m_skills = new List<SkillInfo>();
        }
        //当前槽位的技能
        SlotInfo[] m_skillSlots;


        //获得当前槽技能
        SkillInfo Slot_GetSkill(int slot)
        {
            if (slot < m_skillSlots.Length)
                return m_skillSlots[slot].m_curSkill;
            else
                return null;
        }


        void Slot_Clear()
        {
            m_skillSlots = null;
        }


        //初始化技能槽,技能添加完后调用
        void Slot_Init()
        {
            m_skillSlots = new SlotInfo[(int)Slot.MaxCnt];
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                m_skillSlots[i] = new SlotInfo();
            }

            foreach (var p in m_allSkill.Values)
            {
                m_skillSlots[p.cfg.slotid].m_skills.Add(p);
            }

            //初始化技能
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                Slot_SetSkill(i, Switch_GetBestSkill(i),true);
            }
        }


        //初始化面板
        public void Slot_InitUI()
        {
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                m_event.FireEvent<object[]>(EventID.MainPanel_SetSkill, new object[] { i, m_skillSlots[i].m_curSkill });
                Slot_SetSkillState(i);
            }
        }

        //设置技能
        void Slot_SetSkill(int slot, SkillInfo info,bool refreshCD=false)
        {
            if (info == m_skillSlots[slot].m_curSkill)
            {
                //技能点击施放，刷界面需要重新刷新cd
                if (refreshCD && info != null)
                    m_event.FireEvent<object[]>(EventID.MainPanel_SetSkill, new object[] { slot, info });
                return;
            }

            m_skillSlots[slot].m_curSkill = info;
            if (info!=null)
                m_skillSlots[slot].m_nextChangeTime = BattleHelp.timePass + info.cfg.changeSkillTime;

            m_event.FireEvent<object[]>(EventID.MainPanel_SetSkill, new object[] { slot, info });
            Slot_SetSkillState(slot);
        }

        void Slot_SetSkillState(int slot)
        {
            UISkillState state;
            //不能施放技能效果，当前正在放技能不算
            if (m_obj.battle.m_buffMgr.IsFlag(BuffManager.Flag.NoSkill) && !m_obj.battle.m_skillMgr.IsPlaying())
                state = UISkillState.Forbit;
            else
                state = UISkillState.Normal;
                
            m_event.FireEvent<object[]>(EventID.MainPanel_SetSkillState, new object[] { slot, state });
        }

        void Slot_SetAllSkillState()
        {
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
                Slot_SetSkillState(i);
        }
    }

}


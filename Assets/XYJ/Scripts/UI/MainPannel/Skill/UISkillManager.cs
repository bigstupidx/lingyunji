#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Config;
using xys.battle;
using xys.UI;

namespace xys.hot.UI
{
    public class UISkillManager
    {
        UISkill[] m_skills = new UISkill[(int)Slot.MaxCnt];
        Animator m_ani;

        public UISkillManager(Transform root)
        {
            //技能孔位
            for (int i = 0; i < (int)Slot.MaxCnt; i++)
            {
                string name;
                if (i == 6)
                    name = "Offset/Skill/6";
                else
                    name = string.Format("Offset/Skill/Offset/{0}", i);
                Transform tran = root.Find(name);
                if (tran == null)
                    Debug.LogError("找不到技能孔位 " + name);
                else
                    m_skills[i] = new UISkill(tran, i,this);
            }

            //切换目标
            UIBattleHelp.AddBattleEvent(root, "Offset/Skill/Offset/9", UIBattleFunType.SwitchTarget);

            //坐骑
            Transform trans = root.Find("Offset/Skill/ZuoQiYaoGan");
            if (trans)
                trans.gameObject.SetActive(false);

            m_ani = root.Find("Offset/Skill").GetComponent<Animator>();
        }

        public bool GetBattleState()
        {
            if (App.my.localPlayer.battle != null)
                return App.my.localPlayer.battle.m_attrMgr.battleState;
            else
                return true;
        }

        public void OnShow(xys.hot.Event.HotObjectEventAgent Event)
        {
            Event.Subscribe<object[]>(EventID.MainPanel_SetSkill, OnSetSkill);
            Event.Subscribe<object[]>(EventID.MainPanel_SetSkillState, OnSetSkillState);
            Event.Subscribe(NetProto.AttType.AT_State, OnChangeBattleState);

            //根据战斗状态决定是否收起动画
            if (GetBattleState())
                AnimationHelp.PlayNormalizedTime(m_ani, "ui_skill", 1);
            else
                AnimationHelp.PlayNormalizedTime(m_ani, "ui_skill_1", 1);
        }

        void OnSetSkill(object[] para)
        {
            int slot = (int)para[0];
            if (slot >= (int)Slot.MaxCnt)
                return;

            SkillManager.SkillInfo info = (SkillManager.SkillInfo)para[1];

            m_skills[slot].SetSlot(info);
        }

        void OnSetSkillState(object[] para)
        {
            int slot = (int)para[0];
            if (slot >= (int)Slot.MaxCnt)
                return;
            xys.battle.SkillManagerLocal.UISkillState state = (xys.battle.SkillManagerLocal.UISkillState)para[1];
            m_skills[slot].SetSlotState(state);
        }

        void OnChangeBattleState(AttributeChange args)
        {
            if (args.currentValue.intValue == (int)ObjectState.Battle)
                AnimationHelp.PlayAnimation(m_ani, "ui_skill");
            else
                AnimationHelp.PlayAnimation(m_ani, "ui_skill_1");
        }
    }
}
#endif
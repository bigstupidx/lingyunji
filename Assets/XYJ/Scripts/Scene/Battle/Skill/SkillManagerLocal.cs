using UnityEngine;
using System.Collections;
using Config;
using NetProto;


namespace xys.battle
{
    public partial class SkillManagerLocal : SkillManagerBase
    {
        ObjectEventAgent m_event;

        //最后一次按键技能
        public SkillInfo m_lastPressedSkill { get; private set; }
        //使用技能的时间
        public float m_lastPressedSkillTime { get; private set; }

        public override void OnStart()
        {
            m_event = new ObjectEventAgent(m_obj.eventSet);
            m_event.Subscribe<int>(EventID.MainPanel_FireSkill, OnClickSkillSlot);

            m_event.Subscribe(NetProto.AttType.AT_State, OnChangeBattleState);

            m_event.Subscribe(NetProto.AttType.AT_Posture, OnChangePosture);

            m_event.Subscribe<StateType>(ObjEventID.ChangeState, OnChangeState);
            m_event.Subscribe<BuffManager.Flag>(ObjEventID.ChangeBuffFlg, (flg) => { if(flg == BuffManager.Flag.NoSkill) Slot_SetAllSkillState(); });

            //临时加上技能
            foreach (var skillid in m_obj.cfgInfo.defaultSkills)
                AddSkill(skillid);
        }

        public override void OnEnterScene()
        {
            base.OnEnterScene();
            Slot_Init();
        }

        public override void OnExitScene()
        {
            base.OnExitScene();
            Slot_Clear();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            m_event.Release();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            Switch_Update();
        }

        public override void Stop()
        {
            base.Stop();
        }

        //真正施放了技能，进入cd
        protected override void OnSetTruePlaySkill(SkillConfig skill)
        {
            Switch_SetNextSkill(skill);
        }

        //非战斗状态点击进入战斗状态需要一定延时才响应按键
        float m_canClickTime;
        void OnClickSkillSlot(int slot)
        {
            //Debug.Log("施放技能孔位 " + slot);

            SkillInfo info = Slot_GetSkill(slot);
            if (info != null)
            {
                if (slot == 0 && !m_obj.battle.m_stateMgr.isFastRun)
                {
                    //非战斗状态点击时，先进入战斗状态
                    if (!m_obj.battle.m_attrMgr.battleState && BattleHelp.timePass > m_canClickTime)
                    {
                        //m_obj.battle.m_attriLogic.SetBattleState(true);
                        App.my.socket.SendGame(Protoid.A2C_Battle_RequestBattleState);
                        m_canClickTime = BattleHelp.timePass + 0.5f;
                        return;
                    }
                    if (BattleHelp.timePass < m_canClickTime)
                        return;
                }

                //避免按着移动时打断施法动作,需要再次按才能打断
                if (HandelUIClickSkill(info))
                    App.my.input.ResetJoyInputCnt(InputManager.InputFlg.SkillFlg);
            }
        }
        void OnChangeBattleState(AttributeChange args)
        {
            if (args.currentValue.intValue == (int)ObjectState.Battle && args.currentValue.intValue == (int)ObjectState.Idle)
                Switch_CheckAllSkill();
        }
        void OnChangePosture(AttributeChange args)
        {
            Switch_CheckAllSkill();
        }
        void OnChangeState(StateType state)
        {
            Switch_CheckAllSkill();
        }

        public void OnTargetChangeState(StateType state)
        {
            Switch_CheckAllSkill();
        }


        public override void SwitchSkillByCondition(SkillConditionConfig.Condition condition,object para=null)
        {
            Switch_CheckSkillByCondition(condition);
        }

    }
}


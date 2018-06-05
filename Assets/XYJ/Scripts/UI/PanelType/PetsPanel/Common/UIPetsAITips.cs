#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using xys.UI.State;
using xys.UI;
using xys.UI.Dialog;
using NetProto.Hot;
using NetProto;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsAITips
    {
        enum AiType
        {
            PET_AI_ATTACK = 1,
            PET_AI_SKILL,
        }
        protected enum State
        {
            Show,
            Hide,
            TweenAlpha,
        }
        protected State m_State = State.Hide;
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        StateRoot m_RootIcon;
        [SerializeField]
        Animator m_Animator;
        [SerializeField]
        Button m_ShowBtn;
        [SerializeField]
        Button m_CloseBtn;
        [SerializeField]
        Button m_AIAttackButton;
        [SerializeField]
        Button m_AISkillButton;


        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }

        void Awake()
        {
            m_ShowBtn.onClick.AddListener(this.ShowTips);
            m_CloseBtn.onClick.AddListener(this.CloseTips);
            m_AIAttackButton.onClick.AddListener(this.OnAIAttackEvent);
            m_AISkillButton.onClick.AddListener(this.OnAISkillEvent);
        }
        void OnDisable()
        {
            m_State = State.Hide;
        }

        public void ReFresh()
        {
            if (m_Panel.selected == -1)
                return;
            PetsMgr petsMgr = m_Panel.petsMgr;
            int ai_type = petsMgr.m_PetsTable.attribute[m_Panel.selected].ai_type;
            if (ai_type == (int)AiType.PET_AI_ATTACK)
                SetButtonState(true);
            else if (ai_type == (int)AiType.PET_AI_SKILL)
                SetButtonState(false);
        }
        void ShowTips()
        {
            PetsMgr petsMgr = m_Panel.petsMgr;
            int ai_type = petsMgr.m_PetsTable.attribute[m_Panel.selected].ai_type;
            if (ai_type == (int)AiType.PET_AI_ATTACK)
                SetButtonState(true);
            else if (ai_type == (int)AiType.PET_AI_SKILL)
                SetButtonState(false);

            if (m_State == State.Show)
            {
                return;
            }
            this.m_Transform.gameObject.SetActive(true);
            m_State = State.TweenAlpha;
            this.PlayAnimation(true);
        }
        public void CloseTips()
        {
            m_State = State.TweenAlpha;
            this.PlayAnimation(false);
        }

        void OnAIAttackEvent()
        {
            PetsAIRequest request = new PetsAIRequest();
            request.ai_type = (int)AiType.PET_AI_ATTACK;
            request.index = m_Panel.selected;
            App.my.eventSet.FireEvent<PetsAIRequest>(EventID.Pets_SetAi, request);
        }

        void OnAISkillEvent()
        {
            PetsAIRequest request = new PetsAIRequest();
            request.ai_type = (int)AiType.PET_AI_SKILL;
            request.index = m_Panel.selected;
            App.my.eventSet.FireEvent<PetsAIRequest>(EventID.Pets_SetAi, request);
        }

        void SetButtonState(bool attack)
        {
            if (attack)
            {
                m_RootIcon.CurrentState = 0;
                m_AIAttackButton.GetComponent<StateRoot>().CurrentState = 1;
                m_AISkillButton.GetComponent<StateRoot>().CurrentState = 0;
            }
            else
            {
                m_RootIcon.CurrentState = 1;
                m_AIAttackButton.GetComponent<StateRoot>().CurrentState = 0;
                m_AISkillButton.GetComponent<StateRoot>().CurrentState = 1;
            }
        }

        void PlayAnimation(bool isOpen)
        {
            if (isOpen)
                AnimationHelp.PlayAnimation(this.m_Transform.GetComponent<Animator>(), "open", "ui_TanKuang_Tips", this.OpenEvent);
            else
                AnimationHelp.PlayAnimation(this.m_Transform.GetComponent<Animator>(), "close", "ui_TanKuang_Tips_Close", this.CloseEvent);
        }

        void OpenEvent(object obj)
        {
            m_State = State.Show;
        }

        void CloseEvent(object obj)
        {
            m_State = State.Hide;
            this.m_Transform.gameObject.SetActive(false);
        }
    }
}

#endif
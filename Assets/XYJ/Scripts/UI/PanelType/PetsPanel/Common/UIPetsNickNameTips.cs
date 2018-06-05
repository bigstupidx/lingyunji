#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using xys.UI.State;
    using xys.UI;
    using xys.UI.Dialog;
    using NetProto.Hot;
    using NetProto;

    [AutoILMono]
    class UIPetsNickNameTips
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Button m_CloseBtn;
        [SerializeField]
        Button m_ClickBtn;
        [SerializeField]
        Button m_ShowBtn;

        const int MAXCHAR = 10;         //输入限制长度
        protected enum State
        {
            Show,
            Hide,
            TweenAlpha,
        }
        protected State m_State = State.Hide;

        [SerializeField]
        InputField m_NameInput;         //角色名字
        
        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }

        void Awake()
        {
            m_ShowBtn.onClick.AddListener(this.ShowTips);
            m_CloseBtn.onClick.AddListener(this.CloseTips);
            m_ClickBtn.onClick.AddListener(this.OnInputFinish);
            m_NameInput.onValueChanged.AddListener(InputName);
        }

        void OnDisable()
        {
            m_State = State.Hide;
        }

        public void CloseTips()
        {
            m_State = State.TweenAlpha;
            this.PlayAnimation(false);
        }

        public void ShowTips()
        {
            if (m_State == State.Show)
                return;

            PetsMgr petsMgr = m_Panel.petsMgr;
            m_NameInput.text = petsMgr.m_PetsTable.attribute[m_Panel.selected].nick_name;

            this.m_Transform.gameObject.SetActive(true);
            m_State = State.TweenAlpha;
            this.PlayAnimation(true);
        }

        public void OnInputFinish()
        {
            OnClickCreateChar();
        }
        //输入名字
        void InputName(string text)
        {
            //字符数限制
            if (Helper.TextLimit(ref text, MAXCHAR))
                SystemHintMgr.ShowHint("名字不能超过5个中文字符");

            m_NameInput.text = text;
        }
        void OnClickCreateChar()
        {
            if (string.IsNullOrEmpty(m_NameInput.text))
            {
                SystemHintMgr.ShowHint("角色名不能为空");
                return;
            }

            //需要经过字库筛选
            if (TextRegexParser.ContainsSensitiveWord(m_NameInput.text))
            {
                SystemHintMgr.ShowHint("含有屏蔽词");
                return;
            }

            //不可单独使用数字或特殊符号作为角色名，单纯为数字与符号组合也不行
            bool check = true;
            char[] charList = m_NameInput.text.ToCharArray();
            for (int i = 0; i < charList.Length; ++i)
            {
                //必须包含中文或者字母
                if (Helper.CheckStringChineseReg(charList[i]) || Helper.CheckStringCharacterReg(charList[i]))
                {
                    check = true;
                }
            }
            if (!check)
            {
                SystemHintMgr.ShowTipsHint(3204);
                return;
            }
            ////
            PetsNickNameRequest request = new PetsNickNameRequest();
            request.index = m_Panel.selected;
            request.newName = m_NameInput.text;
            App.my.eventSet.FireEvent<PetsNickNameRequest>(EventID.Pets_SetName, request);
            PlayAnimation(false);
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
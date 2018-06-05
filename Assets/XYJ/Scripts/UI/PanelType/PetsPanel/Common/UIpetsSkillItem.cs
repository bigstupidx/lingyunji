#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using xys.UI.State;
using xys.UI;
using xys.UI.Dialog;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIpetsSkillItem
    {
        protected int m_ID;
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        StateRoot m_Tag;
        [SerializeField]
        GameObject m_Lock;
        [SerializeField]
        GameObject m_Text;
        [SerializeField]
        Button m_ClickEvent;

        protected int m_SkillState;

        UIPetsSkillItem_Type m_SlotType = UIPetsSkillItem_Type.None;

        System.Action<int> m_CallBack = null;
        public System.Action<int> callBack { set { m_CallBack = value; } }
        void Awake()
        {
            m_SkillState = -1;
        }

        void OnEnable()
        {
            m_ClickEvent.onClick.AddListener(this.ShowTips);
        }

        void OnDisable()
        {
            m_ClickEvent.onClick.RemoveListener(this.ShowTips);
        }

        void ShowTips()
        {
            //             if (m_ID != 0)
            //                 Utils.EventDispatcher.Instance.TriggerEvent<int>(PetsSystem.Event.ShowSkillTips, m_ID);
            if (m_CallBack != null)
                m_CallBack(m_ID);
        }

        public void SetData(int id, UIPetsSkillItem_Type type, int SkillState, ref int index)
        {
            if (id == 0)
            {
                m_Tag.gameObject.SetActive(false);
                m_Tag.GetComponent<StateRoot>().CurrentState = (int)type;
                m_Icon.GetComponent<StateRoot>().CurrentState = 0;
                m_Lock.SetActive(false);
                return;
            }

            index++;
            m_ID = id;
            m_SlotType = type;

            Config.SkillConfig item = null;
            Config.PassiveSkills data = null;
            if (Config.SkillConfig.GetAll().ContainsKey(id))
                item = Config.SkillConfig.Get(id);
            if (Config.PassiveSkills.GetAll().ContainsKey(id))
                data = Config.PassiveSkills.Get(id);
            if (item == null && data == null)
            {
                Debuger.LogError("UIPetsSkillItem =====>ID不存在 ： " + id);
                m_Tag.gameObject.SetActive(false);
                m_Icon.GetComponent<StateRoot>().CurrentState = 0;
                return;
            }
            else
            {
                m_Tag.gameObject.SetActive(type == UIPetsSkillItem_Type.PassivasSlot ? false : true);
                m_Tag.CurrentState = (int)type;
                m_Icon.gameObject.SetActive(true);
                m_Icon.GetComponent<StateRoot>().CurrentState = 1;
                Helper.SetSprite(m_Icon, Config.SkillIconConfig.Get(id).icon);
                m_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(72.0f, 72.0f);
                m_Lock.gameObject.SetActive(false);//SkillState == 1 ? true : false);
                m_SkillState = SkillState;
            }
            this.m_Transform.gameObject.SetActive(true);
        }

        public int id { get { return m_ID; } }

        public int state { get { return m_SkillState; } }
        public UIPetsSkillItem_Type slotType { get { return m_SlotType; } }
    }
}

#endif
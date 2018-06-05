#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace xys.UI
{
    public enum UIPetsSkillItem_Type
    {
        None = -1,
        TrickSlot = 0,
        TalentSlot = 1,
        PassivasSlot = 2
    }
    [AutoILMono]
    public class UIPetsHandBookSkillItem
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Image m_Skill;
        [SerializeField]
        protected GameObject m_Icon;
        [SerializeField]
        protected UI.State.StateRoot m_Tag;

        UIPetsSkillItem_Type m_SlotType = UIPetsSkillItem_Type.None;

        protected int m_Id;
        public void SetData(int id,UIPetsSkillItem_Type type,ref int index,bool isGetSkill = false)
        {
            if(id == 0)
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }

            index++;
            m_Id = id;
            m_SlotType = type;

            this.m_Transform.gameObject.SetActive(true);

            m_Tag.gameObject.SetActive(type == UIPetsSkillItem_Type.PassivasSlot ? false : true);
            m_Tag.CurrentState = (int)type;

            Helper.SetSprite(m_Skill, Config.SkillIconConfig.Get(id).icon);

            m_Icon.SetActive(isGetSkill);
            //m_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(72.0f, 72.0f);
        }

        public int id { get { return m_Id; } }
        public UIPetsSkillItem_Type slotType { get { return m_SlotType; } }
    }
}



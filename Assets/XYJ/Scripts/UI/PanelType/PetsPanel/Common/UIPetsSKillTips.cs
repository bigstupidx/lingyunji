#if !USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace xys.hot.UI
{
    using Config;
    using xys.UI;
    [AutoILMono]
    class UIPetsSKillTips
    {
        [SerializeField]
        Transform m_Transform;
        public RectTransform content { get { return this.m_Transform.GetChild(0).GetComponent<RectTransform>(); } }
        protected int m_ClickHandlerId = 0;
        void Set(int id)
        {
            if (id == -1)
                return;
            Helper.SetSprite(this.m_Transform.Find("Skill/panel/1/Icon").GetComponent<Image>(), SkillIconConfig.Get(id).icon);

            if (SkillConfig.GetAll().ContainsKey(id))
            {
                SkillConfig data = SkillConfig.Get(id);
                this.m_Transform.Find("Skill/panel/1/Name").GetComponent<Text>().text = data.name;
                this.m_Transform.Find("Skill/panel/1/Text").GetComponent<Text>().text = data.isPetStunt ? "绝技" : "主动技能";
                this.m_Transform.Find("Skill/panel/Text").GetComponent<Text>().text = GlobalSymbol.ToUT(data.des);
                //return;
            }

            if (Config.PassiveSkills.GetAll().ContainsKey(id))
            {
                Config.PassiveSkills pData = Config.PassiveSkills.Get(id);
                this.m_Transform.Find("Skill/panel/1/Name").GetComponent<Text>().text = pData.name;
                this.m_Transform.Find("Skill/panel/1/Text").GetComponent<Text>().text = "被动技能";
                this.m_Transform.Find("Skill/panel/Text").GetComponent<Text>().text = GlobalSymbol.ToUT(pData.des);
                //return;
            }

            this.m_Transform.gameObject.SetActive(true);
            this.PlayAnimation(true);
        }

        public void Set(int id,int lv = 0)
        {
           Transform root = this.m_Transform.Find("Skill/panel/1/Level");
            if(lv != 0)
            {
                root.gameObject.SetActive(true);
                root.GetComponent<Text>().text = lv.ToString();
            }
            else
            {
                root.gameObject.SetActive(false);
            }
            this.Set(id);
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
            if (m_ClickHandlerId != 0)
                EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = EventHandler.pointerClickHandler.Add(this.OnGlobalClick);
        }

        void CloseEvent(object obj)
        {
            EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = 0;
            this.m_Transform.gameObject.SetActive(false);
        }

        protected bool OnGlobalClick(GameObject go, BaseEventData bed)
        {
            if (go == null || !go.transform.IsChildOf(this.m_Transform))
            {
                PlayAnimation(false);
                return false;
            }
            return true;
        }
    }
}


#endif
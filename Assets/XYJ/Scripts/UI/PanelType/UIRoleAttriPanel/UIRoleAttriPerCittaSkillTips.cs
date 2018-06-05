#if !USE_HOT
using UnityEngine;
using UnityEngine.UI;
using Config;
using xys.UI;
using UnityEngine.EventSystems;

namespace xys.hot.UI
{
    [System.Serializable]
    class UIRoleAttriPerCittaSkillTips
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Button m_Check;

        protected int m_ClickHandlerId = 0;

        public void ShowSkill(int id)
        {
            PersonalityCittaDefine citta = PersonalityCittaDefine.Get(id);
            if (citta == null) return;

            Helper.SetSprite(this.m_Transform.Find("Skill/panel/1/Icon").GetComponent<Image>(), citta.icon);

            m_Transform.Find("Skill/panel/1/Name").GetComponent<Text>().text = citta.name;
            m_Transform.Find("Skill/panel/1/title").GetComponent<Text>().text = GetCittaTypeName(citta.type) + "心法:";
            m_Transform.Find("Skill/panel/1/Text").GetComponent<Text>().text = "第" + citta.stage + "阶";
            m_Transform.Find("Skill/panel/Text").GetComponent<Text>().text = GlobalSymbol.ToUT(citta.attriDesc);
            m_Transform.gameObject.SetActive(true);
            PlayAnimation(true);
        }

        string GetCittaTypeName(int type)
        {
            switch (type)
            {
                case 1:
                    return "明理";
                case 2:
                    return "唯心";
                case 3:
                    return "尚武";
                case 4:
                    return "通智";
                case 5:
                    return "正义";
                case 6:
                    return "邪恶";
            }
            return string.Empty;
        }

        public void OnHide()
        {
            m_Transform.gameObject.SetActive(false);
        }

        void PlayAnimation(bool isOpen)
        {
            if (isOpen)
                AnimationHelp.PlayAnimation(m_Transform.GetComponent<Animator>(), "open", "ui_TanKuang_Tips", OpenEvent);
            else
                AnimationHelp.PlayAnimation(m_Transform.GetComponent<Animator>(), "close", "ui_TanKuang_Tips_Close", CloseEvent);
        }

        void OpenEvent(object obj)
        {
            if (m_ClickHandlerId != 0)
                EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = EventHandler.pointerClickHandler.Add(OnGlobalClick);

            m_Check.onClick.RemoveAllListeners();
            m_Check.onClick.AddListener(OpenPerCittaPanel);
        }

        void CloseEvent(object obj)
        {
            EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = 0;
            m_Transform.gameObject.SetActive(false);
        }

        protected bool OnGlobalClick(GameObject go, BaseEventData bed)
        {
            if ((go == null || !go.transform.IsChildOf(m_Transform)) && go != m_Check.gameObject)
            {
                PlayAnimation(false);
                return false;
            }
            return true;
        }

        void OpenPerCittaPanel()
        {
            //Debug.Log("打开个性心法界面");
            //打开个性心法界面
        }
    }
}
#endif
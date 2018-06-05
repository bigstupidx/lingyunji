#if !USE_HOT
using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class PonitSkill
    {
        public int pointId;
        public int skillId;
    }

    [AutoILMono]
    class SkillKeyboard
    {
        [SerializeField]
        ILMonoBehaviour m_ILSkillKeyboardPoint;
        SkillKeyboardPoint m_skillKeyboardPoint;

        [SerializeField]
        ILMonoBehaviour m_ILSkillKBChooseSkill;
        SkillKBChooseSkill m_skillKBChooseSkill;

        [SerializeField]
        ILMonoBehaviour m_ILSkillInfo;
        SkillInfo m_skillInfo;

        [SerializeField]
        GameObject m_listPrompt;
        [SerializeField]
        GameObject m_infoPrompt;

        [SerializeField]
        UIGroup m_group;
        [SerializeField]
        GameObject m_InfoContent;

        SkillPage m_skillPage;
        Dictionary<int, List<PonitSkill>> kBDic;
        int m_curSelet = -1;
        PonitSkill m_curPs;
        GameObject m_curSeletPoint = null;

        public SkillPage skillPage
        {
            get
            {
                return m_skillPage;
            }

            set
            {
                m_skillPage = value;
            }
        }

        private void Awake()
        {
            if (m_ILSkillKBChooseSkill != null) m_skillKBChooseSkill = (SkillKBChooseSkill)m_ILSkillKBChooseSkill.GetObject();
            if (m_ILSkillKeyboardPoint != null) m_skillKeyboardPoint = (SkillKeyboardPoint)m_ILSkillKeyboardPoint.GetObject();
            if (m_ILSkillInfo != null) m_skillInfo = (SkillInfo)m_ILSkillInfo.GetObject();

            m_ILSkillKBChooseSkill.gameObject.SetActive(false);
            for (int i = 0; i < m_ILSkillKeyboardPoint.transform.childCount; i++)
            {
                Button btn = m_ILSkillKeyboardPoint.transform.GetChild(i).GetComponent<Button>();
                if (btn == null) continue;

                btn.onClick.AddListener(() => { OnKeyClick(btn.gameObject); });
            }
        }

        private void OnEnable()
        {
            m_skillKeyboardPoint.ShowPoint(-1);
            EmptyList();
            m_skillInfo.SetEmpty();
        }

        public void RefreshData(bool isEmpty = true)
        {
            // 键位——技能列表
            kBDic = new Dictionary<int, List<PonitSkill>>();
            foreach (int item in skillPage.skillDic.Keys)
            {
                PonitSkill ps = new PonitSkill();
                ps.pointId = item;
                ps.skillId = skillPage.skillDic[item];

                SkillConfig sc = SkillConfig.Get(ps.skillId);
                if (sc == null) continue;
                int slot = sc.slotid;

                if (!kBDic.ContainsKey(slot))
                    kBDic.Add(slot, new List<PonitSkill>());
                kBDic[slot].Add(ps);
            }
            if (!isEmpty && m_curSeletPoint != null)
                OnKeyClick(m_curSeletPoint);
            else
                OnEnable();
        }

        void OnKeyClick(GameObject go)
        {
            m_curSeletPoint = go;
            int count = int.Parse(go.name);
            m_skillKeyboardPoint.ShowPoint(count);

            if (!kBDic.ContainsKey(count))
            {
                EmptyList();
                m_skillInfo.SetEmpty();
                return;
            }

            List<PonitSkill> psList = kBDic[count];
            m_group.SetCount(psList.Count);
            m_listPrompt.SetActive(false);

            for (int i = 0; i < m_group.Count; i++)
            {
                int index = i;
                GameObject obj = m_group.Get(i);
                Button btn = obj.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    m_curSelet = index;
                    m_curPs = psList[index];
                    SetCurSelect(index);
                    m_skillInfo.RefreshInfo(m_curPs.pointId, m_curPs.skillId);
                });

                ILMonoBehaviour ilMonoBehaviour = obj.GetComponent<ILMonoBehaviour>();
                if (ilMonoBehaviour != null)
                {
                    SkillTalentItem skillTalentItem = (SkillTalentItem)ilMonoBehaviour.GetObject();
                    skillTalentItem.SetBookClick(OnClickBookBtn);
                    skillTalentItem.SetTalentItem(psList[i].pointId, psList[i].skillId);
                }
            }

            for (int i = 0; i < psList.Count; i++)
            {
                SkillConfig skill = SkillConfig.Get(psList[i].skillId);
                if (skill == null) continue;
                if (skill.openLevel <= App.my.localPlayer.levelValue)
                {
                    m_curPs = psList[i];
                    SetCurSelect(i);
                    m_skillInfo.RefreshInfo(m_curPs.pointId, m_curPs.skillId);
                    break;
                }
            }
        }

        void SetCurSelect(int index)
        {
            for (int i = 0; i < m_group.Count; i++)
            {
                m_group.Get(i).GetComponent<StateRoot>().CurrentState = 0;
            }
            m_group.Get(index).GetComponent<StateRoot>().CurrentState = 1;
        }

        void EmptyList()
        {
            m_group.SetCount(0);
            m_listPrompt.SetActive(true);
        }

        void OnClickBookBtn(PonitSkill ps)
        {
            m_skillKBChooseSkill.Show(ps.skillId, SkillTalentShowConfig.GetByKeyAndPointId((int)App.my.localPlayer.job, ps.pointId));
            m_ILSkillKBChooseSkill.gameObject.SetActive(true);
        }
    }
}
#endif
#if !USE_HOT
using Config;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class SkillTalentItem
    {
        [SerializeField]
        Text m_tagText;
        [SerializeField]
        Image m_icon;
        [SerializeField]
        GameObject m_active;
        [SerializeField]
        Text m_level;
        [SerializeField]
        Text m_name;
        [SerializeField]
        Text m_grade;
        [SerializeField]
        Text m_talentLevel;
        [SerializeField]
        Text m_skillPostureType;
        [SerializeField]
        Button m_bookBtn;

        SkillTalentConfig m_talent;
        SkillConfig m_skill;

        System.Action<PonitSkill> m_clickEvent = null;

        public void SetBookClick(System.Action<PonitSkill> clickEvent)
        {
            m_clickEvent = clickEvent;
        }

        public void RefreshItem()
        {
            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            if (skillMgr == null) return;

            if (skillMgr.m_SkillSchemeDBData.allSkills.ContainsKey(m_talent.skillSeries))
            {
                int newSkillId = skillMgr.m_SkillSchemeDBData.allSkills[m_talent.skillSeries];
                if (newSkillId != m_skill.id)
                {
                    m_talent = SkillTalentConfig.GetByKeyAndId(m_talent.key, newSkillId);
                    if (m_talent == null) return;

                    m_skill = SkillConfig.Get(newSkillId);
                    SetItemData();
                }
            }

        }

        public void SetTalentItem(int pointId, int skillId)
        {
            SetTalentItem(SkillTalentConfig.GetByKeyAndId(pointId, skillId), SkillTalentConfig.GetSkillSeriesGroupByKey(pointId).Count);
        }

        public void SetTalentItem(SkillTalentConfig talent, int skillGradeCount)
        {
            if (m_active != null) m_active.SetActive(false);

            m_talent = talent;
            if (m_talent == null) return;

            if (m_bookBtn != null)
                m_bookBtn.gameObject.SetActive(skillGradeCount > 1 ? true : false);

            m_skill = SkillConfig.Get(m_talent.skillId);
            SetItemData();
        }

        void SetItemData()
        {
            if (m_skill == null) return;

            m_name.text = m_skill.name;
            Helper.SetSprite(m_icon, SkillIconConfig.Get(m_skill.id).icon);
            m_level.text = string.Format("{0}级解锁", m_skill.openLevel);
            m_icon.GetComponent<StateRoot>().CurrentState = m_skill.openLevel > App.my.localPlayer.levelValue ? 1 : 0;
            if (m_tagText != null) m_tagText.text = m_talent.tag;
            if (m_grade != null) m_grade.text = m_talent.gradeDes;
            if (m_talentLevel != null) m_talentLevel.text = "";

            SkillPostureType();
            ItemBtnInit();
        }

        void ItemBtnInit()
        {
            if (m_bookBtn != null)
            {
                m_bookBtn.onClick.RemoveAllListeners();
                m_bookBtn.onClick.AddListener(() => { BookChangeSkill(); });
            }
        }

        void SkillPostureType()
        {
            if (m_skillPostureType != null)
            {
                m_skillPostureType.text = "";
                RoleJob roleJob = RoleJob.Get((int)App.my.localPlayer.job);
                SkillTalentShowConfig stsc = SkillTalentShowConfig.GetByKeyAndPointId((int)App.my.localPlayer.job, m_talent.key);
                if (roleJob != null && stsc != null)
                {
                    switch (stsc.colu)
                    {
                        case 1:
                        case 2:
                            m_skillPostureType.text = roleJob.postureName1;
                            break;
                        case 3:
                        case 4:
                            m_skillPostureType.text = roleJob.postureName2;
                            break;
                        case 5:
                            m_skillPostureType.text = "受身技";
                            break;
                    }
                }
            }
        }

        void BookChangeSkill()
        {
            if (m_clickEvent != null)
            {
                PonitSkill ps = new PonitSkill();
                ps.pointId = m_talent.key;
                ps.skillId = m_talent.skillId;
                m_clickEvent(ps);
            }
        }

        public void SetActiveIcon(int activeId)
        {
            m_active.SetActive(m_skill.id == activeId ? true : false);
        }
    }
}
#endif
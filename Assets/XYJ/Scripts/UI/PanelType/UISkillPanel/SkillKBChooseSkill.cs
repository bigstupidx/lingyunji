#if !USE_HOT
using Config;
using NetProto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class SkillKBChooseSkill
    {
        [SerializeField]
        Transform m_transform;
        [SerializeField]
        UIGroup m_uiGroup;
        [SerializeField]
        Button m_switchBtn;
        [SerializeField]
        Button m_useBtn;
        [SerializeField]
        StateRoot m_saveBtnState;
        [SerializeField]
        GameObject m_tipsSkillScheme;
        [SerializeField]
        GameObject m_mask;

        [SerializeField]
        ILMonoBehaviour m_ILSkillInfo;
        SkillInfo m_skillInfo;

        int m_EventHanderId = -1;
        int m_curSkillId = 0;
        int m_originalSkillId = 0;
        int m_curSkillPointId = 0;

        Dictionary<int, int> skillTalentDic;
        protected System.Action<int> m_clickEvent = null;
        SkillMgr skillMgr;

        private void Awake()
        {
            if (m_ILSkillInfo != null) m_skillInfo = (SkillInfo)m_ILSkillInfo.GetObject();
            skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;

            m_switchBtn.onClick.AddListener(OnSwitchBtnClick);
            m_useBtn.onClick.AddListener(OnUseBtnClick);
        }

        private void OnEnable()
        {
            m_EventHanderId = EventHandler.pointerClickHandler.Add(CloseTips);
            if (m_curSkillPointId != 0 && m_originalSkillId != 0)
                m_skillInfo.RefreshInfo(m_curSkillPointId, m_originalSkillId);
        }

        private void OnDisable()
        {
            EventHandler.pointerClickHandler.Remove(m_EventHanderId);
        }

        public void Show(int skillId, SkillTalentShowConfig show, System.Action<int> click_event = null)
        {
            m_originalSkillId = skillId;
            m_clickEvent = click_event;

            skillTalentDic = new Dictionary<int, int>();

            if (show == null) return;
            Dictionary<int, List<SkillTalentConfig>> dic = SkillTalentConfig.GetSkillSeriesGroupByKey(show.skillPointId);
            m_curSkillPointId = show.skillPointId;

            // 默认技能排最前
            List<int> keyList = new List<int>();
            foreach (int item in dic.Keys)
            {
                if (show.defaultSkillSeries == item)
                    keyList.Insert(0, item);
                else
                    keyList.Add(item);
            }

            m_uiGroup.SetCount(dic.Count);

            // 设置每个SkillItem
            for (int i = 0; i < m_uiGroup.Count; i++)
            {
                int index = i;
                int talentskillId = GetTalentSkillId(show.key, keyList[i]);
                if (talentskillId == 0) continue;

                GameObject go = m_uiGroup.Get(i);
                Button btn = go.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    m_curSkillId = GetTalentSkillId(show.key, keyList[index]);
                    SetCurSelect(index);
                    m_skillInfo.RefreshInfo(show.skillPointId, GetTalentSkillId(show.key, keyList[index]), m_originalSkillId);
                });

                ILMonoBehaviour ilMonoBehaviour = go.GetComponent<ILMonoBehaviour>();
                if (ilMonoBehaviour != null)
                {
                    SkillTalentItem skillTalentItem = (SkillTalentItem)ilMonoBehaviour.GetObject();
                    SkillTalentConfig skillTalent = SkillTalentConfig.GetByKeyAndId(show.key, talentskillId);
                    if (skillTalent != null)
                    {
                        skillTalentItem.SetTalentItem(skillTalent, dic[keyList[i]].Count);
                        skillTalentItem.SetActiveIcon(m_originalSkillId);
                    }
                }
                skillTalentDic.Add(i, talentskillId);

                if (skillId == talentskillId)
                {
                    m_curSkillId = skillId;
                    SetCurSelect(i);
                }
            }
        }

        bool CloseTips(GameObject obj, BaseEventData bed)
        {
            if (obj != null && obj == m_mask)
            {
                m_transform.gameObject.SetActive(false);
                return false;
            }
            return true;
        }

        void OnSwitchBtnClick()
        {
            m_tipsSkillScheme.SetActive(true);
        }

        void OnUseBtnClick()
        {
            if (m_saveBtnState.CurrentState == 0) return;

            if (m_clickEvent != null)
                m_clickEvent(m_curSkillId);
            SkillSave skillSave = new SkillSave();
            skillSave.skillPointId = m_curSkillPointId;
            skillSave.skillId = m_curSkillId;
            App.my.eventSet.FireEvent(EventID.Skill_SaveSkill, skillSave);
            m_transform.gameObject.SetActive(false);
        }

        int GetTalentSkillId(int pointId, int seriesId)
        {
            int talentskillId = 0;
            if (skillMgr == null)
                skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;

            if (skillMgr.m_SkillSchemeDBData.allSkills.ContainsKey(seriesId))
                talentskillId = skillMgr.m_SkillSchemeDBData.allSkills[seriesId];
            else
                talentskillId = SkillTalentConfig.GetPrimaryBySkillSeries(seriesId);
            return talentskillId;
        }

        void SetCurSelect(int index)
        {
            for (int i = 0; i < m_uiGroup.Count; i++)
            {
                m_uiGroup.Get(i).GetComponent<StateRoot>().CurrentState = 0;
            }
            m_uiGroup.Get(index).GetComponent<StateRoot>().CurrentState = 1;
            ResetSaveBtnState(skillTalentDic[index]);
        }

        void ResetSaveBtnState(int skillId)
        {
            // 0 不可点击（置灰） 1 可点击
            if (skillId == m_originalSkillId)
            {
                m_saveBtnState.CurrentState = 0;
                return;
            }

            SkillConfig sc = SkillConfig.Get(skillId);
            if (sc != null)
                m_saveBtnState.CurrentState = sc.openLevel > App.my.localPlayer.levelValue ? 0 : 1;
        }
    }
}
#endif
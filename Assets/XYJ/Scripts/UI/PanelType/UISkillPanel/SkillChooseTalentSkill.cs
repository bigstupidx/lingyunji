#if !USE_HOT
// Author : ZhangXiaoWen
// Create Date : 2017/07/12

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
    class SkillChooseTalentSkill
    {
        [SerializeField]
        GameObject m_gameObject;
        [SerializeField]
        Button m_saveBtn;
        [SerializeField]
        StateRoot m_saveBtnState;

        [SerializeField]
        UIGroup m_uiGroup;

        [SerializeField]
        Transform m_skillItemGroup;
        [SerializeField]
        GameObject m_comperBtn;     // 领悟按钮
        [SerializeField]
        Transform m_clickBtn;       // 可点击按钮
        [SerializeField]
        ILMonoBehaviour m_ILSkillInfo;
        SkillInfo m_skillInfo;

        int m_EventHanderId = -1;
        float m_height = 230;
        int m_curSkillId = 0;
        int m_curSkillPoint = 0;
        int m_originalSkillId = 0;

        Dictionary<int, int> skillTalentDic;
        protected System.Action<int> m_ClickEvent = null;

        public int OriginalSkillId
        {
            get
            {
                return m_originalSkillId;
            }

            set
            {
                m_originalSkillId = value;
            }
        }

        private void Awake()
        {
            if (m_ILSkillInfo != null) m_skillInfo = (SkillInfo)m_ILSkillInfo.GetObject();
            m_height = m_gameObject.GetComponent<RectTransform>().sizeDelta.y;              //只有一个Item的初始高度
            m_saveBtn.onClick.AddListener(OnSaveSkill);
        }

        private void OnEnable()
        {
            m_EventHanderId = EventHandler.pointerClickHandler.Add(CloseTips);
        }

        private void OnDisable()
        {
            EventHandler.pointerClickHandler.Remove(m_EventHanderId);
        }

        public void Refresh()
        {
            for (int i = 0; i < m_uiGroup.Count; i++)
            {
                ILMonoBehaviour ilMonoBehaviour = m_uiGroup.Get(i).GetComponent<ILMonoBehaviour>();
                if (ilMonoBehaviour != null)
                {
                    SkillTalentItem skillTalentItem = (SkillTalentItem)ilMonoBehaviour.GetObject();
                    skillTalentItem.RefreshItem();
                }
            }
        }

        public void Show(int skillId, SkillTalentShowConfig show, System.Action<int> click_event = null)
        {
            m_curSkillPoint = show.skillPointId;
            m_originalSkillId = skillId;
            m_ClickEvent = click_event;

            skillTalentDic = new Dictionary<int, int>();
            Dictionary<int, List<SkillTalentConfig>> dic = SkillTalentConfig.GetSkillSeriesGroupByKey(show.skillPointId);

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

            // 适配排版高度
            m_uiGroup.gameObject.GetComponent<xys.UI.ContentSizeFitter>().SetDirty();
            Transform tf = m_uiGroup.transform.GetChild(0);

            float addheight = 0;
            if (dic.Count - 1 > 0)
                addheight = (dic.Count - 1) * (tf.GetComponent<RectTransform>().sizeDelta.y + m_uiGroup.GetComponent<VerticalLayoutGroup>().spacing);

            m_gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(m_gameObject.GetComponent<RectTransform>().sizeDelta.x, m_height + addheight);

            // 设置每个SkillItem
            for (int i = 0; i < m_uiGroup.Count; i++)
            {
                int index = i;
                int talentskillId = GetTalentSkillId(keyList[i]);
                if (talentskillId == 0) continue;

                GameObject go = m_uiGroup.Get(i);
                Button btn = go.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    m_curSkillId = GetTalentSkillId(keyList[index]);
                    SetCurSelect(index);
                    m_skillInfo.RefreshInfo(show.skillPointId, GetTalentSkillId(keyList[index]), m_originalSkillId);
                });

                ILMonoBehaviour ilMonoBehaviour = go.GetComponent<ILMonoBehaviour>();
                if (ilMonoBehaviour != null)
                {
                    SkillTalentItem skillTalentItem = (SkillTalentItem)ilMonoBehaviour.GetObject();
                    SkillTalentConfig skillTalent = SkillTalentConfig.GetByKeyAndId(show.skillPointId, talentskillId);
                    if (skillTalent != null)
                    {
                        skillTalentItem.SetTalentItem(skillTalent, dic[keyList[i]].Count);
                        skillTalentItem.SetActiveIcon(m_originalSkillId);
                    }
                }

                if (!skillTalentDic.ContainsKey(i))
                    skillTalentDic.Add(i, talentskillId);

                if (skillId == talentskillId)
                {
                    m_curSkillId = skillId;
                    SetCurSelect(i);
                }
            }

            m_gameObject.SetActive(true);
        }

        int GetTalentSkillId(int seriesId)
        {
            int talentskillId = 0;

            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            if (skillMgr != null && skillMgr.m_SkillSchemeDBData.allSkills.ContainsKey(seriesId))
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
            // 0 不可点击（置灰） 1 可点击  2 使用中（不可点击、置灰）
            if (skillId == m_originalSkillId)
            {
                m_saveBtnState.CurrentState = 2;
                return;
            }

            SkillConfig sc = SkillConfig.Get(skillId);
            if (sc != null)
                m_saveBtnState.CurrentState = sc.openLevel > App.my.localPlayer.levelValue ? 0 : 1;
        }

        void OnSaveSkill()
        {
            if (m_saveBtnState.CurrentState != 1) return;

            if (m_ClickEvent != null)
                m_ClickEvent(m_curSkillId);
            SkillSave skillSave = new SkillSave();
            skillSave.skillPointId = m_curSkillPoint;
            skillSave.skillId = m_curSkillId;
            App.my.eventSet.FireEvent(EventID.Skill_SaveSkill, skillSave);
            m_gameObject.SetActive(false);
        }

        bool CloseTips(GameObject obj, BaseEventData bed)
        {
            if (obj == null || !(obj.transform.IsChildOf(m_gameObject.transform) || obj.transform.IsChildOf(m_skillItemGroup.transform) || obj.transform.IsChildOf(m_clickBtn)))
            {
                m_gameObject.SetActive(false);
                return false;
            }
            return true;
        }
    }
}
#endif
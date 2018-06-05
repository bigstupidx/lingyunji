#if !USE_HOT
// Author : ZhangXiaoWen
// Create Date : 2017/07/12

using Config;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using UnityEngine.UI;
using xys.UI.State;
using NetProto;

namespace xys.hot.UI
{
    [AutoILMono]
    class SkillType
    {
        [SerializeField]
        Text m_pose1;
        [SerializeField]
        Text m_pose2;
        [SerializeField]
        UIGroup uiGroup;
        [SerializeField]
        GameObject m_selectEffect;
        [SerializeField]
        GameObject m_tipsAllEffect;
        [SerializeField]
        Button m_allEffect;
        [SerializeField]
        Button m_swicthScheme;

        [SerializeField]
        ILMonoBehaviour m_ILTipsChooseTalentSkill;
        SkillChooseTalentSkill m_skillChooseTalentSkill;

        [SerializeField]
        ILMonoBehaviour m_ILSkillKeyboardPoint;
        SkillKeyboardPoint m_skillKeyboardPoint;

        [SerializeField]
        ILMonoBehaviour m_ILSkillInfo;
        SkillInfo m_skillInfo;

        [SerializeField]
        GameObject m_tipsSkillScheme;
        [SerializeField]
        SkillTypePosParam posParam;

        SkillPage m_skillPage;
        GameObject m_curSelectGo;
        int m_curSelet;
        int m_curPointId;
        int m_curEffectType;
        Dictionary<int, SkillItem> skillPointDic;
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
            if (m_ILTipsChooseTalentSkill != null) m_skillChooseTalentSkill = (SkillChooseTalentSkill)m_ILTipsChooseTalentSkill.GetObject();
            if (m_ILSkillKeyboardPoint != null) m_skillKeyboardPoint = (SkillKeyboardPoint)m_ILSkillKeyboardPoint.GetObject();
            if (m_ILSkillInfo != null) m_skillInfo = (SkillInfo)m_ILSkillInfo.GetObject();

            m_swicthScheme.onClick.AddListener(OnSwicthSchemeBtnClick);
            foreach (Transform child in m_tipsAllEffect.transform)
            {
                child.GetComponent<Button>().onClick.AddListener(() => SetEffectType(child.gameObject));
            }

            m_curSelet = 0;
            m_curPointId = 0;
            m_curEffectType = 0;
        }

        private void OnEnable()
        {
            m_ILTipsChooseTalentSkill.gameObject.SetActive(false);
            m_ILSkillKeyboardPoint.gameObject.SetActive(false);
            m_allEffect.GetComponent<StateRoot>().CurrentState = 0;

            RoleJob roleJob = RoleJob.Get((int)App.my.localPlayer.job);
            m_pose1.text = roleJob.postureName1;
            m_pose2.text = roleJob.postureName2;

            posParam.transform.localPosition = Vector3.zero;
        }

        void OnSwicthSchemeBtnClick()
        {
            m_tipsSkillScheme.SetActive(true);
        }

        public void ShowSkillList()
        {
            List<SkillTalentShowConfig> showList = SkillTalentShowConfig.GetGroupBykey((int)App.my.localPlayer.job);
            if (showList == null)
            {
                m_selectEffect.SetActive(false);
                uiGroup.SetCount(0);
                return;
            }
            uiGroup.SetCount(showList.Count);
            skillPointDic = new Dictionary<int, SkillItem>();
            for (int i = 0; i < showList.Count; i++)
            {
                SkillTalentShowConfig talentShow = showList[i];
                int index = i;
                int skillId = GetPointSkillId(talentShow);
                if (skillId == 0) continue;

                GameObject go = uiGroup.Get(i);
                ILMonoBehaviour ilMonoBehaviour = go.GetComponent<ILMonoBehaviour>();
                if (ilMonoBehaviour != null)
                {
                    SkillItem skillItem = (SkillItem)ilMonoBehaviour.GetObject();
                    skillItem.SetSkillPoint(talentShow);
                    skillItem.SetSkillData(skillId, m_curEffectType);
                    SkillTalentConfig stc = SkillTalentConfig.GetByKeyAndId(talentShow.skillPointId, skillId);
                    if (stc != null) skillItem.effectType = stc.effectType;
                    if (!skillPointDic.ContainsKey(talentShow.skillPointId))
                        skillPointDic.Add(talentShow.skillPointId, skillItem);
                    else
                        go.SetActive(false);
                }

                ButtonPress lp = go.GetComponent<ButtonPress>();
                lp.onPress.RemoveAllListeners();
                lp.onRelease.RemoveAllListeners();
                lp.onClick.RemoveAllListeners();
                lp.onPress.AddListener(() => { OnPress(GetPointSkillId(talentShow)); });
                lp.onRelease.AddListener(OnUp);
                lp.onClick.AddListener(() =>
                {
                    if (m_ILSkillKeyboardPoint.gameObject.activeSelf) return;
                    m_curSelet = index;
                    m_curPointId = talentShow.skillPointId;
                    SetSelectEffect(go);

                    m_skillInfo.RefreshInfo(m_curPointId, GetPointSkillId(talentShow));
                    m_skillChooseTalentSkill.Show(GetPointSkillId(talentShow), talentShow);
                    m_ILTipsChooseTalentSkill.gameObject.GetComponent<RectTransform>().anchoredPosition3D = talentShow.colu > posParam.m_boundLRByColu ? posParam.m_posLeft : posParam.m_posRight;
                });

                //设置默认选项
                if (m_curSelet == i)
                {
                    SetSelectEffect(go);
                    m_curPointId = talentShow.skillPointId;
                    m_skillInfo.RefreshInfo(m_curPointId, skillId);
                }
            }

            RefreshTalentSkill();
        }

        int GetPointSkillId(SkillTalentShowConfig talentShow)
        {
            int skillId = 0;
            if (m_skillPage.skillDic.ContainsKey(talentShow.skillPointId))
                skillId = m_skillPage.skillDic[talentShow.skillPointId];
            else
                skillId = SkillTalentConfig.GetPrimaryBySkillSeries(talentShow.defaultSkillSeries);
            return skillId;
        }

        void SetSelectEffect(GameObject go)
        {
            m_selectEffect.transform.parent = go.transform.Find("Content");
            m_selectEffect.transform.localScale = Vector3.one;
            m_selectEffect.transform.localPosition = Vector3.zero;
            m_selectEffect.SetActive(true);
        }

        //刷新技能孔数据
        public void RefreshSkillData(SkillSave skillSave)
        {
            if (skillPointDic.ContainsKey(skillSave.skillPointId))
                skillPointDic[skillSave.skillPointId].SetSkillData(skillSave.skillId, m_curEffectType);

            // 刷新技能信息部分
            if (m_curPointId == skillSave.skillPointId)
                m_skillInfo.RefreshInfo(skillSave.skillPointId, skillSave.skillId, skillSave.skillId);
        }

        void OnPress(int skillId)
        {
            SkillConfig skill = SkillConfig.Get(skillId);
            if (skill != null)
                m_skillKeyboardPoint.ShowPoint(skill.slotid);
            m_ILSkillKeyboardPoint.gameObject.SetActive(true);
        }

        void OnUp()
        {
            m_ILSkillKeyboardPoint.gameObject.SetActive(false);
        }

        //设置技能效果类型
        private void SetEffectType(GameObject obj)
        {
            m_curEffectType = int.Parse(obj.name);
            m_tipsAllEffect.SetActive(false);
            m_allEffect.transform.Find("Text").GetComponent<Text>().text = obj.transform.Find("Text").GetComponent<Text>().text;
            m_allEffect.GetComponent<StateRoot>().CurrentState = 0;
            ShowSkillList();
        }

        void RefreshTalentSkill()
        {
            if (m_ILTipsChooseTalentSkill.gameObject.activeSelf)
            {
                m_skillChooseTalentSkill.OriginalSkillId = m_skillPage.skillDic[m_curPointId];
                m_skillChooseTalentSkill.Refresh();
            }
        }
    }
}
#endif
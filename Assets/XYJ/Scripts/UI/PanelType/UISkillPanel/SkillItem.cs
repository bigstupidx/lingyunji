#if !USE_HOT
using Config;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class SkillItem
    {
        [SerializeField]
        RectTransform m_rectTransform;
        [SerializeField]
        RectTransform m_rectLine;
        [SerializeField]
        StateRoot m_stateRoot;
        [SerializeField]
        Image m_image;
        [SerializeField]
        Text m_level;
        [SerializeField]
        GameObject m_top;

        [SerializeField]
        SkillTypePosParam m_skillTypePosParam;
        int m_effectType = 0;

        public int effectType
        {
            get
            {
                return m_effectType;
            }

            set
            {
                m_effectType = value;
            }
        }

        public void SetSkillPoint(SkillTalentShowConfig showConfig)
        {
            //图标位置
            m_rectTransform.anchoredPosition = new Vector2(m_skillTypePosParam.posXList[showConfig.colu - 1], m_skillTypePosParam.posYList[showConfig.row - 1]);
            //连线长度
            float height = 0;
            if (showConfig.releSkillPointId != 0)
            {
                SkillTalentShowConfig releConfig = SkillTalentShowConfig.GetByKeyAndPointId(showConfig.key, showConfig.releSkillPointId);
                if (showConfig.colu == releConfig.colu && releConfig.row > showConfig.row)
                    height = Mathf.Abs(m_skillTypePosParam.posYList[releConfig.row - 1] - m_skillTypePosParam.posYList[showConfig.row - 1]);
            }
            m_rectLine.sizeDelta = new Vector2(m_rectLine.sizeDelta.x, height);
            m_top.SetActive(SkillTalentConfig.GetSkillSeriesGroupByKey(showConfig.skillPointId).Count > 1 ? true : false);
        }

        public void SetSkillData(int skillId, int effectType)
        {
            SkillConfig skillConfig = SkillConfig.Get(skillId);
            if (skillConfig == null) return;

            Helper.SetSprite(m_image, SkillIconConfig.Get(skillConfig.id).icon);
            m_level.text = string.Format("{0}级解锁", skillConfig.openLevel);
            m_stateRoot.CurrentState = skillConfig.openLevel > App.my.localPlayer.levelValue ? 1 : 0;

            m_rectTransform.gameObject.AddComponentIfNoExist<CanvasGroup>().alpha = effectType == 0 || m_effectType == effectType ? 1f : 0.5f;
        }
    }
}
#endif
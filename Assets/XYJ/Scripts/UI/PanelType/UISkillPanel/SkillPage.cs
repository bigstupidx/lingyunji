#if !USE_HOT
using Config;
using NetProto;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class SkillPage : HotTablePageBase
    {
        [SerializeField]
        StateToggle m_toggle;

        [SerializeField]
        GameObject m_tipsComprehend;

        [SerializeField]
        ILMonoBehaviour m_ILSkillType;
        SkillType m_skillType;

        [SerializeField]
        ILMonoBehaviour m_ILSkillKeyboard;
        SkillKeyboard m_skillKeyboard;

        [SerializeField]
        ILMonoBehaviour m_ILSkillScheme;
        SkillScheme m_skillScheme;

        Dictionary<int, int> m_skillDic;

        public Dictionary<int, int> skillDic
        {
            get
            {
                return m_skillDic;
            }

            set
            {
                m_skillDic = value;
            }
        }

        SkillPage() : base(null) { }

        public SkillPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            m_toggle.OnSelectChange = OnChange;
            m_toggle.OnPreChange = PreChange;

            if (m_ILSkillType != null) m_skillType = (SkillType)m_ILSkillType.GetObject();
            if (m_ILSkillKeyboard != null) m_skillKeyboard = (SkillKeyboard)m_ILSkillKeyboard.GetObject();
            if (m_ILSkillScheme != null) m_skillScheme = (SkillScheme)m_ILSkillScheme.GetObject();
            m_skillScheme.skillPage = this;

            m_ILSkillScheme.gameObject.SetActive(false);
            m_tipsComprehend.SetActive(false);
        }

        protected override void OnShow(object p)
        {
            Event.Subscribe(EventID.Skill_RefreshSkillScheme, RefreshCurSkillData);
            Event.Subscribe(EventID.Skill_RefreshSkillSchemeName, m_skillScheme.RefreshCustomSchemeData);
            Event.Subscribe(EventID.Skill_ComprehendSucceed, ComprehendSucceed);
            Event.Subscribe<SkillSave>(EventID.Skill_RefreshSkill, RefreshSkillSave);

            m_toggle.Select = 0;
            RefreshCurSkillData();
        }

        void RefreshSkillSave(SkillSave skillSave)
        {
            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            if (skillMgr == null) return;
            if (skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(skillMgr.m_SkillSchemeDBData.curSchemeId))
                m_skillDic = new Dictionary<int, int>(skillMgr.m_SkillSchemeDBData.allScheme[skillMgr.m_SkillSchemeDBData.curSchemeId].skills);

            switch (m_toggle.Select)
            {
                case 0:
                    m_skillType.RefreshSkillData(skillSave);
                    break;
                case 1:
                    m_skillKeyboard.RefreshData(false);
                    break;
            }
        }

        void RefreshCurSkillData()
        {
            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            if (skillMgr == null) return;
            if (skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(skillMgr.m_SkillSchemeDBData.curSchemeId))
            {
                m_skillDic = new Dictionary<int, int>(skillMgr.m_SkillSchemeDBData.allScheme[skillMgr.m_SkillSchemeDBData.curSchemeId].skills);
                RefreshPage();
            }
        }

        void RefreshPage()
        {
            OnChange(null, m_toggle.Select);
        }

        bool PreChange(StateRoot sr, int index)
        {
            return true;
        }

        void OnChange(StateRoot sr, int index)
        {
            switch (index)
            {
                case 0:
                    ShowTypePage();
                    break;
                case 1:
                    ShowKeyboardPage();
                    break;
            }
        }

        private void ShowTypePage()
        {
            m_skillType.skillPage = this;
            m_skillType.ShowSkillList();
        }

        private void ShowKeyboardPage()
        {
            m_skillKeyboard.skillPage = this;
            m_skillKeyboard.RefreshData();
        }

        #region 领悟成功
        void ComprehendSucceed()
        {
            m_tipsComprehend.SetActive(true);
            TweenFill tf = m_tipsComprehend.transform.Find("Quan").GetComponent<TweenFill>();
            TweenRotation tr = m_tipsComprehend.transform.Find("Light").GetComponent<TweenRotation>();
            tf.ResetToBeginning();
            tr.ResetToBeginning();
            tf.PlayForward();
            tr.PlayForward();

            App.my.mainTimer.Register(2, 1, ComprehendFinish);
        }

        void ComprehendFinish()
        {
            m_tipsComprehend.SetActive(false);
            xys.UI.Utility.TipContentUtil.Show("skill_comprehend_succed");
            RefreshCurSkillDic();
        }
        #endregion

        void RefreshCurSkillDic()
        {
            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            if (skillMgr == null)
                return;

            List<int> keys = new List<int>();
            foreach (int item in m_skillDic.Keys)
            {
                keys.Add(item);
            }
            for (int i = 0; i < keys.Count; i++)
            {
                SkillTalentConfig stc = SkillTalentConfig.GetByKeyAndId(keys[i], m_skillDic[keys[i]]);
                if (stc != null)
                {
                    if (skillMgr.m_SkillSchemeDBData.allSkills.ContainsKey(stc.skillSeries))
                        m_skillDic[keys[i]] = skillMgr.m_SkillSchemeDBData.allSkills[stc.skillSeries];
                }
            }
            RefreshPage();
        }
    }
}
#endif
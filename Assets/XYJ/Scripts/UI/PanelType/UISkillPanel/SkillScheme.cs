#if !USE_HOT
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
    class SkillScheme
    {
        [SerializeField]
        GameObject m_gameobject;
        [SerializeField]
        Button m_closeBtn;
        [SerializeField]
        Button m_maskBtn;

        [SerializeField]
        StateToggle m_toggleType;

        [SerializeField]
        Button m_useRecommendBtn;

        [SerializeField]
        Button m_saveBtn;
        [SerializeField]
        Button m_useCustomBtn;

        [SerializeField]
        StateRoot m_contentA;
        [SerializeField]
        StateRoot m_contentB;
        [SerializeField]
        Transform m_nameA;
        [SerializeField]
        Transform m_nameB;

        [SerializeField]
        GameObject m_editorName;
        [SerializeField]
        Button m_confirmNameBtn;
        [SerializeField]
        InputField m_newName;

        xys.UI.Dialog.TwoBtn m_twoBtn;
        SkillPage m_skillPage;
        public SkillPage skillPage
        {
            set { m_skillPage = value; }
        }
        SkillSchemeType m_curEditorName;

        private void Awake()
        {
            m_closeBtn.onClick.AddListener(Close);
            m_maskBtn.onClick.AddListener(Close);
            m_saveBtn.onClick.AddListener(SaveScheme);
            m_useCustomBtn.onClick.AddListener(UseScheme);
            m_useRecommendBtn.onClick.AddListener(UseScheme);
            m_confirmNameBtn.onClick.AddListener(EditorNameConfirm);
            m_editorName.transform.Find("Mask").GetComponent<Button>().onClick.AddListener(CloseEditorName);
            m_nameA.Find("Bg").GetComponent<Button>().onClick.AddListener(() =>
            {
                m_curEditorName = SkillSchemeType.SST_Custom_A;
                EditorName();
            });
            m_nameB.Find("Bg").GetComponent<Button>().onClick.AddListener(() =>
            {
                m_curEditorName = SkillSchemeType.SST_Custom_B;
                EditorName();
            });

            m_editorName.SetActive(false);
            RefreshCustomSchemeData();
        }

        private void OnEnable()
        {
            m_toggleType.Select = -1;
        }

        void UseScheme()
        {
            // 未选择方案
            if (!StateToggleHasChoose(m_toggleType))
            {
                xys.UI.Utility.TipContentUtil.Show("skill_use_scheme");
                return;
            }

            // 自定义方案不存在
            Transform content = m_toggleType.list[m_toggleType.Select].transform.Find("Content");
            if (content != null && content.GetComponent<StateRoot>().CurrentState == 0)
            {
                xys.UI.Utility.TipContentUtil.Show("skill_use_tip");
                return;
            }

            m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                "", xys.UI.Utility.TipContentUtil.GenText("skill_use_scheme_tip"),
                "取消", () => false,
                "确定", () =>
                {
                    App.my.eventSet.FireEvent(EventID.Skill_UseSkillScheme, m_toggleType.Select + 1);
                    return false;
                }, true, true, () => { m_twoBtn = null; });
        }

        void SaveScheme()
        {
            if (!StateToggleHasChoose(m_toggleType))
            {
                xys.UI.Utility.TipContentUtil.Show("skill_save_scheme");
                return;
            }

            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            string name = "";
            if (skillMgr != null && skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey(m_toggleType.Select + 1))
                name = skillMgr.m_SkillSchemeDBData.allScheme[m_toggleType.Select + 1].skillSchemeName;
            name = string.IsNullOrEmpty(name) ? xys.UI.Utility.TipContentUtil.GenText("skill_scheme_name_" + (m_toggleType.Select + 1)) : name;

            m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                "", xys.UI.Utility.TipContentUtil.GenText("skill_save_scheme_tip", name),
                "取消", () => false,
                "确定", () =>
                {
                    SkillSchemeData input = new SkillSchemeData();
                    input.schemeId = m_toggleType.Select + 1;
                    input.skills = m_skillPage.skillDic;
                    App.my.eventSet.FireEvent(EventID.Skill_SaveSkillScheme, input);
                    return false;
                }, true, true, () => { m_twoBtn = null; });
        }

        bool StateToggleHasChoose(StateToggle toggle)
        {
            return toggle.Select < 0 ? false : true;
        }

        void EditorName()
        {
            m_editorName.SetActive(true);
            switch (m_curEditorName)
            {
                case SkillSchemeType.SST_Custom_A:
                    m_newName.text = m_nameA.Find("Text").GetComponent<Text>().text;
                    break;
                case SkillSchemeType.SST_Custom_B:
                    m_newName.text = m_nameB.Find("Text").GetComponent<Text>().text;
                    break;
            }
        }

        void EditorNameConfirm()
        {
            if (string.IsNullOrEmpty(m_newName.text))
            {
                xys.UI.Utility.TipContentUtil.Show("skill_set_name");
                return;
            }
            if (!UICommon.CheckSensitiveWord(m_newName.text)) return;

            SkillSchemeName input = new SkillSchemeName();
            input.schemeId = (int)m_curEditorName;
            input.skillSchemeName = m_newName.text;
            App.my.eventSet.FireEvent(EventID.Skill_SetSkillSchemeName, input);

            m_editorName.SetActive(false);
        }

        void Close()
        {
            m_gameobject.SetActive(false);
        }

        public void RefreshCustomSchemeData()
        {
            SetSchemeData(SkillSchemeType.SST_Custom_A, m_contentA, m_nameA);
            SetSchemeData(SkillSchemeType.SST_Custom_B, m_contentB, m_nameB);
        }

        void SetSchemeData(SkillSchemeType type, StateRoot sr, Transform nameGo)
        {
            SkillMgr skillMgr = App.my.localPlayer.GetModule<SkillModule>().skillMgr as SkillMgr;
            if (skillMgr.m_SkillSchemeDBData.allScheme.ContainsKey((int)type))
            {
                sr.CurrentState = 1;
                string name = skillMgr.m_SkillSchemeDBData.allScheme[(int)type].skillSchemeName;
                nameGo.Find("Text").GetComponent<Text>().text = string.IsNullOrEmpty(name) ? xys.UI.Utility.TipContentUtil.GenText("skill_scheme_name_" + (int)type) : name;
            }
            else
            {
                sr.CurrentState = 0;
            }
        }

        void CloseEditorName()
        {
            m_editorName.SetActive(false);
        }
    }
}
#endif
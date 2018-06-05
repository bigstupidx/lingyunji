#if !USE_HOT
using Config;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIRoleAttriPersonalityItem
    {
        [SerializeField]
        Transform trans;
        [SerializeField]
        Transform iconL;
        [SerializeField]
        Transform iconR;
        [SerializeField]
        Slider sliderL;
        [SerializeField]
        Slider sliderR;
        [SerializeField]
        Transform valueTf;
        [SerializeField]
        Text personality;
        [SerializeField]
        Text idealistic;
        [SerializeField]
        Text skill;

        [SerializeField]
        UIRoleAttriPerSectionInfo m_SectionInfo;
        [SerializeField]
        UIRoleAttriPerCittaSkillTips m_CittaSkillTips;

        EventAgent m_EventAgent;

        int index;
        int left;
        int right;
        bool isLeft;

        int value = 10;


        public void SetEvent(EventAgent eventAgent = null)
        {
            m_EventAgent = eventAgent;
        }

        void Awake()
        {
            trans.Find("Center").GetComponent<Button>().onClick.AddListener(() => { OnCenterClick(); });
            iconL.Find("SkillIcon1").GetComponent<Button>().onClick.AddListener(() => { OnCittaSkillClick(true); });
            iconR.Find("SkillIcon2").GetComponent<Button>().onClick.AddListener(() => { OnCittaSkillClick(false); });
        }

        private void OnEnable()
        {
            if (trans.name == "Item") return;
            index = int.Parse(trans.name);
            left = index % 2 == 1 ? index : index - 1;
            right = left + 1;
            isLeft = index == left;

            value = index * 20 + 20;

            ShowIcon();
            ShowBar();
            ShowText();
        }

        void ShowIcon()
        {
            iconL.Find("NameIcon1").GetComponent<StateRoot>().CurrentState = isLeft ? (left - 1) * 2 : (left - 1) * 2 + 1;
            iconL.Find("NameIcon1").GetComponent<Image>().SetNativeSize();
            iconR.Find("NameIcon2").GetComponent<StateRoot>().CurrentState = !isLeft ? (right - 1) * 2 : (right - 1) * 2 + 1;
            iconR.Find("NameIcon2").GetComponent<Image>().SetNativeSize();

            Image imageL = iconL.Find("SkillIcon1").GetComponent<Image>();
            Image imageR = iconR.Find("SkillIcon2").GetComponent<Image>();
            imageL.color = isLeft ? Color.white : Color.gray;
            imageR.color = !isLeft ? Color.white : Color.gray;

            if (GetSectionByPerIdAndValue(left, value) != null && PersonalityCittaDefine.Get(GetSectionByPerIdAndValue(left, value).cittaId) != null)
                Helper.SetSprite(imageL, PersonalityCittaDefine.Get(GetSectionByPerIdAndValue(left, value).cittaId).icon);
            if (GetSectionByPerIdAndValue(right, value) != null && PersonalityCittaDefine.Get(GetSectionByPerIdAndValue(right, value).cittaId) != null)
                Helper.SetSprite(imageR, PersonalityCittaDefine.Get(GetSectionByPerIdAndValue(right, value).cittaId).icon);
        }

        void ShowBar()
        {
            sliderL.value = isLeft ? value * 100 / PersonalityDefine.Get(left).sectionGroup[PersonalityDefine.sectionCount - 1].upperLimit * 0.01f : 0;
            sliderR.value = (!isLeft) ? value * 100 / PersonalityDefine.Get(right).sectionGroup[PersonalityDefine.sectionCount - 1].upperLimit * 0.01f : 0;

            valueTf.Find("Text").GetComponent<Text>().text = value.ToString();
            valueTf.SetParent(isLeft ? sliderL.transform.Find("Handle") : sliderR.transform.Find("Handle"));
            valueTf.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        void ShowText()
        {
            PersonalityDefine.PersonalitySection section = GetSectionByPerIdAndValue(index, value);
            if (section == null) return;

            PersonalityCittaDefine data = PersonalityCittaDefine.Get(section.cittaId);
            if (data != null)
                skill.text = "激活技能：" + data.name;

            personality.text = "个性：" + PersonalityDefine.Get(index).name + "【第" + section.rank + "阶】";
            idealistic.text = PersonalityDefine.Get(index).name + "值：" + value;
        }

        public static PersonalityDefine.PersonalitySection GetSectionByPerIdAndValue(int perId, int value)
        {

            PersonalityDefine data = PersonalityDefine.Get(perId);
            if (data == null || value < 0) return null;

            for (int i = 0; i < PersonalityDefine.sectionCount; i++)
            {
                if (value >= data.sectionGroup[i].lowerLimit && value <= data.sectionGroup[i].upperLimit)
                {
                    return data.sectionGroup[i];
                }
            }

            if (value > data.sectionGroup[PersonalityDefine.sectionCount - 1].upperLimit)
                return data.sectionGroup[PersonalityDefine.sectionCount - 1];

            return null;
        }

        void OnCenterClick()
        {
            m_SectionInfo.OnInit(left);
        }

        void OnCittaSkillClick(bool isBtnLeft)
        {
            int id = GetSectionByPerIdAndValue(isBtnLeft ? left : right, value).cittaId;
            m_CittaSkillTips.ShowSkill(id);
        }
    }
}
#endif
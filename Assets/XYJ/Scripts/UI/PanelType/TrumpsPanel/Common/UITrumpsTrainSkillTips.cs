#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using NetProto;
    using Config;
    using xys.UI;
    using xys.UI.State;

    [System.Serializable]
    class UITrumpsTrainSkillTips
    {
        [SerializeField]
        Transform m_Transform;
        public Transform transform { get { return m_Transform; } }

        [SerializeField]
        Transform m_ActiveSkillRoot;
        [SerializeField]
        Transform m_PassiveSkillRoot;

        [SerializeField]
        Text m_SkillName;
        [SerializeField]
        Text m_SkillLv;
        [SerializeField]
        StateRoot m_SkillType;
        [SerializeField]
        Text m_NowDes;
        [SerializeField]
        Text m_NextDes;

        [SerializeField]
        Text m_RequireDes;
        [SerializeField]
        GameObject m_MarRoot;
        [SerializeField]
        Image m_MarIcon;
        [SerializeField]
        Image m_MarQuality;
        [SerializeField]
        Text m_MarNum;
        [SerializeField]
        Button m_MarBtn;

        [SerializeField]
        Button m_SkillUpgradeBtn;

        [SerializeField]
        Button m_CloseBtn;

        TrumpsMgr m_TrumpMgr;

        int m_TrumpId;
        int m_SelectedSkillType;

        HotTablePageBase m_Page;
        public void OnInit(HotTablePageBase page)
        {
            m_Page = page;
            m_TrumpMgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr;

            m_ActiveSkillRoot.GetComponent<Button>().onClick.AddListener(() => { this.OnSelectedEvent((int)TrumpSkillType.Active); });
            m_PassiveSkillRoot.GetComponent<Button>().onClick.AddListener(() => { this.OnSelectedEvent((int)TrumpSkillType.Passive); });

            m_CloseBtn.onClick.AddListener(() => { this.m_Transform.gameObject.SetActive(false); });
        }
        public void Set(int trumpId,int skillType)
        {
            if (!m_TrumpMgr.CheckTrumps(trumpId))
                this.m_Transform.gameObject.SetActive(false);
            this.m_Transform.gameObject.SetActive(true);

            TrumpAttribute attribute = m_TrumpMgr.table.attributes[trumpId];
            if(attribute.activeskill.id != 0 && SkillIconConfig.GetAll().ContainsKey(attribute.activeskill.id))
                Helper.SetSprite(m_ActiveSkillRoot.Find("Icon").GetComponent<Image>(), SkillIconConfig.Get(attribute.activeskill.id).icon);
            else
                m_ActiveSkillRoot.gameObject.SetActive(false);

            if (attribute.passiveskill.id != 0 && PassiveSkills.GetAll().ContainsKey(attribute.passiveskill.id))
                Helper.SetSprite(m_PassiveSkillRoot.Find("Icon").GetComponent<Image>(), PassiveSkills.Get(attribute.passiveskill.id).icon);
            else
                m_PassiveSkillRoot.gameObject.SetActive(false);

            m_ActiveSkillRoot.Find("Effect").gameObject.SetActive(m_TrumpMgr.CanUpgradeSkill(attribute.activeskill.id, attribute.tastelv));
            m_PassiveSkillRoot.Find("Effect").gameObject.SetActive(m_TrumpMgr.CanUpgradeSkill(attribute.passiveskill.id, attribute.tastelv));

            m_SelectedSkillType = (int)TrumpSkillType.Active;
            m_TrumpId = trumpId;

            this.OnSelectedEvent(skillType);
        }

        public void OnRefresh()
        {
            this.Set(m_TrumpId, (int)m_SelectedSkillType);
        }

        void OnSelectedEvent(int skillType)
        {
            TrumpSkillData SkillData = skillType == (int)TrumpSkillType.Active ?
               m_TrumpMgr.table.attributes[m_TrumpId].activeskill : m_TrumpMgr.table.attributes[m_TrumpId].passiveskill;

            Config.TrumpSkill skillProperty = Config.TrumpSkill.Get(SkillData.id);
            Config.TrumpSkill nextSkillProperty = Config.TrumpSkill.Get(skillProperty.nextid);

            m_SkillName.text = TrumpProperty.GetColorName(m_TrumpId);
            m_SkillLv.text = SkillData.lv + "级";
            m_SkillType.CurrentState = skillType;
            m_NowDes.text = skillProperty.des;
            m_NextDes.text = nextSkillProperty == null ? string.Empty : nextSkillProperty.des;

            m_RequireDes.text = nextSkillProperty == null ? "该技能已满级" : skillProperty.needtastelv > m_TrumpMgr.table.attributes[m_TrumpId].tastelv ? GlobalSymbol.ToUT(string.Format("#[R1]法宝需要达到第{0}重时可升级#n", skillProperty.needtastelv)) : string.Empty;

            if(nextSkillProperty != null)
            {
                m_MarRoot.SetActive(true);
                Helper.SetSprite(m_MarIcon, Item.Get(skillProperty.itemid).icon);
                Helper.SetSprite(m_MarQuality, QualitySourceConfig.Get(Item.Get(skillProperty.itemid).quality).icon);
                int hasCount = hotApp.my.GetModule<HotPackageModule>().GetItemCount(skillProperty.itemid);
                m_MarNum.text = GlobalSymbol.ToUT(string.Format("#[{0}]{1}/{2}", hasCount >= skillProperty.itemcount ? "G2" : "R1", hasCount, skillProperty.itemcount));
                
                m_MarBtn.onClick.RemoveAllListeners();
                m_MarBtn.onClick.AddListener(() => { this.ShowItemTips(skillProperty.itemid); });

                m_SkillUpgradeBtn.gameObject.SetActive(true);
                m_SkillUpgradeBtn.onClick.RemoveAllListeners();
                m_SkillUpgradeBtn.onClick.AddListener(this.OnSkillUpgradeEvent);
            }
            else
            {
                m_MarRoot.SetActive(false);
                m_SkillUpgradeBtn.gameObject.SetActive(false);

                m_MarBtn.onClick.RemoveAllListeners();
                m_SkillUpgradeBtn.onClick.RemoveAllListeners();
            }
        }

        void OnSkillUpgradeEvent()
        {
            TrumpAttribute attribute = m_TrumpMgr.table.attributes[m_TrumpId];
            int tasteLv = attribute.tastelv;
            TrumpSkillData skillData = m_SelectedSkillType == (int)TrumpSkillType.Active ? attribute.activeskill : attribute.passiveskill;
            if (!Config.TrumpSkill.GetAll().ContainsKey(skillData.id))
                return;
            Config.TrumpSkill skillProperty = Config.TrumpSkill.Get(skillData.id);
            //境界不足
            if (skillProperty.needtastelv > attribute.tastelv)
            {
                SystemHintMgr.ShowHint(string.Format("法宝需要达到第{0}重时可升级", skillProperty.needtastelv));
                return;
            }
            //道具不足
            if (App.my.localPlayer.GetModule<PackageModule>().GetItemCount(skillProperty.itemid) < skillProperty.itemcount)
            {
                this.ShowItemTips(skillProperty.itemid);
                return;
            }

            TrumpsSkillRequest request = new TrumpsSkillRequest();
            request.trumpid = m_TrumpId;
            request.skillType = m_SelectedSkillType;
            m_Page.Event.FireEvent(EventID.Trumps_SkillUpgrade, request);
        }

        void ShowItemTips(int itemId)
        {
            UICommon.ShowItemTips(itemId);
        }
    }
}
#endif
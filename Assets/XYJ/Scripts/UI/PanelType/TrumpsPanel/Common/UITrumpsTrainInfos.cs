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

    [System.Serializable]
    class UITrumpsTrainInfos 
    {
        [SerializeField]
        Text m_TrumpName;
        [SerializeField]
        RawImage m_TrumpRawIcon;
        [SerializeField]
        Transform m_ActiveRoot;
        [SerializeField]
        Transform m_PassiveRoot;

        [SerializeField]
        UITrumpsTrainSkillTips m_SkillTips;

        RTTModelPartHandler m_Rtt;
        string m_ModelName;

        TrumpsMgr m_TrumpMgr;

        int m_TrumpId;

        public void OnInit(HotTablePageBase page)
        {
            m_TrumpMgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr as TrumpsMgr;
            this.OnCreateModel();

            m_SkillTips.OnInit(page);
        }

        public void OnShow()
        {
            if (m_Rtt == null) this.OnCreateModel();
            m_Rtt.SetRenderActive(true);
        }

        public void OnHide()
        {
            m_Rtt.SetRenderActive(false);
        }

        public void OnDestroy()
        {
            this.OnDelectModel();
        }

        public void Set(int trumpId)
        {
            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
                return;
            if (!m_TrumpMgr.CheckTrumps(trumpId))
                return;

            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(trumpId);

            m_TrumpName.text = TrumpProperty.GetColorName(trumpId);

            //skill
            if(attribute.activeskill.id != 0 && SkillIconConfig.GetAll().ContainsKey(attribute.activeskill.id))
            {
                m_ActiveRoot.gameObject.SetActive(true);
                if(SkillIconConfig.GetAll().ContainsKey(attribute.activeskill.id))
                    Helper.SetSprite(m_ActiveRoot.Find("Icon").GetComponent<Image>(), SkillIconConfig.Get(attribute.activeskill.id).icon);
                m_ActiveRoot.Find("Level").GetComponent<Text>().text = attribute.activeskill.lv + "级";

                m_ActiveRoot.GetComponent<Button>().onClick.RemoveAllListeners();
                m_ActiveRoot.GetComponent<Button>().onClick.AddListener(()=> { this.ShowSkillTipsEvent((int)TrumpSkillType.Active); });
            }
            else
            {
                m_ActiveRoot.gameObject.SetActive(false);
            }

            if (attribute.passiveskill.id != 0 && PassiveSkills.GetAll().ContainsKey(attribute.passiveskill.id))
            {
                m_PassiveRoot.gameObject.SetActive(true);

                if (SkillIconConfig.GetAll().ContainsKey(attribute.passiveskill.id))
                    Helper.SetSprite(m_PassiveRoot.Find("Icon").GetComponent<Image>(), PassiveSkills.Get(attribute.passiveskill.id).icon);
                m_PassiveRoot.Find("Level").GetComponent<Text>().text = attribute.passiveskill.lv + "级";

                m_PassiveRoot.GetComponent<Button>().onClick.RemoveAllListeners();
                m_PassiveRoot.GetComponent<Button>().onClick.AddListener(() => { this.ShowSkillTipsEvent((int)TrumpSkillType.Passive); });
            }
            else
            {
                m_PassiveRoot.gameObject.SetActive(false);
            }
            //model
            if (property.modelname != string.Empty)
            {
                if (property.modelname != m_ModelName)
                {
                    m_ModelName = property.modelname;
                    m_Rtt.SetRenderActive(true);
                    m_Rtt.ReplaceModel(property.modelname, (go) => { m_Rtt.SetCameraState(property.camView, new Vector3(property.campos[0], property.campos[1], property.campos[2])); });
                }
            }
            else
            {
                m_Rtt.SetRenderActive(false);
                m_ModelName = string.Empty;
            }

            m_TrumpId = trumpId;

            if (this.m_SkillTips.transform.gameObject.activeSelf)
                this.m_SkillTips.OnRefresh();
        }

        void ShowSkillTipsEvent(int skillType)
        {
            this.m_SkillTips.Set(m_TrumpId, skillType);
        }

        #region inside
        void OnCreateModel()
        {
            m_Rtt = new RTTModelPartHandler("RTTModelPart", m_TrumpRawIcon.GetComponent<RectTransform>(), "", true, new Vector3(1100, 1000, 0),
               () =>
               {
                   m_Rtt.SetModelRotate(new Vector3(0.0f, 150.0f, 0.0f));
               });
        }
        void OnDelectModel()
        {
            if (m_Rtt != null) m_Rtt.Destroy();
            m_Rtt = null;
            m_ModelName = string.Empty;
        }
        #endregion
    }
}
#endif
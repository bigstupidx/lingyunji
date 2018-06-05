#if !USE_HOT
namespace xys.hot
{
    using System;
    using UnityEngine;
    using battle;
    using Config;
    using UnityEngine.UI;
    using NetProto;
    using NetProto.Hot;
    using xys.UI;
    using System.Collections.Generic;
    using UnityEngine.EventSystems;

    class UITrumpTipsParam
    {
        public int trumpId;
        public int skillId;
        public int skillLv;
        public Vector2 pos;
    }

    class UITrumpTipsPanel : UI.HotPanelBase
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        RectTransform m_Infos;
        [SerializeField]
        RectTransform m_JoiningsRoot;
        //
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Image m_Quality;
        [SerializeField]
        Text m_TrumpName;
        [SerializeField]
        Text m_Level;
        [SerializeField]
        Text m_TasteLv;
        [SerializeField]
        GameObject m_EquipIcon;
        [SerializeField]
        Button m_JoiningBtn;
        [SerializeField]
        GameObject m_Joiningobj;

        [SerializeField]
        Text m_Des;
        [SerializeField]
        Transform m_ItemGrid;
        [SerializeField]
        Transform m_DropItems;
        [SerializeField]
        GameObject m_ItemPrefab;
        [SerializeField]
        Transform m_PropertyRoot;

        [SerializeField]
        Transform m_ActiveRoot;
        [SerializeField]
        Transform m_PassiveRoot;

        [SerializeField]
        UIGroup m_JoiningGroup;

        [SerializeField]
        uAnchor m_ScrollAnchor;
        [SerializeField]
        RectTransform m_InfoRoot;
        [SerializeField]
        RectTransform m_ScrollRoot;
        [SerializeField]
        float m_MaxLen;

        [SerializeField]
        Transform m_SkillTransform;
        
        float m_MarginButton;

        int m_ClickHandlerId;
        int m_SkillTipsClickHandlerId;

        TrumpsMgr m_Mgr;

        public UITrumpTipsPanel(xys.UI.UIHotPanel parent) : base(parent)  {  }

        public UITrumpTipsPanel() : base(null) { }

        protected override void OnInit()
        {
            m_JoiningBtn.onClick.AddListener(() => { this.m_Joiningobj.SetActive(true); });
            m_Mgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr;
        }

        protected override void OnShow(object p)
        {
            UITrumpTipsParam param = p as UITrumpTipsParam;
            if (p == null)
                return;
            if (param.trumpId != 0 && TrumpProperty.GetAll().ContainsKey(param.trumpId))
            {
                this.m_Infos.gameObject.SetActive(true);
                this.SetAttribute(param.trumpId, param.pos);
                this.OpenEvent();
            }
            else
            {
                this.m_Infos.gameObject.SetActive(false);
                this.SetSkillTips(param.skillId, param.skillLv, param.pos);
            }
        }

        protected override void OnHide()
        {
            this.CloseEvent();
            this.CloseSkillTipEvent();
        }

        void SetAttribute(int trumpId,Vector3 pos)
        {
            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
            {
                App.my.uiSystem.HidePanel(PanelType.UITrumpTipsPanel);
                return;
            }
            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = m_Mgr.GetTrumpAttribute(trumpId);
            this.SetUI(property, attribute,pos);
        }

        void SetProperty(int trumpId,Vector3 pos)
        {
            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
            {
                App.my.uiSystem.HidePanel(PanelType.UITrumpTipsPanel);
                return;
            }
            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = m_Mgr.GetNewTrumpAttribute(trumpId);
            this.SetUI(property, attribute,pos);
        }

        void SetUI(TrumpProperty property,TrumpAttribute attribute,Vector3 pos)
        {
            //pos
            m_Infos.localPosition = pos;
            float resPos = pos.x > 0 ?
                pos.x - m_Infos.sizeDelta.x * 0.5f - m_JoiningsRoot.sizeDelta.x * 0.5f :
                 pos.x + m_Infos.sizeDelta.x * 0.5f + m_JoiningsRoot.sizeDelta.x * 0.5f;
            //infos
            Helper.SetSprite(m_Icon, property.icon);
            Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).tips);
            m_TrumpName.text = TrumpProperty.GetColorName(property.id);
            m_Des.text = property.des;
            m_TasteLv.text = m_Mgr.GetTasteDes(property.id);// string.Format("境界•{0}", attribute.tastelv);
            m_Level.text = m_Mgr.GetInfusedDes(property.id);// string.Format("<color=#b3c1d8>品级</color> : 第{0}重 第{1}层", attribute.tastelv, attribute.infuseds.Count + 1);
            m_EquipIcon.SetActive(m_Mgr.GetEquipPos(property.id) != -1);
            //属性加成
            TrumpObj trumpObj = new TrumpObj();
            trumpObj.Init(attribute);
            this.Clear();
            foreach (int attributeIndex in trumpObj.battleAttri.GetKeys())
                this.NewPropertyItems(attributeIndex, AttributeDefine.GetValueStr(attributeIndex, trumpObj.battleAttri.Get(attributeIndex)));
            //技能
            if (property.activeskill != 0 && SkillIconConfig.GetAll().ContainsKey(attribute.activeskill.id))
            {
                m_ActiveRoot.gameObject.SetActive(true);
                Helper.SetSprite(m_ActiveRoot.Find("bg").GetComponent<Image>(), SkillIconConfig.GetIcon(attribute.activeskill.id));
                m_ActiveRoot.Find("name").GetComponent<Text>().text = SkillConfig.Get(attribute.activeskill.id).name;
                m_ActiveRoot.Find("level").GetComponent<Text>().text = attribute.activeskill.lv + "级";
                m_ActiveRoot.GetComponent<Button>().onClick.RemoveAllListeners();
                m_ActiveRoot.GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.SetSkillTips(attribute.activeskill.id, attribute.activeskill.lv, new Vector3(resPos, m_Infos.localPosition.y - m_Infos.sizeDelta.y*0.5f + m_SkillTransform.GetComponent<RectTransform>().sizeDelta.y, 0.0f));
                });
            }
            else
            {
                m_ActiveRoot.gameObject.SetActive(false);
            }
            if (property.passiveskill != 0 && PassiveSkills.GetAll().ContainsKey(attribute.passiveskill.id))
            {
                m_PassiveRoot.gameObject.SetActive(true);
                Helper.SetSprite(m_PassiveRoot.Find("bg").GetComponent<Image>(), SkillIconConfig.GetIcon(attribute.passiveskill.id));
                m_PassiveRoot.Find("name").GetComponent<Text>().text = PassiveSkills.Get(attribute.passiveskill.id).name;
                m_PassiveRoot.Find("level").GetComponent<Text>().text = attribute.passiveskill.lv + "级";
                m_PassiveRoot.GetComponent<Button>().onClick.RemoveAllListeners();
                m_PassiveRoot.GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.SetSkillTips(attribute.passiveskill.id, attribute.passiveskill.lv, new Vector3(resPos, m_Infos.localPosition.y - m_Infos.sizeDelta.y * 0.5f+ m_SkillTransform.GetComponent<RectTransform>().sizeDelta.y, 0.0f));
                });
            }
            else
            {
                m_PassiveRoot.gameObject.SetActive(false);
            }
            //joining
            m_JoiningGroup.SetCount(property.joinings.Length);
            for (int i = 0; i < property.joinings.Length; i++)
            {
                Transform item = m_JoiningGroup.transform.GetChild(i);
                item.GetComponent<WXB.SymbolText>().text = TrumpJoining.GetUTJoiningDes(property.joinings[i]);
            }
            if (m_Infos.sizeDelta.y < m_MaxLen)
            {
                m_Infos.GetComponent<xys.UI.ContentSizeFitter>().enabled = true;
                m_ScrollAnchor.enabled = true;
            }
            m_JoiningsRoot.localPosition = new Vector3(resPos, 0.0f, 0.0f);

            m_MarginButton = m_Infos.GetComponent<EasyLayout.EasyLayout>().MarginBottom;
        }

        void SetSkillTips(int skillId,int skillLv,Vector3 pos)
        {
            if (skillId == -1)
            {
                App.my.uiSystem.HidePanel(PanelType.UITrumpTipsPanel);
                return;
            }

            this.m_SkillTransform.gameObject.SetActive(true);
            //this.m_SkillTransform.transform.localPosition = pos;
            this.m_SkillTransform.GetComponent<RectTransform>().anchoredPosition = pos;
            Transform root = this.m_SkillTransform.Find("Skill/panel/1/Level");
            root.gameObject.SetActive(skillLv != 0);
            root.GetComponent<Text>().text = skillLv.ToString();
            
            if (SkillConfig.GetAll().ContainsKey(skillId))
            {
                SkillConfig data = SkillConfig.Get(skillId);
                Helper.SetSprite(this.m_SkillTransform.Find("Skill/panel/1/Icon").GetComponent<Image>(), SkillIconConfig.GetIcon(skillId));

                this.m_SkillTransform.Find("Skill/panel/1/Name").GetComponent<Text>().text = data.name;
                this.m_SkillTransform.Find("Skill/panel/1/Text").GetComponent<Text>().text = data.isPetStunt ? "绝技" : "主动技能";
                this.m_SkillTransform.Find("Skill/panel/Text").GetComponent<Text>().text = GlobalSymbol.ToUT(data.des);
            }

            if (Config.PassiveSkills.GetAll().ContainsKey(skillId))
            {
                Config.PassiveSkills pData = Config.PassiveSkills.Get(skillId);
                Helper.SetSprite(this.m_SkillTransform.Find("Skill/panel/1/Icon").GetComponent<Image>(), pData.icon);

                this.m_SkillTransform.Find("Skill/panel/1/Name").GetComponent<Text>().text = pData.name;
                this.m_SkillTransform.Find("Skill/panel/1/Text").GetComponent<Text>().text = "被动技能";
                this.m_SkillTransform.Find("Skill/panel/Text").GetComponent<Text>().text = GlobalSymbol.ToUT(pData.des);
            }
            this.PlayAnimation(true);
        }

        #region property
        Dictionary<int, Transform> m_PropertyItems = new Dictionary<int, Transform>();
        void Clear()
        {
            foreach (int index in m_PropertyItems.Keys)
            {
                Transform item = m_PropertyItems[index];
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
            m_PropertyItems.Clear();
        }

        void NewPropertyItems(int attributeIndex, string value)
        {
            GameObject obj = null;
            if (m_DropItems.childCount == 0)
            {
                obj = GameObject.Instantiate(m_ItemPrefab);
                if (obj == null) return;
            }
            else
            {
                obj = m_DropItems.GetChild(0).gameObject;
            }

            obj.SetActive(true);
            obj.transform.SetParent(m_ItemGrid, false);
            obj.transform.localScale = Vector3.one;
            obj.transform.SetSiblingIndex(m_PropertyRoot.GetSiblingIndex() + m_PropertyItems.Count + 1);

            obj.GetComponentInChildren<Text>().text = string.Format("{0}  {1}", AttributeDefine.GetTitleStr(attributeIndex), value);
            m_PropertyItems.Add(attributeIndex, obj.transform);
        }
        #endregion

        #region Event
        void PlayAnimation(bool isOpen)
        {
            if (isOpen)
            {
                AnimationHelp.PlayAnimation(this.m_SkillTransform.GetComponent<Animator>(), "open", "ui_TanKuang_Tips", null);
                this.OpenSkillTipEvent();
            }
            else
            {
                AnimationHelp.PlayAnimation(this.m_SkillTransform.GetComponent<Animator>(), "close", "ui_TanKuang_Tips_Close", null);
                this.CloseSkillTipEvent();
            }
        }

        void OpenSkillTipEvent()
        {
            if (m_SkillTipsClickHandlerId != 0)
                xys.UI.EventHandler.pointerClickHandler.Remove(m_SkillTipsClickHandlerId);
            m_SkillTipsClickHandlerId = xys.UI.EventHandler.pointerClickHandler.Add(this.OnGlobalSkillTipsClick);
        }

        void CloseSkillTipEvent()
        {
            xys.UI.EventHandler.pointerClickHandler.Remove(m_SkillTipsClickHandlerId);
            m_SkillTipsClickHandlerId = 0;
            this.m_SkillTransform.gameObject.SetActive(false);
        }

        void OpenEvent()
        {
            if (m_ClickHandlerId != -1)
                xys.UI.EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = xys.UI.EventHandler.pointerClickHandler.Add(this.OnGlobalClick);
        }

        void CloseEvent()
        {
            if (m_ClickHandlerId != -1)
                xys.UI.EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = -1;
            App.my.uiSystem.HidePanel(PanelType.UITrumpTipsPanel);
        }

        protected bool OnGlobalClick(GameObject go, BaseEventData bed)
        {
            if (go == null || !go.transform.IsChildOf(this.m_Transform))
            {
                this.CloseEvent();
                return false;
            }
         
            return true;
        }

        protected bool OnGlobalSkillTipsClick(GameObject go, BaseEventData bed)
        {
            if (go == null || !go.transform.IsChildOf(this.m_SkillTransform))
            {
                if (m_Infos.gameObject.activeSelf)
                    PlayAnimation(false);
                else
                    App.my.uiSystem.HidePanel(PanelType.UITrumpTipsPanel);
                return false;
            }
            return true;
        }
        #endregion
    }
}
#endif
#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using NetProto;
    using NetProto.Hot;
    using Config;
    using UnityEngine.EventSystems;

    [AutoILMono]
    class UITrumpTips 
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

        float m_MarginButton;

        int m_ClickHandlerId;

        TrumpsMgr m_Mgr;
        protected void Awake()
        {
            m_JoiningBtn.onClick.AddListener(() => { this.m_Joiningobj.SetActive(true); });
        }

        protected void OnEnable()
        {
            this.OpenEvent();
        }

        protected void OnDisable()
        {
            this.CloseEvent();
            this.m_JoiningsRoot.gameObject.SetActive(false);
        }

        public void Set(int trumpId, Vector3 pos)
        {
            if(m_Mgr == null)
                m_Mgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr;

            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
            {
                this.m_Transform.gameObject.SetActive(false);
                return;
            }
            //pos
            m_Infos.localPosition = pos;
            float resPos = pos.x > 0 ?
                pos.x - m_Infos.sizeDelta.x * 0.5f - m_JoiningsRoot.sizeDelta.x * 0.5f :
                 pos.x + m_Infos.sizeDelta.x * 0.5f + m_JoiningsRoot.sizeDelta.x * 0.5f;
            m_JoiningsRoot.localPosition = new Vector3(resPos, m_Infos.localPosition.y, 0.0f);
            //infos
            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = m_Mgr.GetTrumpAttribute(trumpId);
            Helper.SetSprite(m_Icon, property.icon);
            Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).tips);
            m_TrumpName.text = TrumpProperty.GetColorName(property.id);
            m_Des.text = property.des;
            m_Level.text = string.Format("<color=#b3c1d8>品级</color> : 第{0}重 第{1}层", attribute.tastelv, attribute.infuseds.Count + 1);
            m_EquipIcon.SetActive(m_Mgr.GetEquipPos(property.id) != -1);
            //属性加成
            TrumpObj trumpObj = new TrumpObj();
            trumpObj.Init(attribute);
            this.Clear();
            foreach (int attributeIndex in trumpObj.battleAttri.GetKeys())
                this.NewPropertyItems(attributeIndex, AttributeDefine.GetValueStr(attributeIndex,trumpObj.battleAttri.Get(attributeIndex)));
            //技能
            if(property.activeskill != 0 && SkillConfig.GetAll().ContainsKey(property.activeskill))
            {
                m_ActiveRoot.gameObject.SetActive(true);
                m_ActiveRoot.Find("name").GetComponent<Text>().text = SkillConfig.Get(attribute.activeskill.id).name;
                m_ActiveRoot.Find("level").GetComponent<Text>().text = attribute.activeskill.lv + "级";
            }
            else
            {
                m_ActiveRoot.gameObject.SetActive(false);
            }
            if(property.passiveskill != 0 && PassiveSkills.GetAll().ContainsKey(property.passiveskill))
            {
                m_PassiveRoot.gameObject.SetActive(true);
                m_PassiveRoot.Find("name").GetComponent<Text>().text = PassiveSkills.Get(attribute.passiveskill.id).name;
                m_PassiveRoot.Find("level").GetComponent<Text>().text = attribute.passiveskill.lv + "级";
            }
            else
            {
                m_PassiveRoot.gameObject.SetActive(false);
            }
            //joining
            m_JoiningGroup.SetCount(property.joinings.Length);
            for(int i = 0;i<property.joinings.Length;i++)
            {
                Transform item = m_JoiningGroup.transform.GetChild(i);
                item.GetComponent<Text>().text = TrumpJoining.GetDefaultJoiningDes(property.joinings[i]);
            }
            this.m_Transform.gameObject.SetActive(true);


            if (m_Infos.sizeDelta.y < m_MaxLen)
            {
                m_Infos.GetComponent<xys.UI.ContentSizeFitter>().enabled = true;
                m_ScrollAnchor.enabled = true;
            }

           m_MarginButton = m_Infos.GetComponent<EasyLayout.EasyLayout>().MarginBottom;
        }
        
        void LateUpdate()
        {
            //reset bg
            if (m_Infos.sizeDelta.y >= m_MaxLen)
            {
                m_Infos.GetComponent<xys.UI.ContentSizeFitter>().enabled = false;
                m_Infos.sizeDelta = new Vector2(m_Infos.sizeDelta.x, m_MaxLen);

                m_ScrollAnchor.enabled = false;
                m_ScrollRoot.sizeDelta = new Vector2(m_Infos.sizeDelta.x, m_Infos.sizeDelta.y - m_InfoRoot.sizeDelta.y - m_MarginButton);
            }
        }
        #region property
        Dictionary<int, Transform> m_PropertyItems = new Dictionary<int, Transform>();
        void Clear()
        {
            foreach(int index in m_PropertyItems.Keys)
            {
                Transform item = m_PropertyItems[index];
                item.SetParent(m_DropItems, false);
                item.gameObject.SetActive(false);
            }
            m_PropertyItems.Clear();
        }

        void NewPropertyItems(int attributeIndex,string value)
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
            obj.transform.SetSiblingIndex(m_PropertyRoot.GetSiblingIndex() + 1);

            obj.GetComponentInChildren<Text>().text = string.Format("{0}  {1}", AttributeDefine.GetTitleStr(attributeIndex), value);
            m_PropertyItems.Add(attributeIndex, obj.transform);
        }
        #endregion
        #region Event
        void OpenEvent(/*object obj*/)
        {
            if (m_ClickHandlerId != -1)
                EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = EventHandler.pointerClickHandler.Add(this.OnGlobalClick);
        }

        void CloseEvent(/*object obj*/)
        {
            if(m_ClickHandlerId != -1)
                EventHandler.pointerClickHandler.Remove(m_ClickHandlerId);
            m_ClickHandlerId = -1;
            this.m_Transform.gameObject.SetActive(false);
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
        #endregion
    }
}
#endif

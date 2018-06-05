#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using NetProto;
    using Config;
    using xys.UI.State;
    using battle;
    using xys.UI;

    [System.Serializable]
    partial class UITrumpsUpgrade 
    {
        #region 
        protected class Param
        {
            Text m_Attribute;
            Text m_Strengthen;
            Text m_Cost;
            Text m_Level;
            Button m_UpBtn;
            StateRoot m_StateRoot;

            int m_AttributeIndex;
            System.Action<int> m_Event = null;
            public void OnInit(Transform root,System.Action<int> action = null)
            {
                m_Attribute = root.Find("Attribute").GetComponent<Text>();
                m_Strengthen = root.Find("Strengthen").GetComponent<Text>();
                m_Cost = root.Find("Cost").GetComponent<Text>();
                m_Level = root.Find("Level").GetComponent<Text>();
                m_StateRoot = root.GetComponent<StateRoot>();
                m_UpBtn = root.Find("UpBtn").GetComponent<Button>();

                m_AttributeIndex = root.GetSiblingIndex() + 1;
                m_UpBtn.onClick.AddListener(this.ClickEvent);
                m_Event = action;
            }

            public void Set(TrumpAttribute attribute,BattleAttri battleAttri,int index,int points)
            {
                //下一个等级的数据
                int lv = attribute.cultivatepoints.ContainsKey(index) ? attribute.cultivatepoints[index].value : 0;
                this.m_Level.text = lv + "级";
                if (!TrumpCultivate.GetAll().ContainsKey(lv + 1) || !TrumpCultivate.GetAll().ContainsKey(lv))
                {
                    m_StateRoot.CurrentState = 1;
                    return;
                }
                m_StateRoot.CurrentState = 0;

                TrumpCultivate nextCultiveteData = TrumpCultivate.Get(lv + 1);
                TrumpCultivate nowCultiveteData = TrumpCultivate.Get(lv);

                m_Strengthen.text = AttributeDefine.Get(index).attrName + "+" + nextCultiveteData.propertyvalue  + "%";
                m_Cost.text = nextCultiveteData.soulpoints.ToString();

                double value = battleAttri.Get(index);
                m_Attribute.text = AttributeDefine.GetValueStr(index, value + Mathf.Ceil((float)value * (float)nextCultiveteData.propertyvalue * 0.01f));

                m_UpBtn.interactable = points >= nextCultiveteData.soulpoints;
                m_AttributeIndex = index;
            }

            void ClickEvent()
            {
                if (m_Event != null)
                    m_Event(m_AttributeIndex);
            }
        }
        #endregion

        [SerializeField]
        Transform m_Transform;
        public Transform transform { get { return m_Transform; } }

        [SerializeField]
        Text m_Level;
        [SerializeField]
        Text m_SoulLvPoint;
        [SerializeField]
        Transform m_AttributeGrid;

        [SerializeField]
        Button m_SoulTipsBtn;
        [SerializeField]
        Button m_ReturnBtn;
        
        System.Action m_ReturnCallback = null;

        TrumpsMgr m_TrumpMgr;

        List<Param> m_Items;

        int m_TrumpId;

        HotTablePageBase m_Page;
        public void OnInit(HotTablePageBase page,System.Action action = null)
        {
            m_TrumpMgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr as TrumpsMgr;

            m_Page = page;
            m_ReturnCallback = action;

            m_SoulTipsBtn.onClick.AddListener(this.ShowSoulTipsEvent);
            m_ReturnBtn.onClick.AddListener(this.HideTips);
            //属性
            m_Items = new List<Param>();
            for(int i = 0;i< m_AttributeGrid.childCount;i++)
            {
                Param param = new Param();
                param.OnInit(m_AttributeGrid.GetChild(i), this.UpgradeAttriEvent);
                m_Items.Add(param);
            }
            //tips道具
            m_UpgradeBtn.onClick.AddListener(this.AddExpEvent);
            m_CloseBtn.onClick.AddListener(() => { this.ShowTips(false); });
            //this.OnInitTips();
        }

        public void Set(int trumpId)
        {
            this.m_Transform.gameObject.SetActive(this.OnRefreshUI(trumpId));
        }

        public void OnRefresh()
        {
            this.Set(m_TrumpId);
            this.ResetTips();
        }

        public bool OnRefreshUI(int trumpId)
        {
            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
                return false;
            if (!m_TrumpMgr.CheckTrumps(trumpId))
                return false;
            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(trumpId);
            TrumpObj trumpObj = new TrumpObj();
            if (!trumpObj.Init(attribute))
                return false;
            m_Level.text = string.Format("潜修{0}级", attribute.souldata.lv);
            m_SoulLvPoint.text = (trumpObj.maxSoulPoint - trumpObj.useSoulPoint).ToString();
            //Attribute
            for (int i = 0; i < m_Items.Count; i++)
                m_Items[i].Set(attribute, trumpObj.battleAttri, i + 1, trumpObj.maxSoulPoint - trumpObj.useSoulPoint);

            m_TrumpId = trumpId;
            return true;
        }

        #region Event
        void UpgradeAttriEvent(int attributeIndex)
        {
            TrumpStrengthenRequest request = new TrumpStrengthenRequest();
            request.index = attributeIndex;
            request.trumpId = m_TrumpId;
            request.point = 1;
            m_Page.Event.FireEvent(EventID.Trumps_Strengthen, request);
        }

        void HideTips()
        {
            m_Transform.gameObject.SetActive(false);
            if (m_ReturnCallback != null) m_ReturnCallback();
        }
        #endregion
    }
}
#endif

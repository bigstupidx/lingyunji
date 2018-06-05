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
    using battle;
    using WXB;

    [System.Serializable]
    class UITrumpsTrainProperty 
    {
        [SerializeField]
        Transform m_Transform;

        [SerializeField]
        Transform m_BaseAttributeRoot;
        [SerializeField]
        Transform m_FirstAttributeRoot;

        [SerializeField]
        UIGroup m_JoiningGroup;
        [SerializeField]
        StateRoot m_JoiningSR;

        [SerializeField]
        Button m_InfusedBtn;
        [SerializeField]
        Button m_UpgradeBtn;

        [SerializeField]
        Transform m_Tips;

//         [SerializeField]
//         ILMonoBehaviour m_ILTrumpTips;
//         UITrumpTips m_TrumpTips;

        [SerializeField]
        StateToggle m_StateToggle;

        [SerializeField]
        UITrumpsUpgrade m_UpgradeTips;

        [SerializeField]
        Button m_JoiningBtn;
        [SerializeField]
        UITrumpsTrainJoiningTips m_JoiningTips;

        TrumpsMgr m_TrumpMgr;

        Dictionary<int, TrumpAttributeItem> m_AttributeDic;

        BattleAttri m_TrumpBAttleAttri;

        int m_TrumpId;

        public void OnInit(HotTablePageBase page)
        {
            m_TrumpMgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr as TrumpsMgr;
            m_AttributeDic = new Dictionary<int, TrumpAttributeItem>();
            //
            m_UpgradeTips.OnInit(page,this.HideUpgradeTips);
            m_JoiningTips.OnInit();
            //
//             if (m_ILTrumpTips != null)
//                 m_TrumpTips = m_ILTrumpTips.GetObject() as UITrumpTips;
            //初始化属性item
            List<AttributeDefine> tempBaseGroud = new List<AttributeDefine>();
            foreach (int attributeIndex in AttributeDefine.GetAll().Keys)
            {
                if (AttributeDefine.Get(attributeIndex).trumpBaseAttrOrder > 0)
                    tempBaseGroud.Add(AttributeDefine.Get(attributeIndex));
            }
            tempBaseGroud.Sort((a, b) => a.trumpBaseAttrOrder.CompareTo(b.trumpBaseAttrOrder));
            int index = 0;
            for (int i = 0; i < m_BaseAttributeRoot.childCount; i++, index++)
            {
                TrumpAttributeItem item = new TrumpAttributeItem(tempBaseGroud[index].id, m_BaseAttributeRoot.GetChild(i).GetComponent<Button>(), ShowAttributeTips);
                m_AttributeDic.Add(tempBaseGroud[index].id, item);
            }
            for (int i = 0; i < m_FirstAttributeRoot.childCount; i++, index++)
            {
                TrumpAttributeItem item = new TrumpAttributeItem(tempBaseGroud[index].id, m_FirstAttributeRoot.GetChild(i).GetComponent<Button>(), ShowAttributeTips);
                m_AttributeDic.Add(tempBaseGroud[index].id, item);
            }
            //event
            m_InfusedBtn.onClick.AddListener(this.ShowInfusedEvent);
            m_UpgradeBtn.onClick.AddListener(this.ShowUpgradeEvent);
            m_JoiningBtn.onClick.AddListener(this.ShowJoiningEvent);
        }

        public void Set(int trumpId)
        {
            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
                return;
            if (!m_TrumpMgr.CheckTrumps(trumpId))
                return;
            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = m_TrumpMgr.GetTrumpAttribute(trumpId);
            //
            TrumpObj trumpObj = new TrumpObj();
            trumpObj.Init(attribute);
            foreach (TrumpAttributeItem item in m_AttributeDic.Values)
                item.RefreshNum(trumpObj.battleAttri);
            //joinings
            int pos = m_TrumpMgr.GetEquipPos(trumpId);
            List<TrumpJoining> joiningList = m_TrumpMgr.GetActiveJoining();
            if (pos == -1 || joiningList.Count == 0)
                m_JoiningSR.CurrentState = 0;
            else
                this.ShowJoinings(trumpId, joiningList);
            m_TrumpBAttleAttri = trumpObj.battleAttri;
            m_TrumpId = trumpId;

            if(m_UpgradeTips.transform.gameObject.activeSelf)
                m_UpgradeTips.OnRefreshUI(m_TrumpId);
        }

        public void OnRefreshUpgrade()
        {
            if (m_UpgradeTips.transform.gameObject.activeSelf)
                m_UpgradeTips.OnRefresh();
        }
        #region inside
        void ShowJoinings(int trumpId, List<TrumpJoining> joiningList)
        {
            m_JoiningSR.CurrentState = 1;
            List<TrumpJoining> resTemp = new List<TrumpJoining>();
            for (int i = 0; i < joiningList.Count; i++)
                for (int j = 0; j < joiningList[i].trump.Length; j++)
                    if (trumpId == joiningList[i].trump[j])
                        resTemp.Add(joiningList[i]);
            //
            m_JoiningGroup.SetCount(resTemp.Count);
            for(int i = 0;i < resTemp.Count;i++)
            {
                Transform root = m_JoiningGroup.transform.GetChild(i);
                root.gameObject.SetActive(true);
                root.GetComponentInChildren<Text>().text = TrumpJoining.GetJoiningDes(resTemp[i].id);
                root.GetComponentInChildren<SymbolTextEvent>().OnClick.AddListener(this.HyperLinkEvent);
            }
        }
        #endregion
        #region Event
        void HyperLinkEvent(NodeBase node)
        {
            HyperlinkNode hNode = node as HyperlinkNode;
            if (hNode == null)
                return;
            //m_TrumpTips.Set(int.Parse(hNode.d_link), Vector3.zero);

            UICommon.ShowTrumpTips(int.Parse(hNode.d_link), Vector2.zero);
        }
        void ShowAttributeTips(int index, AttributeDefine data, Transform root)
        {
            string content = data.attrDescribe;
            if (content == string.Empty)
                return;
            BattleAttri attri = m_TrumpBAttleAttri;
            content = GlobalSymbol.ToUT(content).Replace("\\n", "\n");
            m_Tips.gameObject.SetActive(true);
            m_Tips.SetParent(root.transform, false);
            m_Tips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            m_Tips.GetComponentInChildren<Text>().text = Config.AttributeDefine.GetAttrTipsWithValue(content, attri);
        }

        void ShowInfusedEvent()
        {
            m_StateToggle.Select = 1;
        }
        void ShowUpgradeEvent()
        {
            m_Transform.gameObject.SetActive(false);
            m_UpgradeTips.Set(m_TrumpId);
        }
        void HideUpgradeTips()
        {
            m_Transform.gameObject.SetActive(true);
        }

        void ShowJoiningEvent()
        {
            m_JoiningTips.Set(m_TrumpId);
        }
        #endregion
    }
}
#endif
#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using NetProto;
    using Config;
    using battle;
    using xys.UI.State;
    using WXB;

    [System.Serializable]
    class UITrumpsProperty
    {
        readonly string[] c_TitleDefine = new string[] { "攻击属性", "防御属性", "五行属性", "高级属性" };

        [SerializeField]
        Transform m_BaseAttributeRoot;
        [SerializeField]
        Transform m_FirstAttributeRoot;

        [SerializeField]
        Transform m_MoreAttributeRoot; 
        [SerializeField]
        GameObject m_TitlePrefab;
        [SerializeField]
        GameObject m_ItemPrefab;

        [SerializeField]
        Transform m_Tips;
        [SerializeField]
        Transform m_MoreTips;
        
        [SerializeField]
        UIGroup m_JoiningsGroup;

        [SerializeField]
        StateRoot m_JoiningsSR;
        [SerializeField]
        StateRoot m_PropertySR;

//         [SerializeField]
//         ILMonoBehaviour m_ILTrumpTips;
//         UITrumpTips m_TrumpTips;

        Dictionary<int, TrumpAttributeItem> m_AttributeDic = new Dictionary<int, TrumpAttributeItem>();
        Dictionary<int, TrumpAttributeItem> m_MoreAttributeDic = new Dictionary<int, TrumpAttributeItem>();
        UITrumpsEquipPage m_Page;

        public void OnInit(UITrumpsEquipPage page)
        {
            m_Page = page;
            //
//             if (m_ILTrumpTips != null)
//                 m_TrumpTips = m_ILTrumpTips.GetObject() as UITrumpTips;
            //
            List<AttributeDefine> tempBaseGroud = new List<AttributeDefine>();
            List<AttributeDefine> tempMoreGroud = new List<AttributeDefine>();
            foreach (int attributeIndex in AttributeDefine.GetAll().Keys)
            {
                if (AttributeDefine.Get(attributeIndex).trumpBaseAttrOrder > 0)
                    tempBaseGroud.Add(AttributeDefine.Get(attributeIndex));
                if (AttributeDefine.Get(attributeIndex).trumpAttrOrder > 0)
                    tempMoreGroud.Add(AttributeDefine.Get(attributeIndex));
            }
            #region Baseattribute
            tempBaseGroud.Sort((a, b) => a.trumpBaseAttrOrder.CompareTo(b.trumpBaseAttrOrder));
            int index = 0;
            for (int i = 0; i < m_BaseAttributeRoot.childCount;i++, index++)
            {
                TrumpAttributeItem item = new TrumpAttributeItem(tempBaseGroud[index].id, m_BaseAttributeRoot.GetChild(i).GetComponent<Button>(), ShowAttributeTips);
                m_AttributeDic.Add(tempBaseGroud[index].id, item);
            }
            for (int i = 0; i < m_FirstAttributeRoot.childCount; i++,index++)
            {
                TrumpAttributeItem item = new TrumpAttributeItem(tempBaseGroud[index].id, m_FirstAttributeRoot.GetChild(i).GetComponent<Button>(), ShowAttributeTips);
                m_AttributeDic.Add(tempBaseGroud[index].id, item);
            }
            #endregion
            #region MoreAttribute
            GameObject obj = null;
            List<Transform> titleRoot = new List<Transform>();
            for (int i = 0; i < c_TitleDefine.Length; i++)
            {
                obj = GameObject.Instantiate(m_TitlePrefab);
                if (obj == null)
                    return;
                obj.SetActive(true);
                obj.transform.SetParent(m_MoreAttributeRoot, false);
                obj.transform.localScale = Vector3.one;
                obj.transform.Find("Name").GetComponent<Text>().text = c_TitleDefine[i];
                titleRoot.Add(obj.transform);
            }

            for (int j = 0; j < c_TitleDefine.Length; j++)
            {
                List<Config.AttributeDefine> tempGroud = new List<Config.AttributeDefine>();
                for (int i = 0; i < tempMoreGroud.Count; i++)
                {
                    if (tempMoreGroud[i].attrColumn - 1 == j)
                    {
                        tempGroud.Add(tempMoreGroud[i]);
                    }
                }
                tempGroud.Sort((a, b) => b.trumpAttrOrder.CompareTo(a.trumpAttrOrder));
                for (int i = 0; i < tempGroud.Count; i++)
                {
                    obj = GameObject.Instantiate(m_ItemPrefab);
                    if (obj == null) continue;

                    obj.SetActive(true);
                    obj.transform.SetParent(m_MoreAttributeRoot, false);
                    obj.transform.SetSiblingIndex(titleRoot[j].GetSiblingIndex() +  1);
                    obj.transform.localScale = Vector3.one;

                    TrumpAttributeItem item = new TrumpAttributeItem(tempGroud[i].id, obj.GetComponent<Button>(), ShowDetailAttrItemTips);
                    m_MoreAttributeDic.Add(tempGroud[i].id, item);
                }
            }
            #endregion
        }

        public void Refresh()
        {
            TrumpsMgr mgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
            if(mgr.IsNullEquip())
            {
                this.m_PropertySR.CurrentState = 1;
                return;
            }
            this.m_PropertySR.CurrentState = 0;
            foreach (TrumpAttributeItem data in m_AttributeDic.Values)
                data.RefreshNum(m_Page.equipBattleAttri);

            foreach (TrumpAttributeItem data in m_MoreAttributeDic.Values)
                data.RefreshNum(m_Page.equipBattleAttri);
            #region Joinings
            List<TrumpJoining> joinings =  mgr.GetActiveJoining();
            if(joinings.Count == 0 )
            {
                m_JoiningsSR.CurrentState = 0;
            }
            else
            {
                m_JoiningsSR.CurrentState = 1;
                m_JoiningsGroup.SetCount(joinings.Count);
                for (int i = 0; i < joinings.Count; i++)
                {
                    m_JoiningsGroup.transform.GetChild(i).gameObject.SetActive(true);
                    Transform root = m_JoiningsGroup.transform.GetChild(i);
                    root.GetComponentInChildren<Text>().text = TrumpJoining.GetJoiningDes(joinings[i].id);
                    root.GetComponentInChildren<SymbolTextEvent>().OnClick.AddListener(this.HyperLinkEvent);
                }
            }
            #endregion
        }
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
//             string content = data.attrDescribe;
//             if (content == string.Empty)
//                 return;
//             BattleAttri attri = m_Page.equipBattleAttri;
//             content = GlobalSymbol.ToUT(content).Replace("\\n", "\n");
//             m_Tips.gameObject.SetActive(true);
//             m_Tips.SetParent(root.transform, false);
//             m_Tips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
//             m_Tips.GetComponentInChildren<Text>().text = Config.AttributeDefine.GetAttrTipsWithValue(content, attri);
        }

        void ShowDetailAttrItemTips(int index,AttributeDefine data,Transform root)
        {
//             string content = data.attrDescribe;
//             if (content == string.Empty)
//                 return;
//             BattleAttri attri = m_Page.equipBattleAttri;
//             content = GlobalSymbol.ToUT(content).Replace("\\n", "\n");
//             m_MoreTips.gameObject.SetActive(true);
//             m_MoreTips.SetParent(root.transform, false);
//             m_MoreTips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
//             m_MoreTips.GetComponentInChildren<Text>().text = Config.AttributeDefine.GetAttrTipsWithValue(content, attri);
        }
        #endregion
    }
}
#endif

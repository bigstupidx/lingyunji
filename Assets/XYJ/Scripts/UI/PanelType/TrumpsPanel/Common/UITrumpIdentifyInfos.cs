#if !USE_HOT
namespace xys.hot.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using xys.battle;
    using WXB;

    [System.Serializable]
    public class UITrumpIdentifyInfos
    {
        [SerializeField]
        UIGroup m_TasteGroup;
        [SerializeField]
        UIGroup m_JoiningsGroup;
        [SerializeField]
        Text m_TrumpName;
        [SerializeField]
        RawImage m_TrumpRawIcon;
        [SerializeField]
        Text m_TrumpDes;
        [SerializeField]
        Text m_TrumpTaste;
        [SerializeField]
        Transform m_ActiveSkillRoot;
        [SerializeField]
        Button m_ActiveSkillBtn;
        [SerializeField]
        Transform m_PassiveSkillRoot;
        [SerializeField]
        Button m_PassiveSkillBtn;
        
        [SerializeField]
        Transform m_BaseAttributeRoot;
        [SerializeField]
        Transform m_FirstAttributeRoot;
        [SerializeField]
        Transform m_MoreTips;

        [SerializeField]
        Button m_TasteBtn;//境界btn

        [SerializeField]
        Button m_ItemInfoBtn;

        [SerializeField]
        float m_SkillTipsX = -137.0f;
        [SerializeField]
        float m_SkillTipsY = -275.0f;

//         [SerializeField]
//         ILMonoBehaviour m_ILSkillTips;
//         UIPetsSKillTips m_SkillTips;
//         [SerializeField]
//         ILMonoBehaviour m_ILTrumpTips;
//         UITrumpTips m_TrumpTips;

        RTTModelPartHandler m_Rtt;
        string m_ModelName;

        int m_TasteIndex;
        int m_TrumpId;

        int m_TasteHandleId;

        TrumpsMgr m_Mgr;

        Dictionary<int, TrumpAttributeItem> m_AttributeDic = new Dictionary<int, TrumpAttributeItem>();
        public void OnInit()
        {
            m_Mgr = hotApp.my.GetModule<HotTrumpsModule>().trumpMgr;

            m_ModelName = string.Empty;
            m_TasteIndex = 0;
            m_TrumpId = 0;
            //
            m_TasteBtn.onClick.AddListener(this.ShowTasteList);
            m_ItemInfoBtn.onClick.AddListener(this.ShowItemInfos);
            //
            this.OnCreateModel();
            //
//             if (m_ILSkillTips != null)
//                 m_SkillTips = m_ILSkillTips.GetObject() as UIPetsSKillTips;
//             if (m_ILTrumpTips != null)
//                 m_TrumpTips = m_ILTrumpTips.GetObject() as UITrumpTips;
            //初始化属性item
            List<AttributeDefine> tempBaseGroud = new List<AttributeDefine>();
            foreach(int attributeIndex in AttributeDefine.GetAll().Keys)
            {
                if (AttributeDefine.Get(attributeIndex).trumpBaseAttrOrder > 0)
                    tempBaseGroud.Add(AttributeDefine.Get(attributeIndex));
            }
            tempBaseGroud.Sort((a, b) => a.trumpBaseAttrOrder.CompareTo(b.trumpBaseAttrOrder));
            int index = 0;
            for (int i = 0; i < m_BaseAttributeRoot.childCount;i++, index++)
            {
                TrumpAttributeItem item = new TrumpAttributeItem(tempBaseGroud[index].id, m_BaseAttributeRoot.GetChild(i).GetComponent<Button>(), ShowAttributeTips);
                m_AttributeDic.Add(tempBaseGroud[index].id, item);
            }
            for (int i = 0; i < m_FirstAttributeRoot.childCount; i++, index++)
            {
                TrumpAttributeItem item = new TrumpAttributeItem(tempBaseGroud[index].id, m_FirstAttributeRoot.GetChild(i).GetComponent<Button>(), ShowAttributeTips);
                m_AttributeDic.Add(tempBaseGroud[index].id, item);
            }
        }

        #region 事件
        public void OnShow()
        {
            if (m_Rtt == null) this.OnCreateModel();
            m_Rtt.SetRenderActive(true);
        }
        public void OnHide()
        {
            if (m_TasteHandleId != 0)
                EventHandler.pointerClickHandler.Remove(m_TasteHandleId);
//             if (m_ILSkillTips != null)
//                 m_ILSkillTips.gameObject.SetActive(false);

            m_Rtt.SetRenderActive(false);
        }

        public void OnDestroy()
        {
            this.OnDelectModel();
        }

        void OnSelectedTasteEvent(int index)
        {
            m_TasteIndex = index;
            m_TrumpTaste.text = string.Format("境界•{0}", GlobalSymbol.ToNum(index));
            this.ShowAttribute(m_TrumpId, m_TasteIndex);
        }
        void ShowItemInfos()
        {
            SystemHintMgr.ShowHint("暂无");
        }
        void OnShowSkillTips(int skillId)
        {
            //m_SkillTips.Set(skillId, 0);
            if (skillId == 0)
                return;
            //             UITrumpTipsParam param = new UITrumpTipsParam();
            //             param.skillId = skillId;
            //             param.skillLv = 1;
            //             param.pos = new Vector2(m_SkillTipsX, m_SkillTipsY);
            //             App.my.uiSystem.ShowPanel(PanelType.UITrumpTipsPanel, param);
            UICommon.ShowTrumpSkillTips(skillId, 1, new Vector2(m_SkillTipsX, m_SkillTipsY));
        }

        void ShowAttributeTips(int index,AttributeDefine data,Transform root)
        {
            if (m_TrumpId == 0)
                return;
            string content = data.attrDescribe;
            if (content == string.Empty)
                return;
            BattleAttri attri = this.CalculateAttri(m_TrumpId, m_TasteIndex);
            content = GlobalSymbol.ToUT(content).Replace("\\n", "\n");
            m_MoreTips.gameObject.SetActive(true);
            m_MoreTips.SetParent(root.transform, false);
            m_MoreTips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            m_MoreTips.GetComponentInChildren<Text>().text = Config.AttributeDefine.GetAttrTipsWithValue(content, attri);
        }

        void ShowTasteList()
        {
            m_TasteBtn.GetComponent<StateRoot>().CurrentState = 1;
           m_TasteGroup.transform.parent.gameObject.SetActive(true);
            if (m_TasteHandleId != 0)
                EventHandler.pointerClickHandler.Remove(m_TasteHandleId);
            m_TasteHandleId = EventHandler.pointerClickHandler.Add(this.OnTastePress);
        }

        bool OnTastePress(GameObject go, BaseEventData eventData)
        {
            if (go == null || !go.transform.IsChildOf(this.m_TasteGroup.transform))
            {
                m_TasteBtn.GetComponent<StateRoot>().CurrentState = 0;
                m_TasteGroup.transform.parent.gameObject.SetActive(false);
                return false;
            }
            return true;
        }
        #endregion

        #region UI
        public void Set(TrumpProperty property)
        {
            m_TasteIndex = 0;
            m_TrumpId = property.id;
            m_TrumpName.text = TrumpProperty.GetColorName(property.id);
            m_TrumpTaste.text = string.Format("境界•{0}", GlobalSymbol.ToNum(m_TasteIndex));
            m_TrumpDes.text = property.des;
            //
            Transform root = null;
            root = m_ActiveSkillRoot.Find("Icon");
            m_ActiveSkillBtn.onClick.RemoveAllListeners();
            if (SkillIconConfig.GetAll().ContainsKey(property.activeskill))
            {
                root.gameObject.SetActive(true);
                Helper.SetSprite(root.GetComponent<Image>(), SkillIconConfig.GetIcon(property.activeskill));
                m_ActiveSkillBtn.onClick.AddListener(() => { this.OnShowSkillTips(property.activeskill); });
            }
            else
                root.gameObject.SetActive(false);
            //
            root = m_PassiveSkillRoot.Find("Icon");
            m_PassiveSkillBtn.onClick.RemoveAllListeners();
            if (SkillIconConfig.GetAll().ContainsKey(property.passiveskill))
            {
                root.gameObject.SetActive(true);
                Helper.SetSprite(root.GetComponent<Image>(), SkillIconConfig.GetIcon(property.passiveskill));
                m_PassiveSkillBtn.onClick.AddListener(() => { this.OnShowSkillTips(property.passiveskill); });
            }
            else
                root.gameObject.SetActive(false);

            #region 模型
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
            #endregion
            //属性
            this.ShowAttribute(property.id, m_TasteIndex);
            //连携属性
            this.m_JoiningsGroup.SetCount(property.joinings.Length);
            for (int i = 0; i < property.joinings.Length; i++)
            {
                root = m_JoiningsGroup.transform.GetChild(i);
                root.gameObject.SetActive(true);
                root.GetComponentInChildren<SymbolTextEvent>().OnClick.AddListener(this.HyperLinkEvent);
                root.GetComponentInChildren<Text>().text = TrumpJoining.GetJoiningDes(property.joinings[i]);
            }
            //境界
            if (TrumpSoul.GetAllGroupBytrumpid().ContainsKey(property.id))
                m_TasteGroup.SetCount(TrumpSoul.GetGroupBytrumpid(property.id).Count);
            else
                m_TasteGroup.SetCount(0);
            for (int i = m_TasteGroup.transform.childCount - 1; i >= 0; i--)
            {
                int index = m_TasteGroup.transform.childCount - i - 1;
                m_TasteGroup.transform.GetChild(i).GetComponentInChildren<Text>().text = string.Format("境界•{0}", GlobalSymbol.ToNum(index));
                m_TasteGroup.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                m_TasteGroup.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => { this.OnSelectedTasteEvent(index); });
            }
            this.OnSelectedTasteEvent(m_TasteIndex);
        }

        void ShowAttribute(int trumpId, int tasteLv)
        {
            BattleAttri attri = this.CalculateAttri(trumpId, m_TasteIndex);
            foreach (TrumpAttributeItem item in m_AttributeDic.Values)
                item.RefreshNum(attri);
        }

        BattleAttri CalculateAttri(int trumpId, int tasteLv)
        {
            BattleAttri attri = new BattleAttri();
            if (!TrumpInfused.infusedDic.ContainsKey(trumpId) || !TrumpInfused.infusedDic[trumpId].ContainsKey(tasteLv))
                return attri;
            if (!TrumpProperty.GetAll().ContainsKey(trumpId))
                return attri;
            //
            attri.Add(TrumpProperty.Get(trumpId).battleAttri);
            //
            for(int i = 0;i <= tasteLv;i++)
            {
                List<TrumpInfused> list = TrumpInfused.infusedDic[trumpId][i];
                for (int j = 0; j < list.Count; j++)
                    attri.Add(list[j].battleAttri);
            }
            return attri;
        }

        void OnCreateModel()
        {
            m_Rtt = new RTTModelPartHandler("RTTModelPart", m_TrumpRawIcon.GetComponent<RectTransform>(), "", true, new Vector3(1000, 1000, 0),
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
        void HyperLinkEvent(NodeBase node)
        {
            HyperlinkNode hNode = node as HyperlinkNode;
            if (hNode == null)
                return;
            //m_TrumpTips.Set(int.Parse(hNode.d_link), Vector3.zero);

            UICommon.ShowTrumpTips(int.Parse(hNode.d_link), Vector2.zero);
        }
        #endregion
    }
}
#endif

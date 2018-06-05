#if !USE_HOT
namespace xys.hot.UI
{
    using NetProto;
    using NetProto.Hot;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using xys.UI;
    using Config;
    using xys.UI.State;

    [System.Serializable]
    class UITrumpsEquip
    {
        protected class Param
        {
            Image m_Icon;
            Image m_Quality;
            Text m_Name;
            Image m_ActiveSkill;
            Image m_PassiveSKill;

            GameObject m_Selected;

            StateRoot m_StateRoot;

            int m_TrumpId;
            int m_PosIndex;

            System.Action<Param> m_SelectedCallback = null;
            System.Action<int,int> m_SelectedSkillCallback = null;
            public void OnInit(Transform root,System.Action<Param> action = null,System.Action<int,int> skillAction = null)
            {
                m_StateRoot = root.GetComponent<StateRoot>();
                m_Selected = root.Find("Item/Selected").gameObject;
                m_Icon = root.Find("Item/Icon").GetComponent<Image>();
                m_Quality = root.Find("Item/Quality").GetComponent<Image>();
                m_Name = root.Find("Name").GetComponent<Text>();
                if(root.Find("ActiveSKill/Icon") != null)
                    m_ActiveSkill = root.Find("ActiveSKill/Icon").GetComponent<Image>();
                if (root.Find("PassiveSkill/Icon") != null)
                    m_PassiveSKill = root.Find("PassiveSkill/Icon").GetComponent<Image>();
                
                root.Find("Item/Btn").GetComponent<Button>().onClick.AddListener(this.OnClickEvent);

                m_PosIndex = root.GetSiblingIndex();

                m_SelectedCallback = action;
                m_SelectedSkillCallback = skillAction;
            }

            public void Set(TrumpsMgr mgr,int pos)
            {
                if (!mgr.table.equiptrumps.ContainsKey(pos) || mgr.table.equiptrumps[pos] == 0)
                {
                    m_TrumpId = 0;
                    m_StateRoot.CurrentState = 1;
                    return;
                }
                m_StateRoot.CurrentState = 0;
                TrumpAttribute attribute = mgr.GetTrumpAttribute(mgr.table.equiptrumps[pos]);
                TrumpProperty property = TrumpProperty.Get(attribute.id);
                Helper.SetSprite(m_Icon, property.icon);
                Helper.SetSprite(m_Quality, QualitySourceConfig.Get(property.quality).icon);
                m_Name.text = TrumpProperty.GetColorName(attribute.id);// property.name;
                //
                if(m_ActiveSkill != null)
                {
                    m_ActiveSkill.transform.parent.Find("Btn").GetComponent<Button>().onClick.RemoveAllListeners();
                    if (SkillIconConfig.GetAll().ContainsKey(attribute.activeskill.id))
                    {
                        m_ActiveSkill.gameObject.SetActive(true);
                        Helper.SetSprite(m_ActiveSkill, SkillIconConfig.GetIcon(attribute.activeskill.id));
                        m_ActiveSkill.transform.parent.Find("Btn").GetComponent<Button>().onClick.AddListener(() => { this.OnClickSKillEvent(property.id, (int)TrumpSkillType.Active); });
                    }
                    else
                    {
                        m_ActiveSkill.gameObject.SetActive(false);
                    }
                }

                //
                if (m_PassiveSKill != null)
                {
                    m_PassiveSKill.transform.parent.Find("Btn").GetComponent<Button>().onClick.RemoveAllListeners();
                    if (PassiveSkills.GetAll().ContainsKey(attribute.passiveskill.id))
                    {
                        m_PassiveSKill.gameObject.SetActive(true);
                        Helper.SetSprite(m_PassiveSKill, PassiveSkills.Get(attribute.passiveskill.id).icon);
                        m_PassiveSKill.transform.parent.Find("Btn").GetComponent<Button>().onClick.AddListener(() => { this.OnClickSKillEvent(property.id, (int)TrumpSkillType.Passive); });
                    }
                    else
                    {
                        m_PassiveSKill.gameObject.SetActive(false);
                    }
                }

                m_TrumpId = property.id;
            }

            public void OnSelected(bool isSelect)
            {
                if (m_Selected != null)
                    m_Selected.SetActive(isSelect);
            }

            void OnClickEvent()
            {
                if (m_SelectedCallback != null)
                    m_SelectedCallback(this);
            }

            void OnClickSKillEvent(int skillId,int lv)
            {
                if (m_SelectedSkillCallback != null)
                    m_SelectedSkillCallback(skillId,lv);
            }

            public int trumpId { get { return m_TrumpId; } }
            public int posIndex { get { return m_PosIndex; } }
        }

        [SerializeField]
        Transform m_Root;
        [SerializeField]
        ILMonoBehaviour m_ILSkillTips;
        UIPetsSKillTips m_SkillTips;

        //选择回调
        System.Action m_SelectedCallback = null;
        public System.Action selectedCallback { set { m_SelectedCallback = value; } }
        //法宝管理器
        TrumpsMgr m_TrumpsMgr;
        //item组件
        Dictionary<int, Param> m_ItemDic = new Dictionary<int, Param>();
        //
        Param m_SelectedItem;
        public int selectedTrump { get { return m_SelectedItem != null ? m_SelectedItem.trumpId : 0; } }
        public int selectedIndex { get { return m_SelectedItem != null ? m_SelectedItem.posIndex : 0; } }

        public void OnInit()
        {
            if (m_ILSkillTips != null)
                m_SkillTips = m_ILSkillTips.GetObject() as UIPetsSKillTips;

            this.m_TrumpsMgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
            for(int i = 0; i < m_Root.childCount;i++)
            {
                Param param = new Param();
                param.OnInit(m_Root.GetChild(i),this.OnSelectedItem,this.OnSelectedSkill);
                m_ItemDic.Add(i, param);
            }
        }

        public void Refresh()
        {
            foreach (int pos in m_ItemDic.Keys)
                this.m_ItemDic[pos].Set(m_TrumpsMgr,pos);
        }

        public void OnResetSelected()
        {
            if(m_SelectedItem != null)
                    m_SelectedItem.OnSelected(false);
            m_SelectedItem = null;
        }

        public void OnSelected()
        {
            List<int> keys = new List<int>(this.m_ItemDic.Keys);
            if (m_SelectedItem != m_ItemDic[keys[0]])
            {
                if (m_SelectedItem != null)
                    m_SelectedItem.OnSelected(false);

                m_SelectedItem = m_ItemDic[keys[0]];

                if (m_SelectedItem != null)
                    m_SelectedItem.OnSelected(true);
            }
        }

        #region Event
        void OnSelectedItem(Param selected)
        {
            if(m_SelectedItem != selected)
            {
                if (m_SelectedItem != null)
                    m_SelectedItem.OnSelected(false);

                m_SelectedItem = selected;

                if (m_SelectedItem != null)
                    m_SelectedItem.OnSelected(true);
            }
            if (m_SelectedCallback != null)
                m_SelectedCallback();
        }

        void OnSelectedSkill(int trumpId,int skillType)
        {
            int pos = m_TrumpsMgr.GetEquipPos(trumpId);
            if (pos == -1)
                return;
            TrumpAttribute attribute = m_TrumpsMgr.GetTrumpAttribute(trumpId);
            TrumpProperty property = TrumpProperty.Get(attribute.id);
            int skillId = skillType == (int)TrumpSkillType.Active ? attribute.activeskill.id : attribute.passiveskill.id;
            int skillLv = skillType == (int)TrumpSkillType.Active ? attribute.activeskill.lv : attribute.passiveskill.id;
            if (!SkillConfig.GetAll().ContainsKey(skillId) && !PassiveSkills.GetAll().ContainsKey(skillId))
                return;
            m_SkillTips.Set(skillId, skillLv);
            //pos
            RectTransform rt = m_ILSkillTips.GetComponent<RectTransform>();
            RectTransform rootRt = skillType == (int)TrumpSkillType.Active ?
                m_Root.GetChild(pos).Find("ActiveSKill/Icon").GetComponent<RectTransform>() :
                m_Root.GetChild(pos).Find("PassiveSkill/Icon").GetComponent<RectTransform>();
            rt.position = rootRt.position;
            rt.localPosition = new Vector2(rt.localPosition.x + rt.GetChild(pos).GetComponent<RectTransform>().sizeDelta.x * 0.5f + rootRt.sizeDelta.x * 0.5f, rt.localPosition.y);
        }
        #endregion
    }
}
#endif
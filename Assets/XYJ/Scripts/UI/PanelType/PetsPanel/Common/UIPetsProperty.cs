#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using xys.UI.State;
using xys.UI;
using Config;
using NetProto.Hot;
using NetProto;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsProperty
    {
        readonly string[] c_TitleDefine = new string[] { "攻击属性", "防御属性", "五行属性", "高级属性" };
        readonly string[] c_TitleIconDefine = new string[] { "ui_Function_Icon_Attack", "ui_Function_Icon_Defense", "ui_Function_Icon_Senior", "ui_Function_Icon_Special" };
        [SerializeField]
        public GameObject m_TitlePrefab;
        [SerializeField]
        public GameObject m_TitleItemPrefab;
        [SerializeField]
        protected Transform m_ItemGrid;

        [SerializeField]
        Transform m_PetInfosRoot;
        [SerializeField]
        Transform m_BaseInfosRoot;
        [SerializeField]
        Transform m_BaseAttributeRoot;
        [SerializeField]
        Transform m_MoreAttributeRoot;
        [SerializeField]
        protected Button m_MoreAttributeBtn;

        [SerializeField]
        protected Transform m_Tips;
        [SerializeField]
        protected Transform m_MoreTips;
        [SerializeField]
        Transform m_SkillRoot;
        [SerializeField]
        ILMonoBehaviour m_ILSkillTips;
        UIPetsSKillTips m_SkillTips;

        [SerializeField]
        Button m_SkillPanelBtn;

        Dictionary<int, PetAttributeItem> m_AttDic = new Dictionary<int, PetAttributeItem>();
        Dictionary<int, PetAttributeItem> m_BaseAttDic = new Dictionary<int, PetAttributeItem>();

        float m_PetSavvy;
        float m_Attack;
        float m_AttackCD;

        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }
        //获取子控件
        void Awake()
        {
            if (m_ILSkillTips != null)
                m_SkillTips = (UIPetsSKillTips)m_ILSkillTips.GetObject();

            List<Config.AttributeDefine> attrPropList = new List<Config.AttributeDefine>(Config.AttributeDefine.GetAll().Values);
            List<Transform> titleRoot = new List<Transform>();
            GameObject obj = null;
            #region 更多属性面板
            for (int i = 0; i < c_TitleDefine.Length; i++)
            {
                obj = GameObject.Instantiate(m_TitlePrefab);
                if (obj == null)
                    return;
                obj.SetActive(true);
                obj.transform.SetParent(m_ItemGrid, false);
                obj.transform.localScale = Vector3.one;

                obj.transform.Find("Name").GetComponent<Text>().text = c_TitleDefine[i];

                titleRoot.Add(obj.transform);
            }
            for (int j = 0; j < c_TitleDefine.Length; j++)
            {
                List<Config.AttributeDefine> tempGroud = new List<Config.AttributeDefine>();
                for (int i = 0; i < attrPropList.Count; i++)
                {
                    if (attrPropList[i].attrColumn - 1 == j && attrPropList[i].petAttrOrder > 0)
                    {
                        tempGroud.Add(attrPropList[i]);
                    }
                }

                tempGroud.Sort((Config.AttributeDefine a, Config.AttributeDefine b) => b.petAttrOrder.CompareTo(a.petAttrOrder));

                for (int i = 0; i < tempGroud.Count; i++)
                {
                    obj = GameObject.Instantiate(m_TitleItemPrefab);
                    if (obj == null) continue;

                    obj.SetActive(true);
                    obj.transform.SetParent(m_ItemGrid, false);
                    obj.transform.SetSiblingIndex(titleRoot[j].GetSiblingIndex() + 1);
                    obj.transform.localScale = Vector3.one;

                    PetAttributeItem item = new PetAttributeItem(tempGroud[i].id, obj.GetComponent<Button>(), ShowDetailAttrItemTips);
                    m_AttDic.Add(tempGroud[i].id, item);
                }
            }
            #endregion
            List<Config.AttributeDefine> tempBaseGroud = new List<Config.AttributeDefine>();
            for (int i = Config.AttributeDefine.iStrength - 1; i < Config.AttributeDefine.iCureRate; i++)
            {
                tempBaseGroud.Add(attrPropList[i]);
            }
            tempBaseGroud.Sort((a, b) => a.petBaseAttrOrder.CompareTo(b.petBaseAttrOrder));
            for (int i = 0; i < m_BaseAttributeRoot.childCount; i++)
            {
                PetAttributeItem item = new PetAttributeItem(tempBaseGroud[i].id, m_BaseAttributeRoot.GetChild(i).GetComponent<Button>(), ShowBaseDetailAttrItemTips);
                m_BaseAttDic.Add(tempBaseGroud[i].id, item);
            }

            m_SkillPanelBtn.onClick.AddListener(() => { m_Panel.ShowPetsSKillPanel(); });
        }

        void OnEnable()
        {
            m_MoreAttributeBtn.onClick.AddListener(() =>
            {
                bool isShow = m_MoreAttributeRoot.gameObject.activeSelf;
                m_MoreAttributeRoot.gameObject.SetActive(!isShow);
                m_PetInfosRoot.gameObject.SetActive(isShow);
                m_MoreAttributeBtn.GetComponentInChildren<Text>().text = isShow ? "详细信息" : "返 回";
            });

            m_MoreAttributeRoot.gameObject.SetActive(false);
            m_PetInfosRoot.gameObject.SetActive(true);
        }
        void OnDisable()
        {
            m_MoreAttributeBtn.onClick.RemoveAllListeners();
            m_MoreAttributeBtn.GetComponentInChildren<Text>().text = "详细信息";
        }

        //重设子控件信息
        public void ResetData(int petIndex)
        {
            //重置位置
            PetsMgr petsMgr = m_Panel.petsMgr;
            if (!petsMgr.CheckIndex(petIndex))
                return;
            if (m_Panel.selectedPetObj == null)
                return;
            PetObj petObj = m_Panel.selectedPetObj;
            Config.PetAttribute property = Config.PetAttribute.Get(petObj.id);
            if (property == null)
                return;

            //baseInfo
            int petId = petObj.id;
            int attackId = 0;// Config.RoleDefine.Get(petId).defaultSkills[0];
            m_BaseInfosRoot.Find("Personality").GetComponent<Text>().text = petsMgr.GetPersonality(petObj.personality);
            m_PetSavvy = petObj.savvy;
            if(Config.SkillConfig.GetAll().ContainsKey(petId))
                m_Attack = Config.SkillConfig.Get(attackId).range;
            else
                m_Attack = 0.0f;
            m_AttackCD = Config.PetparametersDefine.Get(petId).attackInterval;
            m_BaseInfosRoot.Find("Attack").GetComponent<Text>().text = m_Attack.ToString();
            m_BaseInfosRoot.Find("AttackCD").GetComponent<Text>().text = m_AttackCD.ToString();
            m_BaseInfosRoot.Find("WuXing").GetComponent<Text>().text = "" + petObj.savvy;// (int)( - 1) * 100;
            m_BaseInfosRoot.Find("Icon4").GetComponent<StateRoot>().CurrentState = property.fiveElements - 1;

            //设置一级属性
            int index = 0;
            foreach (PetAttributeItem item in m_BaseAttDic.Values)
                item.RefreshNum(petObj);

            //设置二级属性
            foreach (PetAttributeItem item in m_AttDic.Values)
                item.RefreshNum(petObj);
            
            //设置技能
            for (int i = 0; i < m_SkillRoot.childCount; i++)
                m_SkillRoot.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            index = 0;
            this.SetSkillData(m_SkillRoot.GetChild(index), petObj.trickSkills, ref index);
            this.SetSkillData(m_SkillRoot.GetChild(index), petObj.talentSkills, ref index);
            for (int i = 0; i < petObj.passiveSkills.Count; i++)
                this.SetSkillData(m_SkillRoot.GetChild(index), petObj.passiveSkills[i], ref index);

            for (int i = index; i < m_SkillRoot.childCount; i++)
            {
                m_SkillRoot.GetChild(i).GetComponent<StateRoot>().CurrentState = 0;
                m_SkillRoot.GetChild(i).Find("Lock").gameObject.SetActive(false);
            }
        }
        
        IEnumerator TipsAutoSize(Transform target)
        {
            yield return new WaitForEndOfFrame();
            target.gameObject.SetActive(true);
            target.GetComponentInChildren<xys.UI.ContentSizeFitter>().SetDirty();
        }

        void SetSkillData(Transform root, PetSkillData data, ref  int index)
        {
            if (data.id == 0)
            {
                root.GetComponent<StateRoot>().CurrentState = 0;
                return;
            }
            //判断技能类型
            Config.SkillConfig skillData = null;
            if (Config.SkillConfig.GetAll().ContainsKey(data.id))
                skillData = Config.SkillConfig.Get(data.id);
            Config.PassiveSkills pSkillData = null;
            if (Config.PassiveSkills.GetAll().ContainsKey(data.id))
                pSkillData = Config.PassiveSkills.Get(data.id);
            if (skillData == null && pSkillData == null)
            {
                root.GetComponent<StateRoot>().CurrentState = 0;
                return;
            }
            index +=1;
            root.GetComponent<StateRoot>().CurrentState = 1;

            int curIndex = 0;
            if (skillData != null)
            {
                curIndex = !skillData.isPetStunt ? 1 : 0;
            }
            if (pSkillData != null)
            {
                curIndex = pSkillData.type == (int)Config.PassiveSkills.Type.Talent ? 3 : 2;
            }
            root.Find("Tag").gameObject.SetActive(true);
            root.Find("Lock").gameObject.SetActive(false);
            if (curIndex == 1 || curIndex == 2)
                root.Find("Tag").gameObject.SetActive(false);
            else
                root.Find("Tag").GetComponent<StateRoot>().CurrentState = curIndex;
            Helper.SetSprite(root.Find("Icon").GetComponent<Image>(), Config.SkillIconConfig.Get(data.id).icon);
            //添加技能tips事件
            int skillIdentity = data.id;
            int skillState = data.islock;
            root.GetComponent<Button>().onClick.AddListener(() => { this.SkillTipsEvent(skillIdentity); });
        }

        void SkillTipsEvent(int id)
        {
            m_SkillTips.Set(id);
        }

        void ShowBaseDetailAttrItemTips(int id, Config.AttributeDefine data,Transform parent)
        {
            if (m_Panel.selectedPetObj == null)
                return;
            if (data.attrDescribe == string.Empty)
                return;

            string content = data.attrDescribe;
            content = GlobalSymbol.ToUT(content).Replace("\\n", "\n");

            m_Panel.selectedPetObj.GetTipsDes(id, ref content);
            m_Tips.transform.SetParent(parent.transform, false);
            m_Tips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            m_Tips.GetComponentInChildren<Text>().text = content;// Config.AttributeDefine.GetAttrTipsWithValue(content,m_Panel.selectedPetObj.battleAttri);
            m_Panel.parent.StartCoroutine(TipsAutoSize(m_Tips));
        }

        void ShowDetailAttrItemTips(int id, Config.AttributeDefine data,Transform parent)
        {
            if (m_Panel.selectedPetObj == null)
                return;

            string content = data.attrDescribe;
            if (content == string.Empty)
                return;
            content = GlobalSymbol.ToUT(content).Replace("\\n", "\n");
            m_Panel.selectedPetObj.GetTipsDes(id, ref content);
            m_MoreTips.transform.SetParent(parent.transform, false);
            m_MoreTips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            m_MoreTips.GetComponentInChildren<Text>().text = Config.AttributeDefine.GetAttrTipsWithValue(content, m_Panel.selectedPetObj.battleAttri);
            m_Panel.parent.StartCoroutine(TipsAutoSize(m_MoreTips));
        }
    }
}

#endif
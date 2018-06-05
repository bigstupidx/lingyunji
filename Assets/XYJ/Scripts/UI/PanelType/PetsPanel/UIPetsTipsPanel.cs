#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using xys.UI.State;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using NetProto.Hot;
using NetProto;

namespace xys.hot.UI
{
    class UIPetsTipsPanel : HotPanelBase
    {
        [SerializeField]
        StateRoot m_PetQuality;
        [SerializeField]
        Text m_PetName;
        [SerializeField]
        Text m_PetLevel;
        [SerializeField]
        GameObject m_VariationObj;
        [SerializeField]
        RawImage m_RawImage;

        [SerializeField]
        Transform m_BaseInfoRoot;
        [SerializeField]
        Transform m_AttributeRoot;
        [SerializeField]
        Transform m_ItemInfoRoot;
        [SerializeField]
        Transform m_SkillRoot;
        [SerializeField]
        int maxValue = 136;
        [SerializeField]
        UIPetsSKillTips m_SkillTips;

        RTTModelPartHandler m_Rtt;
        int m_AttributeIndex = -1;

        public UIPetsTipsPanel() : base(null) { }
        public UIPetsTipsPanel(UIHotPanel parent) : base(parent) { }

        protected override void OnInit()
        {
            this.OnCreateModel();
        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            if (m_Rtt == null) this.OnCreateModel();
            this.ActiveModel();

            if (args is int)
            {
                int itemIndex = (int)args;
                this.Set(itemIndex);
            }
            else if(args is PetsAttribute)
            {
                this.Set(args as PetsAttribute);
            }
            else
            {
                App.my.uiSystem.HidePanel(PanelType.UIPetsTipsPanel);
            }
        }

        protected override void OnHide()
        {
            this.OnDelectModel();
        }

        void Set(int itemIndex)
        {
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;
            NetProto.ItemGrid itemGrid = packageMgr.GetItemInfo(itemIndex);
            if (itemGrid == null)
                return;
            if (itemGrid.data.petattribute == null)
                return;
            PetsAttribute petAttribute = itemGrid.data.petattribute;
            this.Set(petAttribute);
        }

        void Set(PetsAttribute petAttribute)
        {
            PetObj petobj = new PetObj();
            if (!petobj.Load(petAttribute))
                return;

            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;

            //info面板
            Config.PetAttribute property = Config.PetAttribute.Get(petobj.id);
            m_PetName.text = petAttribute.nick_name;
            m_PetLevel.text = petAttribute.lv.ToString() + " 级";
            m_VariationObj.SetActive(property.variation == 1);
            m_PetQuality.CurrentState = property.type;
            //基础信息
            m_BaseInfoRoot.Find("XingGe").GetComponent<Text>().text = petsMgr.GetPersonality(petAttribute.personality);
            m_BaseInfoRoot.Find("ChangZhang").GetComponent<Text>().text = petAttribute.property_grow.ToString();
            m_BaseInfoRoot.Find("PingFe").GetComponent<Text>().text = 0.ToString();// petAttribute.GetScore().ToString();
            m_BaseInfoRoot.Find("Icon").GetComponent<StateRoot>().CurrentState = property.fiveElements - 1;
            //资质
            int index = 0;
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iStrength).name, petobj.power_qualification, property.grow_pow_max, property.grow_pow_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iIntelligence).name, petobj.intelligence_qualification, property.grow_intelligence_max, property.grow_intelligence_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iBone).name, petobj.root_bone_qualification, property.grow_bone_max, property.grow_bone_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iPhysique).name, petobj.bodies_qualification, property.grow_bodies_max, property.grow_bodies_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iAgility).name, petobj.agile_qualification, property.grow_agile_max, property.grow_agile_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iBodyway).name, petobj.body_position_qualification, property.grow_bodyposition_max, property.grow_bodyposition_min, ref index);
            //道具
            index = 0;
            Dictionary<int, PetUseItemData> tempItemUseArr = petAttribute.use_item_list;
            foreach (PetUseItemData item in tempItemUseArr.Values)
                this.SetItemInfo(m_ItemInfoRoot, item.itemid, item.usetimes, ref index);
            for (int i = index; i < m_ItemInfoRoot.childCount; i++)
            {
                m_ItemInfoRoot.GetChild(index).gameObject.SetActive(false);
            }
            //技能
            index = 0;
            this.SetSkillData(m_SkillRoot.GetChild(index), petAttribute.trick_skills, 0);
            this.SetSkillData(m_SkillRoot.GetChild(index), petAttribute.talent_skills, 3);
            for (int i = 0; i < petAttribute.passive_skills.Count; i++, index++)
                this.SetSkillData(m_SkillRoot.GetChild(index), petAttribute.passive_skills[i], 2);
            #region 模型
            if (Config.RoleDefine.GetAll().ContainsKey(property.identity))
            {
                if (m_Rtt.modelId != property.identity)
                {
                    this.ActiveModel();
                    m_Rtt.SetModel(property.identity, (go) => { m_Rtt.SetCameraState(property.camView, new Vector3(property.camPos[0], property.camPos[1], property.camPos[2])); });
                }
            }
            else
            {
                this.UnActiveModel();
            }
            #endregion
        }
        void SetGrownAttribute(Transform nRoot, string attri, int value, int maxvalue, int minvalue, ref int index)
        {
            float growthValue = 0.2f + 0.8f * (float)(value - minvalue) / (float)(maxvalue - minvalue);
            growthValue = growthValue >= 1 ? 1 : growthValue;
            nRoot.GetChild(index).Find("attr").GetComponent<Text>().text = attri + "资质";
            nRoot.GetChild(index).Find("value").GetComponent<Text>().text = value.ToString();
            nRoot.GetChild(index).Find("Exp").GetComponent<RectTransform>().sizeDelta = new Vector2(growthValue * maxValue, 32.0f);

            nRoot.GetChild(index).GetComponent<StateRoot>().CurrentState = growthValue == 1 ? 1 : 0;
            nRoot.GetChild(index).Find("Exp/Light").gameObject.SetActive(growthValue != 1);
            index++;
        }
        void SetItemInfo(Transform nRoot, int itemId, int itemUseCount, ref int index)
        {
            Config.Item itemData = Config.Item.Get(itemId);
            nRoot.Find("Name").GetComponent<Text>().text = itemData.name;
            nRoot.Find("Text").GetComponent<Text>().text = itemUseCount + "/" + itemData.limitNum;
            index += 1;
        }
        void SetSkillData(Transform root, PetSkillData data, int currectIndex)
        {
            if (currectIndex == -1)
            {
                root.GetComponent<StateRoot>().CurrentState = 0;
                return;
            }
            root.GetComponent<StateRoot>().CurrentState = 0;
            root.GetComponent<StateRoot>().CurrentState = 1;// currectIndex;
            //判断技能类型
            Config.SkillConfig skillData = null;
            if (Config.SkillConfig.GetAll().ContainsKey(data.id))
                skillData = Config.SkillConfig.Get(data.id);
            Config.PassiveSkills pSkillData = null;
            if (Config.PassiveSkills.GetAll().ContainsKey(data.id))
                pSkillData = Config.PassiveSkills.Get(data.id);
            if (skillData == null && pSkillData == null)
                return;
            int curIndex = 0;
            if (skillData != null)
            {
                if (!skillData.isPetStunt)
                    curIndex = 1;
                else 
                    curIndex = 0;
            }
            if (pSkillData != null)
            {
                if (pSkillData.type == (int)Config.PassiveSkills.Type.Talent)
                    curIndex = 3;
                else if (pSkillData.type == (int)Config.PassiveSkills.Type.Pets)
                    curIndex = 2;
            }
            root.Find("Tag").gameObject.SetActive(true);
            root.Find("Lock").gameObject.SetActive(data.islock == 1);
            if (curIndex == 1 || curIndex == 2)
                root.Find("Tag").gameObject.SetActive(false);
            else
                root.Find("Tag").GetComponent<StateRoot>().CurrentState = curIndex;
            Helper.SetSprite(root.Find("Icon").GetComponent<Image>(), Config.Item.Get(data.id).icon);
            //添加技能tips事件
            int skillIdentity = data.id;
            int skillState = data.islock;
            root.GetComponent<Button>().onClick.AddListener(() => { this.SkillTipsEvent(skillIdentity); });
        }

        void SkillTipsEvent(int id)
        {
            m_SkillTips.Set(id);
        }

        #region 模型
        void OnCreateModel()
        {
            m_Rtt = new RTTModelPartHandler("RTTModelPart", m_RawImage.GetComponent<RectTransform>(), "", true, new Vector3(1000, 1000, 0),
               () =>
               {
                   m_Rtt.SetModelRotate(new Vector3(0.0f, 150.0f, 0.0f));
               });
        }
        void OnDelectModel()
        {
            if (m_Rtt != null) m_Rtt.Destroy();
            m_Rtt = null;
            m_AttributeIndex = -1;
        }
        void ActiveModel()
        {
            m_Rtt.SetRenderActive(true);
        }
        void UnActiveModel()
        {
            m_Rtt.SetRenderActive(false);
            m_AttributeIndex = -1;
        }
        #endregion
    }
}

#endif
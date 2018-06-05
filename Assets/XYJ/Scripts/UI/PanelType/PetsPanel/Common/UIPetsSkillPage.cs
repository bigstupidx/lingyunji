#if !USE_HOT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using xys.UI.State;
using xys.UI;
using xys.UI.Dialog;

namespace xys.hot.UI
{
    [AutoILMono]
    class UIPetsSkillPage
    {
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        int maxValue = 136;
        [SerializeField]
        Transform m_Material;
        [SerializeField]
        Text m_Copper;
        [SerializeField]
        Text m_GrowthValue;
        [SerializeField]
        Text m_GrowthText;
        [SerializeField]
        Transform m_AttributeRoot;
        [SerializeField]
        Transform m_Grid;

        [SerializeField]
        Transform m_GrowthRoot;
        [SerializeField]
        Transform m_TipsRoot;
        [SerializeField]
        protected Transform m_Tips;

        [SerializeField]
        Button m_TipsBtn;
        [SerializeField]
        Button m_WashBtn;

        int m_SelectedPet;
        int m_SelectedSkill = -1;

        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }
        void OnEnable()
        {
            for (int i = 0; i < m_Grid.childCount - 1; i++)
            {
                int index = i;
                m_Grid.GetChild(i).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.OnSelectSkill(index);
                });
            }

            m_TipsBtn.onClick.AddListener(this.ShowTipsPanel);
            m_WashBtn.onClick.AddListener(this.OnWashPet);
        }
        void OnDisable()
        {
            for (int i = 0; i < m_Grid.childCount - 1; i++)
            {
                m_Grid.GetChild(i).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            }
            m_TipsBtn.onClick.RemoveAllListeners();
            m_WashBtn.onClick.RemoveAllListeners();
        }
        public void ResetData(int selectIndex)
        {
            PetsMgr petsMgr = m_Panel.petsMgr;

            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;

            if (!petsMgr.CheckIndex(selectIndex) )
                return;
            m_SelectedPet = selectIndex;
            //重置位置
            m_Grid.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            Config.PetAttribute property = Config.PetAttribute.Get(petsMgr.m_PetsTable.attribute[selectIndex].id);
            if (property == null)
                return;

            PetObj petAttribute = m_Panel.selectedPetObj;
            if (petAttribute == null)
                return;
            #region 潜能点设置
            m_GrowthValue.text = (petAttribute.growRate*0.001f).ToString("#0.000");
            int selectedIndex = 0;
            int growthValue = (int)(((petAttribute.growRate*0.001f - property.grow_rate_min) / (property.grow_rate_max - property.grow_rate_min)) * 100);
            for (int i = 0; i < Config.kvClient.petsLevelDefine.Count; i++, selectedIndex++)
            {
                if (Config.kvClient.petsLevelDefine[i] >= growthValue)
                    break;
            }
            m_GrowthText.text = "【" + Config.kvClient.petsLevelTextDefine[selectedIndex] + "】";
            m_GrowthText.color = XTools.Utility.ParseColor(Config.kvClient.petsColorDefine[selectedIndex], 0);

            int index = 0;
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iStrength).name, petAttribute.power_qualification, property.grow_pow_max, property.grow_pow_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iIntelligence).name,petAttribute.intelligence_qualification, property.grow_intelligence_max, property.grow_intelligence_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iBone).name, petAttribute.root_bone_qualification, property.grow_bone_max, property.grow_bone_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iPhysique).name, petAttribute.bodies_qualification, property.grow_bodies_max, property.grow_bodies_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iAgility).name, petAttribute.agile_qualification, property.grow_agile_max, property.grow_agile_min, ref index);
            SetGrownAttribute(m_AttributeRoot, Config.AttributeDefine.Get(Config.AttributeDefine.iBodyway).name, petAttribute.body_position_qualification, property.grow_bodyposition_max, property.grow_bodyposition_min, ref index);

            //设置材料ID
            int itemId = property.wash_item_id;
            int needNum = property.wash_item_num;
            int hasNum = packageMgr.GetItemCount(itemId);
            m_Material.Find("Name").GetComponent<Text>().text = Config.Item.Get(itemId).name;
            Helper.SetSprite(m_Material.Find("Bg/Icon").GetComponent<Image>(), Config.Item.Get(itemId).icon);
            Helper.SetSprite(m_Material.Find("Bg/Quality").GetComponent<Image>(), petsMgr.GetQualityBg((int)Config.Item.Get(itemId).quality));
            Text text = m_Material.Find("Text").GetComponent<Text>();
            if (hasNum < needNum)
                text.text = string.Format("<color=#{0}>", "ef3c49") + hasNum + "/" + needNum + "</color>";
            else
                text.text = string.Format("<color=#{0}>", "61e171") + hasNum + "/" + needNum + "</color>";
            //价格
            int nCopper = 0;// property.wash_gold_num;
            text = this.m_Transform.Find("copper").GetComponent<Text>();
            text.text = nCopper.ToString();
            #endregion
            #region 技能设置
            index = 0;
            UIpetsSkillItem item = (UIpetsSkillItem)m_Grid.GetChild(index).GetComponent<ILMonoBehaviour>().GetObject();
            item.SetData(petAttribute.trickSkills.id, UIPetsSkillItem_Type.TrickSlot, petAttribute.trickSkills.islock, ref index);
            item.callBack = this.ShowTips;

            item = (UIpetsSkillItem)m_Grid.GetChild(index).GetComponent<ILMonoBehaviour>().GetObject();
            item.SetData(petAttribute.talentSkills.id, UIPetsSkillItem_Type.TalentSlot, petAttribute.talentSkills.islock, ref index);
            item.callBack = this.ShowTips;

            for (int i = 0; i < petAttribute.passiveSkills.Count; i++)
            {
                item = (UIpetsSkillItem)m_Grid.GetChild(index).GetComponent<ILMonoBehaviour>().GetObject();
                item.SetData(petAttribute.passiveSkills[i].id, UIPetsSkillItem_Type.PassivasSlot, petAttribute.passiveSkills[i].islock, ref index);
                item.callBack = this.ShowTips;
            }

            for (; index < m_Grid.childCount - 1;)
            {
                item = (UIpetsSkillItem)m_Grid.GetChild(index).GetComponent<ILMonoBehaviour>().GetObject();
                item.SetData(0, UIPetsSkillItem_Type.None, 0, ref index);
                item.callBack = null;
                index += 1;
            }
            #endregion

        }
        #region 内部UI逻辑
        void ShowTips(int skillId)
        {
            if(skillId > 0)
                m_Panel.ShowSkillTips(skillId);
        }
        void SetGrownAttribute(Transform nRoot, string attri, int value, int maxvalue, int minvalue, ref int index)
        {
            float growthValue = 0.2f + 0.8f * (float)(value - minvalue) / (float)(maxvalue - minvalue);
            growthValue = growthValue >= 1 ? 1 : growthValue;
            nRoot.GetChild(index).Find("attr").GetComponent<Text>().text = attri + "资质";
            nRoot.GetChild(index).Find("value").GetComponent<Text>().text = value.ToString() /*+ "/" + maxvalue.ToString()*/;
            float res = growthValue * maxValue;
            if (res < 0) value = 0;
            if (res > maxValue) res = maxValue;
            nRoot.GetChild(index).Find("Exp").GetComponent<RectTransform>().sizeDelta = new Vector2(res, 32.0f);// (float)(maxvalue - value) / (float)(maxvalue - minvalue);

            nRoot.GetChild(index).GetComponent<StateRoot>().CurrentState = growthValue == 1 ? 1 : 0;
            nRoot.GetChild(index).Find("Exp/Light").gameObject.SetActive(growthValue != 1);
            index++;
        }
        string GetSkillTagBg(int quality)
        {
            string bg = "ui_pet_Icon_10";
            switch (quality)
            {
                case 11:
                    bg = "ui_pet_Icon_8";
                    break;
                case 1:
                    bg = "ui_pet_Icon_9";
                    break;
                case 2:
                    bg = "ui_pet_Icon_10";
                    break;
            }
            return bg;
        }
        #endregion
        #region 事件
        //打开技能书
        public void OnOpenSkillBook()
        {
            //m_SkillBookRoot.SetActive(!m_SkillBook.isShow);
            //m_SkillBook.EnableWindow(!m_SkillBook.isShow);
        }
        void OnSelectSkill(int index)
        {
            if (index >= m_Grid.childCount)
                return;
            m_SelectedSkill = index;
        }
        //洗炼宠物
        void OnWashPet()
        {
            if (m_SelectedPet == -1)
                return;
            PackageMgr pmMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            PetsMgr petsMgr = m_Panel.petsMgr;
            if (petsMgr == null || pmMgr == null)
                return;

            Config.PetAttribute property = Config.PetAttribute.Get(petsMgr.m_PetsTable.attribute[m_Panel.selected].id);
            if (property == null)
                return;

            int numItem = pmMgr.GetItemCount(property.wash_item_id);
            if (numItem < property.wash_item_num)
            {
                Config.Item data = Config.Item.Get(property.wash_item_id);
                //UIHintManage.Instance.ShowHint(string.Format("材料{0}不足", data != null ? data.name : ""));

                SystemHintMgr.ShowHint(string.Format("材料{0}不足", data != null ? data.name : ""));
                return;
            }

            //             CommonTipsParam param = new CommonTipsParam();
            //             param.text2 = "洗炼后 该灵兽的资质 成长与技能均会重置 灵兽等级变为1级 是否继续？";
            //             param.rightAction = () =>
            //             {
                                //SystemHintMgr.ShowHint(洗炼成功);
            //                 Utils.EventDispatcher.Instance.TriggerEvent<int>(PetsSystem.Event.WashPet, m_SelectedPet);
            //             };
            //             param.promptKey = PetsSystem.m_PetWashKey;
            //             UICommonPannel.ShowCommonTip(param);

            //test
            App.my.eventSet.FireEvent<int>(EventID.Pets_Wash, m_Panel.selected);
        }

        void ShowTipsPanel()
        {
            if (m_Panel.selected == -1)
                return;
            PetsMgr petsMgr = m_Panel.petsMgr;
            Config.PetAttribute property = Config.PetAttribute.Get(petsMgr.m_PetsTable.attribute[m_Panel.selected].id);
            if (property == null)
                return;

            int itemId = property.wash_item_id;
            m_Panel.ShowTipsPanel(itemId);
        }

        public void ShowThirdAttributeTips(int index)
        {
            string content = string.Empty;
            Transform root = m_TipsRoot;
            switch (index)
            {
                case 6:
                    content = Config.kvClient.Get("PetGrowth").value;
                    root = m_GrowthRoot;
                    index = 0;
                    break;
                case 0:
                    content = Config.kvClient.Get("PetTalent_Strength").value;
                    break;
                case 1:
                    content = Config.kvClient.Get("PetTalent_Intelligence").value;
                    break;
                case 2:
                    content = Config.kvClient.Get("PetTalent_Bone").value;
                    break;
                case 3:
                    content = Config.kvClient.Get("PetTalent_Physique").value;
                    break;
                case 4:
                    content = Config.kvClient.Get("PetTalent_Agility").value;
                    break;
                case 5:
                    content = Config.kvClient.Get("PetTalent_Bodyway").value;
                    break;
            }
            if (content == string.Empty)
                return;
            m_Tips.transform.SetParent(root.GetChild(index));
            m_Tips.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            m_Tips.GetComponentInChildren<Text>().text = GlobalSymbol.ToUT(content).Replace("\\n", "\n");
            m_Panel.parent.StartCoroutine(TipsAutoSize());
        }
        IEnumerator TipsAutoSize()
        {
            yield return new WaitForEndOfFrame();
            m_Tips.gameObject.SetActive(true);
            m_Tips.GetComponentInChildren<xys.UI.ContentSizeFitter>().SetDirty();
        }
        #endregion
    }
}


#endif
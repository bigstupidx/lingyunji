#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using xys.UI.State;
using UnityEngine.UI;
using System.Collections.Generic;
using NetProto;
using NetProto.Hot;

namespace xys.hot.UI
{
    [System.Serializable]
    public class PetsQualificationsData
    {
        public int itemId;
        public int petIndex;
    }

    class UIPetsQualificationsPanel : HotPanelBase
    {
        [SerializeField]
        List<string> m_cText = new List<string>() { "需达到", "需低于", "提升上限", "已满" };
        [SerializeField]
        Transform m_ItemGrid;

        [SerializeField]
        Text m_PetNameText;
        [SerializeField]
        Text m_PetLevelText;
        [SerializeField]
        Image m_MaterialImage;
        [SerializeField]
        Image m_MaterialQualityImage;
        [SerializeField]
        Text m_MaterialText;
        [SerializeField]
        Text m_MaxUseCountText;
        [SerializeField]
        Button m_Button;
        [Header("进度条最大长度")]
        [SerializeField]
        float m_MaxLen = 370.0f;

        int m_PetIndex;
        int m_ItemID;
        int m_SelectedIndex = -1;

        UIHotPanel m_Parent;

        PetObj m_PetObj = new PetObj();
        public PetObj selectedPetObj { get { return m_PetObj; } }
        public UIPetsQualificationsPanel() : base(null) { }
        public UIPetsQualificationsPanel(UIHotPanel parent) : base(parent)
        {

        }

        protected override void OnInit()
        {
            this.m_Button.onClick.AddListener(this.UseEvent);
            for (int i = 0; i < m_ItemGrid.childCount; i++)
            {
                int index = i;
                m_ItemGrid.GetChild(i).GetComponent<Button>().onClick.AddListener(() => { this.ClickQualificationsEvent(index); });
            }

            this.SetItem(m_ItemGrid.GetChild(0), Config.AttributeDefine.Get(Config.AttributeDefine.iStrength).name);
            this.SetItem(m_ItemGrid.GetChild(1), Config.AttributeDefine.Get(Config.AttributeDefine.iIntelligence).name);
            this.SetItem(m_ItemGrid.GetChild(2), Config.AttributeDefine.Get(Config.AttributeDefine.iBone).name);
            this.SetItem(m_ItemGrid.GetChild(3), Config.AttributeDefine.Get(Config.AttributeDefine.iPhysique).name);
            this.SetItem(m_ItemGrid.GetChild(4), Config.AttributeDefine.Get(Config.AttributeDefine.iAgility).name);
            this.SetItem(m_ItemGrid.GetChild(5), Config.AttributeDefine.Get(Config.AttributeDefine.iBodyway).name);

        }

        void OnDisable()
        {
            if (m_SelectedIndex != -1)
            {
                m_ItemGrid.GetChild(m_SelectedIndex).Find("bg1").gameObject.SetActive(false);
                m_ItemGrid.GetChild(m_SelectedIndex).Find("bg2").gameObject.SetActive(false);
            }
            m_SelectedIndex = -1;
        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            PetsQualificationsData data = args as PetsQualificationsData;

            int petIndex = data.petIndex;
            int itemId = data.itemId;
            if (petIndex == -1 || itemId == 0)
                return;

            this.Set(petIndex, itemId);
            this.m_Parent.Event.Subscribe(EventID.Pets_RefreshUI, this.Reset);
            this.m_Parent.Event.Subscribe(EventID.Package_UpdatePackage, this.Reset);
        }

        void Set(int petIndex, int itemId)
        {
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;
            Config.Item itemdata = Config.Item.Get(itemId);
            if (itemdata == null)
                return;

            PetsAttribute attribute = petsMgr.m_PetsTable.attribute[petIndex];
            Config.PetAttribute property = Config.PetAttribute.Get(attribute.id);
            if (property == null)
                return;
            int petItemID = property.pet2item;
            if (petItemID == -1)
                return;
            int petQuality = property.type;
            if (petQuality == -1)
                return;
            m_PetIndex = petIndex;
            m_ItemID = itemId;

            this.CreatePetobj();

            int index = 0;
            this.SetItemData(m_ItemGrid.GetChild(index), m_PetObj.basepower_qualification, property.grow_pow_min, property.grow_pow_max, itemdata);
            index++;
            this.SetItemData(m_ItemGrid.GetChild(index), m_PetObj.baseintelligence_qualification, property.grow_intelligence_min, property.grow_intelligence_max, itemdata);
            index++;
            this.SetItemData(m_ItemGrid.GetChild(index), m_PetObj.baseroot_bone_qualification, property.grow_bone_min, property.grow_bone_max, itemdata);
            index++;
            this.SetItemData(m_ItemGrid.GetChild(index), m_PetObj.basebodies_qualification, property.grow_bodies_min, property.grow_bodies_max, itemdata);
            index++;
            this.SetItemData(m_ItemGrid.GetChild(index), m_PetObj.baseagile_qualification, property.grow_agile_min, property.grow_agile_max, itemdata);
            index++;
            this.SetItemData(m_ItemGrid.GetChild(index), m_PetObj.basebody_position_qualification, property.grow_bodyposition_min, property.grow_bodyposition_max, itemdata);

            m_PetNameText.text = m_PetObj.nickName;
            string colorstr = Config.QualitySourceConfig.Get((Config.ItemQuality)petQuality).color;
            m_PetNameText.color = ColorUT.ToColor(colorstr);
            m_PetLevelText.text = "" + m_PetObj.lv;

            Helper.SetSprite(m_MaterialImage, itemdata.icon);
            Helper.SetSprite(m_MaterialQualityImage, Config.QualitySourceConfig.Get(itemdata.quality).icon);
            int itemCount = packageMgr.GetItemCount(itemId);
            if (itemCount == 0)
                m_MaterialText.text = string.Format("<color=#{0}>{1}/{2}</color>", "ef3c49", itemCount, 1);
            else
                m_MaterialText.text = itemCount + "/" + 1;

            if (itemdata.limitNum != 0 )
            {
                m_MaxUseCountText.gameObject.SetActive(true);
                int useItems = attribute.use_item_list.ContainsKey(itemId) ? attribute.use_item_list[itemId].usetimes : 0;
                m_MaxUseCountText.text = "<color=#B3C1D8FF>剩余使用次数： </color>" + (itemdata.limitNum - useItems);
            }
            else
                m_MaxUseCountText.gameObject.SetActive(false);

            if (m_SelectedIndex == -1)
                this.ClickQualificationsEvent(0);
        }

        void SetItem(Transform root, string attName)
        {
            root.Find("Name").GetComponent<Text>().text = attName;
        }

        void SetItemData(Transform root, int petAttributeValue, int minValue, int maxValue, Config.Item itemData)
        {
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;
            float barValue = (float)petAttributeValue / (float)maxValue;
            //bar
            root.Find("Amount").GetComponent<RectTransform>().sizeDelta = new Vector2(barValue > 1.0f ? m_MaxLen : barValue * m_MaxLen, 34.0f);
            root.Find("Amount/Light").gameObject.SetActive(barValue > 1 ? false : true);

            string sCondition = string.Empty;
            string conditionValue = string.Empty;
            if (itemData.qualMax == 0 && itemData.qualMin > petAttributeValue)
            {
                sCondition = string.Format("<color=#{0}>", "ef3c49") + m_cText[0] + "</color>";
                conditionValue = itemData.qualMin.ToString();
            }
            else if (itemData.qualMin == 0 && petAttributeValue > itemData.qualMax)
            {
                sCondition = string.Format("<color=#{0}>", "ef3c49") + m_cText[1] + "</color>";
                conditionValue = itemData.qualMax.ToString();
            }
            else if (itemData.qualMin <= petAttributeValue && petAttributeValue <= itemData.qualMax)
            {
                sCondition = m_cText[2];
                conditionValue = itemData.qualMax.ToString();
            }

            if (petAttributeValue >= maxValue)
            {
                conditionValue = string.Format("<color=#{0}>", "ef3c49") + m_cText[3] + "</color>";
                sCondition = string.Empty;
            }

            root.Find("Condition").GetComponent<Text>().text = sCondition;
            root.Find("Condition/ConditionText").GetComponent<Text>().text = "" + conditionValue;

            if (petAttributeValue >= maxValue)
                root.Find("Value").GetComponent<Text>().text = petAttributeValue + "/" + maxValue;
            else
                root.Find("Value").GetComponent<Text>().text = petAttributeValue + string.Format("<color=#{0}>(+{1}~{2})</color>", "61e171", itemData.qualMax, itemData.qualMax) + "/" + maxValue;
        }

        void UseEvent()
        {
            if (m_PetObj == null)
                return;
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (petsMgr == null)
                return;
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;
            if (packageMgr.GetItemCount(m_ItemID) <= 0)
            {
                //UIHintManage.Instance.ShowHint("打开快捷商店");
                return;
            }
            if (m_SelectedIndex == -1)
            {
                SystemHintMgr.ShowHint("请先选择需要提升的资质");
                return;
            }

            Config.Item itemData = Config.Item.Get(m_ItemID);
            if (itemData == null)
                return;

            if (itemData.limitNum != 0 && !petsMgr.CheckPetsUseItem(m_PetIndex, m_ItemID, itemData.limitNum))
            {
                SystemHintMgr.ShowHint("该灵兽的培养次数已满 无法继续领悟");
                return;
            }
            PetsAttribute attribute = petsMgr.m_PetsTable.attribute[m_PetIndex];
            Config.PetAttribute property = Config.PetAttribute.Get(attribute.id);
            if (property == null)
                return;
            int petItemID = property.pet2item;
            if (petItemID == -1)
                return;
            
            int qualificationRequestMax = itemData.petQuali[1];
            int qualificationRequestMin = itemData.petQuali[0];
            int qualificationMax = 0;
            int qualificationValue = 0;
            switch ((PetsDefines)m_SelectedIndex)
            {
                case PetsDefines.power:
                    qualificationMax = property.grow_pow_max;
                    qualificationValue = m_PetObj.basepower_qualification;
                    break;
                case PetsDefines.intelligence:
                    qualificationMax = property.grow_intelligence_max;
                    qualificationValue = m_PetObj.baseintelligence_qualification;
                    break;
                case PetsDefines.root:
                    qualificationMax = property.grow_bone_max;
                    qualificationValue = m_PetObj.baseroot_bone_qualification;
                    break;
                case PetsDefines.bodies:
                    qualificationMax = property.grow_bodies_max;
                    qualificationValue = m_PetObj.basebodies_qualification;
                    break;
                case PetsDefines.agile:
                    qualificationMax = property.grow_agile_max;
                    qualificationValue = m_PetObj.baseagile_qualification;
                    break;
                case PetsDefines.bodyposition:
                    qualificationMax = property.grow_bodyposition_max;
                    qualificationValue = m_PetObj.basebody_position_qualification;
                    break;
            }

            if (qualificationRequestMax == 0 && qualificationRequestMin > qualificationValue)
            {
                SystemHintMgr.ShowHint("该项资质需高于" + qualificationRequestMin);
                return;
            }
            else if (qualificationRequestMin == 0 && qualificationRequestMax < qualificationValue)
            {
                SystemHintMgr.ShowHint("该项资质需低于" + qualificationRequestMax);
                return;
            }
            else if (qualificationRequestMin > qualificationValue && qualificationRequestMax < qualificationValue)
            {
                SystemHintMgr.ShowHint("该项资质提升范围");
                return;
            }

            if (qualificationValue >= qualificationMax)
            {
                SystemHintMgr.ShowHint("当前资质已满 无需提升");
                return;
            }

            //发送信息
            PetQualificationRequest request = new PetQualificationRequest();
            request.index = m_PetIndex;
            request.itemId = m_ItemID;
            request.attributeIndex = m_SelectedIndex;
            App.my.eventSet.FireEvent(EventID.Pets_SetQualification, request);
        }

        void ClickQualificationsEvent(int index)
        {
            if (m_SelectedIndex == index)
                return;
            if (m_SelectedIndex != -1)
            {
                m_ItemGrid.GetChild(m_SelectedIndex).Find("bg1").gameObject.SetActive(false);
                m_ItemGrid.GetChild(m_SelectedIndex).Find("bg2").gameObject.SetActive(false);
            }

            m_SelectedIndex = index;

            if (m_SelectedIndex != -1)
            {
                m_ItemGrid.GetChild(m_SelectedIndex).Find("bg1").gameObject.SetActive(true);
                m_ItemGrid.GetChild(m_SelectedIndex).Find("bg2").gameObject.SetActive(true);
            }
        }

        void Reset()
        {
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;

            int itemCount = packageMgr.GetItemCount(m_ItemID);
            if (itemCount == 0)
                m_MaterialText.text = string.Format("<color=#{0}>{1}/{2}</color>", "ef3c49", itemCount, 1);
            else
                m_MaterialText.text = itemCount + "/" + 1;


            this.Set(m_PetIndex, m_ItemID);
        }

        void CreatePetobj()
        {
            PetsMgr petsMgr = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (!petsMgr.CheckIndex(m_PetIndex))
                return;

            m_PetObj.Load(petsMgr.m_PetsTable.attribute[m_PetIndex]);
        }
    }
}

#endif
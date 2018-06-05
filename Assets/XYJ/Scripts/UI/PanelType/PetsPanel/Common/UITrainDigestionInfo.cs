#if !USE_HOT
using NetProto;
using NetProto.Hot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    [System.Serializable]
    class UITrainDigestionInfo
    {
        [SerializeField]
        Image m_Icon;
        [SerializeField]
        Image m_QualityIcon;
        [SerializeField]
        Text m_Count;
        [SerializeField]
        Text m_ItemName;
        [SerializeField]
        Text m_ItemDes;
        [SerializeField]
        StateRoot m_ItemState;

        [Header("StateRoot_1")]
        [SerializeField]
        Text m_ExpValueText;
        [SerializeField]
        Text m_ExpLevelTex;
        [SerializeField]
        Text m_ExpText;
        [SerializeField]
        Image m_ExpIcon;
        [SerializeField]
        Button m_OneUseBtn;
        [SerializeField]
        Button m_TenUseBtn;

        [Header("StateRoot_2")]
        [SerializeField]
        Button m_UseBtn;
        [SerializeField]
        Text m_UseCount;
        [SerializeField]
        Text m_Growthvalue;
        [SerializeField]
        Text m_UseValue;
        [SerializeField]
        Text m_QualityText;

        [Header("StateRoot_3")]
        [SerializeField]
        Text m_UseDes;
        [SerializeField]
        Text m_UseCount_3;

        [Header("StateRoot_4")]
        [SerializeField]
        Text m_PersonalityDes;

        [Header("StateRoot_5")]
        [SerializeField]
        Text m_NowTalentvalue;
        [SerializeField]
        Text m_UpTalentValue;
        [SerializeField]
        Text m_TalentCount;
        [SerializeField]
        float m_MaxExpLen = 470.0f;

        int m_PetIndex;
        int m_ItemID;

        PetsPanel m_Panel;
        public PetsPanel panel { set { m_Panel = value; } }

        public void OnInit()
        {
            m_OneUseBtn.onClick.AddListener(this.OneUseEvent);
            m_TenUseBtn.onClick.AddListener(this.TenUseEvent);
            m_UseBtn.onClick.AddListener(this.OneUseEvent);
        }
        #region 事件

        void OneUseEvent()
        {
            PetObj petobj = m_Panel.selectedPetObj;
            if (petobj == null)
                return;

            Config.Item itemData = Config.Item.Get(m_ItemID);
            if (itemData == null)
                return;

            if (itemData.sonType == (int)Config.ItemChildType.petExpDrug)
            {
                this.OnItemExp(1);
            }
            else if (itemData.sonType == (int)Config.ItemChildType.petTrainItem)
            {
                if (itemData.qualMin != 0)//走资质界面逻辑
                    this.ShowPanel();
                else if (itemData.growMin > 0)
                    this.OnItemGrowth(itemData, petobj);
                //Utils.EventDispatcher.Instance.TriggerEvent<int, int>(PetsSystem.Event.SetGrowth, m_PetIndex, m_ItemID);
                else if (itemData.perceImpro.list.Count > 0)
                    this.OnItemSavvy(itemData, petobj);
                // Utils.EventDispatcher.Instance.TriggerEvent<int, int>(PetsSystem.Event.SetSavvy, m_PetIndex, m_ItemID);
            }
            else if (itemData.sonType == (int)Config.ItemChildType.petPersonalityResetItem)
                this.OnItemPersonality(itemData);
            //Utils.EventDispatcher.Instance.TriggerEvent<int, int>(PetsSystem.Event.SetPersonality, m_PetIndex, m_ItemID);
        }

        void TenUseEvent()
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null || this.m_Panel.selectedPetObj == null)
                return;
            Config.Item itemData = Config.Item.Get(m_ItemID);
            if (itemData == null)
                return;

            if (itemData.limitNum != 0 && !pm.CheckPetsUseItem(m_PetIndex, m_ItemID, itemData.limitNum))
            {
                SystemHintMgr.ShowHint("道具使用次数已达上限");
                return;
            }

            if (itemData.sonType == (int)Config.ItemChildType.petExpDrug)
                this.OnItemExp(10);
        }
        //性格反馈
        void OnItemPersonality(Config.Item itemData)
        {
            if (hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemData.id) <= 0)
            {
               // SystemHintMgr.ShowHint("快捷购买窗口");
                return;
            }
            PetItemRequest request = new PetItemRequest();
            request.index = m_PetIndex;
            request.itemId = itemData.id;
            App.my.eventSet.FireEvent<PetItemRequest>(EventID.Pets_SetPersonality, request);
        }
        //悟性反馈
        void OnItemSavvy(Config.Item itemData, PetObj petobj)
        {
            if (hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemData.id) <= 0)
            {
                //SystemHintMgr.ShowHint("快捷购买窗口");
                return;
            }
            Config.PetAttribute property = Config.PetAttribute.Get(petobj.id);
            if (property == null)
                return;

            //成长值上限判断
            float savvyValue = petobj.savvy;
            int savvyMax = -1;
            if (!int.TryParse(Config.kvCommon.Get("PetSavvyMax").value, out savvyMax))
                return;
            if (savvyValue >= savvyMax)
            {
                SystemHintMgr.ShowHint("悟性值已达到上限 无法继续领悟");
                return;
            }
            PetItemRequest request = new PetItemRequest();
            request.index = m_PetIndex;
            request.itemId = itemData.id;
            App.my.eventSet.FireEvent<PetItemRequest>(EventID.Pets_SetPavvy, request);
        }
        //成长反馈
        void OnItemGrowth(Config.Item itemData, PetObj petobj)
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null)
                return;
            if (hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(itemData.id) <= 0)
            {
                //UIHintManage.Instance.ShowHint("快捷购买窗口");
                return;
            }
            if (itemData.limitNum != 0 && !pm.CheckPetsUseItem(m_PetIndex, m_ItemID,itemData.limitNum))
            {
                SystemHintMgr.ShowHint("该灵兽的培养次数已满 无法继续领悟");
                return;
            }
            Config.PetAttribute property = Config.PetAttribute.Get(petobj.id);
            if (property == null)
                return;
            float growthValue = petobj.growRate;
            float maxGrowthValue = property.grow_rate_max * 1000;
            if (growthValue >= maxGrowthValue)
            {
                SystemHintMgr.ShowHint("成长已满 无法继续领悟");
                return;
            }
            PetItemRequest request = new PetItemRequest();
            request.index = m_PetIndex;
            request.itemId = itemData.id;
            App.my.eventSet.FireEvent< PetItemRequest>(EventID.Pets_SetGrowth, request);
        }
        //经验反馈
        void OnItemExp(int count)
        {
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null)
                return;

            int itemCount = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(m_ItemID);
            if (itemCount == 0)
            {
                //UIHintManage.Instance.ShowHint("快捷购买窗口");
                return;
            }
            //
            if (!pm.CanLvUp(m_PetIndex))
                SystemHintMgr.ShowHint("你的灵兽太饱了 无法继续喂食了！");

            if (count >= itemCount && itemCount > 0)
                count = itemCount;
            PetItemRequest request = new PetItemRequest();
            request.index = m_PetIndex;
            request.itemId = m_ItemID;
            request.itemCount = count;
            App.my.eventSet.FireEvent<PetItemRequest>(EventID.Pets_AddExp,request);
        }

        #endregion
        public void Set(int petIndex, int itemId)
        {
            PackageMgr packageMgr =  hotApp.my.GetModule<HotPackageModule>().packageMgr;
            PetsMgr pm = hotApp.my.GetModule<HotPetsModule>().petsMgr;
            if (pm == null || packageMgr == null || m_Panel.selectedPetObj == null)
                return;

            PetObj petobj = m_Panel.selectedPetObj;
            Config.PetAttribute property = Config.PetAttribute.Get(petobj.id);
            if (property == null)
            {
                Debuger.LogError("宠物ID不存在：" + petobj.id);
                return;
            }
            Config.Item itemData = Config.Item.Get(itemId);
            if (itemData == null)
                return;
            m_PetIndex = petIndex;
            m_ItemID = itemId;

            Helper.SetSprite(m_Icon, itemData.icon);
            Helper.SetSprite(m_QualityIcon, Config.QualitySourceConfig.Get(itemData.quality).icon);
            m_ItemName.text = itemData.name;
            m_ItemDes.text = GlobalSymbol.ToUT(itemData.desc);
            int itemCount = packageMgr.GetItemCount(itemId);
            if (itemCount == 0)
                m_Count.text = string.Format("<color=#{0}>", "ef3c49") + itemCount + "</color>";
            else if (itemCount == 1)
                m_Count.text = string.Empty;
            else
                m_Count.text = itemCount.ToString();

            PetUseItemData useItemData = pm.GetPetsUseItemData(m_PetIndex, m_ItemID);

            //经验
            if (itemData.sonType == (int)Config.ItemChildType.petExpDrug)
            {
                int level = petobj.lv;
                int exp = petobj.exp;
                
                Config.RoleExp upgradeExp = Config.RoleExp.Get(petobj.lv);
                if (upgradeExp != null)
                    m_ExpIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp01(1.0f * petobj.exp / upgradeExp.pet_exp) * m_MaxExpLen, 12.0f);
                else
                    m_ExpIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(m_MaxExpLen, 12.0f);

                m_ExpLevelTex.text = level + "级";
                m_ExpValueText.text = exp + "/" + upgradeExp.pet_exp;

                
                m_ExpText.text = "" + Config.RewardDefine.Get(itemData.rewardId).pet_exp.GetValue(null);
                
                m_ItemState.CurrentState = 0;
            }
            else if (itemData.sonType == (int)Config.ItemChildType.petTrainItem)
            {
                //成长
                if (itemData.growMin > 0)
                {
                    m_Growthvalue.text = (petobj.growRate*0.001f).ToString("#0.000");
                    m_UseValue.text = itemData.growMin + "~" + itemData.growMax;

                    int selectedIndex = 0;
                    int growthValue = (int)(((petobj.growRate * 0.001f - property.grow_rate_min) / (property.grow_rate_max - property.grow_rate_min)) * 100);
                    for (int i = 0; i < Config.kvClient.petsLevelDefine.Count; i++, selectedIndex++)
                    {
                        if (Config.kvClient.petsLevelDefine[i] >= growthValue)
                            break;
                    }
                    m_QualityText.text = "    【" + Config.kvClient.petsLevelTextDefine[selectedIndex] + "】";
                    m_QualityText.color = XTools.Utility.ParseColor(Config.kvClient.petsColorDefine[selectedIndex], 0);

                    m_UseCount.transform.parent.GetComponent<Text>().text = itemData.limitNum != 0 ? "<color=#ACCEF4FF>剩余使用次数 ： </color>" : string.Empty;
                    m_UseCount.text = itemData.limitNum != 0 ? "" + (itemData.limitNum - useItemData.usetimes) : string.Empty;
                    m_ItemState.CurrentState = 1;
                }
                //悟性提升  
                else if (itemData.perceImpro.list.Count > 0)
                {
                    m_NowTalentvalue.text = "" + petobj.savvy;// System.Math.Round(( - 1), 2);
                    if (itemData.perceImpro.list.Count == 1)
                        m_UpTalentValue.text = "" + itemData.perceImpro.list[0].value;
                    else
                        m_UpTalentValue.text = "" + itemData.perceImpro.list[0].value + "~" + itemData.perceImpro.list[itemData.perceImpro.list.Count - 1].value;
                    m_TalentCount.transform.parent.GetComponent<Text>().text = itemData.limitNum != 0 ? "<color=#ACCEF4FF>剩余使用次数 ： </color>" : string.Empty;
                    m_TalentCount.text = itemData.limitNum != 0 ? "" + (itemData.limitNum - useItemData.usetimes) : string.Empty;
                    m_ItemState.CurrentState = 4;
                }
                //资质
                else if (itemData.qualMin != 0)
                {
                    m_UseCount_3.transform.parent.GetComponent<Text>().text = itemData.limitNum != 0 ? "<color=#ACCEF4FF>剩余使用次数 ： </color>" : string.Empty;
                    m_UseCount_3.text = itemData.limitNum != 0 ? "" + (itemData.limitNum - useItemData.usetimes) : string.Empty;
                    m_ItemState.CurrentState = 2;
                }
            }
            //性格
            else if (itemData.sonType == (int)Config.ItemChildType.petPersonalityResetItem)
            {
                m_PersonalityDes.text = pm.CheckIndex(petIndex) ? pm.GetPersonality(pm.m_PetsTable.attribute[petIndex].personality) : string.Empty;
                m_ItemState.CurrentState = 3;
            }

        }

        void ShowPanel()
        {
            PetsQualificationsData data = new PetsQualificationsData();
            data.itemId = m_ItemID;
            data.petIndex = m_PetIndex;
            App.my.uiSystem.ShowPanel(PanelType.UIPetsQualificationsPanel, data);
        }
    }
}

#endif
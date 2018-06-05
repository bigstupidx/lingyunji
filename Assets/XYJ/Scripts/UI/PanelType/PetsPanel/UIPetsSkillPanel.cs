#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using xys.UI.State;
using UnityEngine.UI;
using System.Collections.Generic;
using xys.UI.Dialog;
using NetProto;
using NetProto.Hot;

namespace xys.hot.UI
{
    class UIPetsSkillPanel : HotPanelBase
    {
        [SerializeField]
        UIPetsSkillInfomation m_SkillInfos;
        [SerializeField]
        UIPetsSkillScorllView m_ScrollView;

        [SerializeField]
        Button m_ShowAllSkillsBtn;
        [SerializeField]
        Button m_LearnBtn;
        [SerializeField]
        Button m_LockBtn;

        UIHotPanel m_Parent;

        PetsModule m_Module;
        EventAgent m_EventAgent { get; set; }
        
        PetsPanel m_Panel;

        public UIPetsSkillPanel() :base(null) { }
        public UIPetsSkillPanel(UIHotPanel parent) : base(parent)
        {
            this.m_Parent = parent;
        }

        protected override void OnInit()
        {
            this.m_EventAgent = this.m_Parent.Event;
            this.m_Module = App.my.localPlayer.GetModule<PetsModule>();

            m_ScrollView.OnInit();

            this.m_LearnBtn.onClick.AddListener(this.LearnSkillEvent);
            this.m_LockBtn.onClick.AddListener(this.LockSkillEvent);
            this.m_ShowAllSkillsBtn.onClick.AddListener(() => { m_ScrollView.ShowAll(); });
        }

        protected override void OnShow(object args)
        {
            this.m_Panel = args as PetsPanel;
            if(m_Panel == null)
            {
                Debuger.Log("Panel null");
                this.m_Parent.gameObject.SetActive(false);
                return;
            }

            this.m_SkillInfos.panel = m_Panel;
            this.m_ScrollView.panel = m_Panel;

            m_SkillInfos.OnShow();
            m_ScrollView.OnShow();

            m_SkillInfos.Reset();
            //m_ScrollView.Refresh();

            //this.m_Parent.Event.Subscribe(EventID.Pets_RefreshUI, this.ResetPage);
            this.m_Parent.Event.Subscribe(EventID.Pets_DataRefresh, this.ResetPage);
            this.m_Parent.Event.Subscribe(EventID.Package_UpdatePackage, () =>
            {
                this.m_ScrollView.Refresh();
            });
        }

        protected override void OnHide()
        {
            m_SkillInfos.OnHide();
            m_ScrollView.OnHide();
        }

        void ResetPage()
        {
            //UISystem.Instance.HidePanel("UIItemTipsPanel", false);
            this.HideTips();
            m_ScrollView.Refresh();

            m_SkillInfos.Reset();
            m_SkillInfos.Refresh();
        }

        #region 技能锁定
        void LockSkillEvent()
        {
            if (m_Screen != null)
                return;
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            if (packageMgr == null)
                return;

            if (m_Panel.selectedPetObj == null)
                return;
            PetObj petobj = m_Panel.selectedPetObj;
            int materialID = Config.kvCommon.petLockInfo[petobj.lockNumSKills][0];
            int count = Config.kvCommon.petLockInfo[petobj.lockNumSKills][1];

            if (packageMgr.GetItemCount(materialID) < count)
            {
                SystemHintMgr.ShowHint(string.Format("{0}不足,无法锁定", Config.Item.Get(materialID).name));
                return;
            }

            string des = string.Empty;
            int skillId = m_SkillInfos.skillID;

            #region 主动
            if (Config.SkillConfig.GetAll().ContainsKey(skillId))
            {
                Config.SkillConfig data = Config.SkillConfig.Get(skillId);
                if (data.isPetStunt)
                {
                    SystemHintMgr.ShowHint("绝技无法锁定");
                    return;
                }
               PetSkillData skillInfo = petobj.passiveSkills.Find(delegate (PetSkillData item) { return item.id == skillId; });
                if (skillInfo.id == int.MinValue)
                    return;
                if (skillInfo.islock == 1)
                {
                    SystemHintMgr.ShowHint("该技能已锁定");
                    return;
                }
                else
                {
                    if (petobj.lockNumSKills >= 3)
                    {
                        SystemHintMgr.ShowHint("进行锁定技能，最多同时锁定3个技能");
                        return;
                    }
//                     if (App.my.localPlayer.silverShellValue < petobj.lv * 5 * 1000)
//                     {
//                         SystemHintMgr.ShowHint("金钱不足");
//                         return;
//                     }
                    string needCopper = string.Format("<sprite n={0} w=24 h=24>", "item_copper");
                    string copper = (petobj.lv * 5 * 1000).ToString();
                    des = string.Format("锁定后技能将不可解锁，只有洗炼才会重置技能状态，确定锁定技能吗？", copper, needCopper);
                }
            }
            #endregion
            #region 被动
            if (Config.PassiveSkills.GetAll().ContainsKey(skillId))
            {
                Config.PassiveSkills pData = Config.PassiveSkills.Get(skillId);
                PetSkillData skillInfo = petobj.passiveSkills.Find(delegate (PetSkillData item) { return item.id == skillId; });
                if (skillInfo.id == int.MinValue)
                    return;
                if (skillInfo.islock == 1)
                {
                    SystemHintMgr.ShowHint("该技能已锁定");
                    return;
                }
                else
                {
                    if (petobj.lockNumSKills >= 3)
                    {
                        SystemHintMgr.ShowHint("进行锁定技能，最多同时锁定3个技能");
                        return;
                    }

//                     if (App.my.localPlayer.silverShellValue < petobj.lv * 5 * 1000)
//                     {
//                         SystemHintMgr.ShowHint("金钱不足");
//                         return;
//                     }

                    string needCopper = string.Format("<sprite n={0} w=24 h=24>", "item_copper");
                    string copper = (petobj.lv * 5 * 1000).ToString();
                    des = string.Format("锁定后技能将不可解锁，只有洗炼才会重置技能状态，确定锁定技能吗？", copper, needCopper);
                }
            }
            #endregion
            m_Screen = TwoBtn.Show(
                "",
                des,
                "取消", () =>
                {
                    return false;
                },
                "确定", () =>
                {
                    LockSkillPetRequest request = new LockSkillPetRequest();
                    request.index = m_Panel.selected;
                    request.skillId = skillId;
                    App.my.eventSet.FireEvent(EventID.Pets_LockSkill, request);
                    return false;
                }, true, true, () => { m_Screen = null; });
        }
        #endregion

        #region 技能学习

        TwoBtn m_Screen;
        void LearnSkillEvent()
        {
            this.HideTips();
            if (m_Panel.selectedPetObj == null)
                return;
            if (m_ScrollView.selectedItemID == 0 || !m_ScrollView.selectedActive)
            {
                SystemHintMgr.ShowHint("请选择要学习的技能书");
                return;
            }
            PackageMgr packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            PetsMgr petsMgr = m_Panel.petsMgr;
            if (petsMgr == null || packageMgr == null)
                return;
            //判断是否拥有技能书
            if (packageMgr.GetItemCount(m_ScrollView.selectedItemID) <= 0)
                return;
            //判断技能书类型
            Config.Item itemPrototype = Config.Item.Get(m_ScrollView.selectedItemID);
            int skillId = itemPrototype.petSkill;
            if (!Config.SkillConfig.GetAll().ContainsKey(skillId) && !Config.PassiveSkills.GetAll().ContainsKey(skillId))
                return;

            PetObj petobj = m_Panel.selectedPetObj;
            //判断技能类型
            string des = string.Empty;
            if (itemPrototype.sonType == (int)Config.ItemChildType.petSkillBook)
            {
                for (int i = 0; i < petobj.passiveSkills.Count; i++)
                {
                    //判断是否已学技能
                    if (petobj.passiveSkills[i].id == skillId)
                    {
                        SystemHintMgr.ShowHint("您已有相同的技能，不需重新学习!");
                        return;
                    }
                }
                des = "新技能有几率覆盖未锁定的被动技能，确定学习？";
            }
            else if (itemPrototype.sonType == (int)Config.ItemChildType.petSuperSkill)
            {
                int type = Config.PetAttribute.Get(petobj.id).type;
                //判断宠物类型是否为珍兽或者神兽
                if (type == (int)PetType.PRECIOUS || type == (int)PetType.THERION)
                {
                    //UIHintManage.Instance.ShowHint("宠物类型类型为珍兽或者神兽,无法学习新绝招!");
                    return;
                }
                if (petobj.trickSkills.id == skillId)
                    return;
                des = "原有的绝技将被新技能覆盖，确定学习？";
            }
            //增加新技能
            //             if (Config.SkillConfig.GetAll().ContainsKey(skillId))
            //                 UIHintManage.Instance.ShowHint("成功学习" + SkillPrototypeManage.instance.GetData(skillId).name);
            //             else if (PassiveSkillPrototypeManage.instance.ContainsSkill(skillId))
            //                 UIHintManage.Instance.ShowHint("成功学习" + PassiveSkillPrototypeManage.instance.GetData(skillId).skillName);

            //发送协议
            //Utils.EventDispatcher.Instance.TriggerEvent<int, int>(PetsSystem.Event.LearningSkill, m_SkillInfos.selectedPet, m_ScrollView.selectedItemID);
            LearnSkillPetRequest request = new LearnSkillPetRequest();
            request.index = m_Panel.selected;
            request.bookId = m_ScrollView.selectedItemID;
            App.my.eventSet.FireEvent(EventID.Pets_LearnSkill, request);
        }
        #endregion

        void HideTips()
        {
            App.my.uiSystem.HidePanel(PanelType.UIItemTipsPanel);
        }
    }
}

#endif
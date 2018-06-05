#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using Config;
using NetProto;
using NetProto.Hot;

namespace xys.hot.UI
{
    class PetsPanel : HotTablePanelBase
    {
        public enum Page
        {
            Identify = 0,
            Train = 1,
            Attribute = 2,
        }

        const string sAttriubtePage = "AttributePage";
        const string sTrainPage = "TrainPage";
        const string sIdentifyPage = "IdentifyPage";

        PetsMgr petsMgr_;

        public PetsMgr petsMgr
        {
            get
            {
                if (petsMgr_ == null)
                    petsMgr_ = hotApp.my.GetModule<HotPetsModule>().petsMgr;

                return petsMgr_;
            }
        }

        PetsPanel(): base(null) { }
        PetsPanel(HotTablePanel parent) : base(parent)
        {

        }

        [SerializeField]
        UIPetsInfomation m_Infos;
        public UIPetsInfomation infos { get { return m_Infos; } }

        [SerializeField]
        ILMonoBehaviour m_ILScrollView;
        UIPetsScrollView m_ScrollView;

        [SerializeField]
        ILMonoBehaviour m_ILSkillTips;
        UIPetsSKillTips m_SkillTips;

        [SerializeField]
        ILMonoBehaviour m_ILAiTips;
        UIPetsAITips m_AiTips;
        [SerializeField]
        ILMonoBehaviour m_ILNickNameTips;
        UIPetsNickNameTips m_NickNameTips;
        //初始化
        protected override void OnInit()
        {
            if (m_ILScrollView != null)
            {
                m_ScrollView = (UIPetsScrollView)m_ILScrollView.GetObject();
                m_ScrollView.changePetCallback = this.ChangePetData;
            }

            if (m_ILSkillTips != null)
                m_SkillTips = (UIPetsSKillTips)m_ILSkillTips.GetObject();


            if (m_ILNickNameTips)
            {
                m_NickNameTips = (UIPetsNickNameTips)m_ILNickNameTips.GetObject();
                m_NickNameTips.panel = this;
            }
            if (m_ILAiTips)
            {
                m_AiTips = (UIPetsAITips)m_ILAiTips.GetObject();
                m_AiTips.panel = this;
            }
        }

        protected override bool OnPreChange(HotTablePage page)
        {
            if (petsMgr.m_PetsTable.attribute.Count == 0)
            {
                SystemHintMgr.ShowHint("你当前还没有灵兽");
                return false;
            }
            if (page.pageType.Equals(sAttriubtePage) || page.pageType.Equals(sTrainPage))
            {
                m_ILScrollView.gameObject.SetActive(true);
                m_Infos.OnShow();
                this.m_Infos.ResetData(this.selected);
                this.tableParent.ShowType(page.pageType, this.GetPanelInfos());
                return false;
            }
            else
            {
                m_ILScrollView.gameObject.SetActive(false);
                m_Infos.OnHide();
                return true;
            }
        }

        //显示
        protected override void OnShow(object args)
        {
            ShowPetPanelObject target = args as ShowPetPanelObject;
            if (petsMgr == null)
                return;

            //每次打开界面初始化个组件
            m_Infos.panel = this;
            m_Infos.OnInit();

            //
            if (petsMgr.m_PetsTable.attribute.Count == 0)
            {
                this.tableParent.ShowType((int)Page.Identify, this.GetPanelInfos());
            }
            else if (args != null && target != null && target.itemId != 0)
            {
               if(!this.CheckNavigationInfo(target.itemId))
                {
                    Debuger.LogError("ItemId error");
                    App.my.uiSystem.HidePanel(PanelType.UIPetsPanel, false);
                    return;
                }
            }
            else
            {
                this.tableParent.ShowType((int)Page.Attribute, this.GetPanelInfos());
            }

            //
            if (this.tableParent.CurrentPage != tableParent.GetPageList()[(int)Page.Identify].Get().pageType)
            {
                this.m_ILScrollView.gameObject.SetActive(true);
                this.m_ScrollView.Show(petsMgr.GetFightPetID());
                this.m_Infos.OnShow();
                this.m_Infos.ResetData(m_ScrollView.selected);

                if (m_ILAiTips.gameObject.activeSelf)
                    m_AiTips.ReFresh();
            }
            //注册
            Event.Subscribe(EventID.Pets_RefreshUI, this.Refresh);
            Event.Subscribe(EventID.Pets_CreateRefresh, this.RefreshPet);
        }

        protected override void OnHide()
        {
            if (m_Infos != null)
                m_Infos.OnDestroy();
        }
        void RefreshPet()
        {
            int petCount = petsMgr.m_PetsTable.attribute.Count;
            //宠物为0自动切换回图鉴界面
            if (petCount == 0)
            {
                m_Infos.OnHide();
                m_ILScrollView.gameObject.SetActive(false);
                this.OnHandBookPage();
            }
            else if (petCount == 1)
            {
                m_Infos.OnShow();
                m_ILScrollView.gameObject.SetActive(true);
                this.OnAttributePage(this);
            }
            else
                this.Refresh();
        }

        void Refresh()
        {
            if (this.tableParent.CurrentPage == this.tableParent.GetPageList()[(int)Page.Identify].Get().pageType)
            {
                if (m_ILAiTips.gameObject.activeSelf)
                    m_AiTips.CloseTips();
                if (m_ILNickNameTips.gameObject.activeSelf)
                    m_NickNameTips.CloseTips();
            }
            else
            {
                if (petsMgr.m_PetsTable.attribute.Count == 0)
                {
                    this.tableParent.ShowType((int)Page.Identify, this.GetPanelInfos());
                }
                else
                {
                    this.ChangePetData();
                    m_ScrollView.Show();
                    this.m_Infos.ResetData(this.selected);
                    this.m_AiTips.ReFresh();
                }
            }
        }

        public void OnAttributePage(object args = null)
        {
            if (petsMgr.m_PetsTable.attribute.Count == 0)
                return;
            this.tableParent.ShowType((int)Page.Attribute, this.GetPanelInfos());
            m_ScrollView.Show();
            //this.tableParent.StartCoroutine();
        }

        public void OnTrainPage(object args = null)
        {
            if (this.tableParent.CurrentPage == this.tableParent.GetPageList()[(int)Page.Train].Get().pageType)
                return;
            if (petsMgr.m_PetsTable.attribute.Count == 0)
                return;
            this.tableParent.ShowType(this.tableParent.GetPageList()[(int)Page.Train].Get().pageType, this.GetPanelInfos());
        }

        public void OnHandBookPage()
        {
            this.tableParent.ShowType((int)Page.Identify, null);
        }
        public void ShowTipsPanel(int itemId)
        {
            //道具tips
        }
       public void ShowSkillTips(int skillIndex)
        {
            if(m_SkillTips != null)
                m_SkillTips.Set(skillIndex);
        }
        public void ShowPetsSKillPanel()
        {
            if (this.selected == -1)
                return;
            App.my.uiSystem.ShowPanel(PanelType.UIPetsSkillPanel, this);
        }
        void ChangePetData()
        {
            if (!petsMgr.CheckIndex(m_ScrollView.selected))
                return;

            m_PetObj.Load(petsMgr.m_PetsTable.attribute[m_ScrollView.selected]);
            Event.fireEvent(EventID.Pets_DataRefresh);
        }
        public int selectedPetPlay
        {
            set
            {               
                m_ScrollView.SetPetPlay(value);

                //发送保存出战宠物列表
                SetPetPlayRequest sppr = new SetPetPlayRequest();
                sppr.index = value;
                Event.FireEvent<SetPetPlayRequest>(EventID.Pets_SetPetPlay, sppr);
            }
        }

        bool CheckNavigationInfo(int itemId)
        {
            Config.Item itemData = Config.Item.Get(itemId);
            int pageIndex = -1;
            switch (itemData.sonType)
            {
                case (int)ItemChildType.petExpDrug:
                case (int)ItemChildType.petTrainItem:
                case (int)ItemChildType.petPersonalityResetItem:
                    pageIndex = (int)Page.Train;
                    break;
                case (int)ItemChildType.petLockSkillItem:
                    pageIndex = (int)Page.Attribute;
                    break;
                case (int)ItemChildType.petResetAttrItem:
                case (int)ItemChildType.petTogetherItem:
                case (int)ItemChildType.petOpenGridItem:
                case (int)ItemChildType.petAddPointResetItem:
                    pageIndex = (int)Page.Attribute;
                    break;
                case (int)ItemChildType.petSoul:
                    pageIndex = (int)Page.Attribute;
                    break;
            }

            if(pageIndex != -1)
            {
                tableParent.ShowType(pageIndex, this.GetPanelInfos(itemId));
                return true;
            }
            return false;
        }

        PetsInfos GetPanelInfos(int item = 0)
        {
            PetsInfos infos = new PetsInfos();
            infos.itemId = item;
            infos.panel = this;
            return infos;
        }

        public int selected
        {
            get { return m_ScrollView.selected; }
        }

        PetObj m_PetObj = new PetObj();
        public PetObj selectedPetObj    { get { return m_PetObj; }}
    }

    class PetsInfos
    {
        public PetsPanel panel = null;
        public int itemId = 0;
    }
}


#endif
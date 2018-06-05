#if !USE_HOT
///////////////////////
//Author : Shen Lintao
//CreateDate : 2017.7.14
///////////////////////
namespace xys.hot.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using xys.UI;
    using xys.UI.State;
    using Config;
    using NetProto.Hot;
    using NetProto;
    class EquipRefineryPage : HotTablePageBase
    {
        [SerializeField]
        Button m_RefineBtn;     //炼化按钮
        [SerializeField]
        StateRoot m_ReplaceRefBtn;   //替换炼化属性按钮
        [SerializeField]
        Button m_materialBtn;   //选择炼化材料按钮
        [SerializeField]
        Transform m_Transform;
        [SerializeField]
        Transform m_CurrencyTrans;
        [SerializeField]
        Image equipImage;     
        [SerializeField]
        UIRefArrayInfo m_UIRefArrayInfo;
        [SerializeField]
        ILMonoBehaviour m_ILEquipListView;
        UIEquipListView m_EquipListView;
        [SerializeField]
        Image m_AddSp;              //中部材料图标上的+号图片
        [SerializeField]
        Transform m_MaterialTrans;  //弹出的装备选择小面板
        [SerializeField]
        Transform m_MaterialTipsContent;  //弹出的装备选择小面板
        [SerializeField]
        ILMonoBehaviour m_ILEquipIcon;  //顶部装备icon
        EquipIcon m_EquipIcon;
        [SerializeField]
        Transform m_EmptyInfo; //无装备时的显示信息
        [SerializeField]
        Transform m_Content;   //页面中部内容
        [SerializeField]
        Transform m_Buttons;   //页面底部内容
        //[SerializeField]
        //ILMonoBehaviour m_ILMaterialItem;

        Image m_CurrencyImage;
        Text m_CurrencyCost;
        MaterialItem m_MaterialItem;
        List<RefMaterialItem> m_matList = new List<RefMaterialItem>();
        int m_refType = 0;
        static bool isShowMater = false;
        EquipMgr m_EquipMgr;
        Config.EquipPrototype m_CurrentEquipCfg;
        ItemData m_CurrentItemData = new ItemData();
        EquipRefineryPage() : base(null) { }
        public EquipRefineryPage(HotTablePage page) : base(page)
        {
        }
        protected override void OnInit()
        {
            if (m_RefineBtn != null) m_RefineBtn.onClick.AddListener(this.OnClickRef);
            if (m_ReplaceRefBtn != null) m_ReplaceRefBtn.onClick.AddListener(this.OnClickChange);

            //m_UIRefArrayInfo.OnInit();
            if (m_ILEquipListView != null)
                m_EquipListView = (UIEquipListView)m_ILEquipListView.GetObject();
            if (m_ILEquipIcon != null)
                m_EquipIcon = (EquipIcon)m_ILEquipIcon.GetObject();
            if(m_UIRefArrayInfo!=null)
                m_UIRefArrayInfo.OnInit();
            if (m_materialBtn!=null)
            {
                m_MaterialItem = new MaterialItem(m_materialBtn.transform);
                m_MaterialItem.SetOnClickListener(OnSelectRefineMaterBtn);
            }
            if (m_CurrencyTrans!=null)
            {
                m_CurrencyImage = m_CurrencyTrans.Find("Image").GetComponent<Image>();
                m_CurrencyCost = m_CurrencyTrans.Find("Text").GetComponent<Text>();
            }
            //if (m_ILMaterialItem != null)
            //    m_MaterialItem = (MaterialItem)m_ILMaterialItem.GetObject();
        }

        protected override void OnShow(object args)
        {
            //reset code
            m_EquipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            if (m_EquipListView != null)
                m_EquipListView.SetCallBack(this.ResetPage, ()=> {
                    SetInfoActive(false);
                });
            //m_Page.parent.Event.Subscribe(EventID.Equip_RefreshRefine,m_EquipListView.OnShow);
            Event.Subscribe(EventID.Equip_RefreshUI, this.ResetPage);
            Event.Subscribe<int>(EventID.Equip_SelectRefMaterial, OnMatItem);
            Event.Subscribe(EventID.Package_UpdatePackage, ResetMaterial);
            //reset ui
            ResetMaterial();
            m_EquipListView.currentItemType = UIEquipDynamicDisplay.ShowType.TYPE_LV;
            m_EquipListView.currentPageType = UIEquipListView.PageType.TYPE_REFINE;
            if (m_UIRefArrayInfo != null)
                m_UIRefArrayInfo.ClearPropertyInfo();
            m_EquipListView.RefreshUI();
        }

        void ResetMaterial()
        {
            m_matList.Clear();
            m_MaterialItem.ResetNums();
            for (int i = 0; i < m_MaterialTipsContent.childCount; i++)
            {
                var trans = m_MaterialTipsContent.GetChild(i);
                RefMaterialItem matItem = new RefMaterialItem(trans);
                
                if (Config.RefinePropertyTable.FindIndex(i+1)>=0)
                    matItem.SetData(i+1);
                m_matList.Add(matItem);
            }
        }
        void OnMatItem(int refType)
        {
            m_refType = refType;
            var propertyCfg = Config.RefinePropertyTable.Get(m_refType);
            if (propertyCfg == null)
            {
                Debug.Log("Invalid refType:" + refType);
                return;
            }
            m_MaterialItem.SetData(propertyCfg.materialCostId, propertyCfg.materialCostCount, false);
            m_CurrencyTrans.gameObject.SetActive(true);
            m_CurrencyCost.text = propertyCfg.copperCostCount.ToString();
            m_AddSp.gameObject.SetActive(false);
            m_MaterialTrans.gameObject.SetActive(false);
        }

        void OnClickRef()
        {
            if (m_refType != 0)
            {
                if (m_MaterialItem.IsMatSufficient())
                {
                    if (Config.RefinePropertyTable.Get(m_refType).copperCostCount<= hotApp.my.localPlayer.silverShellValue)
                    {
                        int totalOperationTimes = int.Parse(Config.kvCommon.Get("equipDailyRefineTime").value);
                        if ((!m_EquipMgr.isOperationTimeActive) 
	                        || m_CurrentItemData.equipdata.equipBasicData.refTimes <= totalOperationTimes)
	                    {
		                    RefineryEquipMsg msg = new RefineryEquipMsg();
		                    int tag;
		                    m_EquipListView.GetInstanceInfo(m_EquipListView.currentEquipIndex, out tag, out msg.isPackageEquip);
		                    if (msg.isPackageEquip)
		                        msg.gridIndex = tag;
		                    else
		                        msg.subType = tag;
		                    msg.refIndex = m_refType;
		                    Event.FireEvent<RefineryEquipMsg>(EventID.Equip_Refine, msg);
	                    }
	                    else
	                        SystemHintMgr.ShowHint(TipsContent.Get(6007).des);
                    }
                    else
                        SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                }
                else
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
            }
            else
                SystemHintMgr.ShowHint(TipsContent.Get(6009).des);

        }
        void OnClickChange()
        {
            //var data = m_EquipListView.GetItemData(m_EquipListView.currentEquipIndex);
            if (m_CurrentItemData.equipdata.equipBasicData.tempRefAtts.Count>0)
            {
                EquipRequest request = new EquipRequest();
                int tag;
                m_EquipListView.GetInstanceInfo(m_EquipListView.currentEquipIndex, out tag, out request.isPackageEquip);
                if (request.isPackageEquip)
                    request.gridIndex = tag;
                else
                    request.subType = tag;
                Event.FireEvent<EquipRequest>(EventID.Equip_ReplaceRefine, request);
            }
        }

        void OnSelectRefineMaterBtn()
        {
            for (int i = 0; i < m_matList.Count; i++)
                m_matList[i].ResetNums();
            m_MaterialTrans.gameObject.SetActive(!m_MaterialTrans.gameObject.activeSelf);
        }

        void ResetPage()
        {
            m_CurrentItemData = m_EquipListView.GetItemData(m_EquipListView.currentEquipIndex);
            m_CurrentEquipCfg = Config.EquipPrototype.Get(m_CurrentItemData.id);
            SetInfoActive(true);
            if (m_UIRefArrayInfo!=null)
                m_UIRefArrayInfo.RefreshPropInfo(m_CurrentItemData);

            bool isActive = m_CurrentItemData.equipdata.equipBasicData.tempRefAtts.Count > 0;
            m_ReplaceRefBtn.SetCurrentState(isActive ? 0:1,false);
            m_ReplaceRefBtn.uCurrentButton.enabled = isActive;

            m_EquipIcon.SetData(m_CurrentItemData, m_CurrentEquipCfg);
        }

        void SetInfoActive(bool active)
        {
            m_EquipIcon.Reset();
            m_Content.gameObject.SetActive(active);
            m_Buttons.gameObject.SetActive(active);
            m_EmptyInfo.gameObject.SetActive(!active);
            m_CurrencyTrans.gameObject.SetActive(active&&(m_refType!=0));
        }
    }
}

#endif
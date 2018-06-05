#if !USE_HOT
///////////////////////
//Author : Shen Lintao
//CreateDate : 2017.7.14
///////////////////////
namespace xys.hot.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using xys.UI.State;
    using xys.UI;
    using NetProto;
    using Config;
    using System;

    class EquipRecastPage : HotTablePageBase
    {
        [SerializeField]
        StateRoot m_ReplacePropertyBtn;
        [SerializeField]
        Button m_OperationBtn;
        [SerializeField]
        UIRecastArrayInfo m_RecastArrayInfo;
        [SerializeField]
        ILMonoBehaviour m_ILEquipListView;
        UIEquipListView m_EquipListView;
        [SerializeField]
        ILMonoBehaviour m_ILEquipIcon;
        EquipIcon m_EquipIcon;
        [SerializeField]
        StateRoot m_ChangeToRecastBtn;
        [SerializeField]
        StateRoot m_ChangeToConsiceBtn;
        [SerializeField]
        Text m_OperationBtnText;   //重铸按钮文字
        [SerializeField]
        Text m_PageInfoText;    //底部提示文字
        [SerializeField]
        Text m_EmptyInfoTextUp;    //空属性提示文字上半部分
        [SerializeField]
        Text m_EmptyInfoTextDown;    //空属性提示文字下半部分
        [SerializeField]
        Transform m_MaterialTrans1;
        [SerializeField]
        Transform m_MaterialTrans2;
        [SerializeField]
        Text m_CurrencyCost;
        [SerializeField]
        Transform m_EmptyInfo;      //无装备时的显示信息
        [SerializeField]
        Transform m_Content;         //页面中部内容
        [SerializeField]
        Transform m_Buttons;        //页面底部内容

        EquipMgr m_EquipMgr;
        UIRecastArrayInfo.ShowType m_CurrentShowType;
        Config.EquipPrototype m_CurrentEquipCfg;
        MaterialItem m_Material1, m_Material2;
        Vector3 m_MatMiddlePos = new Vector3();
        Vector3 m_Material1InitPos = new Vector3();
        ItemData m_CurrentItemData = new ItemData();
        EquipRecastPage() : base(null) { }
        public EquipRecastPage(HotTablePage page) : base(page)
        {
        }
        protected override void OnInit()
        {
            if (m_ReplacePropertyBtn != null)
                m_ReplacePropertyBtn.onClick.AddListener(OnReplacePropertyBtn);
            if (m_OperationBtn != null)
                m_OperationBtn.onClick.AddListener(OnOperationBtn);
            if (m_RecastArrayInfo != null)
                m_RecastArrayInfo.OnInit();
            if (m_ChangeToRecastBtn)
                m_ChangeToRecastBtn.onClick.AddListener(OnChangeToRecast);
            if (m_ChangeToConsiceBtn)
                m_ChangeToConsiceBtn.onClick.AddListener(OnChangeToConcise);
            if (m_ILEquipListView != null)
                m_EquipListView = (UIEquipListView)m_ILEquipListView.GetObject();
            if (m_ILEquipIcon != null)
                m_EquipIcon = (EquipIcon)m_ILEquipIcon.GetObject();
            if (m_MaterialTrans1 != null)
            {
                m_Material1 = new MaterialItem(m_MaterialTrans1);
                //m_Material1.SetOnClickListener(()=> { OnMaterial(m_Material1.itemId); });
            }
            if (m_MaterialTrans2 != null)
            {
                m_Material2 = new MaterialItem(m_MaterialTrans2);
                //m_Material2.SetOnClickListener(() => { OnMaterial(m_Material2.itemId); });
            }
            m_CurrentShowType = UIRecastArrayInfo.ShowType.TYPE_RECAST;
            m_MatMiddlePos = (m_MaterialTrans1.localPosition + m_MaterialTrans2.localPosition) / 2;
            m_Material1InitPos = m_MaterialTrans1.localPosition;
        }
        protected override void OnShow(object args)
        {
            //reset code
            m_EquipMgr = App.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            if (m_EquipListView != null)
                m_EquipListView.SetCallBack(ResetPage, () => { SetInfoActive(false); });
            Event.Subscribe(EventID.Equip_RefreshUI, this.ResetPage);
            Event.Subscribe(EventID.Package_UpdatePackage, () => { ResetMaterial(m_CurrentShowType); });
            //reset ui
            m_EquipListView.currentItemType =UIEquipDynamicDisplay.ShowType.TYPE_LV;
            m_EquipListView.currentPageType = UIEquipListView.PageType.TYPE_RECAST;
            if (m_RecastArrayInfo != null)
                m_RecastArrayInfo.ClearPropertyInfo();
            m_EquipListView.RefreshUI();
            //OnChangeToConsice();
        }

        void ResetPage()
        {
            m_CurrentItemData = m_EquipListView.GetItemData(m_EquipListView.currentEquipIndex);

            if (m_CurrentItemData != null)
            {
                m_CurrentEquipCfg = Config.EquipPrototype.Get(m_CurrentItemData.id);

                SetInfoActive(true);
                //reload
                if (m_RecastArrayInfo != null)
                    m_RecastArrayInfo.RefreshPropInfo(m_CurrentItemData, m_CurrentShowType);
                bool isActive = false;
                switch (m_CurrentShowType)
                {
                    case UIRecastArrayInfo.ShowType.TYPE_RECAST:
                            isActive = m_CurrentItemData.equipdata.equipBasicData.tempBaseAttsByRecast.Count > 0;
                        break;
                    case UIRecastArrayInfo.ShowType.TYPE_CONCISE:
                            isActive = m_CurrentItemData.equipdata.equipBasicData.tempBaseAttsByConcise.Count > 0;
                        break;
                    default:
                        break;
                }
                m_ReplacePropertyBtn.SetCurrentState(isActive ? 0 : 1, false);
                m_ReplacePropertyBtn.uCurrentButton.enabled = isActive;
                m_EquipIcon.SetData(m_CurrentItemData, m_CurrentEquipCfg);

                ResetMaterial(m_CurrentShowType);
            }
            else
                Debug.Log(string.Format("currentEquipIndex error:{0}", m_EquipListView.currentEquipIndex));
        }

        void ResetMaterial(UIRecastArrayInfo.ShowType showType)
        {
            ItemCount material1, material2;
            int coinCost;
            switch (showType)
            {
                case UIRecastArrayInfo.ShowType.TYPE_RECAST:
                    {
                        material1 = m_CurrentEquipCfg.recastMaterialId1;
                        material2 = m_CurrentEquipCfg.recastMaterialId2;
                        coinCost = m_CurrentEquipCfg.recastCoinCost;
                    }
                    break;
                case UIRecastArrayInfo.ShowType.TYPE_CONCISE:
                    {
                        material1 = m_CurrentEquipCfg.consiceMaterialId1;
                        material2 = m_CurrentEquipCfg.consiceMaterialId2;
                        coinCost = m_CurrentEquipCfg.consiceCoinCost;
                    }
                    break;
                default:
                    {
                        material1 = null;
                        material2 = null;
                        coinCost = 0;
                    }
                    break;
            }

            m_MaterialTrans1.localPosition = m_Material1InitPos;
            if (material1 != null)
            {
                m_MaterialTrans1.gameObject.SetActive(true);
                m_Material1.SetData(material1.id, material1.count, true);
            }
            else
                m_MaterialTrans1.gameObject.SetActive(false);
            if (material2 != null)
            {
                m_MaterialTrans2.gameObject.SetActive(true);
                m_Material2.SetData(material2.id, material2.count, true);
            }
            else
            {
                m_MaterialTrans2.gameObject.SetActive(false);
                m_MaterialTrans1.localPosition = m_MatMiddlePos;
            }

            //silver
            m_CurrencyCost.text = coinCost.ToString();
        }

        void OnReplacePropertyBtn()
        {
            //var data = m_EquipListView.GetItemData(m_EquipListView.currentEquipIndex);
            if (m_CurrentItemData.equipdata.equipBasicData.tempBaseAttsByConcise.Count > 0 
                || m_CurrentItemData.equipdata.equipBasicData.tempBaseAttsByRecast.Count > 0)
            {
	            EquipRequest request = new EquipRequest();
	            int tag;
	            m_EquipListView.GetInstanceInfo(m_EquipListView.currentEquipIndex, out tag, out request.isPackageEquip);
	            if (request.isPackageEquip)
	                request.gridIndex = tag;
	            else
	                request.subType = tag;
	            switch (m_CurrentShowType)
	            {
	                case UIRecastArrayInfo.ShowType.TYPE_RECAST:
	                    Event.FireEvent<EquipRequest>(EventID.Equip_ReplaceRecast, request);
	                    break;
	                case UIRecastArrayInfo.ShowType.TYPE_CONCISE:
	                    Event.FireEvent<EquipRequest>(EventID.Equip_ReplaceConcise, request);
	                    break;
	                default:
	                    break;
	            }
            }
        }
        void OnOperationBtn()
        {
            if (m_Material1.IsMatSufficient()&& m_Material2.IsMatSufficient())
            {
                EquipRequest request = new EquipRequest();
                int tag;
                m_EquipListView.GetInstanceInfo(m_EquipListView.currentEquipIndex, out tag, out request.isPackageEquip);
                if (request.isPackageEquip)
                    request.gridIndex = tag;
                else
                    request.subType = tag;
                if (m_CurrentShowType == UIRecastArrayInfo.ShowType.TYPE_RECAST)
                {
                    if (m_CurrentEquipCfg.recastCoinCost<= App.my.localPlayer.silverShellValue)
                    {
                        int totalOperationTimes = int.Parse(Config.kvCommon.Get("equipDailyRecastTime").value);
                        if ((!m_EquipMgr.isOperationTimeActive)
	                        ||m_CurrentItemData.equipdata.equipBasicData.recastTimes <= totalOperationTimes)
	                        Event.FireEvent<EquipRequest>(EventID.Equip_Recast, request);
	                    else
	                        SystemHintMgr.ShowHint(TipsContent.Get(6006).des);
                    }
                    else
                        SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                }
                else
                {
                    if (m_CurrentEquipCfg.consiceCoinCost <= hotApp.my.localPlayer.silverShellValue)
                    {
                        int totalOperationTimes = int.Parse(Config.kvCommon.Get("equipDailyConciseTime").value);
                        if ((!m_EquipMgr.isOperationTimeActive) 
	                        || m_CurrentItemData.equipdata.equipBasicData.consiceTimes <= totalOperationTimes)
	                        Event.FireEvent<EquipRequest>(EventID.Equip_Concise, request);
	                    else
	                        SystemHintMgr.ShowHint(TipsContent.Get(6008).des);
                    }
                    else
                        SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                }
            }
            else
            {
                SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                Debug.Log("材料不足");
            }
        }
        private void OnChangeToConcise()
        {
            m_ChangeToRecastBtn.SetCurrentState(0,false);
            m_ChangeToRecastBtn.uCurrentButton.enabled = true;
            m_ChangeToConsiceBtn.SetCurrentState(1, false);
            m_ChangeToConsiceBtn.uCurrentButton.enabled = false;
            m_PageInfoText.text = "单件装备每日可凝练50次";
            m_EmptyInfoTextUp.text = "选择材料后点击【凝练】按钮生成新的属性";
            m_EmptyInfoTextDown.text = "凝练后可重生成凝练属性";
            m_CurrentShowType = UIRecastArrayInfo.ShowType.TYPE_CONCISE;
            ResetPage(); 
        }

        private void OnChangeToRecast()
        {
            m_ChangeToRecastBtn.SetCurrentState(1, false);
            m_ChangeToRecastBtn.uCurrentButton.enabled = false;
            m_ChangeToConsiceBtn.SetCurrentState(0, false);
            m_ChangeToConsiceBtn.uCurrentButton.enabled = true;
            m_PageInfoText.text = "单件装备每日可重铸50次";
            m_EmptyInfoTextUp.text = "选择材料后点击【重铸】按钮生成新的属性";
            m_EmptyInfoTextDown.text = "重铸后可重生成重铸属性";
            m_CurrentShowType = UIRecastArrayInfo.ShowType.TYPE_RECAST;
            ResetPage();
        }
        private void OnMaterial(int itemId)
        {
            UICommon.ShowItemTips(itemId);
        }

        void SetInfoActive(bool active)
        {
            m_EquipIcon.Reset();
            m_Content.gameObject.SetActive(active);
            m_Buttons.gameObject.SetActive(active);
            m_CurrencyCost.text = "";
            m_ChangeToRecastBtn.gameObject.SetActive(active);
            m_ChangeToConsiceBtn.gameObject.SetActive(active);
            m_EmptyInfo.gameObject.SetActive(!active);
        }
    }
}

#endif
#if !USE_HOT
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
    /// <summary>
    ///  Author:Shen Lintao
    ///  [7/11/2017]
    /// </summary>
    class EquipForgePage : HotTablePageBase
    {
        [SerializeField]
        Transform m_TransForm;
        [SerializeField]
        ILMonoBehaviour m_ILEquipIcon;
        EquipIcon m_EquipIcon;
        [SerializeField]
        Dropdown m_JobDropDown;
        [SerializeField]
        Dropdown m_LvDropDown;
        [SerializeField]
        ILMonoBehaviour m_ILUIForgeListView;
        UIEnforgeListView m_UIForgeListView;
        [SerializeField]
        Transform m_EquipMaterialTrans;
        [SerializeField]
        Transform m_commonMaterialTrans1;
        [SerializeField]
        Transform m_commonMaterialTrans2;
        [SerializeField]
        Transform m_ChooseEquipTrans;
        [SerializeField]
        Text m_ChooseEquipText;
        [SerializeField]
        Image m_ChooseEquipSp;
        [SerializeField]
        GameObject m_ChooseEquipIcon;
        [SerializeField]
        Transform m_ChooseEquipGrid;
        [SerializeField]
        Text m_CurrencyCost;
        [SerializeField]
        StateRoot m_ForgeBtn;

        MaterialItem material1;
        List<MaterialItem> m_matList = new List<MaterialItem>();
        List<int> m_PackageEquipList = new List<int>();   //背包中装备的gridIndex列表
        Config.EquipPrototype m_CurrentEquipCfg;
        int m_OptionalEquipMatCount = 0; //在弹出的装备选择界面中可选择装备的数量
        int m_CurrentMatEquipGridIndex = 0;
        int m_CurrentMatEquipID = 0;
        bool m_IsChooseInUseEquip = false; //是否选取了当前使用中的装备作为材料
        bool m_IsChoosingEquip = false; //装备选择界面是否打开
        int m_CurrentTargetEquipID = 0;
        HotEquipModule m_HotEquipModule;
        xys.UI.Dialog.TwoBtn m_twoBtn;
        public EquipForgePage() : base(null) { }
        public EquipForgePage(HotTablePage parent) : base(parent)
        {
        }

        protected override void OnInit()
        {
            if (m_ILUIForgeListView!=null)
                m_UIForgeListView =(UIEnforgeListView) m_ILUIForgeListView.GetObject();
            if (m_ILEquipIcon != null)
                m_EquipIcon = (EquipIcon)m_ILEquipIcon.GetObject();
            if (m_ForgeBtn != null)
                m_ForgeBtn.onClick.AddListener(OnForgeBtn);
            m_JobDropDown.onValueChanged.AddListener(OnJobChange);
            m_LvDropDown.onValueChanged.AddListener(OnLevelChange);
            m_HotEquipModule = hotApp.my.GetModule<HotEquipModule>();
        }
        protected override void OnShow(object p)
        {
            Event.Subscribe(EventID.Package_UpdatePackage, RefreshMaterial);
            m_UIForgeListView.SetSelectedCallBack(ResetPage,()=> { SetInfoActive(false); });
            ResetJobDropDown();
            ResetLvDropDown();
        }

        private void ResetJobDropDown()
        {
            //change to current player job
            OnJobChange((int)(hotApp.my.localPlayer.job)-1);
        }

        private void ResetLvDropDown()
        {
            m_LvDropDown.ClearOptions();
            List<string> options = new List<string>();
            int currentLv = hotApp.my.localPlayer.levelValue;
            int optionNum = (currentLv / 10) * 10;
            options.Add(optionNum.ToString());
            options.Add("神兵");
            m_LvDropDown.AddOptions(options);
            OnLevelChange(0);
        }

        public void ResetPage(int index)
        {
            SetInfoActive(true);
            m_CurrentTargetEquipID = m_UIForgeListView.GetEquipID(index);
            m_CurrentEquipCfg = Config.EquipPrototype.Get(m_CurrentTargetEquipID);
            m_PackageEquipList.Clear();
            m_IsChooseInUseEquip = false;
            m_IsChoosingEquip = false;
            //RefreshEquipIcon           
            m_EquipIcon.SetData(new ItemData(), m_CurrentEquipCfg);
            
            m_EquipIcon.SetOnClickListener(OnEquipIcon);
            RefreshMaterial();
        }

        /// <summary>
        /// 刷新装备材料，如果不需要装备作为材料，装备已选置真，否则交由刷新装备材料逻辑决定
        /// </summary>
        /// <param name="equipID"></param>
        /// <returns></returns>
        void RefreshMaterial()
        {
            m_CurrentMatEquipGridIndex = -1;
            m_matList.Clear();
            var cfg = Config.EquipForgePrototype.Get(m_CurrentTargetEquipID);
            m_CurrencyCost.text = cfg.costMoney.ToString();
            if (cfg.materialEquip!=0)
            {
                RefeshEquipMaterial(m_EquipMaterialTrans, cfg.materialEquip, 1);
                RefreshCommonMaterial(m_commonMaterialTrans1, cfg.materialId1, cfg.materialNum1);
                RefreshCommonMaterial(m_commonMaterialTrans2, cfg.materialId2, cfg.materialNum2);
            }
            else
            {
                RefreshCommonMaterial(m_commonMaterialTrans1, cfg.materialId1, cfg.materialNum1);
                RefreshCommonMaterial(m_EquipMaterialTrans, cfg.materialId2, cfg.materialNum2);
                RefreshCommonMaterial(m_commonMaterialTrans2, cfg.materialId3, cfg.materialNum3);
            }
        }
        void RefreshCommonMaterial(Transform trans, int matID, int consumNum)
        {
            var matItem = new MaterialItem(trans);
            if (matID != 0)
            {
                trans.gameObject.SetActive(true);
                matItem.SetData(matID, consumNum, true);
            }
            else
                trans.gameObject.SetActive(false);
            m_matList.Add(matItem);
        }
        void RefeshEquipMaterial(Transform trans, int matID, int consumNum)
        {
            m_CurrentMatEquipID = matID;
            RefreshPackageEquipList();
            var packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            var equipMgr = hotApp.my.localPlayer.GetModule<EquipModule>().equipMgr as EquipMgr;
            var matItem = new MaterialItem(trans);
            var cfg = Config.EquipPrototype.Get(matID);
            m_matList.Add(matItem);
            if (equipMgr.isWearingEquip(matID))
            {
                m_IsChoosingEquip = true;
                m_OptionalEquipMatCount = packageMgr.GetItemCount(matID) + 1;
                SetEquipMaterialActive(matItem,true);
            }
            else
            {
                m_OptionalEquipMatCount = 0;
                if(m_CurrentMatEquipGridIndex < 0)
                    m_CurrentMatEquipGridIndex = GetFirstEquipGridIndex(m_CurrentMatEquipID);
                matItem.SetData(matID, consumNum, true);
                SetEquipMaterialActive(matItem, false);
            }
        }
        void SetEquipMaterialActive(MaterialItem matItem, bool active)
        {
            m_ChooseEquipText.gameObject.SetActive(active);
            m_ChooseEquipSp.gameObject.SetActive(active);
            matItem.SetTextActive(!active);
            matItem.SetIconActive(!active);
            if (active)
                matItem.SetOnClickListener(ShowChooseEquipTip);
            else
            {
                matItem.RemoveAllListener(); //need to reset listener to default or to a special one
                matItem.SetOnClickListener(ShowEquipTip);
            }
                
        }

        /// <summary>
        /// 点击装备材料时显示选择装备材料提示
        /// </summary>
        /// <returns></returns>
        void ShowChooseEquipTip()
        {
            m_ChooseEquipGrid.DestroyChildren();
            m_ChooseEquipTrans.gameObject.SetActive(true);
            if (m_OptionalEquipMatCount != 0)
            {
                for (int i =0;i<m_OptionalEquipMatCount; i++)
                {
                    GameObject go = GameObject.Instantiate(m_ChooseEquipIcon);
                    go.SetActive(true);
                    go.transform.SetParent(m_ChooseEquipGrid,false);
                    go.transform.localScale = Vector3.one;
                    EquipMaterialItem item = new EquipMaterialItem(go.transform);

                    item.SetData(m_CurrentMatEquipID,i>0?(i-1):0);
                    item.SetTextActive(false);
                    if(0==i)
                    {
                        item.SetEquipedActive(true);
                        item.SetOnClickListener(OnEquipInUse);
                    }
                    else
                    {
                        item.SetEquipedActive(false);
                        item.SetOnClickListener(OnEquipInPackage);
                    }
                }
            }
        }
        void ShowEquipTip()
        {
            NetProto.ItemGrid itemGrid = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(m_CurrentMatEquipGridIndex);
            if (itemGrid!=null)
            {
	            InitItemTipsData param = new InitItemTipsData();
	            param.itemData = itemGrid;
	            param.type = InitItemTipsData.Type.CommonEquipTips;
	            App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
            }
        }
        void OnEquipInUse()
        {
            m_IsChooseInUseEquip = true;
            m_ChooseEquipTrans.gameObject.SetActive(false);
            m_IsChoosingEquip = false;
            if (m_CurrentMatEquipID!=0)
            {
                var cfg = Config.EquipPrototype.Get(m_CurrentMatEquipID);
                if(cfg != null)
                    m_HotEquipModule.UnLoadEquip(cfg.sonType);
            }            
        }
        void OnEquipInPackage(int index)
        {
            m_IsChoosingEquip = false;
            m_CurrentMatEquipGridIndex = m_PackageEquipList[index];
            m_ChooseEquipTrans.gameObject.SetActive(false);
            m_ChooseEquipSp.gameObject.SetActive(false);
            m_ChooseEquipText.gameObject.SetActive(false);
            m_matList[0].SetData(m_CurrentMatEquipID, 1, true);
            m_matList[0].SetTextActive(true);
            m_matList[0].SetIconActive(true);
        }

        private void OnLevelChange(int arg)
        {
            int currentLv = hotApp.my.localPlayer.levelValue;
            int optionNum = (currentLv / 10) * 10;
            if(arg == 0 )
                m_UIForgeListView.SetLv(optionNum);
            else
            {
                //神兵暂留
            }
            
        }
        private void OnJobChange(int arg)
        {
            m_UIForgeListView.SetJobType((RoleJob.Job)(arg + 1));
            
        }
        void OnEquipIcon()
        {
            InitItemTipsData param = new InitItemTipsData();
            NetProto.ItemGrid itemGrid = new NetProto.ItemGrid();
            itemGrid.data.id = m_CurrentTargetEquipID;
            param.itemData = itemGrid;
            param.type = InitItemTipsData.Type.EquipPrototypeTips;
            App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
        }
        private void OnForgeBtn()
        {
            if (!m_IsChoosingEquip)
            {
                if (isMaterialSufficient())
                {
                    if (Config.EquipForgePrototype.Get(m_CurrentTargetEquipID).costMoney <= hotApp.my.localPlayer.silverShellValue)
                    {
                        ForgeEquipMsg msg = new ForgeEquipMsg();
                        msg.equipId = m_CurrentTargetEquipID;
                        msg.gridIndex = m_CurrentMatEquipGridIndex;
                        if (CanTransferProperty(m_CurrentMatEquipGridIndex))
	                    {
                            if (null == m_twoBtn)
                            {
                                m_twoBtn = xys.UI.Dialog.TwoBtn.Show(
                                    "", TipsContent.Get(6005).des,
                                    "取消", () =>
                                    {
                                        msg.isTransferProperty = false;
                                        Event.FireEvent<ForgeEquipMsg>(EventID.Equip_Forge, msg);
                                        return false;
                                    },
                                    "确定", () =>
                                    {
                                        msg.isTransferProperty = true;
                                        Event.FireEvent<ForgeEquipMsg>(EventID.Equip_Forge, msg);
                                        return false;
                                    }, true, true, () => { m_twoBtn = null; });
                                return;
                            }
                        }
                        else
                        {
                            msg.isTransferProperty = false;
                            Event.FireEvent<ForgeEquipMsg>(EventID.Equip_Forge, msg);
                        }
                    }
                    else
                        SystemHintMgr.ShowHint(TipsContent.Get(6014).des);
                }
                else
                    SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
            }
            else
                SystemHintMgr.ShowHint(TipsContent.Get(6013).des);
        }
        bool isMaterialSufficient()
        {
            var itr = m_matList.GetEnumerator();
            while (itr.MoveNext())
            {
                if (!itr.Current.IsMatSufficient())
                    return false;
            }
            return true;
        }
        bool CanTransferProperty(int gridIndex)
        {
            var itemData = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemInfo(gridIndex).data;
            return (itemData.equipdata.equipBasicData.enforceLv != 0 || itemData.equipdata.equipBasicData.refAtts.Count != 0);
        }
        int GetFirstEquipGridIndex(int equipID)
        {
            int ret = -1;
            var packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            int gridNum = packageMgr.package.Count;

            for (int i = 0; i < gridNum; i++)
            {
                var item = packageMgr.GetItemInfo(i);
                if (item != null&&EquipCfgMgr.IsEquipID(item.data.id)&&(equipID == item.data.id))
                {
                    ret = i;
                    break;
                }
                    
            }
            return ret;
        }
        void RefreshPackageEquipList()
        {
            var packageMgr = hotApp.my.GetModule<HotPackageModule>().packageMgr;
            int gridNum = packageMgr.package.Count;

            for (int i = 0; i < gridNum; i++)
            {
                var item = packageMgr.GetItemInfo(i);
                if (item != null && EquipCfgMgr.IsEquipID(item.data.id) && (m_CurrentMatEquipID == item.data.id))
                {
                    if (m_PackageEquipList.Contains(i))
                        continue;
                    else
                    {
                        if (m_IsChooseInUseEquip) 
                            m_CurrentMatEquipGridIndex = i;
                        m_PackageEquipList.Add(i);
                    }
                }
            }
            m_IsChooseInUseEquip = false;
        }
        void  SetInfoActive(bool active)
        {
            m_EquipIcon.Reset();
            for (int i = 0; i < m_matList.Count; i++)
                m_matList[i].Reset();
            m_CurrentMatEquipGridIndex = -1;
            m_CurrentMatEquipID = 0;
            m_CurrentTargetEquipID = 0;
            m_CurrencyCost.text = "";
            m_CurrencyCost.gameObject.SetActive(active);
            m_ForgeBtn.SetCurrentState(active ? 0 : 1, false);
            m_ForgeBtn.uCurrentButton.enabled = active;
        }
    }
}
#endif

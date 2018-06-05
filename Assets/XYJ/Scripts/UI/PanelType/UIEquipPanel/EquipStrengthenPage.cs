#if !USE_HOT
namespace xys.hot.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using xys.UI.State;
    using xys.UI;
    using NetProto.Hot;
    using Config;
    using NetProto;
    using System;

    class EquipStrengthenPage : HotTablePageBase
    {
        [SerializeField]
        UIArrayInfo m_UIArrayInfo;
        [SerializeField]
        Text m_MatAmount;  
        [SerializeField]
        Button m_InforceBtn;     
        [SerializeField]
        Text InforStatus;  
        [SerializeField]
        Button m_TipsBtn;     

        [SerializeField]
        protected Transform m_DropEquip;

        //equip icon
        [SerializeField]
        ILMonoBehaviour m_ILEquipIcon;
        EquipIcon m_EquipIcon;

        //mat icon
        [SerializeField]
        Image m_MatIcon; 
            
        [SerializeField]
        ILMonoBehaviour m_ILEquipListView;
        UIEquipListView m_EquipListView;
        [SerializeField]
        Transform m_IconPanel;
        [SerializeField]
        Transform m_InfoPanel;
        [SerializeField]
        Transform m_EmptyPanel;
        [SerializeField]
        StateRoot m_EnforceBtn;
        [SerializeField]
        Text m_EnforceText;
        [SerializeField]
        Transform m_MaterialTrans;
        [SerializeField]
        Button m_PageTipsBtn;
        [SerializeField]
        Transform m_PageTipsTrans;

        int m_CurIncreasePercent = 0;
        int infType = 0;
        int m_MatUseAmount = 0;
        int m_MatInPack = 0;
        int m_CurrentMatID = 0;
        EquipMgr m_EquipMgr;
        MaterialItem m_materialItem;
        Config.EquipPrototype m_CurrentEquipCfg;
        MatType m_CurrentMatType;
        public enum textType
        {
            enInforLv = 1,
            enEquipName = 2,
        }

        public enum InfType
        {
            enNormalInf = 0,  
            enAwaken = 1,   
            enAwakenInf = 2, 
        }

        enum EnforceBtnState
        {
            STATE_ENFORCE_ENABLE,
            STATE_ENFORCE_DISABLE,
            STATE_DISABLE,
            STATE_AWAKE_ENFORCE_ENABLE,
            STATE_AWAKE_ENFORCE_DISABLE,
            STATE_AWAKE_ENABLE,
        }
        enum MatType
        {
            TYPE_NONE,
            TYPE_ENFORCE,
            TYPE_AWAKE
        }
        EquipStrengthenPage() : base(null) { }
        EquipStrengthenPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
            if(m_ILEquipListView!=null)
                m_EquipListView = (UIEquipListView)m_ILEquipListView.GetObject();
            if(m_ILEquipIcon != null)
                m_EquipIcon = (EquipIcon)m_ILEquipIcon.GetObject();

            //    m_InforceBtn.onClick.AddListener(this.OnClick);
            //m_MaterialTrans.GetComponent<Button>().onClick.AddListener(ShowMaterialTips);
            if (m_EnforceBtn != null)
                m_EnforceBtn.onClick.AddListener(OnEnforceBtn);
            if (m_PageTipsBtn != null)
                m_PageTipsBtn.onClick.AddListener(()=> { m_PageTipsTrans.gameObject.SetActive(true); });
            if (m_MaterialTrans!=null)
            {
                m_materialItem = new MaterialItem(m_MaterialTrans);
            }
            m_UIArrayInfo.OnInit();
        }


        protected override void OnShow(object args)
        {
            //reset code
            if (m_EquipListView != null)
                m_EquipListView.SetCallBack(this.ResetPage, ()=> { SetInfoActive(false); });
            //Event.Subscribe(EventID.Equip_RefreshUI, m_EquipListView.OnShow);
            Event.Subscribe(EventID.Equip_RefreshUI, this.ResetPage);
            Event.Subscribe(EventID.Package_UpdatePackage, ()=> { ResetMaterial(m_CurrentMatType); });
            //reset ui
            //CheckEquipStatus();
            m_EquipListView.currentItemType =UIEquipDynamicDisplay.ShowType.TYPE_ENFORCE;
            m_EquipListView.currentPageType = UIEquipListView.PageType.TYPE_ENFORCE;
            m_EquipListView.RefreshUI();
            m_PageTipsTrans.gameObject.SetActive(false);
        }
        void ResetPage()
        {
            SetInfoActive(true);
            ItemData data = m_EquipListView.GetItemData(m_EquipListView.currentEquipIndex);
            m_CurrentEquipCfg = Config.EquipPrototype.Get(data.id);
            //icon
            m_EquipIcon.SetData(data,m_CurrentEquipCfg);

            //property
            if (m_UIArrayInfo != null)
            {
                m_UIArrayInfo.RefreshPropInfo(data);
            }

            //enforce btn
            //can enforce?
            if (data.equipdata.equipBasicData.enforceLv < m_CurrentEquipCfg.InforceValue)
            {
                m_CurrentMatType = MatType.TYPE_ENFORCE;
                m_EnforceBtn.SetCurrentState((int)EnforceBtnState.STATE_ENFORCE_ENABLE, false);
            }
            else
            {
                // if is awake
                if (data.equipdata.equipBasicData.awakenStatus)
                {
                    //can enforce
                    if (data.equipdata.equipBasicData.awakenEnforceLV < m_CurrentEquipCfg.awake)
                    {
                        //change btn to awakeEnforce
                        m_CurrentMatType = MatType.TYPE_ENFORCE;
                        m_EnforceBtn.SetCurrentState((int)EnforceBtnState.STATE_AWAKE_ENFORCE_ENABLE, false);
                    }
                    else
                    {
                        //change btn to disable
                        m_CurrentMatType = MatType.TYPE_NONE;
                        m_EnforceBtn.SetCurrentState((int)EnforceBtnState.STATE_AWAKE_ENFORCE_DISABLE, false);
                    }
                }
                else
                {
                    //can awake
                    if (false/*m_EquipCfg.isCanAwake*/)
                    {
                        //repalce mat type 
                        //replace btn
                        m_CurrentMatType = MatType.TYPE_AWAKE;
                        m_EnforceBtn.SetCurrentState((int)EnforceBtnState.STATE_AWAKE_ENABLE, false);
                    }
                    else
                    {
                        //change btn to disable
                        m_CurrentMatType = MatType.TYPE_NONE;
                        m_EnforceBtn.SetCurrentState((int)EnforceBtnState.STATE_ENFORCE_DISABLE, false);
                    }
                }
            }

            //material
            //type is between awake and enforce
            ResetMaterial(m_CurrentMatType);
        }

        void ResetMaterial(MatType type)
        {
            if (type == MatType.TYPE_NONE)
            {
                m_MaterialTrans.gameObject.SetActive(false);
                return;
            }
            else
                m_MaterialTrans.gameObject.SetActive(true);
            var data = m_EquipListView.GetItemData(m_EquipListView.currentEquipIndex);
            var equipCfg = Config.EquipPrototype.Get(data.id);
            if (equipCfg == null)
            {
                Debug.LogError("Invalid equipID:"+ data.id);
                return;
            }
            //enforce&& awakeEnforce
            if (MatType.TYPE_ENFORCE == type)
            {
                EquipBasicData equipBasicData = data.equipdata.equipBasicData;
                EquipCfgMgr.GetEquipEnforceMatData(equipBasicData.nSubType,
                                                    equipBasicData.enforceLv,
                                                    equipBasicData.awakenEnforceLV,
                                                    equipBasicData.awakenStatus,
                                                    out m_CurrentMatID,
                                                    out m_MatUseAmount);

                m_materialItem.SetData(m_CurrentMatID, m_MatUseAmount, true);
            }
            //awake
            else
            {

            }

        }

        void OnEnforceBtn()
        {
            if(m_materialItem.IsMatSufficient())
            {
                EquipRequest request = new EquipRequest();
                int tag;
                m_EquipListView.GetInstanceInfo(m_EquipListView.currentEquipIndex, out tag,out request.isPackageEquip);
                if (request.isPackageEquip)
                    request.gridIndex = tag;
                else
                    request.subType = tag;
                Event.FireEvent<EquipRequest>(EventID.Equip_Inforce, request);
            }
            else
            {
                SystemHintMgr.ShowHint(TipsContent.Get(6003).des);
                Debug.Log("材料不足");
            }
        }
        void SetInfoActive(bool active)
        {
            m_EquipIcon.Reset();
            m_materialItem.Reset();
            m_InfoPanel.gameObject.SetActive(active);
            m_EmptyPanel.gameObject.SetActive(!active);
            m_EnforceBtn.SetCurrentState(active ? (int)EnforceBtnState.STATE_ENFORCE_ENABLE : (int)EnforceBtnState.STATE_DISABLE, false);
            m_EnforceBtn.uCurrentButton.enabled = active;
        }

        private void ShowMaterialTips()
        {
            UICommon.ShowItemTips(m_CurrentMatID);
        }
    }
}

#endif
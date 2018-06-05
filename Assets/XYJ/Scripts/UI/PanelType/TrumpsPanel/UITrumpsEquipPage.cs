#if !USE_HOT
namespace xys.hot.UI
{
    using battle;
    using Config;
    using NetProto;
    using NetProto.Hot;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using xys.UI;
    using xys.UI.State;

    class UITrumpsEquipPage : HotTablePageBase
    {
        UITrumpsEquipPage() : base(null) { }
        public UITrumpsEquipPage(HotTablePage parent) : base(parent) {}

        [SerializeField]
        UITrumpsEquip m_Equip;

        [SerializeField]
        UITrumpsProperty m_Property;

        [SerializeField]
        UITrumpsEquipScroll m_EquipScrollView;

        [SerializeField]
        StateToggle m_EquipPageST;
        
        [SerializeField]
        float xPos = 223.0f;
        [SerializeField]
        float yPos = 279.0f;

        BattleAttri m_EquipBattleAttri;

        int m_SelectedPos;

        TrumpsMgr m_TrumpMgr;

        protected override void OnInit()
        {
            m_TrumpMgr = App.my.localPlayer.GetModule<TrumpsModule>().trumpMgr as TrumpsMgr;
            m_EquipBattleAttri = new BattleAttri();
            //
            m_Property.OnInit(this);
            m_Equip.OnInit();
            m_Equip.selectedCallback = this.SelectedEquip;
            m_EquipScrollView.OnInit();
            m_EquipScrollView.selectedTrump = this.SelectedTrump;

            m_EquipPageST.OnSelectChange = this.SelectedPageEvent;
        }

        protected override void OnShow(object p)
        {
            //
            Event.Subscribe(EventID.Trumps_RefleashUI, this.OnRefresh);
            //
            m_SelectedPos = -1;
            m_EquipPageST.Select = 0;
            this.OnRefresh();
        }

        void OnRefresh()
        {
            this.RefreshEquipAttri();
            m_Equip.Refresh();
            m_Property.Refresh();
            m_EquipScrollView.Refreash(m_Equip.selectedIndex);
        }
        #region Event
        //子页面选择回调
        void SelectedPageEvent(StateRoot sr,int index)
        {
            if(index == 0)
            {
                m_Equip.OnResetSelected();
            }
            else
            {
                m_Equip.OnSelected();
                m_EquipScrollView.Refreash(m_Equip.selectedIndex);
                m_SelectedPos = 0;
            }
        }
        //选中装备栏回调
        void SelectedEquip()
        {
            m_SelectedPos = m_Equip.selectedIndex;
            //若法宝是激活并且装备，打开tips
            if (m_TrumpMgr.CheckTrumps(m_Equip.selectedTrump))
                UICommon.ShowTrumpTips(m_Equip.selectedTrump, new Vector2(xPos, yPos));

            m_EquipPageST.list[0].CurrentState = 0;
            m_EquipPageST.list[1].CurrentState = 1;

            m_EquipScrollView.Refreash(m_Equip.selectedIndex);
        }

        void SelectedTrump(int trumpId)
        {
            ////获得法宝装备槽位
            //int equipPos = m_TrumpMgr.GetEquipPos(trumpId);
            ////
            //if(equipPos == -1)
            //{
            //    if (m_SelectedPos == -1 || m_SelectedPos < 0 || m_SelectedPos > TrumpsMgr.MAX_EQUIP_POS)
            //    {
            //        SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Equip_Selecte").des);
            //        return;
            //    }
            //    equipPos = m_SelectedPos;
            //}
            if (m_SelectedPos == -1 || m_SelectedPos < 0 || m_SelectedPos > TrumpsMgr.MAX_EQUIP_POS)
            {
                SystemHintMgr.ShowHint(TipsContent.GetByName("Trump_Equip_Selecte").des);
                return;
            }
            //发送法宝装备协议
            TrumpsEquipRequest request = new TrumpsEquipRequest();
            request.equippos = m_SelectedPos;
            request.trumpid = trumpId;
            Event.FireEvent<TrumpsEquipRequest>(EventID.Trumps_Equip, request);
        }

        void RefreshEquipAttri()
        {
            //重新计算装备法宝属性
            m_EquipBattleAttri.Clear();
            m_TrumpMgr.CalculateTrumps(ref m_EquipBattleAttri);
        }

        public BattleAttri equipBattleAttri { get { return m_EquipBattleAttri; } }
        #endregion
    }
}
#endif
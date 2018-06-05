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
    ///  [7/27/2017]
    /// </summary>
    class EquipAttachSoulPage : HotTablePageBase
    {
        [SerializeField]
        ILMonoBehaviour m_ILSoulListView;
        UISoulListView m_SoulListView;
        [SerializeField]
        Transform m_SoulIconGridTrans;

        EquipSoulMgr soulMgr;
        List<UISoulIcon> m_SoulIconList = new List<UISoulIcon>();
        NetProto.SoulGrids m_CurrentSoulGrids;
        int m_CurrentGridIndex;
        xys.UI.Dialog.TwoBtn m_TwoBtn;
        public EquipAttachSoulPage() : base(null) { }
        public EquipAttachSoulPage(HotTablePage parent) : base(parent)
        {
        }
        protected override void OnInit()
        {
            if (m_ILSoulListView != null)
                m_SoulListView =(UISoulListView) m_ILSoulListView.GetObject();
        }
        protected override void OnShow(object p)
        {
            soulMgr = hotApp.my.GetModule<HotEquipSoulModule>().equipSoulMgr as EquipSoulMgr;
            m_SoulIconList.Clear();
            for (int i = 0;i < m_SoulIconGridTrans.childCount; i++)
            {
                var icon = new UISoulIcon(m_SoulIconGridTrans.GetChild(i), i, OnSoulIcon);
                m_SoulIconList.Add(icon);
            }
            m_SoulListView.SetSelectedCallback(ResetPage);
            m_SoulListView.OnShow();
        }
        protected override void OnHide()
        {
            m_SoulListView.OnHide();
        }

        public void ResetPage()
        {
            m_CurrentSoulGrids = m_SoulListView.GetSoulGrids(m_SoulListView.currentSubType);
            var itr = m_CurrentSoulGrids.soulData.GetEnumerator();
            while (itr.MoveNext())
                m_SoulIconList[itr.Current.Key].SetData(itr.Current.Value.soulID);
        }

        public void OnSoulIcon(int index)
        {
            //set focus
            m_SoulIconList[m_CurrentGridIndex].SetFocus(true);
            m_SoulIconList[index].SetFocus(true);
            m_CurrentGridIndex = index;
            //callback
            if (m_CurrentSoulGrids.soulData.ContainsKey(index))
            {
                var soulGrid = m_CurrentSoulGrids.soulData[index];
                if (soulGrid.isActive && soulGrid.soulID!=0)
                    ShowItemTips(soulGrid.soulID);
                else
                    ShowOpenGridDialog();
            }
        }

        public void ShowItemTips(int id)
        {

        }
        public void ShowOpenGridDialog()
        {
            int requireLv = 0;
            int currencyCost = 0;
            if (requireLv <= hotApp.my.localPlayer.levelValue)
            {
                if (m_TwoBtn != null)
                {
                    m_TwoBtn = xys.UI.Dialog.TwoBtn.Show(
                        "", string.Format(TipsContent.Get(6208).des, currencyCost),
                        "取消", () =>
                        {
                            return false;
                        },
                        "确定", () =>
                        {
                            OpenSoulGridRequest msg = new OpenSoulGridRequest();
                            msg.index = m_CurrentGridIndex;
                            msg.subType = m_SoulListView.currentSubType;
                            Event.FireEvent<OpenSoulGridRequest>(EventID.EquipSoul_OpenGrid, msg);
                            return false;
                        }, true, true, () => { m_TwoBtn = null; });
                    return;
                }
            }
            else
                SystemHintMgr.ShowHint(string.Format(TipsContent.Get(6207).des, requireLv));
        }
    }
}
#endif

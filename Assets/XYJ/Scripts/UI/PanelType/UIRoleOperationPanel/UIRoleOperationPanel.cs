#if !USE_HOT
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using xys.UI;

namespace xys.hot.UI
{
    class UIRoleOperationPanel : HotTablePanelBase
    {
        [SerializeField]
        ILMonoBehaviour m_ILRoleInfo;
        [SerializeField]
        UIRoleOperationInfo m_RoleInfo;

        [SerializeField]
        ILMonoBehaviour m_ILBtnList;
        [SerializeField]
        UIRoleOperationBtnList m_BtnList;

        int m_EventHanderId = -1;

        UIRoleOperationPanel() : base(null) { }
        UIRoleOperationPanel(HotTablePanel parent) : base(parent)
        {

        }

        protected override void OnInit()
        {
            if (m_ILRoleInfo != null) m_RoleInfo = (UIRoleOperationInfo)m_ILRoleInfo.GetObject();
            if (m_ILBtnList != null) m_BtnList = (UIRoleOperationBtnList)m_ILBtnList.GetObject();
        }

        protected override void OnShow(object args)
        {
            m_EventHanderId = EventHandler.pointerClickHandler.Add(CloseTips);

            UIRoleOperationData data = (UIRoleOperationData)args;
            parent.transform.localPosition = data.panelPos;
            m_RoleInfo.SetRoleInfo(data.jobSex, data.job, data.name, data.level, data.clamName, data.team);
            m_BtnList.SetBtnShowList(data.handleList, data.showType, data.playerId);
            m_BtnList.panel = this;
            SetBGHeight();
        }

        public void RefreshTeamInfo(string teamInfo)
        {
            m_RoleInfo.SetRoleTeamInfo(teamInfo);
        }

        public void SetBGHeight()
        {
            RectTransform bgRt = parent.transform.Find("Bg").GetComponent<RectTransform>();
            RectTransform roleInfoRt = parent.transform.Find("RoleInfo").GetComponent<RectTransform>();
            GridLayoutGroup gridGroup = parent.transform.Find("BtnList").GetComponent<GridLayoutGroup>();
            int column = (m_BtnList.m_ActiveItemCount + 1) / 2;

            bgRt.sizeDelta = new Vector2(bgRt.sizeDelta.x, roleInfoRt.sizeDelta.y + gridGroup.cellSize.y * column + gridGroup.spacing.y * (column - 1));
        }

        protected override void OnHide()
        {
            EventHandler.pointerClickHandler.Remove(m_EventHanderId);
        }

        bool CloseTips(GameObject obj, BaseEventData bed)
        {
            if (obj == null || !obj.transform.IsChildOf(parent.transform))
            {
                App.my.uiSystem.HidePanel(PanelType.UIRoleOperationPanel, false);
                return false;
            }
            return true;
        }
    }
}
#endif
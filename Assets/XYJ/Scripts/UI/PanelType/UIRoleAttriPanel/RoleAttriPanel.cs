#if !USE_HOT
using System;
using UnityEngine;
using UnityEngine.UI;
using xys.battle;
using xys.UI;

namespace xys.hot.UI
{
    class RoleAttriPanel : HotTablePanelBase
    {
        public enum Page
        {
            RoleSttribute = 0,
            Personality = 1,
        }

        RoleAttriPanel() :base(null) { }
        RoleAttriPanel(HotTablePanel panel) : base(panel)
        {

        }

        [SerializeField]
        Transform m_RolePartRoot;

        [SerializeField]
        ILMonoBehaviour m_ILRoleInfo;
        [SerializeField]
        UIRoleAttriRoleInfo m_RoleInfo;


        protected override void OnInit()
        {
            if (m_ILRoleInfo != null) m_RoleInfo = (UIRoleAttriRoleInfo)m_ILRoleInfo.GetObject();
        }

        protected override void OnShow(object p)
        {

        }

        void Refresh()
        {
            if (m_ILRoleInfo.gameObject.activeSelf)
                m_RoleInfo.ShowRoleInfo();
        }
    }
}
#endif
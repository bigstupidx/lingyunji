#if !USE_HOT
namespace xys.hot
{
    using UnityEngine;
    using NetProto;
    class AppearanceMgr
    {
        public AppearanceData m_AppearanceData;
        
        private RoleDisguiseHandle m_roleDisguiseHandle;
        public AppearanceMgr()
        {
            m_AppearanceData = null;
        }
        public RoleDisguiseHandle GetDisguiHandle()
        {
            if(m_roleDisguiseHandle==null)
            {
                m_roleDisguiseHandle = new RoleDisguiseHandle();
                int roleId = hotApp.my.localPlayer.cfgInfo.id;
                int job = hotApp.my.localPlayer.carrerValue;
                int sex = hotApp.my.localPlayer.sexValue;
                m_roleDisguiseHandle.SetRoleAppearance(roleId, job, sex, m_AppearanceData);
            }
            return m_roleDisguiseHandle;
        }
        public void RefreshDisguisehandle()
        {
            int roleId = hotApp.my.localPlayer.cfgInfo.id;
            int job = hotApp.my.localPlayer.carrerValue;
            int sex = hotApp.my.localPlayer.sexValue;
            m_roleDisguiseHandle.SetRoleAppearance(roleId, job, sex, m_AppearanceData);
        }
        public WeaponHandle GetWeaponHandle()
        {
            if (m_roleDisguiseHandle == null)
            {
                m_roleDisguiseHandle = new RoleDisguiseHandle();
                int roleId = hotApp.my.localPlayer.cfgInfo.id;
                int job = hotApp.my.localPlayer.carrerValue;
                int sex = hotApp.my.localPlayer.sexValue;
                m_roleDisguiseHandle.SetRoleAppearance(roleId, job, sex, m_AppearanceData);
            }
            return m_roleDisguiseHandle.m_weaponHandle;
        }
        public RideHandle GetRideHandle()
        {
            if (m_roleDisguiseHandle == null)
            {
                m_roleDisguiseHandle = new RoleDisguiseHandle();
                int roleId = hotApp.my.localPlayer.cfgInfo.id;
                int job = hotApp.my.localPlayer.carrerValue;
                int sex = hotApp.my.localPlayer.sexValue;
                m_roleDisguiseHandle.SetRoleAppearance(roleId, job, sex, m_AppearanceData);
            }
            return m_roleDisguiseHandle.m_rideHandle;
        }
        public ClothHandle GetClothHandle()
        {
            if (m_roleDisguiseHandle == null)
            {
                m_roleDisguiseHandle = new RoleDisguiseHandle();
                int roleId = hotApp.my.localPlayer.cfgInfo.id;
                int job = hotApp.my.localPlayer.carrerValue;
                int sex = hotApp.my.localPlayer.sexValue;
                m_roleDisguiseHandle.SetRoleAppearance(roleId, job, sex, m_AppearanceData);
            }
            return m_roleDisguiseHandle.m_clothHandle;
        }
        public HairHandle GetHairHandle()
        {
            if (m_roleDisguiseHandle == null)
            {
                m_roleDisguiseHandle = new RoleDisguiseHandle();
                int roleId = hotApp.my.localPlayer.cfgInfo.id;
                int job = hotApp.my.localPlayer.carrerValue;
                int sex = hotApp.my.localPlayer.sexValue;
                m_roleDisguiseHandle.SetRoleAppearance(roleId, job, sex, m_AppearanceData);
            }
            return m_roleDisguiseHandle.m_hairHandle;
        }
    }

}
#endif
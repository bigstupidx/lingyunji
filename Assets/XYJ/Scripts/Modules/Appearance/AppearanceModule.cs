
namespace xys
{
    using NetProto;
    using System.Reflection;
    
    public class AppearanceModule : HotModule
    {

        public static double TimeRatio = 24 * 60 * 60;
        public AppearanceModule() : base("xys.hot.HotAppearanceModule")
        {
        }
        
        private object m_appearanceMgr;
        public object appearanceMgr
        {
            get
            {
                if (null == m_appearanceMgr)
                    m_appearanceMgr = refType.GetField("appearanceMgr");

                return m_appearanceMgr;
            }
        }

        public AppearanceData GetAppearanceData()
        {
            if (appearanceMgr == null)
                return null;

            return refType.InvokeMethodReturn("GetAppearanceData") as AppearanceData;
        }

        public RoleDisguiseHandle GetDisguiseHandle()
        {
            if (appearanceMgr == null)
                return null;
            return refType.InvokeMethodReturn("GetDisguiHandle") as RoleDisguiseHandle;
        }

        public WeaponHandle GetWeaponHandle()
        {
            if (appearanceMgr == null)
                return null;
            return refType.InvokeMethodReturn("GetWeaponHandle") as WeaponHandle;
        }

        public RideHandle GetRideHandle()
        {
            if (appearanceMgr == null)
                return null;
            return refType.InvokeMethodReturn("GetRideHandle") as RideHandle;
        }
        public ClothHandle GetClothHandle()
        {
            if (appearanceMgr == null)
                return null;
            return refType.InvokeMethodReturn("GetClothHandle") as ClothHandle;
        }
        public HairHandle GetHairHandle()
        {
            if (appearanceMgr == null)
                return null;
            return refType.InvokeMethodReturn("GetHairHandle") as HairHandle;
        }

    }
}
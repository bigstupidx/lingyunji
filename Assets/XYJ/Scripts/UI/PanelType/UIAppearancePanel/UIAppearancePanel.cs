#if !USE_HOT
using xys.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Config;

namespace xys.hot.UI
{   
    class UIAppearancePanel : HotTablePanelBase
    {

       
        public enum Page
        {
            Cloth = 0,   //服饰
            HairDress = 1,  //头饰
            WeaponDress = 2,  //武饰
            Ride = 3,     //坐骑
            FaceMake=4,//易容
        }


        public UIAppearancePanel():base(null) { }
        public UIAppearancePanel(HotTablePanel _self):base(_self)
        {

        }


        [SerializeField]
        Button m_backBtn;

        HotAppearanceModule module;

        RoleClothData m_clothData;
        RoleHairData m_hairData;
        RoleWeaponData m_weaponData;
        RoleRideData m_rideData;
        RoleDisguiseHandle m_roleDisguiseHandle = null;
        public RoleTempApp m_roletempData = null;

        public RectTransform m_rttUIObject;
        public RectTransform m_rttRideUIObject;
        RTTModelPartHandler m_rttHandler = null;

        RTTModelPartHandler m_rttRideHandler = null;
        protected override void OnInit()
        {
            if(m_backBtn!=null)
            {
                m_backBtn.onClick.RemoveAllListeners();
                m_backBtn.onClick.AddListener(() =>
                {
                    parent.Hide(false);
                });
            }

            module = hotApp.my.GetModule<HotAppearanceModule>();
            AppearanceMgr appearanceMgr = module.GetMgr();
            m_clothData = appearanceMgr.GetClothHandle().m_roleClothData;
            m_hairData = appearanceMgr.GetHairHandle().m_roleHairData;
            m_weaponData = appearanceMgr.GetWeaponHandle().m_roleWeaponData;
            m_rideData = appearanceMgr.GetRideHandle().m_roleRideData;
            
            RoleSkinConfig.Cache();
            if (m_rttHandler == null)
            {
                m_rttHandler = new RTTModelPartHandler("RTT_Avatar", m_rttUIObject, true, Vector3.up*100, () =>
                {
                    GameObject.DontDestroyOnLoad(m_rttHandler.GetRTT().gameObject);
                });
            }
            if (m_rttRideHandler == null)
            {
                m_rttRideHandler = new RTTModelPartHandler("RTTModelPart", m_rttRideUIObject, true, Vector3.up * 200, () =>
                {
                    m_rttRideHandler.SetModelRotate(new Vector3(0, 225, 0));
                    GameObject.DontDestroyOnLoad(m_rttRideHandler.GetRTT().gameObject);
                });
            }
        }
        protected override void OnShow(object arg)
        {
            m_roleDisguiseHandle = module.GetMgr().GetDisguiHandle();
            m_rttHandler.LoadModelWithAppearence(m_roleDisguiseHandle);
            if (arg==null)
            {
                ShowPageByName("UIClothPage");
            }
            else
            {
                Item item = arg as Item;
                ClothItem tempCloth = m_roleDisguiseHandle.m_clothHandle.m_clothConfig.GetClothList()[0];
                int id = 0;
                int num = 0;
                tempCloth.GetUnlockInfo(out id,out num);
                if(item.id==id)
                {
                    ShowPageByName("UIClothPage");
                    return;
                }
                HairItem tempHair = m_roleDisguiseHandle.m_hairHandle.m_hairConfig.GetHairList()[0];
                tempHair.GetUnlockInfo(out id, out num);
                if (item.id!=id)
                {
                    ShowPageByName("UIHairPage");
                    return;
                }
                WeaponItem tempWeapon = m_roleDisguiseHandle.m_weaponHandle.m_weaponConfig.GetWeaponList()[0];
                tempWeapon.GetUnlockInfo(1, out id, out num);
                if (item.id != id)
                {
                    ShowPageByName("UIWeaponPage");
                    return;
                }
                
                if(RideDefine.Get(item.mountId)!=null)
                {
                    GetRoleTempApp();
                    m_roletempData.m_rideId = item.mountId;
                    object args = GetRoleTempApp();
                    ShowPageByName("UIRidePage");
                    return;
                }



            }
            
        }
        protected override bool OnPreChange(HotTablePage page)
        {
            ShowPageByName(page.pageType);
            return true;
        }
        void ShowPageByName(string pageType)
        {
            object arg = GetRoleTempApp();
            tableParent.ShowType(pageType, arg);
        }
        void ShowPageByName(string pageType,object arg)
        {
            tableParent.ShowType(pageType, arg);
        }
        protected override void OnHide()
        {
            m_roletempData = null;
            m_roleDisguiseHandle.m_clothHandle.ResetColor();
            m_roleDisguiseHandle.m_weaponHandle.ResetEffect();
            m_roleDisguiseHandle.m_rideHandle.ResetColor();
            if (m_rttHandler != null)
            {
                m_rttHandler.DestoryModel();
            }
            if(m_rttRideHandler!=null)
            {
                m_rttRideHandler.DestoryModel();
            }
        }

        RoleTempApp GetRoleTempApp()
        {
            if(m_roletempData==null)
            {
                m_roletempData = new RoleTempApp(m_clothData, m_hairData, m_weaponData, m_rideData,m_rttHandler,m_rttRideHandler);
            }  
            return m_roletempData;
        }

        protected override void OnDestroy()
        {
            if (m_rttHandler != null)
            {
                m_rttHandler.Destroy();
                m_rttHandler = null;
            }
            if (m_rttRideHandler != null)
            {
                m_rttRideHandler.Destroy();
                m_rttRideHandler = null;
            }

        }
    }
    class RoleTempApp
    {
        public int m_clothId;
        public int m_hairDressId;
        public int m_weaponId;
        public int m_rideId;
        public RTTModelPartHandler m_rttHandler;
        public RTTModelPartHandler m_rttRideHandler;
        public RoleTempApp(RoleClothData clothData,RoleHairData hairData,RoleWeaponData weaponData,RoleRideData rideData, RTTModelPartHandler rttHandler, RTTModelPartHandler rttRideHandler)
        {
            m_clothId = clothData.m_clothId;
            m_hairDressId = hairData.m_hairDressId;
            m_weaponId = weaponData.m_weaponId;
            m_rideId = rideData.m_curRide;
            m_rttHandler = rttHandler;
            m_rttRideHandler = rttRideHandler;
        }
    }
}
#endif
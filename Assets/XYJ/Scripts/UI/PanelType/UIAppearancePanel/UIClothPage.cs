#if !USE_HOT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;
using xys.UI.State;
using Config;
using NetProto;
namespace xys.hot.UI
{

    class UIClothPage : HotTablePageBase
    {
        public UIClothPage() : base(null) { }
        public UIClothPage(HotTablePage _page) : base(_page)
        {
        }
       
        [SerializeField]
        ILMonoBehaviour m_ilClothContent;
        UIClothContent m_clothContent;

        [SerializeField]
        ILMonoBehaviour m_ilColorMake;
        UIColorMake m_colorMake;

        [SerializeField]
        ILMonoBehaviour m_ilOperation;
        UIOperation m_operation;

        HotAppearanceModule module;
        ClothHandle m_clothHandle;
        private RoleTempApp m_roleTempData;

        protected override void OnInit()
        {
            if(m_ilClothContent!=null)
            {
                m_clothContent = m_ilClothContent.GetObject() as UIClothContent;  
            }
            if(m_ilColorMake!=null)
            {
                m_colorMake = m_ilColorMake.GetObject() as UIColorMake; 
            }
            if(m_ilOperation!=null)
            {
                m_operation = m_ilOperation.GetObject() as UIOperation;
            }
            module = hotApp.my.GetModule<HotAppearanceModule>();
            m_clothHandle = module.GetMgr().GetClothHandle();

        }
        protected override void OnShow(object arg)
        {
            if (arg == null) return;
            m_roleTempData = arg as RoleTempApp;
            m_roleTempData.m_rttHandler.SetRenderActive(true);
            m_clothContent.m_roleTempData = m_roleTempData;
            m_clothContent.m_eventAgent = Event;
            m_colorMake.m_eventAgent = Event;
            m_clothContent.OpenContent();
            
        }
        protected override void OnHide()
        {
            if (m_operation.m_isChangeRole)
            {
                NetProto.WearRequest request = new NetProto.WearRequest();
                request.aprType = NetProto.AprType.Cloth;
                request.itemId = m_clothHandle.m_roleClothData.m_clothId;
                request.colorIndex = m_clothHandle.m_roleClothData.m_curColor;
                module.Event.FireEvent<NetProto.WearRequest>(EventID.Ap_WearItem, request);
                m_operation.m_isChangeRole = false;
        
            }
            m_clothContent.CloseContent();
        }

    }  
}
#endif

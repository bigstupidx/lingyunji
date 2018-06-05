#if USE_HOT
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {
        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Int32_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Int64_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Array_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            Debuger_Binding.Register(app);
            wProtobuf_MessageStream_Binding.Register(app);
            Network_BitStream_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            xys_App_Binding.Register(app);
            xys_UI_UISystem_Binding.Register(app);
            xys_ProtocolHandler_Binding.Register(app);
            xys_GateSocket_Binding.Register(app);
            xys_UI_UIPanelBase_Binding.Register(app);
            xys_UI_UIHotPanel_Binding.Register(app);
            xys_UI_HotTablePanel_Binding.Register(app);
            xys_UI_HotTablePage_Binding.Register(app);
            xys_ModuleBase_Binding.Register(app);
            xys_LocalPlayer_Binding.Register(app);
            xys_EventSet_Binding.Register(app);
        }
    }
}
#endif

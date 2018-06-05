#if USE_HOT
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class xys_UI_UIPanelBase_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(xys.UI.UIPanelBase);
            args = new Type[]{};
            method = type.GetMethod("get_panelType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_panelType_0);
            args = new Type[]{};
            method = type.GetMethod("get_ShowPanelFrameCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ShowPanelFrameCount_1);
            args = new Type[]{};
            method = type.GetMethod("get_PanelName", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PanelName_2);
            args = new Type[]{};
            method = type.GetMethod("get_Event", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Event_3);
            args = new Type[]{};
            method = type.GetMethod("get_state", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_state_4);
            args = new Type[]{};
            method = type.GetMethod("get_isFixedDepth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isFixedDepth_5);
            args = new Type[]{};
            method = type.GetMethod("get_cameraType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_cameraType_6);
            args = new Type[]{};
            method = type.GetMethod("get_panelHeadRoot", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_panelHeadRoot_7);
            args = new Type[]{};
            method = type.GetMethod("get_canvas", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_canvas_8);
            args = new Type[]{};
            method = type.GetMethod("get_cachedGameObject", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_cachedGameObject_9);
            args = new Type[]{};
            method = type.GetMethod("get_cachedTransform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_cachedTransform_10);
            args = new Type[]{};
            method = type.GetMethod("get_isVisible", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isVisible_11);
            args = new Type[]{};
            method = type.GetMethod("get_isClickSpaceClose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isClickSpaceClose_12);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_isClickSpaceClose", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_isClickSpaceClose_13);
            args = new Type[]{};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_14);
            args = new Type[]{};
            method = type.GetMethod("get_temporaryHide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_temporaryHide_15);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_temporaryHide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_temporaryHide_16);
            args = new Type[]{};
            method = type.GetMethod("get_isBlurExcep", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_isBlurExcep_17);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_isBlurExcep", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_isBlurExcep_18);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("SetDepth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetDepth_19);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("AddDepth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddDepth_20);
            args = new Type[]{};
            method = type.GetMethod("DefaultDepth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DefaultDepth_21);
            args = new Type[]{};
            method = type.GetMethod("get_currentDepth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_currentDepth_22);
            args = new Type[]{};
            method = type.GetMethod("get_maxDepth", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_maxDepth_23);
            args = new Type[]{};
            method = type.GetMethod("MoveToFront", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MoveToFront_24);
            args = new Type[]{typeof(System.Object), typeof(System.Boolean)};
            method = type.GetMethod("Show", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Show_25);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Hide", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Hide_26);
            args = new Type[]{};
            method = type.GetMethod("DestroySelf", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DestroySelf_27);
            args = new Type[]{};
            method = type.GetMethod("IsCanCloseByBtn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsCanCloseByBtn_28);

            field = type.GetField("isShowGravity", flag);
            app.RegisterCLRFieldGetter(field, get_isShowGravity_0);
            app.RegisterCLRFieldSetter(field, set_isShowGravity_0);
            field = type.GetField("isShowPanelHead", flag);
            app.RegisterCLRFieldGetter(field, get_isShowPanelHead_1);
            app.RegisterCLRFieldSetter(field, set_isShowPanelHead_1);
            field = type.GetField("panelHeadState", flag);
            app.RegisterCLRFieldGetter(field, get_panelHeadState_2);
            app.RegisterCLRFieldSetter(field, set_panelHeadState_2);
            field = type.GetField("isHideMainPanel", flag);
            app.RegisterCLRFieldGetter(field, get_isHideMainPanel_3);
            app.RegisterCLRFieldSetter(field, set_isHideMainPanel_3);
            field = type.GetField("isRecordingPanel", flag);
            app.RegisterCLRFieldGetter(field, get_isRecordingPanel_4);
            app.RegisterCLRFieldSetter(field, set_isRecordingPanel_4);
            field = type.GetField("panelName", flag);
            app.RegisterCLRFieldGetter(field, get_panelName_5);
            app.RegisterCLRFieldSetter(field, set_panelName_5);
            field = type.GetField("isExclusive", flag);
            app.RegisterCLRFieldGetter(field, get_isExclusive_6);
            app.RegisterCLRFieldSetter(field, set_isExclusive_6);
            field = type.GetField("openSound", flag);
            app.RegisterCLRFieldGetter(field, get_openSound_7);
            app.RegisterCLRFieldSetter(field, set_openSound_7);
            field = type.GetField("closeSound", flag);
            app.RegisterCLRFieldGetter(field, get_closeSound_8);
            app.RegisterCLRFieldSetter(field, set_closeSound_8);
            field = type.GetField("startBlurExcep", flag);
            app.RegisterCLRFieldGetter(field, get_startBlurExcep_9);
            app.RegisterCLRFieldSetter(field, set_startBlurExcep_9);


            app.RegisterCLRCreateArrayInstance(type, s => new xys.UI.UIPanelBase[s]);


        }


        static StackObject* get_panelType_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.panelType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_ShowPanelFrameCount_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ShowPanelFrameCount;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_PanelName_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.PanelName;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Event_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Event;

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_state_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.state;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_isFixedDepth_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isFixedDepth;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_cameraType_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.cameraType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_panelHeadRoot_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.panelHeadRoot;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_canvas_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.canvas;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_cachedGameObject_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.cachedGameObject;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_cachedTransform_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.cachedTransform;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_isVisible_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isVisible;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_isClickSpaceClose_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isClickSpaceClose;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_isClickSpaceClose_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean value = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.isClickSpaceClose = value;

            return __ret;
        }

        static StackObject* Init_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Init();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_temporaryHide_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.temporaryHide;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_temporaryHide_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean value = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.temporaryHide = value;

            return __ret;
        }

        static StackObject* get_isBlurExcep_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.isBlurExcep;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* set_isBlurExcep_18(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean value = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.isBlurExcep = value;

            return __ret;
        }

        static StackObject* SetDepth_19(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 depth = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SetDepth(depth);

            return __ret;
        }

        static StackObject* AddDepth_20(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single depth = *(float*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddDepth(depth);

            return __ret;
        }

        static StackObject* DefaultDepth_21(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DefaultDepth();

            return __ret;
        }

        static StackObject* get_currentDepth_22(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.currentDepth;

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_maxDepth_23(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.maxDepth;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* MoveToFront_24(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MoveToFront();

            return __ret;
        }

        static StackObject* Show_25(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean isPlayAnim = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Object args = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Show(args, isPlayAnim);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Hide_26(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean isPlayAnim = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Hide(isPlayAnim);

            return __ret;
        }

        static StackObject* DestroySelf_27(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DestroySelf();

            return __ret;
        }

        static StackObject* IsCanCloseByBtn_28(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            xys.UI.UIPanelBase instance_of_this_method;
            instance_of_this_method = (xys.UI.UIPanelBase)typeof(xys.UI.UIPanelBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsCanCloseByBtn();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }


        static object get_isShowGravity_0(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).isShowGravity;
        }
        static void set_isShowGravity_0(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).isShowGravity = (System.Boolean)v;
        }
        static object get_isShowPanelHead_1(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).isShowPanelHead;
        }
        static void set_isShowPanelHead_1(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).isShowPanelHead = (System.Boolean)v;
        }
        static object get_panelHeadState_2(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).panelHeadState;
        }
        static void set_panelHeadState_2(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).panelHeadState = (System.Int32)v;
        }
        static object get_isHideMainPanel_3(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).isHideMainPanel;
        }
        static void set_isHideMainPanel_3(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).isHideMainPanel = (System.Boolean)v;
        }
        static object get_isRecordingPanel_4(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).isRecordingPanel;
        }
        static void set_isRecordingPanel_4(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).isRecordingPanel = (System.Boolean)v;
        }
        static object get_panelName_5(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).panelName;
        }
        static void set_panelName_5(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).panelName = (System.String)v;
        }
        static object get_isExclusive_6(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).isExclusive;
        }
        static void set_isExclusive_6(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).isExclusive = (System.Boolean)v;
        }
        static object get_openSound_7(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).openSound;
        }
        static void set_openSound_7(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).openSound = (System.String)v;
        }
        static object get_closeSound_8(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).closeSound;
        }
        static void set_closeSound_8(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).closeSound = (System.String)v;
        }
        static object get_startBlurExcep_9(ref object o)
        {
            return ((xys.UI.UIPanelBase)o).startBlurExcep;
        }
        static void set_startBlurExcep_9(ref object o, object v)
        {
            ((xys.UI.UIPanelBase)o).startBlurExcep = (System.Boolean)v;
        }



    }
}
#endif

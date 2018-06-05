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
    unsafe class wProtobuf_MessageStream_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(wProtobuf.MessageStream);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("SkipLastField", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SkipLastField_0);
            args = new Type[]{};
            method = type.GetMethod("ReadTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadTag_1);
            args = new Type[]{};
            method = type.GetMethod("ReadInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadInt32_2);
            args = new Type[]{};
            method = type.GetMethod("ReadUInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadUInt32_3);
            args = new Type[]{};
            method = type.GetMethod("ReadInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadInt64_4);
            args = new Type[]{};
            method = type.GetMethod("ReadUInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadUInt64_5);
            args = new Type[]{};
            method = type.GetMethod("ReadString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadString_6);
            args = new Type[]{};
            method = type.GetMethod("ReadBool", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadBool_7);
            args = new Type[]{};
            method = type.GetMethod("ReadDouble", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadDouble_8);
            args = new Type[]{};
            method = type.GetMethod("ReadFloat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadFloat_9);
            args = new Type[]{};
            method = type.GetMethod("ReadSInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadSInt32_10);
            args = new Type[]{};
            method = type.GetMethod("ReadSInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadSInt64_11);
            args = new Type[]{};
            method = type.GetMethod("ReadEnum", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadEnum_12);
            args = new Type[]{};
            method = type.GetMethod("ReadFixed32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadFixed32_13);
            args = new Type[]{};
            method = type.GetMethod("ReadFixed64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadFixed64_14);
            args = new Type[]{};
            method = type.GetMethod("ReadSFixed32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadSFixed32_15);
            args = new Type[]{};
            method = type.GetMethod("ReadSFixed64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadSFixed64_16);
            args = new Type[]{};
            method = type.GetMethod("ReadBytes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadBytes_17);
            args = new Type[]{typeof(wProtobuf.IMessage)};
            method = type.GetMethod("ReadMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadMessage_18);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("ReadMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadMessage_19);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("ReadBytes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadBytes_20);
            args = new Type[]{typeof(System.Byte[])};
            method = type.GetMethod("ReaderCreate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReaderCreate_21);
            args = new Type[]{};
            method = type.GetMethod("get_WriteRemain", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_WriteRemain_22);
            args = new Type[]{};
            method = type.GetMethod("get_ReadPos", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ReadPos_23);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_ReadPos", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ReadPos_24);
            args = new Type[]{};
            method = type.GetMethod("get_WritePos", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_WritePos_25);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_WritePos", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_WritePos_26);
            args = new Type[]{};
            method = type.GetMethod("get_ReadSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ReadSize_27);
            args = new Type[]{};
            method = type.GetMethod("get_size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_size_28);
            args = new Type[]{};
            method = type.GetMethod("get_Buffer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Buffer_29);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("ensureCapacity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ensureCapacity_30);
            args = new Type[]{};
            method = type.GetMethod("Reset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Reset_31);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("Move", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Move_32);
            args = new Type[]{typeof(System.Byte)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_33);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_34);
            args = new Type[]{typeof(System.Byte[])};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_35);
            args = new Type[]{typeof(System.Int16)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_36);
            args = new Type[]{typeof(System.UInt16)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_37);
            args = new Type[]{};
            method = type.GetMethod("ReadByte", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadByte_38);
            args = new Type[]{typeof(System.Byte[]), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("ReadBytes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadBytes_39);
            args = new Type[]{};
            method = type.GetMethod("ReadInt16", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadInt16_40);
            args = new Type[]{};
            method = type.GetMethod("ReadUInt16", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadUInt16_41);
            args = new Type[]{typeof(System.Byte[]), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetMethod("Write", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Write_42);
            args = new Type[]{};
            method = type.GetMethod("ReadLength", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ReadLength_43);
            args = new Type[]{typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_44);
            args = new Type[]{typeof(System.Byte), typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_45);
            args = new Type[]{typeof(System.Byte), typeof(System.Byte), typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_46);
            args = new Type[]{typeof(System.Byte), typeof(System.Byte), typeof(System.Byte), typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_47);
            args = new Type[]{typeof(System.Byte), typeof(System.Byte), typeof(System.Byte), typeof(System.Byte), typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_48);
            args = new Type[]{typeof(System.Byte[])};
            method = type.GetMethod("WriteRaw", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRaw_49);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteInt32_50);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("WriteInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteInt64_51);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("WriteUInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteUInt32_52);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("WriteUInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteUInt64_53);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteLength", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteLength_54);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteEnum", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteEnum_55);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("WriteString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteString_56);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("WriteBool", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteBool_57);
            args = new Type[]{typeof(System.Double)};
            method = type.GetMethod("WriteDouble", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteDouble_58);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("WriteFloat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteFloat_59);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteSInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteSInt32_60);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("WriteSInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteSInt64_61);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("WriteFixed32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteFixed32_62);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("WriteFixed64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteFixed64_63);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteSFixed32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteSFixed32_64);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("WriteSFixed64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteSFixed64_65);
            args = new Type[]{typeof(wProtobuf.ByteString)};
            method = type.GetMethod("WriteBytes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteBytes_66);
            args = new Type[]{typeof(wProtobuf.IMessage)};
            method = type.GetMethod("WriteMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteMessage_67);



            app.RegisterCLRCreateArrayInstance(type, s => new wProtobuf.MessageStream[s]);

            args = new Type[]{typeof(System.Byte[])};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_1);

        }


        static StackObject* SkipLastField_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 tag = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SkipLastField(tag);

            return __ret;
        }

        static StackObject* ReadTag_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadTag();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadInt32_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadInt32();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadUInt32_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadUInt32();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadInt64_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadInt64();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadUInt64_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadUInt64();

            __ret->ObjectType = ObjectTypes.Long;
            *(ulong*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadString_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadString();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ReadBool_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadBool();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* ReadDouble_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadDouble();

            __ret->ObjectType = ObjectTypes.Double;
            *(double*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadFloat_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadFloat();

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadSInt32_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadSInt32();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadSInt64_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadSInt64();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadEnum_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadEnum();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadFixed32_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadFixed32();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = (int)result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadFixed64_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadFixed64();

            __ret->ObjectType = ObjectTypes.Long;
            *(ulong*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadSFixed32_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadSFixed32();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadSFixed64_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadSFixed64();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadBytes_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadBytes();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ReadMessage_18(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.IMessage builder = (wProtobuf.IMessage)typeof(wProtobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReadMessage(builder);

            return __ret;
        }

        static StackObject* ReadMessage_19(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action fun = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReadMessage(fun);

            return __ret;
        }

        static StackObject* ReadBytes_20(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 lenght = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadBytes(lenght);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ReaderCreate_21(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] bytes = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = wProtobuf.MessageStream.ReaderCreate(bytes);

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_WriteRemain_22(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.WriteRemain;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ReadPos_23(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadPos;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_ReadPos_24(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 value = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReadPos = value;

            return __ret;
        }

        static StackObject* get_WritePos_25(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.WritePos;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_WritePos_26(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 value = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WritePos = value;

            return __ret;
        }

        static StackObject* get_ReadSize_27(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadSize;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_size_28(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.size;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Buffer_29(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Buffer;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ensureCapacity_30(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 length = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ensureCapacity(length);

            return __ret;
        }

        static StackObject* Reset_31(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Reset();

            return __ret;
        }

        static StackObject* Move_32(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 offset = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Move(offset);

            return __ret;
        }

        static StackObject* Write_33(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte value = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(value);

            return __ret;
        }

        static StackObject* Write_34(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean value = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(value);

            return __ret;
        }

        static StackObject* Write_35(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] buffer = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(buffer);

            return __ret;
        }

        static StackObject* Write_36(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int16 value = (short)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(value);

            return __ret;
        }

        static StackObject* Write_37(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt16 value = (ushort)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(value);

            return __ret;
        }

        static StackObject* ReadByte_38(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadByte();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadBytes_39(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 count = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 offset = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Byte[] bytes = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReadBytes(bytes, offset, count);

            return __ret;
        }

        static StackObject* ReadInt16_40(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadInt16();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ReadUInt16_41(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadUInt16();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Write_42(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 count = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 offset = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Byte[] data = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Write(data, offset, count);

            return __ret;
        }

        static StackObject* ReadLength_43(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ReadLength();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* WriteRawTag_44(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte b1 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(b1);

            return __ret;
        }

        static StackObject* WriteRawTag_45(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte b2 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte b1 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(b1, b2);

            return __ret;
        }

        static StackObject* WriteRawTag_46(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte b3 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte b2 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Byte b1 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(b1, b2, b3);

            return __ret;
        }

        static StackObject* WriteRawTag_47(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte b4 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte b3 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Byte b2 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Byte b1 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(b1, b2, b3, b4);

            return __ret;
        }

        static StackObject* WriteRawTag_48(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte b5 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte b4 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Byte b3 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Byte b2 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Byte b1 = (byte)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(b1, b2, b3, b4, b5);

            return __ret;
        }

        static StackObject* WriteRaw_49(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] bytes = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRaw(bytes);

            return __ret;
        }

        static StackObject* WriteInt32_50(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 value = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteInt32(value);

            return __ret;
        }

        static StackObject* WriteInt64_51(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 value = *(long*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteInt64(value);

            return __ret;
        }

        static StackObject* WriteUInt32_52(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 value = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteUInt32(value);

            return __ret;
        }

        static StackObject* WriteUInt64_53(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 value = *(ulong*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteUInt64(value);

            return __ret;
        }

        static StackObject* WriteLength_54(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 length = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteLength(length);

            return __ret;
        }

        static StackObject* WriteEnum_55(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 value = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteEnum(value);

            return __ret;
        }

        static StackObject* WriteString_56(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteString(value);

            return __ret;
        }

        static StackObject* WriteBool_57(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean value = ptr_of_this_method->Value == 1;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteBool(value);

            return __ret;
        }

        static StackObject* WriteDouble_58(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double value = *(double*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteDouble(value);

            return __ret;
        }

        static StackObject* WriteFloat_59(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single value = *(float*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteFloat(value);

            return __ret;
        }

        static StackObject* WriteSInt32_60(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 value = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteSInt32(value);

            return __ret;
        }

        static StackObject* WriteSInt64_61(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 value = *(long*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteSInt64(value);

            return __ret;
        }

        static StackObject* WriteFixed32_62(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 value = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteFixed32(value);

            return __ret;
        }

        static StackObject* WriteFixed64_63(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 value = *(ulong*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteFixed64(value);

            return __ret;
        }

        static StackObject* WriteSFixed32_64(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 value = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteSFixed32(value);

            return __ret;
        }

        static StackObject* WriteSFixed64_65(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 value = *(long*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteSFixed64(value);

            return __ret;
        }

        static StackObject* WriteBytes_66(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.ByteString value = (wProtobuf.ByteString)typeof(wProtobuf.ByteString).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteBytes(value);

            return __ret;
        }

        static StackObject* WriteMessage_67(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            wProtobuf.IMessage value = (wProtobuf.IMessage)typeof(wProtobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            wProtobuf.MessageStream instance_of_this_method;
            instance_of_this_method = (wProtobuf.MessageStream)typeof(wProtobuf.MessageStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteMessage(value);

            return __ret;
        }




        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] data = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = new wProtobuf.MessageStream(data);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Ctor_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 capacity = ptr_of_this_method->Value;

            var result_of_this_method = new wProtobuf.MessageStream(capacity);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
#endif

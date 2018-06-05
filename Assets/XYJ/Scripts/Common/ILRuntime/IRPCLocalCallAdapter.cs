// #if USE_HOT
// using System;
// using ILRuntime.Runtime.Enviorment;
// using ILRuntime.Runtime.Intepreter;
// 
// public class IRPCLocalCallAdapter : CrossBindingAdaptor
// {
//     public override Type BaseCLRType
//     {
//         get
//         {
//             return typeof(wProtobuf.RPC.ILocalCall);
//         }
//     }
// 
//     public override Type[] BaseCLRTypes
//     {
//         get
//         {
//             return null;
//         }
//     }
// 
//     public override Type AdaptorType
//     {
//         get
//         {
//             return typeof(Adaptor);
//         }
//     }
// 
//     public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
//     {
//         return new Adaptor(appdomain, instance);
//     }
// 
//     internal class Adaptor : wProtobuf.RPC.ILocalCall, CrossBindingAdaptorType
//     {
//         ILTypeInstance instance;
//         ILRuntime.Runtime.Enviorment.AppDomain appdomain;
// 
//         public Adaptor()
//         {
// 
//         }
// 
//         public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
//         {
//             this.appdomain = appdomain;
//             this.instance = instance;
//         }
// 
//         public ILTypeInstance ILInstance { get { return instance; } }
// 
//         void wProtobuf.RPC.ILocalCall.Call<Param, Result>(string key, Param param, Action<wProtobuf.RPC.Error, Result> onEnd)
//         {
//             var method = instance.Type.GetMethod("Call", new );
//             appdomain.Invoke(method, instance, key, param, onEnd);
//         }
// 
//         void wProtobuf.RPC.ILocalCall.Call(string key, Action<wProtobuf.RPC.Error> fun)
//         {
//             var method = instance.Type.GetMethod("Call", 2);
//             appdomain.Invoke(method, instance, key);
//         }
// 
//         void wProtobuf.RPC.ILocalCall.Call<Result>(string key, Action<wProtobuf.RPC.Error, Result> fun)
//         {
//             var method = instance.Type.GetMethod("Call", 2, );
//             appdomain.Invoke(method, instance, key);
//         }
// 
//         void wProtobuf.RPC.ILocalCall.Call<Param>(string key, Param param, Action<wProtobuf.RPC.Error> fun)
//         {
// 
//         }
//     }
// }
// #endif
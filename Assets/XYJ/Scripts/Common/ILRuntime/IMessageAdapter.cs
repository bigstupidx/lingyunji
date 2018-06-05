// #if USE_HOT
// using System;
// using ILRuntime.Runtime.Enviorment;
// using ILRuntime.Runtime.Intepreter;
// 
// public class IMessageAdapter : CrossBindingAdaptor
// {
//     public override Type BaseCLRType
//     {
//         get
//         {
//             return typeof(wProtobuf.IMessage);
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
//     internal class Adaptor : wProtobuf.IMessage, CrossBindingAdaptorType
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
//         int wProtobuf.IMessage.CalculateSize()
//         {
//             var method = instance.Type.GetMethod("CalculateSize", 0);
//             return (int)appdomain.Invoke(method, instance);
//         }
// 
//         void wProtobuf.IMessage.MergeFrom(wProtobuf.IReadStream input)
//         {
//             var method = instance.Type.GetMethod("MergeFrom", 1);
//             appdomain.Invoke(method, instance, input);
//         }
// 
//         void wProtobuf.IMessage.WriteTo(wProtobuf.IWriteStream output)
//         {
//             var method = instance.Type.GetMethod("WriteTo", 1);
//             appdomain.Invoke(method, instance, output);
//         }
//     }
// }
// #endif
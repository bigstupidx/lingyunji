#if USE_HOT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    public static class GenerateCLRBinding
    {
        [UnityEditor.MenuItem("PackTool/ILRuntime/Generate")]
        static void Generate()
        {
            List<System.Type> types = new List<System.Type>();
            types.Add(typeof(int));
            types.Add(typeof(float));
            types.Add(typeof(long));
            types.Add(typeof(object));
            types.Add(typeof(string));
            types.Add(typeof(System.Array));
            types.Add(typeof(Vector2));
            types.Add(typeof(Vector3));
            types.Add(typeof(Quaternion));
            types.Add(typeof(GameObject));
            types.Add(typeof(Object));
            types.Add(typeof(Transform));
            types.Add(typeof(RectTransform));
            types.Add(typeof(Time));
            types.Add(typeof(Debug));
            types.Add(typeof(Debuger));
            types.Add(typeof(wProtobuf.MessageStream));
            types.Add(typeof(Network.BitStream));

            //所有DLL内的类型的真实C#类型都是ILTypeInstance
            types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

            types.Add(typeof(App));
            types.Add(typeof(UI.UISystem));
            types.Add(typeof(ProtocolHandler));
            types.Add(typeof(GateSocket));
            types.Add(typeof(UI.UIPanelBase));
            types.Add(typeof(UI.UIHotPanel));
            types.Add(typeof(UI.HotTablePanel));
            types.Add(typeof(UI.HotTablePage));
            types.Add(typeof(ModuleBase));
            types.Add(typeof(LocalPlayer));
            types.Add(typeof(EventSet));

            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, "Assets/Scripts/Common/ILRuntime/AutoCode");
        }

//         [UnityEditor.MenuItem("PackTool/ILRuntime/GenerateBindingCode")]
//         static void GenerateBindingCode()
//         {
//             ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(ILEditor.appdomain, "Assets/Scripts/Common/ILRuntime/AutoCode/BindingCode.cs");
//         }
    }
}
#endif
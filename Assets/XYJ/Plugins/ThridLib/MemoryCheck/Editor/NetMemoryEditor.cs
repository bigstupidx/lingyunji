using UnityEngine;
using UnityEditor;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

#if MEMORY_CHECK
partial class NetMemoryEditor : NetDebugBaseEditor<MemoryInfo>
{
    static NetMemoryEditor()
    {
        protocol = DebugProtocol.ObjectMemory;

        ReadStream = (Network.BitStream bs) => 
        {
            MemoryInfo info = new MemoryInfo();
            info.Read(bs);
            return info;
        };

        Compare = (MemoryInfo x, MemoryInfo y) => { return x == y; };
    }

    static public void Open()
    {
        GetWindow<NetMemoryEditor>(false, "NetMemoryEditor", true);
    }

    protected override void BeginCompare(MemoryInfo x, MemoryInfo y)
    {
        MemoryDifferEditor.Open().Show(x, y);
    }

    protected override void Show(MemoryInfo x, ParamList pl, bool isForce)
    {
        MemoryGUIShow sss = pl.Get<MemoryGUIShow>("MemoryGUIShow");
        sss.mMemoryInfo = x;
        sss.OnGUI(isForce);
    }
}
#endif
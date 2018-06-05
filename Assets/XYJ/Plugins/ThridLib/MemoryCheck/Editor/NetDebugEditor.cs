using UnityEngine;
using UnityEditor;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

#if MEMORY_CHECK

partial class NetDebugEditor : BaseEditorWindow
{
    [MenuItem("PackTool/调试/真机", false, 9)]
    [MenuItem("Assets/调试/真机", false, 0)]
    static public void Open()
    {
        GetWindow<NetDebugEditor>(false, "NetDebugEditor", true);
    }

    static NetChannel sChannel;

    public static NetChannel Channel
    {
        get { return sChannel; }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (sChannel != null)
        {
            sChannel.Release();
            sChannel = null;
        }

        var addressList = Dns.GetHostAddresses(Dns.GetHostName());
        if (addressList.Length == 0)
        {
            Debug.LogErrorFormat("获取本机IP地址失败!");
            return;
        }

        IPAddress address = addressList[0];
        for (int i = 1; i < addressList.Length; ++i)
        {
            if (addressList[i].IsIPv6LinkLocal)
                continue;

            address = addressList[i];
            break;
        }

        sChannel = new NetChannel();
        sChannel.Start(address.ToString(), 55555);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (sChannel != null)
        {
            sChannel.Release();
            sChannel = null;
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("打开内存对象面板"))
        {
            NetMemoryEditor.Open();
        }

        if (GUILayout.Button("资源查看面板"))
        {
            NetRunningAssetsEditor.Open();
        }
    }
}
#endif
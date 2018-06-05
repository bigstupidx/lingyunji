using PackTool;
using UnityEngine;
using UnityEditor;

#if MEMORY_CHECK
partial class NetRunningAssetsEditor : NetDebugBaseEditor<AssetSyncInfo>
{
    static NetRunningAssetsEditor()
    {
        protocol = DebugProtocol.AllAsset;

        ReadStream = (Network.BitStream bs) => 
        {
            AssetSyncInfo info = new AssetSyncInfo();
            info.Reader(bs);
            return info;
        };

        Compare = (AssetSyncInfo x, AssetSyncInfo y) => { return x == y; };
    }

    static public void Open()
    {
        GetWindow<NetRunningAssetsEditor>(false, "NetRunningAssetsEditor", true);
    }

    protected override void BeginCompare(AssetSyncInfo x, AssetSyncInfo y)
    {
        RunningAssetsDifferEditor.Open().Show(x, y);
    }

    protected override void Show(AssetSyncInfo x, ParamList pl, bool isForce)
    {
        RunningAssetsShow sss = pl.Get<RunningAssetsShow>("RunningAssetsShow");
        sss.mInfo = x;
        sss.OnGUI();
    }
}
#endif
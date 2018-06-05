#if MEMORY_CHECK
using XTools;
using Network;

// 加载基础资源
namespace PackTool
{
    public class AssetSyncMgr : Singleton<AssetSyncMgr>
    {
        static public bool isDirty = false; // 资源是否有变化

        int timerid = 0;

        public void Start()
        {
            if (timerid == 0)
            {
                MagicThread.StartForeground(()=> 
                {
                    timerid = TimerMgrObj.Instance.register_timer(1f, int.MaxValue, Update);
                });
            }
        }

        public void Release()
        {
            if (timerid != 0)
                TimerMgrObj.Instance.cannel_timer(timerid);
            timerid = 0;
        }

        void Update(object p)
        {
            if (!isDirty)
                return;

            isDirty = false;
            AssetSyncInfo info = new AssetSyncInfo();
            info.ticks = System.DateTime.Now.Ticks;
            info.textures.AddRange(TextureLoad.GetAllList().Keys);
            info.materials.AddRange(MaterialLoad.GetAllList().Keys);
            info.atlas.AddRange(AtlasLoad.GetAllList().Keys);
            info.meshs.AddRange(MeshLoad.GetAllList().Keys);
            info.controls.AddRange(ResourcesLoad<UnityEngine.RuntimeAnimatorController>.GetAllList().Keys);
            info.avatars.AddRange(ResourcesLoad<UnityEngine.Avatar>.GetAllList().Keys);
            info.audioclips.AddRange(AudioClipLoad.GetAllList().Keys);

            foreach (var itor in PrefabBeh.GetAll())
                info.mPrefabs.Add(itor.Value);

            DebugNet.Instance.Send(DebugProtocol.AllAsset, (BitStream bitStream) =>
            {
                info.Write(bitStream);
            });
        }
    }
}
#endif
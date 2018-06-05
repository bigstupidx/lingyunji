#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    using wProtobuf;

    partial class HotEquipSoulModule : HotModuleBase
    {
        public EquipSoulMgr equipSoulMgr = new EquipSoulMgr();
        public HotEquipSoulModule(ModuleBase parent) : base(parent)
        {
        }

        protected override void OnAwake()
        {
            
        }

        protected override void OnDeserialize(IReadStream output)
        {
            equipSoulMgr.soulGrids.MergeFrom(output);
        }
    }
}
#endif

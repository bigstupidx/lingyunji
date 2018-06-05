#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;

    partial class HotEquipModule : HotModuleBase
    {
        EquipMgr equipMgr = new EquipMgr();

        public HotEquipModule(EquipModule m) : base(m)
        {
        }

        protected override void OnAwake()
        {
            this.Init();
        }

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            equipMgr.equipTable.MergeFrom(output);
        }
    }
}
#endif
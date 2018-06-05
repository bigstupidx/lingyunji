#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;
    partial class HotPetsModule : HotModuleBase
    {
        public PetsMgr petsMgr { get; private set; }

        public HotPetsModule(PetsModule m) : base(m)
        {
            petsMgr = new PetsMgr();
        }

        protected override void OnAwake()
        {
            Init();
        }

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            this.petsMgr.m_PetsTable.MergeFrom(output);
        }

        int GetPetCount()
        {
            return petsMgr.GetPetCount();
        }

        int GetCurrentPetId()
        {
            return petsMgr.GetFightPetID();
        }

        string GetCurrentPetNIckName()
        {
            return petsMgr.GetCurrentPetNickName();
        }
    }
}
#endif
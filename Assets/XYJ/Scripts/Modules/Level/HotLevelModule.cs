#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using Network;

    partial class HotLevelModule : HotModuleBase
    {
        LevelMgr m_levelMgr = new LevelMgr();
        public HotLevelModule(LevelModule m) : base(m)
        {

        }

        protected override void OnAwake()
        {
            m_levelMgr.OnInit(Event);
        }

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            m_levelMgr.levelData.MergeFrom(output);
        }
    }
}
#endif

#if !USE_HOT
namespace xys.hot
{
    using NetProto;

    partial class HotSkillModule : HotModuleBase
    {
        SkillMgr skillMgr = new SkillMgr();

        public HotSkillModule(SkillModule m) : base(m)
        {
        }

        protected override void OnAwake()
        {
            Init();
        }

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            if (output != null && skillMgr.m_SkillSchemeDBData != null)
            {
                skillMgr.m_SkillSchemeDBData.MergeFrom(output);
            }
        }

        public bool IsItemComprehend(int itemId)
        {
            for (int i = 0; i < skillMgr.m_SkillSchemeDBData.comprehendItems.Count; i++)
            {
                if (skillMgr.m_SkillSchemeDBData.comprehendItems[i] == itemId)
                    return true;
            }
            return false;
        }
    }
}
#endif
#if !USE_HOT
namespace xys.hot
{
    using NetProto;

    public class SkillMgr
    {
        public SkillSchemeDBData m_SkillSchemeDBData { get; private set; }

        public SkillMgr()
        {
            m_SkillSchemeDBData = new SkillSchemeDBData();
        }
    }
}
#endif
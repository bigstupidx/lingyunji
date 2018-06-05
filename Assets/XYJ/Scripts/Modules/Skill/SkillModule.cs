namespace xys
{
    using NetProto;
    using System.Reflection;

    public class SkillModule : HotModule
    {
        public SkillModule() : base("xys.hot.HotSkillModule")
        {
        }

        public object skillMgr { get { return refType.GetField("skillMgr"); } }

        public bool IsItemComprehend(int itemId)
        {
            return (bool)refType.InvokeMethodReturn("IsItemComprehend", itemId);
        }
    }
}
namespace xys
{
    using NetProto;
    using System.Reflection;

    public partial class TrumpsModule : HotModule
    {
        public TrumpsModule() : base("xys.hot.HotTrumpsModule")
        {
        }
        public object trumpMgr
        {
            get
            {
                return refType.GetField("trumpsMgr");
            }
        }
    }
}
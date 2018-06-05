namespace xys
{
    using NetProto;
    using System.Reflection;

    public partial class DemonplotsModule : HotModule
    {
        public DemonplotsModule() : base("xys.hot.HotDemonplotsModule")
        {

        }

        public object demonplotMgr
        {
            get
            {
                return refType.GetField("demonplotsMgr");
            }
        }
    }
}

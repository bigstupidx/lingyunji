namespace xys
{
    using NetProto;
    using System.Reflection;

    public partial class EquipSoulModule : HotModule
    {
        public EquipSoulModule() : base("xys.hot.HotEquipSoulModule")
        {

        }

        public object equipSoulMgr
        {
            get
            {
                return refType.GetField("equipSoulMgr");
            }
        }
    }
}

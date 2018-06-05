namespace xys
{
    using NetProto;
    using System.Reflection;

    public partial class EquipModule : HotModule
    {
        public EquipModule() : base("xys.hot.HotEquipModule")
        {

        }

        public object equipMgr
        {
            get
            {
                return refType.GetField("equipMgr");
            }
        }
    }
}


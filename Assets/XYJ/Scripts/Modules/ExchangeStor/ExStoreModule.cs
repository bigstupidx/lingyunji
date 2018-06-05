using System;
using NetProto;
using System.Reflection;

namespace xys
{
    public partial class ExStoreModule : HotModule
    {
        public ExStoreModule() : base("xys.hot.HotExStoreModule")
        {
        }

        public object exStoreMgr
        {
            get
            {
                return refType.GetField("exstoreMgr");
            }
        }
    }
}

using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace xys
{
    using NetProto;

    public class ClanModule : HotModule
    {
        public ClanModule() : base("xys.hot.HotClanModule")
        {

        }

        public object clanMgr
        {
            get
            {
                return refType.GetField("clanMgr");
            }
        }
    }
}

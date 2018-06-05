using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys
{
    public class RankModule : HotModule
    {
        public RankModule() : base("xys.hot.HotRankModule")
        {
            var v = typeof(Dictionary<NetProto.RankType, object>);
        }
    }
}

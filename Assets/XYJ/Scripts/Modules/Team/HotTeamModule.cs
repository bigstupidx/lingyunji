#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot
{
    partial class HotTeamModule : HotModuleBase
    {
        xys.hot.TeamMgr teamMgr = new xys.hot.TeamMgr();

        public HotTeamModule(xys.TeamModule m) : base(m)
        {

        }

        protected override void OnAwake()
        {
            teamMgr.OnInit();
            teamMgr.OnAwake();
        }

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {

        }
    }
}
#endif
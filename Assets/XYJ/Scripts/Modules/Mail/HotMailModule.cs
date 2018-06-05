#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot
{
    partial class HotMailModule : HotModuleBase
    {
        public MailMgr mailMgr { get; private set; }

        public HotMailModule(xys.MailModule m) : base(m)
        {

        }

        protected override void OnAwake()
        {
            mailMgr = new MailMgr();
            mailMgr.OnInit();
            mailMgr.OnAwake();
        }

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {

        }
    }
}
#endif
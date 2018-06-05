using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProto;
using System.Reflection;

namespace xys
{
    public class MailModule : HotModule
    {
        public MailModule() : base("xys.hot.HotMailModule")
        {
        }

        private object m_mailMgr;
        public object mailMgr
        {
            get
            {
                if (null == m_mailMgr)
                    m_mailMgr = refType.GetField("mailMgr");
                return m_mailMgr;
            }
        }
    }
}

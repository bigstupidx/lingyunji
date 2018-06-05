using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys
{
    public class TeamModule : HotModule
    {
        public TeamModule() : base("xys.hot.HotTeamModule")
        {
        }

        private RefType m_teamMgrType;
        private object m_teamMgr;
        public object teamMgr
        {
            get
            {
                if (null == m_teamMgr)
                {
                    m_teamMgr = refType.GetField("teamMgr");
                    m_teamMgrType = new RefType(m_teamMgr);
                }
                return m_teamMgr;
            }
        }
        public bool InTeam()
        {
            bool ret = false;
            if (null != teamMgr)
            {
                object retVal = m_teamMgrType.InvokeMethodReturn("InTeam");
                ret = null != retVal ? (bool)retVal : false;
            }
            return ret;
        }
        public bool IsLeader()
        {
            bool ret = false;
            if (null != teamMgr)
            {
                object retVal = m_teamMgrType.InvokeMethodReturn("IsLeader");
                ret = null != retVal ? (bool)retVal : false;
            }
            return ret;
        }
        public int TeamNum()
        {
            int ret = 0;
            if (null != teamMgr)
            {
                object retVal = m_teamMgrType.InvokeMethodReturn("TeamNum");
                ret = null != retVal ? (int)retVal : 0;
            }
            return ret;
        }
        public NetProto.TeamAllTeamInfo GetTeamAllInfo()
        {
            NetProto.TeamAllTeamInfo ret = null;
            if (null != teamMgr)
            {
                object retVal = m_teamMgrType.InvokeMethodReturn("GetTeamAllInfo");
                ret = null != retVal ? (NetProto.TeamAllTeamInfo)retVal : null;
            }
            return ret;
        }
    }
}

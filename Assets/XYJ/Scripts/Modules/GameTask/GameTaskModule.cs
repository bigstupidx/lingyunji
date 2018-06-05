namespace xys
{
    using NetProto;
    using System.Reflection;

    public class GameTaskModule : HotModule
    {

        public GameTaskModule()
            : base("xys.hot.HotGameTaskModule")
        {
        }

        private object m_gameTaskMgr;
        public object gameTaskMgr
        {
            get
            {
                if (null == m_gameTaskMgr)
                    m_gameTaskMgr = refType.GetField("gameTaskMgr");

                return m_gameTaskMgr;
            }
        }

        public TaskData GetGameTaskData()
        {
            if (m_gameTaskMgr == null)
                return null;

            return refType.InvokeMethodReturn("GetGameTaskData") as TaskData;
        }
    }
}

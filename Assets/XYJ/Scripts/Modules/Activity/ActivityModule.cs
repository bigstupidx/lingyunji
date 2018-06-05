// Author : PanYuHuan
// Create Date : 2017/7/12

namespace xys
{
    public class ActivityModule : HotModule
    {
        public ActivityModule() : base("xys.hot.HotActivityModule")
        {
           
        }

        public HotModule module { get; private set; }

        public object activityMgr
        {
            get
            {
                return refType.GetField("activityMgr");
            }
        }
    }
}

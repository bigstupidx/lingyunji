using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace xys
{
    using NetProto;

    public class FriendModule : HotModule
    {
        public FriendModule() : base("xys.hot.HotFriendModule")
        {

        }

        // 包裹类
        public object friendMgr
        {
            get
            {
                return refType.GetField("friendMgr");
            }
        }
    }
}

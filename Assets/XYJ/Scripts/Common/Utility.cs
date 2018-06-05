using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.Common
{
    public static class Utility
    {
        // 场景ID规则,64位
        // 28位(268435456)  4位(16)   16位(65536) 16位(65536)
        //   自增序列      ZoneType      服ID       静态ID
        public static long Zone(NetProto.ZoneType zt, int auid, ushort serverid, ushort configid)
        {
            long v = ((long)zt << 32);
            v |= ((long)auid << 36);
            v |= ((long)serverid << 16);
            v |= configid;

            return v;
        }

        public static void Zone(long id, out NetProto.ZoneType zt, out int auid, out ushort serverid, out ushort configid)
        {
            zt = (NetProto.ZoneType)((id << 28) >> 60);
            auid = (int)(id >> 36);
            serverid = (ushort)((id & 0x00000000FFFF0000) >> 16);
            configid = (ushort)(id & 0x000000000000FFFF);
        }
    }
}

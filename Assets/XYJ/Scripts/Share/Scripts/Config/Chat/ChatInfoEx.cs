using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ChatInfo
    {
        static public ChatInfo GetByName(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                List<ChatInfo> ret = ChatInfo.GetGroupByname(key);
                if (ret.Count > 0)
                    return ret[0];
            }
            return null;
        }
    }
}




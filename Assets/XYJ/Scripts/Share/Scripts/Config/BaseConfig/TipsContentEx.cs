using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TipsContent 
    {
        static public TipsContent GetByName(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                List<TipsContent> ret = TipsContent.GetGroupByname(key);
                if (ret.Count > 0)
                    return ret[0];
            }
            return null;
        }
    }
}



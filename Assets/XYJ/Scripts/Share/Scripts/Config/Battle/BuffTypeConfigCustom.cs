using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class BuffTypeConfig 
    {
        static public BuffTypeConfig Get(string name)
        {
            foreach(var p in DataList)
            {
                if (p.Value.name == name)
                    return p.Value;
            }
            return null;
        }
    }

}
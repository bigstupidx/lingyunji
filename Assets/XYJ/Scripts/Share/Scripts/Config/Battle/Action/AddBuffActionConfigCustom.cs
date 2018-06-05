using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{


    public partial class AddBuffActionConfig:ActionConfigBase
    {
        public List<IAction> actionList { get; private set; }

        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new AddBuffAction());
            }
        }
    }

}

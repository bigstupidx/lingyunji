using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class BlockActionConfig : ActionConfigBase
    {
        public List<IAction> actionList { get; private set; }


        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new BlockAction());
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        //所有配置表加载完调用
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.actionList = ActionManager.ParseActionList(p.actions);

            }
        }

    }

}

using System.Collections.Generic;
using xys.battle;
namespace Config
{
    public partial class SimpleActionConfig : ActionConfigBase
    {

        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                SimpleAction act = SimpleActionFactory.Create(p);
                if (act != null)
                {
                    if (act.ParsePara(p, p.typeParas))
                        p.AddAction(p, p.id, act);
                    else
                        XYJLogger.LogError("action参数解释出错 " + p.id);
                }
            }
        }
    }
}
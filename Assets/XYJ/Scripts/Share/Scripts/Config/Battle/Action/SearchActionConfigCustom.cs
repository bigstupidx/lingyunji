using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using xys;

namespace Config
{
    public partial class SearchActionConfig : ActionConfigBase
    {
        //搜索原点
        public enum SearchPoint
        {
            Source,     //施法者
            Target,     //目标
            SkillPoint, //技能目标点
        }

        //搜索范围
        public enum SearchSharp
        {
            Angle,          //扇形
            Rect,           //直线
            MultiRect,      //多个直线(程序自动设置)
        }

        public List<IAction> actionList { get; private set; }
        public bool isCircle { get; private set; }
        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new SearchAction());
                if (p.searchType == SearchSharp.Angle && p.searchPara[1] > 300)
                    p.isCircle = true;

                if (p.searchPara == null || p.searchPara.Length != 2
                    || (p.searchType == SearchSharp.Rect && p.searchPara.Length % 3 == 0)) ////矩形填3的倍数也可以
                {
                    XYJLogger.LogError(string.Format("searchAction的范围参数有误 id={0}", p.id));
                }

                if (p.searchType == SearchSharp.Rect && p.searchPara.Length % 3 == 0)
                    p.searchType = SearchSharp.MultiRect;
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        //所有配置表加载完调用
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.actionList = ActionManager.ParseActionList(p.actionStrs);
            }
        }
    }
}

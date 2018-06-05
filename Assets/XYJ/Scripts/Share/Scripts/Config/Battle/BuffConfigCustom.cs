using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class BuffConfig 
    {
        public enum FinishType
        {
            ByTime, //根据时间
            NoFinish,//永久
        }

        //
        public enum AddType
        {
            Normal,     //叠加不区分施法者
            ObjectAdd,  //不同施法者独立叠加
        }

        public BuffTypeConfig typeCfg { get; private set; }
        public BuffType type { get { return typeCfg==null?BuffType.Empty:(BuffType)typeCfg.id; } }
        public List<IAction> intervalActionList { get; private set; }
        public List<IAction> destroyActionList { get; private set; }
        public List<IAction> createActionList { get; private set; }
        public IBuffTypeLogic logic { get; private set; }
        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }
        

        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.typeCfg = BuffTypeConfig.Get(p.typename);
                p.intervalActionList = ActionManager.ParseActionList(p.tickActionStrs);
                p.destroyActionList = ActionManager.ParseActionList(p.endActionStrs);
                p.createActionList = ActionManager.ParseActionList(p.beginActionStrs);
                p.logic = BuffTypeFactory.Create(p);
                if (p.addMaxCnt == 0)
                    p.addMaxCnt = 1;
                if (p.logic != null)
                    p.logic.ParsePara(p.para.Split(':'));
            }
        }
    }
}

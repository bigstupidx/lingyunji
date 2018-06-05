using System.Collections.Generic;
using xys.battle;
namespace Config
{
    public partial class OmnislashActionConfig:ActionConfigBase
    {
        public enum FinishPos
        {
            SourcePos,  //角色自身坐标
            TargetPos,  //最后攻击目标的坐标
        };

        public enum MoveType
        {
            Move,//action开始时移动到目标
            Flash,//下个action开始时立马闪现到目标位置
        };

        public List<IAction> actionList { get; private set; }

        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new OmnislashAction());
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        //所有配置表加载完调用
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.actionList = ActionManager.ParseActionList(p.actionsName);
            }
        }
    }
}
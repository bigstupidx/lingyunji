// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class SkillEventConfig 
    {
        public enum FrameType
        {
            AniFrame,   //动画帧
            TimeFrame,  //真实时间
        }
        public List<IAction> actionList { get; private set; }
        public IAction aniAction { get; private set; }
        static void OnLoadEnd()
        {

            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }

        static void OnLoadAllCsv()
        {
            foreach (var p in DataList)
            {
                p.actionList = ActionManager.ParseActionList(p.actions);
            }
        }
    }
}

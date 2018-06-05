using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class BulletActionConfig : ActionConfigBase
    {
        //目标点
        public enum TargetPos
        {
            SkillTargetPos, //技能时刻目标贴地
            Target,         //当前目标
            NoTarget,       //无目标
        };

        public enum FirePos
        {
            Source,     //施放者
            Target,     //目标
            SkillTargetPos,//技能目标点
            BindSource, //子弹绑定发射者骨骼
            BindTarget, //子弹绑定目标骨骼
        };



        public enum MoveType
        {
            Line,
            Parabolic,//抛物线
        }


        //目标定位类型
        public enum TargetPosType
        {
            SkillTime,  //施放技能时选定目标点
            EventTime,  //施放执行事件时选目标点
        }

        public List<IAction> intervalActionList { get; private set; }
        public List<IAction> destroyActionList { get; private set; }
        public List<IAction> createActionList { get; private set; }

        public MoveType moveType { get; private set; }

        //抛物线加速度
        public float paraAcc { get { return pathParas[4]; } }
        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new BulletAction());

                if (p.pathParas != null && p.pathParas.Length == 5)
                    p.moveType = MoveType.Parabolic;
                else
                    p.moveType = MoveType.Line;
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        //所有配置表加载完调用
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.intervalActionList = ActionManager.ParseActionList(p.intervalActions);
                p.destroyActionList = ActionManager.ParseActionList(p.destroyActions);
                p.createActionList = ActionManager.ParseActionList(p.createActions);
            }
        }
    }
}

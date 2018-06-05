using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class AttackActionConfig:ActionConfigBase
    {
        public StateType state { get; private set; }

        //状态是否能被护体免疫
        public bool isStateImmuneByHuti { get; private set; }
        //命中action
        public List<IAction> hitActionList { get; private set; }
        //命中仅执行一次的action
        public List<IAction> hitActionListOnce { get; private set; }

        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.state = GetTypeByName(p.id, p.stateName);

                if ( p.statePara.Length == 0 && p.state!= StateType.Empty)
                {
                    //受击可以使用默认时间
                    if(p.state == StateType.BeHit)
                    {
                        p.statePara = new float[] { 0.6f};
                    }
                    else
                    {
                        XYJLogger.LogError(string.Format("没有配状态参数 id={0} type={1}",p.id,p.stateName));
                        continue;
                    }
                }
                p.AddAction(p, p.id, new AttackAction());
            }

            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }

        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.isStateImmuneByHuti = false;
                foreach (var st in kvBattle.hutiImmuneState)
                {
                    if (st == p.stateName)
                    {
                        p.isStateImmuneByHuti = true;
                        break;
                    }
                }

                p.hitActionList = ActionManager.ParseActionList(p.hitActionStr);
                p.hitActionListOnce = ActionManager.ParseActionList(p.hitActionStrOnce);            
            }
        }

        static public StateType GetTypeByName(string id, string name)
        {
            if (string.IsNullOrEmpty(name))
                return StateType.Empty;
            switch (name)
            {
                case "冻结": return StateType.Ice;
                case "石化": return StateType.Stone;
                case "浮空": return StateType.Float;
                case "倒地": return StateType.KnockDown;
                case "虚弱": return StateType.Weak;
                case "眩晕": return StateType.Dizzy;
                case "禁锢": return StateType.Suppress;
                case "受击": return StateType.BeHit;
                case "击退": return StateType.BeatBack;
                case "击飞": return StateType.HitOut;
                case "抱摔": return StateType.BaoShuai;
                case "牵引": return StateType.Tow;
                case "轻功": return StateType.Jump;
            }
            XYJLogger.LogError(string.Format("attackAction id={0} 找不到状态 name={1}", id, name));
            return StateType.Empty;
        }

    }
}

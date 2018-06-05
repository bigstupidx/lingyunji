using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using xys;
using System;

namespace Config
{
    public partial class ConditionActionConfig : ActionConfigBase
    {
        public List<IAction> suscessActions { get; private set; }
        public List<IAction> failActions { get; private set; }

        public List<Func<IObject, ConditionActionConfig, IAction.ActionInfo, bool>> m_checkTargetConditon { get; private set;}


        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new ConditionAction());
                p.m_checkTargetConditon = ConditionAction.ParseCheckTarget(p);
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        //所有配置表加载完调用
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.suscessActions = ActionManager.ParseActionList(p.sucessActionStr);
                p.failActions = ActionManager.ParseActionList(p.failActopmStr);
            }
        }



    }
}

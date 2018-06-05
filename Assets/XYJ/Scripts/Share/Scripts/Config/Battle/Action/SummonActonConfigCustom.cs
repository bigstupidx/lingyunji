using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;
using xys;
using System;

namespace Config
{
    public partial class SummonActionConfig : ActionConfigBase
    {
        public List<IAction> suscessActions { get; private set; }
        public List<IAction> failActions { get; private set; }

        public List<Func<IObject, ConditionActionConfig, IAction.ActionInfo, bool>> m_checkTargetConditon { get; private set; }


        static void OnLoadEnd()
        {
            foreach (var p in DataList.Values)
            {
                p.AddAction(p, p.id, new SummonAction());
            }
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }


        //所有配置表加载完调用
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                foreach( var roleid in p.objIds )
                {
                    if (RoleDefine.Get(roleid) == null)
                        XYJLogger.LogError("召唤物id={0}找不到 action={1}", roleid, p.id);

                    if(p.objCnt > p.bornPos.Length/2)
                        XYJLogger.LogError("召唤事件召唤个数大于出生点集合 action={0}", p.id);

                    if ( p.bornPos.Length % 2 != 0)
                        XYJLogger.LogError("召唤事件生点集合个数应该是双 action={0}", p.id);
                }
            }
        }
    }
}

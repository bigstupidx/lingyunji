using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    //simpleaction不需要单独定义表，统一使用simple_action表
    abstract class SimpleAction : IAction<SimpleActionConfig>
    {
        public virtual bool ParsePara(SimpleActionConfig cfg, string[] para) { return true; }

    }


    static class SimpleActionFactory
    {
        static public SimpleAction Create(SimpleActionConfig cfg)
        {
            switch (cfg.actionType)
            {
                case "切换姿态": return new ChangePostureAction();
                case "真气变化": return new ChangeZhenqiAction();
                default:
                    XYJLogger.LogError(string.Format("simpleaction类型没有实现 id={0} 类型={1}", cfg.id, cfg.actionType));
                    return null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
namespace xys.battle
{

    public interface IBuffTypeLogic
    {
        void ParsePara(string[] para);
        //source为施法者，target为buff作用对象
        void OnEnter(IObject source, IObject target);
        void OnExit(IObject target);
    }


    static class BuffTypeFactory
    {
        static public IBuffTypeLogic Create(BuffConfig cfg)
        {
            if (string.IsNullOrEmpty(cfg.typename))
                return null;
            switch (cfg.typename)
            {
                case "霸体": return new BuffBati();
                case "属性": return new BuffAddAttribute();
                default:
                    XYJLogger.LogError(string.Format("状态类型没有实现 buffid={0} 类型={1}", cfg.id, cfg.typename));
                    return null;
            }
        }
    }
}

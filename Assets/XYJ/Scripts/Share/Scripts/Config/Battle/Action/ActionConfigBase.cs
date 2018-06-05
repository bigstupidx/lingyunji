using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{

    public class ActionConfigBase
    {
        public int idhash { get; private set; }

        protected void AddAction<T>(T cfg, string id, IAction<T> action) where T : ActionConfigBase
        {
            idhash = BattleHelp.HashCode(id);
            action.SetCfg(cfg);
            ActionManager.AddAction(id, idhash, action);
        }
    }

}

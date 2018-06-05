using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{

    public class BuffBati : IBuffTypeLogic
    {
        public void ParsePara(string[] para) 
        { 
        }
        public void OnEnter(IObject source, IObject target)
        {
            target.battle.m_buffMgr.AddFlag(BuffManager.Flag.Bati);
        }
        public void OnExit(IObject target)
        {
            target.battle.m_buffMgr.RemoveFlag(BuffManager.Flag.Bati);

        }
    }
}

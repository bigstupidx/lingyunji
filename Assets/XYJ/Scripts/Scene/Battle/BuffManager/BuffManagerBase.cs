using xys.battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace xys.battle
{
    public class BuffManagerBase : BuffManager
    {
        public override Buff Create(BuffType id)
        {
            return new Buff();
        }

        protected override void OnAddBuffEffect(BuffConfig cfg)
        {
            m_obj.battle.m_effectMgr.AddEffect(cfg.effect);
        }

        protected override void OnChangeFlag(Flag flag, bool add)
        {
            m_obj.eventSet.FireEvent<Flag>(ObjEventID.ChangeBuffFlg, flag);
        }

        protected override void OnRemoveBuffEffect(BuffConfig cfg)
        {
            m_obj.battle.m_effectMgr.RemoveEffect(cfg.effect);
        }

    }

}

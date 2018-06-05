using UnityEngine;
using System.Collections;


namespace xys.battle
{
    public class TargetManagerLocal : TargetManager
    {
        public override void SetTarget(IObject obj)
        {
            base.SetTarget(obj);
            ((BattleManagerLocal)m_obj.battle).m_autoChooseTarget.SetChooseTarget(obj);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

using xys.battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
namespace xys.battle
{
    public class BulletMoveLine : BulletLogicClient
    {
        protected override void OnMove()
        {
            if (cfg.speed == 0)
                return;

            Vector3 toPos = GetToPos();
            float mul = m_timePass / m_lifeTime;
            if(mul>1)
                mul = 1;
            m_go.transform.position = m_goFromPos + (toPos - m_goFromPos) * mul;
        }
    }
}
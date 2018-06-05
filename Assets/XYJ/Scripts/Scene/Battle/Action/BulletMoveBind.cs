using xys.battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
namespace xys.battle
{
    public class BulletMoveBind : BulletLogicClient
    {
        protected override void OnMove()
        {
            m_go.transform.position = GetToPos();
        }
    }
}
using UnityEngine;
using System.Collections;
using Config;
using System.Collections.Generic;


//轻功移动逻辑
namespace xys.battle
{
    class JumpManagerRemote : JumpManagerBase
    {

        protected override void OnUpdateLogic()
        {
            Vector3 move = BattleHelp.RotateAngle(m_speed * Time.deltaTime, m_obj.rotateAngle);
            Vector3 movePos = m_obj.position + move;

            //着地动画结束就切换到待机
            if(m_state == State.Land)
            {
                if (m_obj.battle.m_aniMgr.IsAniFinish())
                {
                    m_obj.battle.m_stateMgr.ChangeState(StateType.Idle);
                    Stop();
                }

                //保证有下降的速度,不过要避免陷入地面
                movePos.y -= 10 * Time.deltaTime;
                if (movePos.y < m_topos.y)
                    movePos.y = m_topos.y;

                //如果移动超过目标点，则直接设置到目标点
                if (m_speed.z>0 && Vector3.Dot(m_topos - movePos, m_topos - m_obj.position) < 0)
                {
                    m_speed.z = 0;
                    movePos = m_topos;
                }
            }
            

            m_obj.SetPosition(movePos);     
        }

        Vector3 m_topos;
        public void MoveToPos( Vector3 topos,float zSpeed )
        {
            if (zSpeed != 0)
                m_speed.z = zSpeed;
            m_topos = topos;

            //距离有点远，重新修正角度
            float dis = BattleHelp.GetDistance(m_obj.position,topos) ;
            if(dis > 0.5f)
                m_obj.SetRotate(BattleHelp.Vector2Angle(topos-m_obj.position));
        }
    }
}
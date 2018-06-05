using UnityEngine;
using System.Collections;

namespace xys.battle
{
    /// <summary>
    /// 弧线跟随目标,跟随到受击点
    /// </summary>
    public class BulletMoveParabolic : BulletLogicClient
    {
        //前进初始速度
        float m_forwardSpeed;
        //一开始目标间距离
        float m_needMoveDistance;

        //抛物线偏移
        float m_maxOffX;
        float m_maxOffY;
        //开始移动
        protected override void OnBeginMove()
        {
            m_needMoveDistance = BattleHelp.GetDistance(m_fromPos, m_toPos);
            //不能小于0,不然下面会除0
            if (m_needMoveDistance <= 0)
                m_needMoveDistance = 0.3f;
            float acc = cfg.paraAcc;
            //由于时间计算出来可能是负数，被修正，所以初速度需要重新计算
            m_forwardSpeed = m_needMoveDistance / m_lifeTime - acc * m_lifeTime / 2;

            m_maxOffX = Random.Range(cfg.pathParas[0], cfg.pathParas[1]);
            m_maxOffY = Random.Range(cfg.pathParas[2], cfg.pathParas[3]);
        }

        //运加速直线运动当前时间运动距离
        float MoveDistanceByTime(float t)
        {
            float v = m_forwardSpeed;
            float a = cfg.paraAcc;
            return (v + a * t / 2) * t;
        }

        //移动偏移，正偏移和反偏移各占用一半时间
        Vector3 GetMoveOff(Vector3 way)
        {
            //移动百分比
            float mul = m_timePass / m_lifeTime*2;
            if (mul > 1)
                mul = 2 - mul;
            mul = 1-(1-mul)*(1-mul);

            Vector3 off = BattleHelp.RotateAngle(new Vector3(m_maxOffX, 0, 0), BattleHelp.Vector2Angle(way));
            off.y = m_maxOffY;
            return off * mul;
        }


        protected override void OnMove()
        {
            Vector3 toPos = GetToPos(); 
            Vector3 fromPos = m_goFromPos;
            //目标点抛物线偏移
            Vector3 posOff = GetMoveOff(toPos - fromPos);

            float mul = MoveDistanceByTime(m_timePass) / m_needMoveDistance;
            if (mul > 1)
                m_go.transform.position = toPos;
            else
            {
                Vector3 desPos = fromPos + (toPos - fromPos) * mul;
                Vector3 finalyToPos = desPos + posOff;
                //设置朝向
                BattleHelp.SetLookAt(m_go.transform, finalyToPos);
                m_go.transform.position = desPos + posOff;
            }

        }
    }
}

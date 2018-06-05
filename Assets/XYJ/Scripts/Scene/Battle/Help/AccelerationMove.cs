using UnityEngine;
using System.Collections;

namespace xys.battle
{
    /// <summary>
    /// 移动
    /// </summary>
    public class AccelerationMove
    {
        float m_beginSpeed;
        float m_acceleration;

        float m_fromLen;
        float m_toLen;

        float m_moveMaxTime;
        float m_playTime;

        bool m_finish = true;

        public void Reset()
        {
            m_finish = true;
        }

        //变速运动
        //如果fromspeed和tospeed填的不合理，会导致运动达不到预期
        public void PlaySpeed(float fromLen,float toLen,float fromSpeed,float moveTime)
        {
            if (moveTime == 0 || fromLen == toLen)
            {
                m_finish = true;
                return;
            }

            float toSpeed = Mathf.Abs(fromLen - toLen) * 2 / moveTime - fromSpeed;

     
            m_acceleration = (toSpeed - fromSpeed) / moveTime;
            m_fromLen = fromLen;
            m_toLen = toLen;
            m_beginSpeed = fromSpeed;
            m_moveMaxTime = moveTime;
            m_playTime = 0;
            m_finish = false;
        }


        //返回移动的位置
        public float UpdateMove()
        {
            if (m_finish)
                return m_toLen;

            m_playTime += Time.deltaTime;

            if (m_playTime > m_moveMaxTime)
            {
                m_finish = true;
                return m_toLen;
            }

            if(m_fromLen>m_toLen)
            {
                float toLen = m_fromLen - (m_beginSpeed + m_acceleration * m_playTime / 2) * m_playTime;

                if (toLen < m_toLen)
                {
                    toLen = m_toLen;
                    m_finish = true;
                }
                return toLen;
            }
            else
            {
                float toLen = m_fromLen + (m_beginSpeed + m_acceleration * m_playTime / 2) * m_playTime; 
                if (toLen > m_toLen)
                {
                    toLen = m_toLen;
                    m_finish = true;
                }
                return toLen;
            }
        }

        public bool IsFinish()
        {
            return m_finish;
        }
    }

}

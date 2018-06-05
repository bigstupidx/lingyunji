using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public abstract partial class BuffManager
    {
        //标签
        public enum Flag
        {
            //ImmuneHit = 0,  //免疫控制(只会掉血，不会切换其他状态，类似霸体)  
            NotTarget,      //不会成为攻击目标
            NoSkill,        //不能使用技能
            NoMove,         //不能移动
            Bati,           //免疫控制
            FlagCnt,
        };

        int[] m_flags = new int[(int)Flag.FlagCnt];

        public void AddFlag( Flag flag )
        {
            m_flags[(int)flag] += 1;

            if (m_flags[(int)flag] == 1)
                OnChangeFlag(flag, true);
        }

        public void RemoveFlag( Flag flag )
        {
            if (m_flags[(int)flag] < 1)
            {
                XYJLogger.LogError("移除flag出错 " + flag);
                return;
            }

            m_flags[(int)flag] -= 1;

            if (m_flags[(int)flag] == 0)
                OnChangeFlag(flag, false);
        }

        public void ChangeFlag(Flag flag,bool isAdd)
        {
            if (isAdd)
                AddFlag(flag);
            else
                RemoveFlag(flag);
        }

        public bool IsFlag( Flag flag)
        {
            return m_flags[(int)flag] > 0;
        }

        public int GetFlag(Flag flag)
        {
            return m_flags[(int)flag];
        }

        protected virtual void OnChangeFlag(Flag flag,bool add)
        {

        }
    }
}

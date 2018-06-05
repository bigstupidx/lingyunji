///*----------------------------------------------------------------
//// 创建者：
//// 创建日期:
//// 模块描述：
////----------------------------------------------------------------*/
//using UnityEngine;
//using System.Collections;
//using xys.battle;

//namespace xys
//{
//    public class CheckInterval
//    {
//        float m_time;

//        //每隔interval返回true,会自动重置时间
//        public bool Check(float interval)
//        {
//            if (BattleHelp.timePass - m_time > interval)
//            {
//                m_time = BattleHelp.timePass;
//                return true;
//            }
//            else
//                return false;
//        }

//        //重置时间
//        public void ResetCheck()
//        {
//            m_time = BattleHelp.timePass;
//        }

//    }

//}

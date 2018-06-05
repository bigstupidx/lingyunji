using System;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    //状态切换规则
    class StateChangeRule
    {
        //状态切换规则
        public static bool IsCanChange(StateType curstate, StateType tostate)
        {
            switch (curstate)
            {
                #region 受控  
                case StateType.Float:
                    switch (tostate)
                    {
                        case StateType.Float:
                        case StateType.KnockDown:
                        case StateType.Suppress:
                        case StateType.BaoShuai:
                            return true;
                        default:
                            return false;
                    }
                case StateType.KnockDown:
                    switch (tostate)
                    {
                        case StateType.Float:
                        case StateType.Stone:
                        case StateType.Weak:
                        case StateType.Dizzy:
                        case StateType.Ice:
                        case StateType.KnockDown:
                        case StateType.BeatBack:
                            return true;
                        default:
                            return false;
                    }
                case StateType.Weak: //虚弱
                    switch(tostate)
                    {
                        case StateType.Weak:
                        case StateType.Ice:
                        case StateType.Stone:
                        case StateType.Float:
                        case StateType.KnockDown:
                            return true;
                        default:
                            return false;
                    }
                case StateType.Dizzy: //眩晕
                    switch (tostate)
                    {
                        case StateType.Dizzy:
                        case StateType.Weak:
                        case StateType.Ice:
                        case StateType.Stone:
                        case StateType.Float:
                        case StateType.KnockDown:
                            return true;
                        default:
                            return false;
                    }
                case StateType.BeHit:
                case StateType.BeatBack:
                case StateType.Ice: 
                case StateType.Stone: 
                case StateType.Suppress: 
                case StateType.HitOut: 
                case StateType.BaoShuai: 
                case StateType.Tow: 
                    return true;
                #endregion
                default:
                    return true;

            }
        }


        //能否移动和使用技能
        public static bool IsCanMoveOrSkill(StateType type)
        {
            switch (type)
            {
                case StateType.Ice:
                case StateType.Stone:
                case StateType.Float:
                case StateType.KnockDown:
                case StateType.Weak:
                case StateType.Dizzy:
                case StateType.Suppress:
                case StateType.BeHit:
                case StateType.BeatBack:
                case StateType.HitOut:
                case StateType.BaoShuai:
                case StateType.Tow:
                    return false;
                default:
                    return true;
            }
        }
    }
}

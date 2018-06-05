using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using xys.battle;

namespace Config
{
    public partial class JumpConfig
    {
        public enum LandType
        {
            Normal,     //落地动作根据高度使用jump_land_1;jump_land_2
            FastRun,    //落地动作根据高度使用jump_land_3,并且结束后进入疾跑
        }
    }

}
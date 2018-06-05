// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Config
{
    public partial class TaskItem 
    {
        public override bool IsCanUse { get { return isCanUse; } } // 是否可使用
        public override bool IsCanSell { get { return false; } }
        public override bool IsCanAllUse { get { return false; } }
        public override bool IsCanLost { get { return false; } } // 是否可丢弃
#if COM_SERVER

        // 是否满足使用条件
        public override bool isCanUseCondition(GameServer.GameUser user)
        {
            return false;
        }

        // 开始使用道具
        public override bool Use(GameServer.GameUser user, int count)
        {
            return false;
        }

        public override void OnCreate(NetProto.ItemData data)
        {

        }

        // 是否售卖成功
        public override bool Sell(GameServer.GameUser user, int count)
        {
            return false;
        }
#else
        public override bool isCanUseCondition()
        {
            return false;
        }

        // 开始使用道具
        public override bool Use()
        {
            return false;
        }
#endif
    }
}



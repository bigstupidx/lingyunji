
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public abstract partial class ItemBase
    {
        public abstract bool IsCanUse { get; } // 是否可使用
        public abstract bool IsCanAllUse { get; } // 是否可以全部使用
        public abstract bool IsCanSell { get; } // 是否可售卖
        public abstract bool IsCanLost { get; } // 是否可丢弃

#if COM_SERVER
        public abstract bool isCanUseCondition(GameServer.GameUser user); // 是否满足使用条件
        public abstract bool Use(GameServer.GameUser user, int count); // 开始使用道具,数量
        public abstract void OnCreate(NetProto.ItemData data); // 创建此物品时的初始化操作
                                                               // 是否售卖成功
        public abstract bool Sell(GameServer.GameUser user, int count);
#else
        public abstract bool isCanUseCondition(); // 是否满足使用条件
        public abstract bool Use(); // 开始使用道具
#endif
    }
}


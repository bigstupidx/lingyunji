namespace xys.battle
{
    using NetProto;
    partial interface IShareObject
    {
        int hpValue // 血量
        {
            get;
            set;
        }
        int maxHpValue // 最大血量
        {
            get;
            set;
        }
        int zhenQiValue // 真气
        {
            get;
            set;
        }
        int lingLiValue // 灵力
        {
            get;
            set;
        }
        int huTiValue // 护体
        {
            get;
            set;
        }
        int speedValue // 移动速度
        {
            get;
            set;
        }
        int maxHuTiValue // 最大护体
        {
            get;
            set;
        }
        int huTiStateValue // 护体状态
        {
            get;
            set;
        }
        int stateValue // 状态
        {
            get;
            set;
        }
        int postureValue // 姿态
        {
            get;
            set;
        }
        ushort levelValue // 等级
        {
            get;
        }
    }
}

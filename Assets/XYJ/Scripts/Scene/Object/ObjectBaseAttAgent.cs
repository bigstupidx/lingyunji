namespace xys
{
    using NetProto;
    partial class ObjectBase
    {
        public int hpValue // 血量
        {
            get { return attributes.Get(AttType.AT_HP).intValue; }
            set { attributes.Get(AttType.AT_HP).intValue = value; }
        }
        public int maxHpValue // 最大血量
        {
            get { return attributes.Get(AttType.AT_MAX_HP).intValue; }
            set { attributes.Get(AttType.AT_MAX_HP).intValue = value; }
        }
        public int zhenQiValue // 真气
        {
            get { return attributes.Get(AttType.AT_ZhenQi).intValue; }
            set { attributes.Get(AttType.AT_ZhenQi).intValue = value; }
        }
        public int lingLiValue // 灵力
        {
            get { return attributes.Get(AttType.AT_LingLi).intValue; }
            set { attributes.Get(AttType.AT_LingLi).intValue = value; }
        }
        public int huTiValue // 护体
        {
            get { return attributes.Get(AttType.AT_HuTi).intValue; }
            set { attributes.Get(AttType.AT_HuTi).intValue = value; }
        }
        public int speedValue // 移动速度
        {
            get { return attributes.Get(AttType.AT_Speed).intValue; }
            set { attributes.Get(AttType.AT_Speed).intValue = value; }
        }
        public int maxHuTiValue // 最大护体
        {
            get { return attributes.Get(AttType.AT_Max_HuTi).intValue; }
            set { attributes.Get(AttType.AT_Max_HuTi).intValue = value; }
        }
        public int huTiStateValue // 护体状态
        {
            get { return attributes.Get(AttType.AT_HuTiState).intValue; }
            set { attributes.Get(AttType.AT_HuTiState).intValue = value; }
        }
        public int stateValue // 状态
        {
            get { return attributes.Get(AttType.AT_State).intValue; }
            set { attributes.Get(AttType.AT_State).intValue = value; }
        }
        public int postureValue // 姿态
        {
            get { return attributes.Get(AttType.AT_Posture).intValue; }
            set { attributes.Get(AttType.AT_Posture).intValue = value; }
        }
        public ushort levelValue // 等级
        {
            get { return attributes.Get(AttType.AT_Level).ushortValue; }
            set { attributes.Get(AttType.AT_Level).ushortValue = value; }
        }
    }
}

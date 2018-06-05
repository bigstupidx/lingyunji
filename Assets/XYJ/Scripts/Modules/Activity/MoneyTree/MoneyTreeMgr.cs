#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/22


namespace xys.hot
{
    using NetProto.Hot;

    public partial class MoneyTreeMgr
    {
        public MoneyTreeDbData m_MoneyTreeDbData { get; private set; }

        public MoneyTreeMgr()
        {
            m_MoneyTreeDbData = new MoneyTreeDbData();
        }
    }
}
#endif
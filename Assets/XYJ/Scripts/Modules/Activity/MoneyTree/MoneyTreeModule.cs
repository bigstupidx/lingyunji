// Author : PanYuHuan
// Create Date : 2017/7/22


namespace xys
{
    public class MoneyTreeModule : HotModule
    {
        public MoneyTreeModule() : base("xys.hot.HotMoneyTreeModule")
        {

        }

        public HotModule module { get; private set; }

        public object moneyTreeMgr
        {
            get
            {
                return refType.GetField("moneyTreeMgr");
            }
        }
    }
}
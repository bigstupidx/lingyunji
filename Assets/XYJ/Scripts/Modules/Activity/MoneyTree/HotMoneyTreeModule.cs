#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/22


namespace xys.hot
{
    using NetProto.Hot;
    using wProtobuf;

    partial class HotMoneyTreeModule : HotModuleBase
    {
        public MoneyTreeMgr moneyTreeMgr = new MoneyTreeMgr();

        public HotMoneyTreeModule(MoneyTreeModule module) : base(module)
        {
        }

        protected override void OnAwake()
        {
            Init();
            RegistMsg();

            moneyTreeMgr.OnInit(Event);
        }

        protected override void OnDeserialize(IReadStream output)
        {
            if (output != null && moneyTreeMgr.m_MoneyTreeDbData != null)
                moneyTreeMgr.m_MoneyTreeDbData.MergeFrom(output);

            moneyTreeMgr.OnShowMoneyTreeBtn();

            if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.Count > 0)
            {
                foreach (OneTreeData data in moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.Values)
                {
                    if (data.roleId == App.my.localPlayer.charid && data.leaveTime > 0)
                    {
                        moneyTreeMgr.CreatePlayerTimer(data);
                        return;
                    }
                }
            }
        }
    }
}
#endif
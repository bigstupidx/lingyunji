#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using Network;
    using System.Collections.Generic;

    class HotTitleModule : HotModuleBase
    {
        TitleList m_TitleListData;

        public C2ATitleRequest request { get; private set; }

        public HotTitleModule(TitleModule m) : base(m)
        {

        }

        protected override void OnAwake()
        {
            m_TitleListData = new TitleList();

            // 注册协议
            this.request = new C2ATitleRequest(App.my.game.local);

            App.my.handler.Reg<TitleDataChange>(Protoid.A2C_TitleDataChange, OnTitleDataChange);
        }

        // 序列化
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            m_TitleListData = new TitleList();
            m_TitleListData.MergeFrom(output);
        }

        // 称号数据改变
        void OnTitleDataChange(IPacket packet, TitleDataChange data)
        {
            if (data.newTitle.Count > 0)
                AddNewTitle(data.newTitle);
            WearTitle(data.wearTitle);

            App.my.eventSet.fireEvent(EventID.Title_Change);
        }

        // 新增称号
        void AddNewTitle(List<TitleData> newList)
        {
            foreach (TitleData v in newList)
            {
                m_TitleListData.list[v.titleId] = v;
                App.my.eventSet.FireEvent(EventID.Title_Unlock, new xys.UI.TitleUnlock(v.titleId));
            }
        }

        // 更换称号
        void WearTitle(int titleId)
        {
            m_TitleListData.currTitle = titleId;
        }
    }
}
#endif
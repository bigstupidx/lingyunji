#if !USE_HOT
using NetProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys.hot
{
    partial class HotRankModule : HotModuleBase
    {
        public HotRankModule(ModuleBase parent) : base(parent)
        {
        }
        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {

        }

        C2WRankRequest request;
        protected override void OnAwake()
        {
            request = new C2WRankRequest(App.my.world.local);

            //hotApp.my.mainTimer.Register(2, int.MaxValue, () =>
            //{
            //    foreach (RankType val in Enum.GetValues(typeof(RankType)))
            //    {
            //        this.QueryRank(val, (RankQueryRankResult) => { });
            //    }
            //});
        }

        public class RankDataCache
        {
            public long cacheSec;
            public RankQueryRankResult data;
            public bool IsExpired()
            {
                long CacheMaxSecond = 0;
                return RankUtil.GetNowSecond() >= cacheSec + CacheMaxSecond;
            }
        }
        Dictionary<RankType, RankDataCache> m_rankDataCaches = new Dictionary<RankType, RankDataCache>();
        public void QueryRank(RankType rankType, System.Action<RankQueryRankResult> cb)
        {
            if (null == cb)
                return;

            RankDataCache cacheData;
            if (m_rankDataCaches.TryGetValue(rankType, out cacheData))
            {
                if (cacheData.IsExpired())
                {
                    m_rankDataCaches.Remove(rankType);
                    cacheData = null;
                }
            }
            if (null != cacheData)
            {
                RankQueryRankResult ret = new RankQueryRankResult();
                ret.rankType = rankType;
                ret.playerRankDatas = cacheData.data.playerRankDatas;
                cb(ret);
                return;
            }

            // 缓存数据无效，就往服务器查询
            request.QueryRank(new RankQueryRank() { rankType = rankType }, 
                (wProtobuf.RPC.Error code, RankQueryRankResult ret) =>
                {
                    if (wProtobuf.RPC.Error.Success == code)
                    {
                        m_rankDataCaches[rankType] = new RankDataCache() { cacheSec=RankUtil.GetNowSecond(), data=ret };
                    }
                    else
                    {
                        ret = new RankQueryRankResult();
                        ret.rankType = rankType;
                    }
                    cb(ret);
                });
        }
        public void QueryRankDetail(RankType rankType, long key, long keyParam, System.Action<RankQueryRankDetailResult> cb)
        {
            if (null == cb)
                return;

            request.QueryRankDetail(new RankQueryRankDetail() { rankType = rankType, keyParam=keyParam, key= key },
                (wProtobuf.RPC.Error code, RankQueryRankDetailResult ret) =>
                {
                    if (wProtobuf.RPC.Error.Success != code)
                    {
                        ret = new RankQueryRankDetailResult();
                        ret.rankType = rankType;
                        ret.key = key;
                        ret.keyParam = keyParam;
                    }
                    cb(ret);
                });
        }
    }
}
#endif
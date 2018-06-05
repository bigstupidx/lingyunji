using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
    public enum ClientRankType
    {
        PlayerTypeMin,
        PlayerLevel = 1,
        PetAbility = 2,
        Fortune = 3,
        Equipment = 4,
        Weapon =  5,
        Trump = 6,
        AllAbility = 7,
        Shitu = 8,
        Xiulian = 9,
        PlayerTypeMax = 9999,

        GuildTypeMin = 10000,
        GuildAbility = 10001,          // 军团实力榜
        GuildLiveness = 10002,         // 军团活跃度
        GuildGold = 10003,				// 军团财富榜
        GuildTypeMax = 19999,

        ActivityMin = 20000,
        ActivityTest = 20001,
        ActivityMax = 29999,
    }

    public enum ClientRankFilterType
    {
        Invalid,
        Prof,
        EquipSlot,
        ServerRankType,
    }

    public partial class RankPanel
    {
        static public RankPanel GetCfg(ClientRankType key)
        {
            List<RankPanel> rets = RankPanel.GetGroupByclientRanktype(key);
            return rets.Count > 0 ? rets[0] : null;
        }
        public static void OnLoadEndLine(RankPanel data, CsvCommon.ICsvLoad reader, int i)
        {
            if (data.filterParams.Length != data.filterStrs.Length)
            {
                Log.Error(string.Format("RankPanel {0} filterParams and filterStrs length not equal", data.clientRanktype));
            }
        }
    }
}

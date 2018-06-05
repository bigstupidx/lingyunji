using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public class WorldCsvLoad
    {
        static public void All(CsvCommon.ICsvLoad load)
        {
            ClanActivityTimeConfig.Load(load);
            ClanBaiBaoConfig.Load(load);
            ClanFoodConfig.Load(load);
            ClanJinKuConfig.Load(load);
            ClanJiXianConfig.Load(load);
            ClanMainBuildConfig.Load(load);
            ClanOiranConfig.Load(load);
            ClanPermissionConfig.Load(load);
            ClanSkillBuildConfig.Load(load);
            ClanWealConfig.Load(load);
            kvClanBanquetBaseConfig.Load(load);
            kvWorld.Load(load);
            PlayerRank.Load(load);
            RankPanel.Load(load);
            RankRefresh.Load(load);
            ShangHuiConfig.Load(load);
            ShangHuiItem.Load(load);
            TeamActionNotification.Load(load);
            TeamGoal.Load(load);
            TeamGoalGroup.Load(load);
            TeamGoalType.Load(load);
            WorldMailTemplate.Load(load);



        }
    }
}
#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/22


using NetProto.Hot;
using System.Collections.Generic;

namespace xys.hot
{
    enum TreeStage
    {
        enumSmall = 0,   // 小树
        enumMedium = 1,  // 中树
        enumBig = 2,     // 大树
        enumMature = 3,  // 成熟
    }

    public class MoneyTreeDef
    {
        public static MoneyTreeMgr moneyTreeMgr { get { return hotApp.my.GetModule<HotMoneyTreeModule>().moneyTreeMgr; } }

        /// <summary>
        /// 判断当前玩家是否已经种树
        /// </summary>
        public static bool HasMoneyTreeBelongToMe()
        {
            foreach (OneTreeData data in moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.Values)
            {
                if (data.roleId == hotApp.my.localPlayer.charid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 得到某一场景的所有摇钱树点集名字数组
        /// </summary>
        public static List<string> GetCurrentLevelSet(int sceneId)
        {
            List<string> pointSet = new List<string>();
            Dictionary<int, Config.ComTree> treeConfs = Config.ComTree.GetAll();

            foreach (Config.ComTree treeData in treeConfs.Values)
            {
                if (treeData.sceneId == sceneId)
                    pointSet.Add(treeData.pointId);
            }

            if (pointSet.Count > 0)
                return pointSet;

            return null;
        }

        /// <summary>
        /// 该树是否在当前场景
        /// </summary>
        public static bool ThisTreeIsInScene(int treeId)
        {
            Config.ComTree treeConf = Config.ComTree.Get(treeId);
            if (treeConf == null)
                return false;

            int currentSceneId = hotApp.my.localPlayer.GetModule<LevelModule>().levelId;
            if (currentSceneId == treeConf.sceneId)
                return true;

            return false;
        }

        /// <summary>
        /// 该树是否属于当前玩家
        /// </summary>
        public static bool ThisTreeBelongToMe(int uId)
        {
            int roleId = 0;
            if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(uId))
                roleId = moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[uId].roleId;

            if (roleId == App.my.localPlayer.charid)
                return true;

            return false;
        }

        /// <summary>
        /// 判断是否还能使用加速道具
        /// </summary>
        public static bool IsCanUseSpeedItem(OneTreeData data)
        {
            Config.ComTree treeConf = Config.ComTree.Get(data.treeId);
            if (treeConf == null)
                return false;

            int currentTime = data.accelerateTimes;
            int maxTime = treeConf.speedUpTimes;

            if (currentTime < maxTime)
                return true;

            return false;
        }

        /// <summary>
        /// 一棵树成长需要总的秒数
        /// </summary>
        public static int GetMaxGrowthTime(int treeId)
        {
            int maxGrowthNum = 0;

            Config.ComTree treeData = Config.ComTree.Get(treeId);
            for (int i = 0; i < treeData.plantStageTime.Length; i++)
                maxGrowthNum += treeData.plantStageTime[i];
            return maxGrowthNum;
        }
    }
}
#endif
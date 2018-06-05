#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/22


namespace xys.hot
{
    using NetProto.Hot;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using wTimer;
    using Config;

    public partial class MoneyTreeMgr
    {
        public class TreeTimerData
        {
            #region 摇钱树倒计时数据
            public int treeUId;   // 树唯一id
            public int modelUId;  // 玩家模型唯一id
            public int leaveTime; // 剩余时间
            public int timer;     // 定时器
            public int stage;     // 阶段
            #endregion
        }

        // 点集随机列表
        private Dictionary<string, int> m_TreeSetRandomDic = new Dictionary<string, int>();
        private Dictionary<int, TreeTimerData> m_TreeTimerDic = new Dictionary<int, TreeTimerData>();

        private SimpleTimer m_Timer;
        private LevelModule m_LevelModule;
        private LocalObjectMgr m_ObjectMgr;

        private const int TenMinutes = 600;

        private List<MoneyTreeActor> m_FakesTreeModel = new List<MoneyTreeActor>();
        private Dictionary<int, MoneyTreeActor> m_PlayerTreeModel = new Dictionary<int, MoneyTreeActor>();

        public void OnInit(Event.HotObjectEventSet Event)
        {
            m_ObjectMgr = new LocalObjectMgr();
            m_Timer = new SimpleTimer(App.my.mainTimer);
            m_LevelModule = hotApp.my.localPlayer.GetModule<LevelModule>();

            Event.Subscribe(EventID.BackToLogin, OnBackToLogin);
            Event.Subscribe(EventID.Login_SelectRole, OnEnterGame);
            Event.Subscribe(EventID.Level_Prepared, OnFinishLoadScene);

            Event.Subscribe<OneTreeData>(EventID.MoneyTree_PlantTreeSuccess, PlantTreeCallBack);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_GetRewardSuccess, GetRewardCallBack);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_Refresh_UI_Data, UseSpeedItemChangeTree);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_UseSpeedUpItemSuccess, UseSpeedItemChangeTree);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_RequestReduceTreeTime, RequestToReduceTreeTime);
        }

        // 控制显示主界面的摇钱树按钮
        public void OnShowMoneyTreeBtn()
        {
            UI.MainPanelItemListener listener = new UI.MainPanelItemListener(ItemShowConditionFunc, null, OnClickMoneyTreeBtn);
            UI.MainPanel.SetItemListener((int)xys.UI.PanelType.UIMoneyTreePanel, listener);
        }

        private bool ItemShowConditionFunc()
        {
            if (m_MoneyTreeDbData.listPlayerTrees.Count > 0)
            {
                foreach (OneTreeData data in m_MoneyTreeDbData.listPlayerTrees.Values)
                {
                    ComTree treeConf = ComTree.Get(data.treeId);
                    if (treeConf == null)
                        return false;

                    Item itemConf = Item.Get(treeConf.speedUpItemId);
                    TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - data.startPlantTime);

                    int maxSeconds = (int)timeSpan.TotalSeconds + (itemConf == null ? 0 : data.accelerateTimes * itemConf.cowAccTimer);
                    int matureSeconds = MoneyTreeDef.GetMaxGrowthTime(data.treeId);
                    int matureTenMinutes = matureSeconds + TenMinutes;

                    if ((data.roleId == App.my.localPlayer.charid && data.leaveTime > 0) || maxSeconds < matureTenMinutes)
                        return true;
                }
            }

            return false;
        }

        private void OnClickMoneyTreeBtn(object obj)
        {
            foreach (OneTreeData data in m_MoneyTreeDbData.listPlayerTrees.Values)
            {
                if (data.roleId == App.my.localPlayer.charid)
                {
                    RequestGoToTreePos(data);
                    return;
                }
            }
        }

        private void RequestGoToTreePos(OneTreeData treeData)
        {
            ComTree treeConf = ComTree.Get(treeData.treeId);
            if (treeConf == null)
            {
                xys.UI.Utility.TipContentUtil.Show("activity_moneyTree_none");
                return;
            }

            UseSeedToPlantTree(treeConf.id, -1, false, treeData.uId);
        }

        /// <summary>
        /// 使用种子种树
        /// </summary>
        public void UseSeedToPlantTree(int treeId, int itemId = -1, bool isPlantTree = true, int treeUId = -1)
        {
            ComTree treeConf = ComTree.Get(treeId);
            if (treeConf != null)
            {
                int currentSceneId = m_LevelModule.levelId;
                if (treeConf.sceneId == currentSceneId)             // 当前场景为种树场景
                {
                    MoveToTreeLocation(treeConf, isPlantTree, treeUId, itemId);
                }
                else
                {
                    App.my.sceneMgr.ChangeScene(treeConf.sceneId, () => // 切换场景
                    {
                        MoveToTreeLocation(treeConf, isPlantTree, treeUId, itemId);
                    });
                }
            }
        }

        // 移动前面随机的位置
        private void MoveToTreeLocation(ComTree treeConf, bool isPlantTree = true, int treeUId = -1, int itemId = -1)
        {
            string pointId = treeConf.pointId;
            LevelDesignConfig.LevelPointData pointSet = m_LevelModule.GetPointData(pointId);

            if (pointSet != null)
            {
                int index = m_TreeSetRandomDic[pointId];
                Vector3 pos = pointSet.m_postions[index];

                App.my.localPlayer.battle.State_PathToPos(pos, (obj) =>
                {
                    if (isPlantTree)
                        RequestPlantTree(treeConf.id, pos, index, itemId);
                    else
                    {
                        if (m_PlayerTreeModel != null && m_PlayerTreeModel.ContainsKey(treeUId))
                        {
                            m_PlayerTreeModel[treeUId].OnClickOwnTree();
                        }
                    }
                });
            }
        }

        private void RequestPlantTree(int treeId, Vector3 pos, int index, int itemId = -1)
        {
            // 开始吟唱，暂时先写死，1是采集动作
            App.my.localPlayer.battle.State_Sing(1, (obj) =>
            {
                PlantTree treeData = new PlantTree();
                treeData.treeId = treeId;
                treeData.posX = pos.x;
                treeData.posY = pos.y;
                treeData.posZ = pos.z;
                treeData.pointSetId = index;
                treeData.subItemId = itemId;

                hotApp.my.eventSet.FireEvent(EventID.MoneyTree_RequestPlantTree, treeData);
            });
        }

        // 切换场景
        private void OnFinishLoadScene()
        {
            m_TreeSetRandomDic.Clear();

            int levelId = m_LevelModule.levelId;
            List<string> pointList = MoneyTreeDef.GetCurrentLevelSet(levelId);
            if (pointList == null)
            {
                DelectAllModels();
                return;
            }

            // 给不同的摇钱树随机一个位置，确保一定给玩家预留位置
            for (int i = 0; i < pointList.Count; ++i)
            {
                string pointSetId = pointList[i];
                LevelDesignConfig.LevelPointData pointSet = m_LevelModule.GetPointData(pointSetId);
                if (pointSet != null)
                {
                    int index = UnityEngine.Random.Range(0, pointSet.m_postions.Count);
                    m_TreeSetRandomDic.Add(pointSetId, index);
                }
            }

            if (MoneyTreeDef.HasMoneyTreeBelongToMe())
            {
                for (int i = 0; i < m_MoneyTreeDbData.listPlayerTrees.Count; i++)
                {
                    OneTreeData treeData = m_MoneyTreeDbData.listPlayerTrees[i];
                    ComTree treeConf = ComTree.Get(treeData.treeId);

                    if (treeConf != null)
                    {
                        if (treeConf.sceneId == levelId)
                        {
                            int pointIndex = treeData.pointSetId;
                            string setId = treeConf.pointId;
                            if (m_TreeSetRandomDic.ContainsKey(setId))
                            {
                                // 如果已经有树了则使用该位置
                                m_TreeSetRandomDic[setId] = pointIndex;
                            }

                            CreatePlayerTree(treeData);
                        }
                    }
                }
            }
            CreateFakeTree(levelId);
        }

        private void DelectAllModels()
        {
            if (m_PlayerTreeModel != null && m_PlayerTreeModel.Count > 0)
            {
                foreach (MoneyTreeActor treeData in m_PlayerTreeModel.Values)
                {
                    m_ObjectMgr.RemoveObj(treeData.uid);
                }
                m_PlayerTreeModel.Clear();
            }

            if (m_FakesTreeModel != null && m_FakesTreeModel.Count > 0)
            {
                for (int i = 0; i < m_FakesTreeModel.Count; i++)
                {
                    m_ObjectMgr.RemoveObj(m_FakesTreeModel[i].uid);
                }
                m_FakesTreeModel.Clear();
            }
        }

        // 开始种树
        private void PlantTreeCallBack(OneTreeData treeData)
        {
            CreatePlayerTimer(treeData);
            CreatePlayerTree(treeData);
        }

        // 创建玩家的树
        private void CreatePlayerTree(OneTreeData data)
        {
            OneTreeData playerTreeData = null;
            if (m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                playerTreeData = m_MoneyTreeDbData.listPlayerTrees[data.uId];

            if (playerTreeData == null)
                return;

            int treeResId = GetTreeStageRes(playerTreeData);
            if (treeResId == -1)
                return;

            LcoalActorCxt playerData = new LcoalActorCxt();
            playerData.roleId = treeResId;
            playerData.bornPos = new Vector3(playerTreeData.posX, playerTreeData.posY, playerTreeData.posZ);
            playerData.name = (data.leaveTime > 0 ? GetLeaveTime(data.leaveTime - 1) : GetLeaveTime(0));

            MoneyTreeActor playerActor = m_ObjectMgr.CreateObj(LocalObjectType.MoneyTreeObject, treeResId, playerData) as MoneyTreeActor;
            if (playerActor != null)
            {
                playerActor.treeUId = data.uId;
                m_PlayerTreeModel[data.uId] = playerActor;
            }

            if (m_TreeTimerDic.Count > 0 && m_TreeTimerDic.ContainsKey(data.uId))
            {
                m_TreeTimerDic[data.uId].modelUId = playerActor.uid;
            }
        }

        // 创建树定时器
        public void CreatePlayerTimer(OneTreeData data)
        {
            TreeTimerData timerData = new TreeTimerData();
            timerData.treeUId = data.uId;
            timerData.modelUId = -1;
            timerData.leaveTime = data.leaveTime - 1; // 消息传过来已经有1s的误差
            timerData.stage = data.stage;

            m_Timer.Release();
            m_MoneyTreeDbData.listPlayerTrees[timerData.treeUId].leaveTime -= 1;

            UI.MainPanel.SetItemText((int)xys.UI.PanelType.UIMoneyTreePanel, GetLeaveTime(timerData.leaveTime, false));
            UI.MainPanel.SetItemReadyActive((int)xys.UI.PanelType.UIMoneyTreePanel, false);
            timerData.timer = m_Timer.Register<TreeTimerData>(1, int.MaxValue, TreeTimesOut, timerData);

            if (m_TreeTimerDic.ContainsKey(timerData.treeUId))
                m_TreeTimerDic[timerData.treeUId] = timerData;
            else
                m_TreeTimerDic.Add(timerData.treeUId, timerData);

            UI.MainPanel.SetItemActive((int)xys.UI.PanelType.UIMoneyTreePanel, true);
        }

        // 创建一些假树，营造氛围
        private void CreateFakeTree(int currentSceneId)
        {
            List<string> pointList = MoneyTreeDef.GetCurrentLevelSet(currentSceneId);
            if (pointList == null)
            {
                return;
            }

            for (int i = 0; i < pointList.Count; i++)
            {
                string pointId = pointList[i];
                LevelDesignConfig.LevelPointData pointSet = m_LevelModule.GetPointData(pointId);
                if (pointSet != null)
                {
                    int playerIndex = -1;
                    if (m_TreeSetRandomDic.ContainsKey(pointId))
                        playerIndex = m_TreeSetRandomDic[pointId];

                    for (int j = 0; j < pointSet.m_postions.Count; j++)
                    {
                        if (j != playerIndex) // 排除玩家种树的位置
                        {
                            int randomNum = UnityEngine.Random.Range(0, 100); // 计算概率
                            if (randomNum <= 90)
                            {
                                int treeResId = UnityEngine.Random.Range(0, 3);
                                ComTree treeConf = ComTree.Get(1); // 默认是第一种摇钱树

                                LcoalActorCxt fakeData = new LcoalActorCxt();
                                fakeData.roleId = treeConf.resId[treeResId];
                                fakeData.bornPos = new Vector3(pointSet.m_postions[j].x, pointSet.m_postions[j].y, pointSet.m_postions[j].z);

                                MoneyTreeActor fakeActor = m_ObjectMgr.CreateObj(LocalObjectType.MoneyTreeObject, fakeData.roleId, fakeData) as MoneyTreeActor;
                                fakeActor.treeUId = -1;
                                m_FakesTreeModel.Add(fakeActor);
                            }
                        }
                    }
                }
            }
        }

        // 获取当前树的模型res
        private int GetTreeStageRes(OneTreeData data)
        {
            ComTree treeConf = ComTree.Get(data.treeId);
            if (treeConf == null)
                return -1;

            OneTreeData treeData = null;
            if (m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                treeData = m_MoneyTreeDbData.listPlayerTrees[data.uId];

            if (treeData == null)
                return -1;

            if (treeData.stage <= treeConf.resId.Length)
                return treeConf.resId[treeData.stage];

            return -1;
        }

        private void TreeTimesOut(TreeTimerData timerData)
        {
            if (m_TreeTimerDic.ContainsKey(timerData.treeUId))
            {
                MoneyTreeActor playerObject = m_ObjectMgr.GetObj(timerData.modelUId) as MoneyTreeActor;
                TreeTimerData data = m_TreeTimerDic[timerData.treeUId];
                data.leaveTime--;

                if (m_MoneyTreeDbData.listPlayerTrees.ContainsKey(timerData.treeUId))
                    m_MoneyTreeDbData.listPlayerTrees[timerData.treeUId].leaveTime--;

                if (data.leaveTime < 0)
                {
                    if (data.timer != 0)
                    {
                        m_Timer.Cannel(data.timer);
                        data.timer = 0;
                    }

                    if (playerObject != null)
                    {
                        string playerName = xys.UI.Utility.TipContentUtil.GenText("moneyTree_player_name");
                        string playerGet = xys.UI.Utility.TipContentUtil.GenText("moneyTree_player_get");
                        playerObject.modelEntity.m_handler.SetName(App.my.localPlayer.name + playerName + playerGet);
                    }

                    UI.MainPanel.SetItemText((int)xys.UI.PanelType.UIMoneyTreePanel, "");
                    UI.MainPanel.SetItemReadyActive((int)xys.UI.PanelType.UIMoneyTreePanel, true);

                    return;
                }

                if (timerData.treeUId != -1)
                {
                    // 更新树头部的倒计时信息
                    // 刷新主界面倒计时信息
                    if (playerObject != null)
                        playerObject.modelEntity.m_handler.SetName(GetLeaveTime(timerData.leaveTime));

                    UI.MainPanel.SetItemText((int)xys.UI.PanelType.UIMoneyTreePanel, GetLeaveTime(timerData.leaveTime, false));
                    UI.MainPanel.SetItemReadyActive((int)xys.UI.PanelType.UIMoneyTreePanel, false);
                }
            }
        }

        // 使用加速道具后，修改树的数据
        private void UseSpeedItemChangeTree(OneTreeData data)
        {
            OneTreeData treeData = null;
            if (m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                treeData = m_MoneyTreeDbData.listPlayerTrees[data.uId];

            if (treeData == null)
                return;

            //阶段显示不同的模型
            int stage = treeData.stage;
            if (m_TreeTimerDic.ContainsKey(data.uId))
            {
                TreeTimerData timerData = m_TreeTimerDic[data.uId];
                timerData.leaveTime = treeData.leaveTime;
                if (timerData.timer != 0)
                {
                    m_Timer.Release();
                    timerData.timer = 0;
                }

                TreeTimesOut(timerData);
                timerData.timer = m_Timer.Register<TreeTimerData>(1, int.MaxValue, TreeTimesOut, timerData);

                if (stage != timerData.stage)
                {
                    timerData.stage = stage;

                    // 只有在当前场景才响应
                    if (MoneyTreeDef.ThisTreeIsInScene(data.treeId))
                    {
                        foreach (MoneyTreeActor modelData in m_PlayerTreeModel.Values)
                        {
                            if (modelData.uid == timerData.modelUId)
                            {
                                modelData.ReplaceModelObject(GetTreeStageRes(treeData));
                                return;
                            }
                        }
                    }
                }
            }
        }

        // 领取奖励返回
        private void GetRewardCallBack(OneTreeData data)
        {
            UI.MainPanel.SetItemActive((int)xys.UI.PanelType.UIMoneyTreePanel, false);

            DeleteTree(data.uId);
        }

        // 删除树模型
        public void DeleteTree(int treeUId, bool isHide = true)
        {
            var panel = xys.UI.UISystem.Get("UIPlantTreePanel");
            if (isHide && panel != null && panel.state == xys.UI.UIPanelBase.State.Show)
                App.my.uiSystem.HidePanel("UIPlantTreePanel");

            foreach (MoneyTreeActor treeModelData in m_PlayerTreeModel.Values)
            {
                if (treeModelData.treeUId == treeUId)
                {
                    treeModelData.Destroy();
                    m_PlayerTreeModel.Remove(treeUId);
                    return;
                }
            }

            if (m_TreeTimerDic.ContainsKey(treeUId))
            {
                TreeTimerData data = m_TreeTimerDic[treeUId];
                m_Timer.Release();
                m_TreeTimerDic.Remove(treeUId);
            }
        }

        // 请求使用加速道具
        private void RequestToReduceTreeTime(OneTreeData data)
        {
            OneTreeData treeData = null;
            if (m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                treeData = m_MoneyTreeDbData.listPlayerTrees[data.uId];

            if (treeData == null)
                return;

            ComTree treeConf = ComTree.Get(treeData.treeId);
            if (treeConf == null)
                return;

            // 查看成长值是否到达上限
            int growUpValue = treeData.growthNum;
            int growUpMax = treeConf.growthMax;
            if (growUpValue >= growUpMax)
            {
                xys.UI.Utility.TipContentUtil.Show("moneyTree_growth_max");
                return;
            }

            int reduceItem = treeConf.speedUpItemId;
            int count = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(reduceItem);
            if (count > 0)
            {
                if (!MoneyTreeDef.IsCanUseSpeedItem(treeData))
                {
                    xys.UI.Utility.TipContentUtil.Show("moneyTree_watering_max");
                    return;
                }

                // 通知服务器
                hotApp.my.eventSet.FireEvent(EventID.MoneyTree_RequestUseSpeedUpItem, treeData);
            }
            else
            {
                xys.UI.Utility.TipContentUtil.Show("moneyTree_not_watering");
            }
        }

        // 显示摇钱树模型上方倒计时
        public string GetLeaveTime(int leaveTime, bool isShowName = true)
        {
            int leaveTimes = leaveTime;
            TimeSpan timeSpan = new TimeSpan(0, 0, leaveTimes);
            string formatStr = leaveTimes > 3600 ? "HH:mm:ss" : "mm:ss";
            string playerName = xys.UI.Utility.TipContentUtil.GenText("moneyTree_player_name");
            string playerGet = xys.UI.Utility.TipContentUtil.GenText("moneyTree_player_get");

            if (leaveTimes > 0)
                return (isShowName ? (App.my.localPlayer.name + playerName) : "") + Convert.ToDateTime(timeSpan.ToString()).ToString(formatStr);
            else
                return (isShowName ? (App.my.localPlayer.name + playerName + playerGet) : "");
        }

        // 重新返回登录界面
        private void OnBackToLogin()
        {
            // 移除当前的计时器
            if (m_TreeTimerDic != null)
            {
                m_Timer.Release();
                m_TreeTimerDic.Clear();
            }

            if (m_TreeSetRandomDic != null)
                m_TreeSetRandomDic.Clear();

            DelectAllModels();
        }

        // 选中角色后，成功进入游戏
        private void OnEnterGame()
        {
            m_TreeSetRandomDic = new Dictionary<string, int>();
            m_TreeTimerDic = new Dictionary<int, TreeTimerData>();

            m_FakesTreeModel = new List<MoneyTreeActor>();
            m_PlayerTreeModel = new Dictionary<int, MoneyTreeActor>();
        }
    }
}
#endif
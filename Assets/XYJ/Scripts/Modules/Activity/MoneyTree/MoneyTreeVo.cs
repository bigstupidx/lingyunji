#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/22


namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;

    partial class HotMoneyTreeModule
    {
        public MoneyTreeModuleRequest m_request { get; private set; }

        private void Init()
        {
            this.m_request = new MoneyTreeModuleRequest(hotApp.my.gameRPC);

            Event.Subscribe<PlantTree>(EventID.MoneyTree_RequestPlantTree, RequestPlantTree);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_RequestUseSpeedUpItem, RequestUseSpeedUpItem);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_RequestGetReward, RequestGetReward);
        }

        // 推送协议注册
        private void RegistMsg()
        {
            hotApp.my.handler.RegHot<OneTreeData>(Protoid.A2C_Activity_MoneyTree_RefreshUI, OnRefreshUIData);
            hotApp.my.handler.RegHot<OneTreeData>(Protoid.A2C_Activity_MoneyTree_Refresh_GrowthNum, OnRefreshGrowthNum);
            hotApp.my.handler.RegHot<OneTreeData>(Protoid.A2C_Activity_MoneyTree_Mature_Push, OnTreeMaturePush);
            hotApp.my.handler.RegHot<OneTreeData>(Protoid.A2C_Activity_MoneyTree_TenMinutes_Push, OnTreeMatureTenMinutesPush);
        }

        // 要求种树
        private void RequestPlantTree(PlantTree treeData)
        {
            this.m_request.RequestPlantOneTree(treeData, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.error == MoneyTreeError.MoneyTreeError_None)
                {
                    xys.UI.Utility.TipContentUtil.Show("activity_moneyTree_none");
                    return;
                }
                else if (respone.error == MoneyTreeError.MoneyTreeError_HasMineTree)
                {
                    xys.UI.Utility.TipContentUtil.Show("moneyTree_has_plant_tree");
                    return;
                }

                moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[respone.treeData.uId] = respone.treeData;
                Event.FireEvent(EventID.MoneyTree_PlantTreeSuccess, respone.treeData); // 刷新ui
            });
        }

        // 要求使用加速道具
        private void RequestUseSpeedUpItem(OneTreeData data)
        {
            // 向后端发送开启协议
            NetProto.Int32 requestData = new NetProto.Int32();
            requestData.value = data.uId;

            this.m_request.RequestSpeedUpOneTree(requestData, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.error == MoneyTreeError.MoneyTreeError_None)
                {
                    xys.UI.Utility.TipContentUtil.Show("activity_moneyTree_none");
                    return;
                }
                else if (respone.error == MoneyTreeError.MoneyTreeError_GrowthMax)
                {
                    xys.UI.Utility.TipContentUtil.Show("moneyTree_growth_max");
                    return;
                }
                else if (respone.error == MoneyTreeError.MoneyTreeError_WateringMax)
                {
                    xys.UI.Utility.TipContentUtil.Show("moneyTree_watering_max");
                    return;
                }
                else if (respone.error == MoneyTreeError.MoneyTreeError_Not_WateringItem)
                {
                    xys.UI.Utility.TipContentUtil.Show("moneyTree_not_watering");
                    return;
                }

                xys.UI.Utility.TipContentUtil.Show("moneyTree_watering_success");

                if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(respone.treeData.uId))
                    moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[respone.treeData.uId] = respone.treeData;

                Event.FireEvent(EventID.MoneyTree_UseSpeedUpItemSuccess, respone.treeData);
            });
        }

        // 请求领取奖励
        private void RequestGetReward(OneTreeData treeData)
        {
            // 向后端发送开启协议
            NetProto.Int32 requestData = new NetProto.Int32();
            requestData.value = treeData.uId;

            this.m_request.RequestReceiveOneTree(requestData, (error, respone) =>
            {
                if (error != wProtobuf.RPC.Error.Success)
                    return;

                if (respone.error == MoneyTreeError.MoneyTreeError_None)
                {
                    xys.UI.Utility.TipContentUtil.Show("activity_moneyTree_none");
                    return;
                }
                else if (respone.error == MoneyTreeError.MoneyTreeError_NotToTimeReceive)
                {
                    xys.UI.Utility.TipContentUtil.Show("moneyTree_notTime_get");
                    return;
                }

                if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(treeData.uId))
                    moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.Remove(treeData.uId);
                Event.FireEvent(EventID.MoneyTree_GetRewardSuccess, treeData);
            });
        }

        // 树成熟推送
        private void OnTreeMaturePush(OneTreeData data)
        {
            if (!MoneyTreeDef.ThisTreeBelongToMe(data.uId))
                return;

            if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[data.uId] = data;

            UI.MainPanel.SetItemText((int)xys.UI.PanelType.UIMoneyTreePanel, "");
            UI.MainPanel.SetItemReadyActive((int)xys.UI.PanelType.UIMoneyTreePanel, true);
            App.my.uiSystem.ShowPanel("UIPlantTreeTipPanel", data, true);

            Event.FireEvent(EventID.MoneyTree_Tree_Mature_Push, data);
        }

        // 树成熟十分钟推送
        private void OnTreeMatureTenMinutesPush(OneTreeData data)
        {
            if (!MoneyTreeDef.ThisTreeBelongToMe(data.uId))
                return;

            if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.Remove(data.uId);

            moneyTreeMgr.DeleteTree(data.uId);

            UI.MainPanel.SetItemActive((int)xys.UI.PanelType.UIMoneyTreePanel, false);
        }

        // 刷新界面
        private void OnRefreshUIData(OneTreeData data)
        {
            if (!MoneyTreeDef.ThisTreeBelongToMe(data.uId))
                return;

            if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(data.uId))
                moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[data.uId] = data;

            Event.FireEvent(EventID.MoneyTree_Refresh_UI_Data, data);
        }

        private void OnRefreshGrowthNum(OneTreeData treeData)
        {
            if (treeData == null)
                return;

            if (moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(treeData.uId))
                moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[treeData.uId].growthNum = treeData.growthNum;

            Event.FireEvent(EventID.MoneyTree_Refresh_UI_GrowthNum, treeData);
        }
    }
}
#endif
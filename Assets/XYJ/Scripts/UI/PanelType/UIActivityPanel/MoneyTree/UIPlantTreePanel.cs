#if !USE_HOT
// Author : PanYuHuan
// Create Date : 2017/7/20


using NetProto.Hot;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;


namespace xys.hot.UI
{
    class UIPlantTreePanel : HotPanelBase
    {
        [SerializeField]
        private Image m_ProgressImage;
        [SerializeField]
        private Image m_PointImage;
        [SerializeField]
        private Image m_IconImage;

        [SerializeField]
        private Text m_TimerText;
        [SerializeField]
        private Text m_GrowthText;
        [SerializeField]
        private Text m_ItemNumText;
        [SerializeField]
        private Text m_TimeDesText;
        [SerializeField]
        private Text m_MatureText;

        [SerializeField]
        private Button m_GetBtn;
        [SerializeField]
        private Button m_WateringBtn;
        [SerializeField]
        private Button m_IconBtn;
        [SerializeField]
        private Button m_CloseBtn;

        private OneTreeData m_TreeData;
        private Config.ComTree m_TreeConf;
        private int leaveTimer = 0;
        private int growthUpNum = 0;

        private MoneyTreeMgr moneyTreeMgr;

        public UIPlantTreePanel() : base(null) { }
        public UIPlantTreePanel(UIHotPanel parent) : base(parent)
        {
        }

        protected override void OnInit()
        {
            m_GetBtn.onClick.AddListener(OnClickGetReward);
            m_CloseBtn.onClick.AddListener(OnClickCloseView);
            m_WateringBtn.onClick.AddListener(OnClickWatering);
            m_IconBtn.onClick.AddListener(OnClickShowItemData);

            moneyTreeMgr = hotApp.my.GetModule<HotMoneyTreeModule>().moneyTreeMgr;
        }

        protected override void OnShow(object args)
        {
            if (args == null)
                return;

            if (moneyTreeMgr != null && moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey((args as OneTreeData).uId))
                m_TreeData = moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[(args as OneTreeData).uId];

            SetData(m_TreeData);

            Event.Subscribe<OneTreeData>(EventID.MoneyTree_Refresh_UI_Data, OnRefreshTreeStage);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_Tree_Mature_Push, OnRefreshGrowthData);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_Refresh_UI_GrowthNum, OnRefreshGrowthData);
            Event.Subscribe<OneTreeData>(EventID.MoneyTree_UseSpeedUpItemSuccess, OnUseSpeedItemCallBack);
        }

        private void SetData(OneTreeData treeData)
        {
            m_TreeConf = Config.ComTree.Get(treeData.treeId);
            if (m_TreeConf == null)
                return;

            // 成长值
            SetGrowthData();

            // 加速道具数量
            growthUpNum = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(m_TreeConf.speedUpItemId);
            SetSpeedItemData();

            // 设置树阶段
            SetStageData();

            // 设置定时器
            m_TimerText.text = GetTimeStr(m_TreeData.leaveTime);
            CreateTimer();

            Config.Item itemData = Config.Item.Get(m_TreeConf.speedUpItemId);
            if (itemData == null)
                return;

            Helper.SetSprite(m_IconImage, itemData.icon);
        }

        private void CreateTimer()
        {
            if (leaveTimer > 0)
            {
                App.my.mainTimer.Cannel(leaveTimer);
                leaveTimer = 0;
            }
            if (m_TreeData.leaveTime > 0)
                leaveTimer = App.my.mainTimer.Register(1, int.MaxValue, OnSetTimerData);
        }

        private void OnSetTimerData()
        {
            m_TimerText.text = GetTimeStr(m_TreeData.leaveTime);

            if (m_TreeData.leaveTime <= 0)
            {
                SetStageData();

                if (leaveTimer > 0)
                {
                    App.my.mainTimer.Cannel(leaveTimer);
                    leaveTimer = 0;
                }
            }
        }

        private string GetTimeStr(int leaveTimes)
        {
            string str = "";
            System.TimeSpan timeSpan = new System.TimeSpan(0, 0, leaveTimes);
            string format = leaveTimes > 3600 ? "HH:mm:ss" : "mm:ss";
            if (leaveTimes > 0)
                str = System.Convert.ToDateTime(timeSpan.ToString()).ToString(format);
            else
                str = "";

            return str;
        }

        private void OnRefreshGrowthData(OneTreeData treeData)
        {
            if (moneyTreeMgr != null && moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(treeData.uId))
                m_TreeData = moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[treeData.uId];

            SetGrowthData();
        }

        private void SetGrowthData()
        {
            m_GrowthText.text = m_TreeData.growthNum + "/" + m_TreeConf.growthMax;

            float progreeNum = (float)m_TreeData.growthNum / (float)m_TreeConf.growthMax;
            m_ProgressImage.fillAmount = progreeNum;
            m_PointImage.rectTransform.anchoredPosition = new Vector2(progreeNum * m_ProgressImage.rectTransform.sizeDelta.x, 0);
        }

        private void OnRefreshTreeStage(OneTreeData treeData)
        {
            if (moneyTreeMgr != null && moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees.ContainsKey(treeData.uId))
                m_TreeData = moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[treeData.uId];

            SetStageData();

            m_TimerText.text = GetTimeStr(m_TreeData.leaveTime);
            CreateTimer();
        }

        private void SetStageData()
        {
            bool isMature = (m_TreeData.stage == (int)TreeStage.enumMature && m_TreeData.leaveTime <= 0);
            if (isMature)
            {
                m_WateringBtn.gameObject.SetActive(false);
                m_GetBtn.gameObject.SetActive(true);
            }
            else
            {
                m_WateringBtn.gameObject.SetActive(true);
                m_GetBtn.gameObject.SetActive(false);
            }

            m_TimeDesText.gameObject.SetActive(!isMature);
            m_TimerText.gameObject.SetActive(!isMature);
            m_MatureText.gameObject.SetActive(isMature);
        }

        private void OnUseSpeedItemCallBack(OneTreeData treeData)
        {
            OnRefreshTreeStage(treeData);
            SetGrowthData();

            growthUpNum -= 1;
            SetSpeedItemData();
        }

        private void SetSpeedItemData()
        {
            m_ItemNumText.text = growthUpNum + "/1";
            m_ItemNumText.color = growthUpNum >= 1 ? Color.green : Color.red;
        }

        // 收获
        private void OnClickGetReward()
        {
            hotApp.my.eventSet.FireEvent(xys.EventID.MoneyTree_RequestGetReward, m_TreeData);
            OnClickCloseView();
        }

        // 浇灌，使用加速道具
        private void OnClickWatering()
        {
            int growthUpNum = hotApp.my.GetModule<HotPackageModule>().packageMgr.GetItemCount(m_TreeConf.speedUpItemId);
            if (growthUpNum > 0)
                hotApp.my.eventSet.FireEvent(xys.EventID.MoneyTree_RequestReduceTreeTime, m_TreeData);
            else
            {
                // 判定道具是否配置了快捷购买
                xys.UI.SystemHintMgr.ShowHint("材料不足，后面统一添加快捷购买！");
            }
        }

        // 显示加速道具详情
        private void OnClickShowItemData()
        {
            Config.ComTree treeConf = Config.ComTree.Get(m_TreeData.treeId);
            if (treeConf == null)
                return;

            Config.Item itemData = Config.Item.Get(treeConf.speedUpItemId);
            if (itemData == null)
                return;

            UICommon.ShowItemTips(itemData.id);
        }

        private void OnClickCloseView()
        {
            App.my.uiSystem.HidePanel("UIPlantTreePanel");
        }

        protected override void OnHide()
        {
            if (leaveTimer > 0)
            {
                App.my.mainTimer.Cannel(leaveTimer);
                leaveTimer = 0;
            }
        }
    }
}
#endif
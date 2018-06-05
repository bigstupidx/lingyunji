namespace xys
{
    //全局事件
    public enum EventID 
    {
        Null,                   // 无效的
        LocalAttributeChange, // 本地玩家的属性变化
        LocalBattleAttChange,   // 本地玩家的战斗属性变化
        BackToLogin,            // 重回登录界面,一些全局数据需要复位
        BeginLogin,             // 开始登陆
        
        ConnectedGate,          // 连接上登陆服
        ConnectedGateError,     // 连接登陆服出错
        OfflineGate,            // 与网关服断开连接

        FinishAppInit,          //游戏初始化结束
        BeginLoadScene,         //开始加载场景
        FinishLoadScene,        //完成加载场景
        FinishLoadSceneParam,   //完成加载场景-带有是否无缝切换的参数
        //EnterAppState,        // 进入App某个状态
        //LeaveAppState,        // 离开App某个状态

        #region 登录的事件
        Login_ConnectLoginSucess,       //连接登录成功
        Login_OnConnectedGate,          //连接网关服成功
        Login_ShowCreateRole,           //显示创建角色
        Login_ShowRoleList,             //显示角色列表
        Login_SelectRoleSucess,         //选择角色成功

        Login_SelectServer,             //选择服务器
        Login_SelectRole,               //选择角色
        Login_CreateRole,               //创建角色
        Login_DeleteRole,               //删除角色
        Login_WaitDeleteRole,           //延迟删除角色
        Login_RestoreRole,              //恢复角色
        Login_DeleteRoleRet,            //删除角色返回
        Login_WaitDeleteRoleRet,        //延迟删除角色返回
        Login_RestoreRoleRet,           //恢复角色返回
        Login_EnterCreate,              //选角的时候选择创建角色
        Login_EnterChoose,              //创角的时候退回到选择角色
        Login_FinishCg,                 //cg播放完成
        Login_Disguise2Login,           //易容返回登录
        #endregion

        #region 宠物相关
        Pets_Create,//宠物创建
        Pets_Delect,//删除宠物
        Pets_Wash,//洗练宠物
        Pets_Refinery,//炼化宠物
        Pets_LearnSkill,//技能学习
        Pets_LockSkill,//锁定技能
        Pets_AddExp,//
        Pets_SetPotential,//天赋点
        Pets_ResetPotential,//重置天赋点
        Pets_SetPetPlay,
        Pets_RequestPlayPet,
        Pets_Slider,//潜能百分比分配
        Pets_SetName,//昵称
        Pets_SetAi,//设置AI类型
        Pets_SetQualification,//洗练宠物资质

        Pets_SetPavvy,//
        Pets_SetGrowth,//
        Pets_SetPersonality,//
        Pets_OpenHoles,//

        Pets_2Items,//宠物转道具

        Pets_HB_Sort,//宠物面板Sort
        Pets_RefreshUI,//UI刷新
        Pets_SliderUI,//加点UI刷新
        Pets_CreateRefresh,
        
        Pets_Page_Refresh,
        Pets_DataRefresh,
        #endregion

        #region 组队相关
        Team_DataChange,
        Team_SundryDataChange,
        Team_PlatformQueryTeamInfos,
        Team_QueryTeamsByFilter,
        Team_QueryTeamsRet,
        Team_ApplyJoinTeam,
        Team_ApplyAutoJoinTeam,
        Team_ReqCreateTeam,
        Team_JoinReqInfoChange,
        Team_ReqQueryNearbyUser,
        Team_RspQueryNearbyUser,
        Team_NewInviteJoinInfo,
        Team_InviteJoinInfoChange,
        Team_UnreadJoinReqInfoFlagChange,
        Team_ReadedJoinReqInfo,
        Team_EnterTeam,
        #endregion

        #region 主界面相关的事件
        MainPanel_ActivityClick, // 点击活动按钮
        MainPanel_IntegralClick, // 积分按钮
        MainPanel_EasyCityClick, // 易市按钮
        MainPanel_WelfareClick, // 福利按钮
        MainPanel_StrongerClick, // 变强按钮
        MainPanel_StoreClick, // 商店按钮
        MainPanel_FirstChargeClick, // 首充按钮
        MainPanel_AscensionClick, // 提升按钮
        MainPanel_PackageClick, // 包裹按钮
        MainPanel_CameraChangeClick, // 相机切换

        MainPanel_FireSkill, // 释放技能
        MainPanel_SetSkill,  // 设置技能
        MainPanel_SetSkillState,//设置技能状态
        MainPanel_BattleFuntion, //战斗功能
        MainPanel_SetTarget,      //设置选中目标

        MainPanel_ChangeMinimap,    //切换小地图
        MainPanel_UIItemReady,  //界面小图标红点显示时间
        MainPanel_UIItemNotReady,   //界面小图标红点消失事件
        MainPanel_ShowItem,  //界面小图标显示event
        MainPanel_HideItem,  //界面小图标显示event
        #endregion

        #region 背包事件
        Package_UpdatePackage,      // 更新背包格子
        Package_CountChange,        // 数量变化
        Package_AddChange,          // 增加新道具
        Package_ReomveChange,       // 删除道具
        Package_UpdateEquip,        // 更新装备
        Package_TipsClose,          // 包裹tips关闭
        Package_ProgessBar,         // 道具进度条事件
        Pacakge_Selected,           // 道具选中事件
        #endregion

        #region 福利
        Welfare_OnSignItem,
        Welfare_OnSign,
        Welfare_OnSubsign,
        Welfare_GetOnlineRwd,
        Welfare_GetLevelRwd,
        Welfare_GetSevendayRwd,
        Welfare_RefreshUI,
        Welfare_GetData,
        Welfare_StateType,
        Welfare_GetItemInfo,
        Welfare_PageRewardReady,
        Welfare_PageRewardNotReady,
        Welfare_NoPageReward,
        Welfare_DaySignRwdReceived,
        Welfare_DayOLRwdReceived,
        Welfare_OLRwdReceived,
        Welfare_LVRwdReceived,
        Welfare_RefreshPageInfo,
        Welfare_Test,
        #endregion

        #region 聊天的事件
        /********************NEW***************/
        // 点击物品信息
        ChatModule_OnSearchItems,
        // 宠物搜索
        ChatModule_OnSearchPets,
        // 更新聊天信息
        ChatModule_OnReceiveMsg,
        // 发送聊天信息
        ChatModule_OnSendMsg,
        // 发送系统消息
        ChatModule_OnSendSystemMsg,
        // 刷新主界面聊天的消息
        ChatMainPanel_RefreshMainChatMsg,
        // 刷新主界面@消息提醒
        ChatMainPanel_Marker,
        // 刷新主界面聊天的内容高度
        ChatMainPanel_RefreshMainChatHeight,
        // 刷新主界面聊天的按钮高度
        ChatMainPanel_RefreshMainChatBtnHeight,
        // 主界面聊天点击扩展按钮时聊天框变化
        ChatMainPanel_ExpandBtnClick,
        // 刷新主界面聊天新消息提示事件
        ChatMainPanel_NewMessageTip,

        // 更新聊天面板信息
        ChatPanel_OnReceiveMsg,
        // 更新输入框@玩家数据
        ChatInput_OnReceiveUserData,
        // 更新输入框道具数据
        ChatInput_OnReceiveItemData,
        // 更新输入框表情数据
        ChatInput_OnReceiveFaceData,
        // 更新输入框便捷输入
        ChatInput_OnReceiveInputSimple,
        // 更新输入框历史输入
        ChatInput_OnReceiveInputHistory,
        // 更新输入框宠物数据
        ChatInput_OnReceivePetsData,

        // 更新系统消息
        ChatPanel_OnReceiveSystemMsg,
        // 更新聊天面板顶起状态
        ChatPanel_OnChatPanelHangUpState,
        // 恢复聊天面板常态
        ChatPanel_OnChatPanelCommonState,

        // 语音提示面板麦克风
        ChatVoiceTips_Misc,
        // 语音提示面板取消
        ChatVoiceTips_Cancle,

        // 更新聊天历史输入面板
        ChatPanel_RefreshHistory,
		#endregion

        #region 装备
        Equip_Inforce,    //强化
        Equip_Refine,     //炼化
        Equip_Recast,    //重铸
        Equip_Concise,   //凝练
        Equip_Forge,     //打造
        Equip_ReplaceRefine,  //替换炼化属性
        Equip_ReplaceRecast, //替换重铸
        Equip_ReplaceConcise, //替换凝炼
        Equip_Create,     //创建
        Equip_LoadFinish,    //穿上装备成功
        Equip_UnLoadFinish,    //脱下装备成功
        Equip_SyncData,  //同步装备属性数据
        Equip_RefreshUI, //刷新界面
        Equip_RefreshEquipList, //刷新装备列表
        Equip_SelectRefMaterial, //选择材料
        Equip_NoneEquiped,  //列表无装备
        Equip_UpdateData,   //装备数据更新
        Equip_ResetTimes,  //重置使用中装备的数据
        Equip_SetOpTimesActive, //设置操作次数是否生效
        #endregion
        #region 装备附魂
        EquipSoul_OpenGrid,
        EquipSoul_Enforce,
        EquipSoul_UpdateData,
        EquipSoul_RefreshUI, //刷新界面
        EquipSoul_Load,    //穿上soul
        EquipSoul_UnLoad,    //脱下soul
        EquipSoul_LoadFinish,    //穿上soul成功
        EquipSoul_UnLoadFinish,    //脱下soul成功
        EquipSoul_RefreshSoulList, //刷新装备列表
        #endregion
        #region 好友
        Friend_Search,   //好友搜索
        Friend_SearchDataChange,  //好友搜索数据改变
        Friend_Apply,    //好友申请
        Friend_ApplyDataChange,   //好友申请列表改变
        Friend_GetApplyData,      //好友申请列表
        Friend_ClearAllApply,     //清空所有申请
        Friend_AddFriend,         //同意申请并添加好友
        Friend_PushFriendData,        //推送好友数据
        Friend_GetFriendData,         //获取好友数据
        Friend_BlakSomeOne,           //拉入黑名单
        Friend_GetRecentlyInfo,       //获取最近聊天
        Friend_RecentlyChatUpdata,    //最近聊天更新
        Friend_RecentlyTeamUpdata,    //最近组队更新
        Friend_FriendItemInfoUpdata,  //好友数据更新
        Friend_ShowRed_Point,         //好友展示红点
        Friend_ResetMessageFrame,     //好友聊天窗口重置

        #endregion

        #region 关卡
        Level_Change,               //切换关卡
        Level_Start,                //关卡开始
        Level_Prepared,             //关卡开始
        Level_FinishCg,             //关卡动画结束
        Level_Exit,                 //关卡结束
        #endregion

        #region 场景
        Scene_AddObj,               //增加场景对象
        Scene_RemoveObj,            //移除场景对象
        #endregion

        #region 相机
        Camera_DoSkill,             //技能相机
        #endregion

        #region 称号
        Title_Change,               //称号
        Title_Unlock,               //解锁称号
        #endregion

        #region 个性
        Personality_ChangeValue,         //个性值变化
        #endregion

        #region 积分商城
        ExchangeStore_RestDay,  //重置每天兑换次数
        ExchangeStore_RestWeek, //重置每周兑换次数
        ExchangeStore_Echange, //兑换物品
        ExchangeStore_RefreshUI,//刷新UI
        ExchangeStore_SearchUsedTime, //查询已兑换次数
        ExchangeStore_Successful,  //兑换成功
        #endregion
		
		#region 炼妖壶
        Demonplot_Matchin,
        Demonplot_AddExp,
        Demonplot_AddExpCurrency,
        Demonplot_RefleashUI,
        #endregion

        #region 法宝相关
        Trumps_Create,//法宝创建
        Trumps_Equip,//法宝装备
        Trumps_AddExp,//法宝潜修
        Trumps_SkillUpgrade,//法宝技能等级提升
        Trumps_TasteUp,//法宝境界提升
        Trumps_InfusedUp,//注灵激活
        Trumps_Strengthen,//强化
        Trumps_RefleashUI,
        Trumps_RefreshEquips,
        #endregion

        #region 邮件
        Mail_AttachmentFetchStateChange,     // 邮件附件领取状态改变
        Mail_NewMailFlagChange,             // 新邮件标记改变

        #endregion

        #region 技能
        Skill_SaveSkill,                // 保存技能
        Skill_SaveSkillScheme,          // 保存技能方案
        Skill_UseSkillScheme,           // 使用技能方案
        Skill_SetSkillSchemeName,       // 设置技能方案名字
        Skill_ComprehendScheme,         // 技能天赋领悟
        Skill_RefreshSkillSchemeName,   // 刷新技能方案名字
        Skill_RefreshSkillScheme,       // 刷新技能方案
        Skill_RefreshSkill,             // 刷新技能
        Skill_ComprehendSucceed,        // 技能天赋领悟成功
        Skill_ComprehendFinish,         // 技能天赋领悟完成
        #endregion

        #region 外观
        Ap_UnlockItem,  //解锁物品
        Ap_RenewalItem,//物品续期
        Ap_WearItem,//穿戴物品
        Ap_PaintCloth,//服装染色
        Ap_DeleteColor,//删除颜色
        Ap_UnlockWeaponEffect,//解锁武器特效
        Ap_UnlockRideColor,        //解锁坐骑颜色
        Ap_LoadRide,//装配坐骑
        AP_RefreshUI,//刷新UI
        Ap_SaveNewFace,//保存新形象
        Ap_ApplyNewFace,//应用新形象
        Ap_DeletePreset,//删除预设

        #endregion

        #region 活动
        Activity_OpenPush,          // 开启关闭提醒
        Activity_UpdatePushUI,      // 协议回调后，更新提醒按钮
        Activity_RequestGetReward,  // 请求领取活跃度奖励
        Activity_UpdateActiveUI,    // 刷新活跃度界面
        Activity_UpdateListData,    // 刷新活动列表
        Activity_UpdateHuoliData,   // 更新活力值
        Activity_SetFinishState,    // 活动完成后弹框
        #endregion

        #region 氏族
        Clan_Create,             //创建氏族           
        Clan_GetAllClan,         //获取所有氏族
        Clan_RecvAllClan,        //收到所有列表
        Clan_GetSelfClan,        //获取自己的氏族信息
        Clan_RecvSelfClan,       //收到氏族信息
        Clan_OneKeyApply,        //一键申请
        Clan_ApplyClan,          //申请
        Clan_Respone,            //响应氏族
        Clan_CancelRespone,      //取消响应
        Clan_ZhaoMu,             //招募
        Clan_ContactLeader,      //联系族长
        Clan_ChangeToRespone,    //跳转到招募界面
        Clan_ToFriend_Select,    //跳转到好友界面选中队长
        Clan_UpdataInfo,         //更新自己的氏族信息
        Clan_Build_LevelUp,      //升级建筑
        #endregion

        #region 商会
        TradeStore_Buy,  //购买物品
        TradeStore_Sell, //售卖物品
        TradeStore_MainRefreshUI, //刷新主UI
        TradeStore_RefreshInfoUI, //刷新信息UI
        TradeStore_BuyRefreshUI, //刷新购买界面UI
        TradeStore_SellRefreshUI, //刷新出售界面UI
        TradeStore_BuySuccessful, //购买成功
        TradeStore_SellSuccessful, //售卖成功
        TradeStore_Rest,  //重置
        TradeStore_Recover, //还原表配置
        TradeStore_Search, //查询
        TradeStore_InputNum,  //保留原有选择的数量
        TradeStore_QuickSellSearch, //快速出售
        TradeStore_QuickSellRefreshUI, //刷新快速出售UI
        #endregion

        #region 任务

        Task_ClearAllTasks,
        Task_AddTask,
        Task_GiveupTask,
        Task_AcceptedTask,
        Task_CompletedTask,
        Task_SubmitTask,
        #endregion

        #region 血池
        BloodPoolWearHp,    // 装备恢复血瓶
        #endregion

        #region 摇钱树
        MoneyTree_RequestPlantTree,            // 请求种树
        MoneyTree_PlantTreeSuccess,            // 种树成功返回
        MoneyTree_RequestReduceTreeTime,       // 请求浇灌
        MoneyTree_RequestUseSpeedUpItem,       // 使用加速道具
        MoneyTree_UseSpeedUpItemSuccess,       // 使用加速道具成功返回
        MoneyTree_RequestGetReward,            // 请求领取奖励
        MoneyTree_GetRewardSuccess,            // 领取奖励成功
        MoneyTree_Refresh_UI_Data,             // 阶段变化推送
        MoneyTree_Refresh_UI_GrowthNum,        // 成长值变化推送
        MoneyTree_Tree_Mature_Push,            // 树成熟推送
        #endregion
    }


    //全局事件
    public static class EID
    {
        public const string OpenPanel = "op";
        public const string ClosePanel = "cp";
    }

    //指定角色事件,一般是战斗使用
    public enum ObjEventID
    {
        Null,
        ChangeState,        //切换状态
        ChangeAttri,        //属性变化
        ChangePosture,      //切换姿态
        ChangeBuffFlg,      //状态变化
    }
}
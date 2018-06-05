// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class LevelDefine 
    {
        static Dictionary<int, LevelDefine> DataList = new Dictionary<int, LevelDefine>();

        static public Dictionary<int, LevelDefine> GetAll()
        {
            return DataList;
        }

        static public LevelDefine Get(int key)
        {
            LevelDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("LevelDefine.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 配置表id
        public string configId { get; set; }

        // 关卡名称
        public string name { get; set; }

        // 关卡场景类型
        public LevelType levelType { get; set; }

        // 关卡子类型
        public int subType { get; set; }

        // 关卡子类型参数
        public int subParam { get; set; }

        // 进入关卡消耗道具
        public ItemCount useItem { get; set; }

        // 托管模式
        public int trusteeshipType { get; set; }

        // 能否伙伴加入
        public bool joinPartner { get; set; }

        // 是否宠物出现
        public bool showPet { get; set; }

        // 是否单机副本
        public bool isSingle { get; set; }

        // 是否需要组队
        public TeamCondition teamCondition { get; set; }

        // 强制组队人数
        public int teamNum { get; set; }

        // 组队进入方式
        public TeamEnterType teamEnterType { get; set; }

        // 是否支持任务同步
        public bool syncTask { get; set; }

        // 是否支持关卡内组队
        public bool formTeamInLevel { get; set; }

        // 是否支持召唤传送
        public bool canBeCall { get; set; }

        // 只允许氏族进入
        public bool guildEnter { get; set; }

        // 主角是否能操作
        public bool canMeCtrl { get; set; }

        // 房间状态
        public string roomBuff { get; set; }

        // 死亡惩罚
        public string deadPunish { get; set; }

        // 隐藏关卡时间
        public bool hideTime { get; set; }

        // 不能主动退出
        public bool cantInitiativeExit { get; set; }

        // UI的收放
        public UIStatus uiStatus { get; set; }

        // 轻功使用
        public int useJump { get; set; }

        // 进入场景buff
        public string enterSceneBuff { get; set; }

        // 关卡等级设置类型
        public LevelGradeSetting levelGradeSetting { get; set; }

        // 关卡等级
        public int levelGrade { get; set; }

        // 推荐等级
        public int recommendLevel { get; set; }

        // 进入等级限制
        public int gradeLimit { get; set; }

        // 进入PK值限制
        public int pkLimit { get; set; }

        // 进入任务类型限制
        public int taskLimit { get; set; }

        // 最大挑战次数
        public int maxChallengeTimes { get; set; }

        // 挑战次数扣除方式
        public DeductType deductType { get; set; }

        // 通关删除入口NPC
        public bool deleteEnterNpc { get; set; }

        // 所属活动id
        public int activityId { get; set; }

        // 默认传送石
        public string defaultTransmitStone { get; set; }

        // 背景音
        public string bgMusic { get; set; }

        // boss战背景音
        public string bossMusic { get; set; }

        // 最长时间
        public float maxStayTime { get; set; }

        // 地图id
        public string mapId { get; set; }

        // 关闭九宫格
        public bool closeSquaredUp { get; set; }

        // 结算界面类型
        public AccountType accountType { get; set; }

        // 隐藏关卡开启几率
        public float concealProbability { get; set; }

        // 对应隐藏关卡id
        public int concealId { get; set; }

        // 奖励id
        public int awardId { get; set; }

        // 道具奖励预览
        public string showItems { get; set; }

        // 额外奖励
        public int extraAward { get; set; }

        // 额外奖励次数
        public int extraAwardCount { get; set; }

        // 场景可容纳人数
        public int maxContain { get; set; }

        // 进入确认窗口
        public bool enterNeedConfirm { get; set; }

        // 确认窗口内容
        public string confirmContent { get; set; }

        // 是否能使用坐骑
        public bool canUseRide { get; set; }

        // 坐骑能否飞行
        public bool canRideFly { get; set; }

        // 坐骑飞行高度
        public float rideFlyLimit { get; set; }

        // 关卡摄像机
        public string levelCamera { get; set; }

        // 关卡摄像机类型
        public LevelCameraType levelCameraType { get; set; }

        // 判断提示ID
        public int judgeTipsId { get; set; }

        // 是否能复活
        public bool canRelive { get; set; }

        // 附近复活
        public bool canReliveNear { get; set; }

        // 附近复活倒计时
        public int reliveNearTime { get; set; }

        // 附近复活回血
        public float reliveNearRecover { get; set; }

        // 附近复活限制
        public NearReliveType reliveNearLimit { get; set; }

        // 原地复活
        public bool canReliveInplace { get; set; }

        // 等待救援
        public bool canReliveWait { get; set; }

        // 其他玩家救助
        public bool canReliveByPlayer { get; set; }

        // 宠物救助
        public bool canReliveByPet { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(LevelDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(LevelDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<LevelDefine> allDatas = new List<LevelDefine>();

            {
                string file = "Level/LevelDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int configId_index = reader.GetIndex("configId");
                int name_index = reader.GetIndex("name");
                int levelType_index = reader.GetIndex("levelType");
                int subType_index = reader.GetIndex("subType");
                int subParam_index = reader.GetIndex("subParam");
                int useItem_index = reader.GetIndex("useItem");
                int trusteeshipType_index = reader.GetIndex("trusteeshipType");
                int joinPartner_index = reader.GetIndex("joinPartner");
                int showPet_index = reader.GetIndex("showPet");
                int isSingle_index = reader.GetIndex("isSingle");
                int teamCondition_index = reader.GetIndex("teamCondition");
                int teamNum_index = reader.GetIndex("teamNum");
                int teamEnterType_index = reader.GetIndex("teamEnterType");
                int syncTask_index = reader.GetIndex("syncTask");
                int formTeamInLevel_index = reader.GetIndex("formTeamInLevel");
                int canBeCall_index = reader.GetIndex("canBeCall");
                int guildEnter_index = reader.GetIndex("guildEnter");
                int canMeCtrl_index = reader.GetIndex("canMeCtrl");
                int roomBuff_index = reader.GetIndex("roomBuff");
                int deadPunish_index = reader.GetIndex("deadPunish");
                int hideTime_index = reader.GetIndex("hideTime");
                int cantInitiativeExit_index = reader.GetIndex("cantInitiativeExit");
                int uiStatus_index = reader.GetIndex("uiStatus");
                int useJump_index = reader.GetIndex("useJump");
                int enterSceneBuff_index = reader.GetIndex("enterSceneBuff");
                int levelGradeSetting_index = reader.GetIndex("levelGradeSetting");
                int levelGrade_index = reader.GetIndex("levelGrade");
                int recommendLevel_index = reader.GetIndex("recommendLevel");
                int gradeLimit_index = reader.GetIndex("gradeLimit");
                int pkLimit_index = reader.GetIndex("pkLimit");
                int taskLimit_index = reader.GetIndex("taskLimit");
                int maxChallengeTimes_index = reader.GetIndex("maxChallengeTimes");
                int deductType_index = reader.GetIndex("deductType");
                int deleteEnterNpc_index = reader.GetIndex("deleteEnterNpc");
                int activityId_index = reader.GetIndex("activityId");
                int defaultTransmitStone_index = reader.GetIndex("defaultTransmitStone");
                int bgMusic_index = reader.GetIndex("bgMusic");
                int bossMusic_index = reader.GetIndex("bossMusic");
                int maxStayTime_index = reader.GetIndex("maxStayTime");
                int mapId_index = reader.GetIndex("mapId");
                int closeSquaredUp_index = reader.GetIndex("closeSquaredUp");
                int accountType_index = reader.GetIndex("accountType");
                int concealProbability_index = reader.GetIndex("concealProbability");
                int concealId_index = reader.GetIndex("concealId");
                int awardId_index = reader.GetIndex("awardId");
                int showItems_index = reader.GetIndex("showItems");
                int extraAward_index = reader.GetIndex("extraAward");
                int extraAwardCount_index = reader.GetIndex("extraAwardCount");
                int maxContain_index = reader.GetIndex("maxContain");
                int enterNeedConfirm_index = reader.GetIndex("enterNeedConfirm");
                int confirmContent_index = reader.GetIndex("confirmContent");
                int canUseRide_index = reader.GetIndex("canUseRide");
                int canRideFly_index = reader.GetIndex("canRideFly");
                int rideFlyLimit_index = reader.GetIndex("rideFlyLimit");
                int levelCamera_index = reader.GetIndex("levelCamera");
                int levelCameraType_index = reader.GetIndex("levelCameraType");
                int judgeTipsId_index = reader.GetIndex("judgeTipsId");
                int canRelive_index = reader.GetIndex("canRelive");
                int canReliveNear_index = reader.GetIndex("canReliveNear");
                int reliveNearTime_index = reader.GetIndex("reliveNearTime");
                int reliveNearRecover_index = reader.GetIndex("reliveNearRecover");
                int reliveNearLimit_index = reader.GetIndex("reliveNearLimit");
                int canReliveInplace_index = reader.GetIndex("canReliveInplace");
                int canReliveWait_index = reader.GetIndex("canReliveWait");
                int canReliveByPlayer_index = reader.GetIndex("canReliveByPlayer");
                int canReliveByPet_index = reader.GetIndex("canReliveByPet");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        LevelDefine data = new LevelDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.configId = reader.getStr(i, configId_index);         
						data.name = reader.getStr(i, name_index);         
						data.levelType = ((LevelType)reader.getInt(i, levelType_index, 0));         
						data.subType = reader.getInt(i, subType_index, 0);         
						data.subParam = reader.getInt(i, subParam_index, 0);         
						data.useItem = ItemCount.InitConfig(reader.getStr(i, useItem_index));         
						data.trusteeshipType = reader.getInt(i, trusteeshipType_index, 0);         
						data.joinPartner = reader.getBool(i, joinPartner_index, false);         
						data.showPet = reader.getBool(i, showPet_index, false);         
						data.isSingle = reader.getBool(i, isSingle_index, false);         
						data.teamCondition = ((TeamCondition)reader.getInt(i, teamCondition_index, 0));         
						data.teamNum = reader.getInt(i, teamNum_index, 0);         
						data.teamEnterType = ((TeamEnterType)reader.getInt(i, teamEnterType_index, 0));         
						data.syncTask = reader.getBool(i, syncTask_index, false);         
						data.formTeamInLevel = reader.getBool(i, formTeamInLevel_index, false);         
						data.canBeCall = reader.getBool(i, canBeCall_index, false);         
						data.guildEnter = reader.getBool(i, guildEnter_index, false);         
						data.canMeCtrl = reader.getBool(i, canMeCtrl_index, false);         
						data.roomBuff = reader.getStr(i, roomBuff_index);         
						data.deadPunish = reader.getStr(i, deadPunish_index);         
						data.hideTime = reader.getBool(i, hideTime_index, false);         
						data.cantInitiativeExit = reader.getBool(i, cantInitiativeExit_index, false);         
						data.uiStatus = ((UIStatus)reader.getInt(i, uiStatus_index, 0));         
						data.useJump = reader.getInt(i, useJump_index, 0);         
						data.enterSceneBuff = reader.getStr(i, enterSceneBuff_index);         
						data.levelGradeSetting = ((LevelGradeSetting)reader.getInt(i, levelGradeSetting_index, 0));         
						data.levelGrade = reader.getInt(i, levelGrade_index, 0);         
						data.recommendLevel = reader.getInt(i, recommendLevel_index, 0);         
						data.gradeLimit = reader.getInt(i, gradeLimit_index, 0);         
						data.pkLimit = reader.getInt(i, pkLimit_index, 0);         
						data.taskLimit = reader.getInt(i, taskLimit_index, 0);         
						data.maxChallengeTimes = reader.getInt(i, maxChallengeTimes_index, 0);         
						data.deductType = ((DeductType)reader.getInt(i, deductType_index, 0));         
						data.deleteEnterNpc = reader.getBool(i, deleteEnterNpc_index, false);         
						data.activityId = reader.getInt(i, activityId_index, 0);         
						data.defaultTransmitStone = reader.getStr(i, defaultTransmitStone_index);         
						data.bgMusic = reader.getStr(i, bgMusic_index);         
						data.bossMusic = reader.getStr(i, bossMusic_index);         
						data.maxStayTime = reader.getFloat(i, maxStayTime_index, 0f);         
						data.mapId = reader.getStr(i, mapId_index);         
						data.closeSquaredUp = reader.getBool(i, closeSquaredUp_index, false);         
						data.accountType = ((AccountType)reader.getInt(i, accountType_index, 0));         
						data.concealProbability = reader.getFloat(i, concealProbability_index, 0f);         
						data.concealId = reader.getInt(i, concealId_index, 0);         
						data.awardId = reader.getInt(i, awardId_index, 0);         
						data.showItems = reader.getStr(i, showItems_index);         
						data.extraAward = reader.getInt(i, extraAward_index, 0);         
						data.extraAwardCount = reader.getInt(i, extraAwardCount_index, 0);         
						data.maxContain = reader.getInt(i, maxContain_index, 0);         
						data.enterNeedConfirm = reader.getBool(i, enterNeedConfirm_index, false);         
						data.confirmContent = reader.getStr(i, confirmContent_index);         
						data.canUseRide = reader.getBool(i, canUseRide_index, false);         
						data.canRideFly = reader.getBool(i, canRideFly_index, false);         
						data.rideFlyLimit = reader.getFloat(i, rideFlyLimit_index, 0f);         
						data.levelCamera = reader.getStr(i, levelCamera_index);         
						data.levelCameraType = ((LevelCameraType)reader.getInt(i, levelCameraType_index, 0));         
						data.judgeTipsId = reader.getInt(i, judgeTipsId_index, 0);         
						data.canRelive = reader.getBool(i, canRelive_index, false);         
						data.canReliveNear = reader.getBool(i, canReliveNear_index, false);         
						data.reliveNearTime = reader.getInt(i, reliveNearTime_index, 0);         
						data.reliveNearRecover = reader.getFloat(i, reliveNearRecover_index, 0f);         
						data.reliveNearLimit = ((NearReliveType)reader.getInt(i, reliveNearLimit_index, 0));         
						data.canReliveInplace = reader.getBool(i, canReliveInplace_index, false);         
						data.canReliveWait = reader.getBool(i, canReliveWait_index, false);         
						data.canReliveByPlayer = reader.getBool(i, canReliveByPlayer_index, false);         
						data.canReliveByPet = reader.getBool(i, canReliveByPet_index, false);         
                        if (lineParseMethod != null)
                            lineParseMethod.Invoke(null, new object[3] {data, reader, i });
                        allDatas.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        CsvCommon.Log.Error("file:{0} line:{1} error!", file, i);
                        CsvCommon.Log.Exception(ex);
                    }
                }
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("LevelDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(LevelDefine);
                    while (null != curType)
                    {
                        method = curType.GetMethod("OnLoadEnd", BindingFlags.Static | BindingFlags.NonPublic);
                        if (null != method)
                            break;
                        curType = curType.BaseType;
                    }
                }
                if (method != null)
                    method.Invoke(null, new object[0]);
            }
        }
    }
}



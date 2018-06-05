// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskDefine 
    {
        static Dictionary<int, TaskDefine> DataList = new Dictionary<int, TaskDefine>();

        static public Dictionary<int, TaskDefine> GetAll()
        {
            return DataList;
        }

        static public TaskDefine Get(int key)
        {
            TaskDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskDefine.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 任务组
        public int groupId { get; set; }

        // 任务章节
        public string taskChapterName { get; set; }

        // 章节排序
        public int chapterOrder { get; set; }

        // 章节名
        public string chapterName { get; set; }

        // 任务名称
        public string taskName { get; set; }

        // 可接任务描述
        public string acceptableDesc { get; set; }

        // 任务描述
        public string taskDesc { get; set; }

        // 可接未接任务追踪描述
        public string acceptableTraceDesc { get; set; }

        // 已接未完成任务追踪描述
        public string acceptedTraceDesc { get; set; }

        // 已完成任务追踪描述
        public string completedTraceDesc { get; set; }

        // 是否显示已达成状态
        public int showCompletedTips { get; set; }

        // 任务类型
        public int taskType { get; set; }

        // 所属环任务ID
        public int loopId { get; set; }

        // 是否能放弃任务
        public bool canGiveup { get; set; }

        // 是否便捷寻路
        public bool isAutoPathfind { get; set; }

        // 是否自动操作
        public bool isAutoOpt { get; set; }

        // 后置任务ID
        public int nextTaskId { get; set; }

        // 后置任务ID-不同条件
        public int[] nextTaskIds { get; set; }

        // 接任务需要等级
        public int acceptLevelMin { get; set; }

        // 接任务等级上限
        public int acceptLevelMax { get; set; }

        // 接任务需要性格差值
        public string acceptPersonal { get; set; }

        // 接任务需要门派
        public int acceptCareer { get; set; }

        // 接任务需要氏族
        public bool acceptHasGuild { get; set; }

        // 是否自动接
        public int autoAcceptType { get; set; }

        // 接任务前对白
        public int beforeAcceptStoryId { get; set; }

        // 接任务NPC
        public int acceptNpcId { get; set; }

        // 接任务触发器
        public string acceptAreaId { get; set; }

        // 接任务后对白
        public int afterAcceptStoryId { get; set; }

        // 接任务触发事件
        public int acceptEventId { get; set; }

        // 完成条件-物品
        public string competedItems { get; set; }

        // 完成条件-任务计数
        public string competedCountId { get; set; }

        // 完成条件-战斗
        public string competedBattleId { get; set; }

        // 完成条件-杀怪
        public string competedKills { get; set; }

        // 完成条件-通关
        public string competedLevelId { get; set; }

        // 完成条件-限时完成
        public float competedTimeLimit { get; set; }

        // 完成条件-氏族供奉
        public string competedGuildItems { get; set; }

        // 完成任务触发事件
        public int competedEventId { get; set; }

        // 是否自动攻击
        public bool isAutoAttack { get; set; }

        // 是否自动交
        public int autoCompetedType { get; set; }

        // 交任务NPC
        public int submitNpcId { get; set; }

        // 交任务触发器
        public string submitAreaId { get; set; }

        // 交任务对白
        public int submitStoryId { get; set; }

        // 交任务触发事件
        public int submitEventId { get; set; }

        // 任务完成后是否显示特效
        public bool isPlaySubmitEffect { get; set; }

        // 分支对白
        public int submitBranchDialogId { get; set; }

        // 选项1对应后续任务
        public int branchId1 { get; set; }

        // 选项2对应后续任务
        public int branchId2 { get; set; }

        // 选项3对应后续任务
        public int branchId3 { get; set; }

        // 选项4对应后续任务
        public int branchId4 { get; set; }

        // 选项1奖励性格值
        public string branchReward1 { get; set; }

        // 选项2奖励性格值
        public string branchReward2 { get; set; }

        // 选项3奖励性格值
        public string branchReward3 { get; set; }

        // 选项4奖励性格值
        public string branchReward4 { get; set; }

        // 奖励性格值
        public string rewardItems { get; set; }

        // 奖励物品
        public int rewardSilvers { get; set; }

        // 奖励银贝
        public int rewardGolds { get; set; }

        // 奖励金贝
        public int rewardJasper { get; set; }

        // 奖励碧玉
        public int rewardExp { get; set; }

        // 奖励经验
        public int rewardXiuwei { get; set; }

        // 奖励修为
        public int rewardPetExp { get; set; }

        // 奖励宠物经验
        public string rewardPersonal { get; set; }

        // 奖励氏族贡献
        public int rewardGuildPresent { get; set; }

        // 奖励门派贡献
        public int rewardCareerPresent { get; set; }

        // 奖励荣誉
        public int rewardHonour { get; set; }

        // 奖励氏族资金
        public int rewardGuildMoney { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskDefine> allDatas = new List<TaskDefine>();

            {
                string file = "Task/TaskDefine#PY.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int groupId_index = reader.GetIndex("groupId");
                int taskChapterName_index = reader.GetIndex("taskChapterName");
                int chapterOrder_index = reader.GetIndex("chapterOrder");
                int chapterName_index = reader.GetIndex("chapterName");
                int taskName_index = reader.GetIndex("taskName");
                int acceptableDesc_index = reader.GetIndex("acceptableDesc");
                int taskDesc_index = reader.GetIndex("taskDesc");
                int acceptableTraceDesc_index = reader.GetIndex("acceptableTraceDesc");
                int acceptedTraceDesc_index = reader.GetIndex("acceptedTraceDesc");
                int completedTraceDesc_index = reader.GetIndex("completedTraceDesc");
                int showCompletedTips_index = reader.GetIndex("showCompletedTips");
                int taskType_index = reader.GetIndex("taskType");
                int loopId_index = reader.GetIndex("loopId");
                int canGiveup_index = reader.GetIndex("canGiveup");
                int isAutoPathfind_index = reader.GetIndex("isAutoPathfind");
                int isAutoOpt_index = reader.GetIndex("isAutoOpt");
                int nextTaskId_index = reader.GetIndex("nextTaskId");
                int nextTaskIds_index = reader.GetIndex("nextTaskIds");
                int acceptLevelMin_index = reader.GetIndex("acceptLevelMin");
                int acceptLevelMax_index = reader.GetIndex("acceptLevelMax");
                int acceptPersonal_index = reader.GetIndex("acceptPersonal");
                int acceptCareer_index = reader.GetIndex("acceptCareer");
                int acceptHasGuild_index = reader.GetIndex("acceptHasGuild");
                int autoAcceptType_index = reader.GetIndex("autoAcceptType");
                int beforeAcceptStoryId_index = reader.GetIndex("beforeAcceptStoryId");
                int acceptNpcId_index = reader.GetIndex("acceptNpcId");
                int acceptAreaId_index = reader.GetIndex("acceptAreaId");
                int afterAcceptStoryId_index = reader.GetIndex("afterAcceptStoryId");
                int acceptEventId_index = reader.GetIndex("acceptEventId");
                int competedItems_index = reader.GetIndex("competedItems");
                int competedCountId_index = reader.GetIndex("competedCountId");
                int competedBattleId_index = reader.GetIndex("competedBattleId");
                int competedKills_index = reader.GetIndex("competedKills");
                int competedLevelId_index = reader.GetIndex("competedLevelId");
                int competedTimeLimit_index = reader.GetIndex("competedTimeLimit");
                int competedGuildItems_index = reader.GetIndex("competedGuildItems");
                int competedEventId_index = reader.GetIndex("competedEventId");
                int isAutoAttack_index = reader.GetIndex("isAutoAttack");
                int autoCompetedType_index = reader.GetIndex("autoCompetedType");
                int submitNpcId_index = reader.GetIndex("submitNpcId");
                int submitAreaId_index = reader.GetIndex("submitAreaId");
                int submitStoryId_index = reader.GetIndex("submitStoryId");
                int submitEventId_index = reader.GetIndex("submitEventId");
                int isPlaySubmitEffect_index = reader.GetIndex("isPlaySubmitEffect");
                int submitBranchDialogId_index = reader.GetIndex("submitBranchDialogId");
                int branchId1_index = reader.GetIndex("branchId1");
                int branchId2_index = reader.GetIndex("branchId2");
                int branchId3_index = reader.GetIndex("branchId3");
                int branchId4_index = reader.GetIndex("branchId4");
                int branchReward1_index = reader.GetIndex("branchReward1");
                int branchReward2_index = reader.GetIndex("branchReward2");
                int branchReward3_index = reader.GetIndex("branchReward3");
                int branchReward4_index = reader.GetIndex("branchReward4");
                int rewardItems_index = reader.GetIndex("rewardItems");
                int rewardSilvers_index = reader.GetIndex("rewardSilvers");
                int rewardGolds_index = reader.GetIndex("rewardGolds");
                int rewardJasper_index = reader.GetIndex("rewardJasper");
                int rewardExp_index = reader.GetIndex("rewardExp");
                int rewardXiuwei_index = reader.GetIndex("rewardXiuwei");
                int rewardPetExp_index = reader.GetIndex("rewardPetExp");
                int rewardPersonal_index = reader.GetIndex("rewardPersonal");
                int rewardGuildPresent_index = reader.GetIndex("rewardGuildPresent");
                int rewardCareerPresent_index = reader.GetIndex("rewardCareerPresent");
                int rewardHonour_index = reader.GetIndex("rewardHonour");
                int rewardGuildMoney_index = reader.GetIndex("rewardGuildMoney");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskDefine data = new TaskDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.groupId = reader.getInt(i, groupId_index, 0);         
						data.taskChapterName = reader.getStr(i, taskChapterName_index);         
						data.chapterOrder = reader.getInt(i, chapterOrder_index, 0);         
						data.chapterName = reader.getStr(i, chapterName_index);         
						data.taskName = reader.getStr(i, taskName_index);         
						data.acceptableDesc = reader.getStr(i, acceptableDesc_index);         
						data.taskDesc = reader.getStr(i, taskDesc_index);         
						data.acceptableTraceDesc = reader.getStr(i, acceptableTraceDesc_index);         
						data.acceptedTraceDesc = reader.getStr(i, acceptedTraceDesc_index);         
						data.completedTraceDesc = reader.getStr(i, completedTraceDesc_index);         
						data.showCompletedTips = reader.getInt(i, showCompletedTips_index, 0);         
						data.taskType = reader.getInt(i, taskType_index, 0);         
						data.loopId = reader.getInt(i, loopId_index, 0);         
						data.canGiveup = reader.getBool(i, canGiveup_index, false);         
						data.isAutoPathfind = reader.getBool(i, isAutoPathfind_index, false);         
						data.isAutoOpt = reader.getBool(i, isAutoOpt_index, false);         
						data.nextTaskId = reader.getInt(i, nextTaskId_index, 0);         
						data.nextTaskIds = reader.getInts(i, nextTaskIds_index, ';');         
						data.acceptLevelMin = reader.getInt(i, acceptLevelMin_index, 0);         
						data.acceptLevelMax = reader.getInt(i, acceptLevelMax_index, 0);         
						data.acceptPersonal = reader.getStr(i, acceptPersonal_index);         
						data.acceptCareer = reader.getInt(i, acceptCareer_index, 0);         
						data.acceptHasGuild = reader.getBool(i, acceptHasGuild_index, false);         
						data.autoAcceptType = reader.getInt(i, autoAcceptType_index, 0);         
						data.beforeAcceptStoryId = reader.getInt(i, beforeAcceptStoryId_index, 0);         
						data.acceptNpcId = reader.getInt(i, acceptNpcId_index, 0);         
						data.acceptAreaId = reader.getStr(i, acceptAreaId_index);         
						data.afterAcceptStoryId = reader.getInt(i, afterAcceptStoryId_index, 0);         
						data.acceptEventId = reader.getInt(i, acceptEventId_index, 0);         
						data.competedItems = reader.getStr(i, competedItems_index);         
						data.competedCountId = reader.getStr(i, competedCountId_index);         
						data.competedBattleId = reader.getStr(i, competedBattleId_index);         
						data.competedKills = reader.getStr(i, competedKills_index);         
						data.competedLevelId = reader.getStr(i, competedLevelId_index);         
						data.competedTimeLimit = reader.getFloat(i, competedTimeLimit_index, 0f);         
						data.competedGuildItems = reader.getStr(i, competedGuildItems_index);         
						data.competedEventId = reader.getInt(i, competedEventId_index, 0);         
						data.isAutoAttack = reader.getBool(i, isAutoAttack_index, false);         
						data.autoCompetedType = reader.getInt(i, autoCompetedType_index, 0);         
						data.submitNpcId = reader.getInt(i, submitNpcId_index, 0);         
						data.submitAreaId = reader.getStr(i, submitAreaId_index);         
						data.submitStoryId = reader.getInt(i, submitStoryId_index, 0);         
						data.submitEventId = reader.getInt(i, submitEventId_index, 0);         
						data.isPlaySubmitEffect = reader.getBool(i, isPlaySubmitEffect_index, false);         
						data.submitBranchDialogId = reader.getInt(i, submitBranchDialogId_index, 0);         
						data.branchId1 = reader.getInt(i, branchId1_index, 0);         
						data.branchId2 = reader.getInt(i, branchId2_index, 0);         
						data.branchId3 = reader.getInt(i, branchId3_index, 0);         
						data.branchId4 = reader.getInt(i, branchId4_index, 0);         
						data.branchReward1 = reader.getStr(i, branchReward1_index);         
						data.branchReward2 = reader.getStr(i, branchReward2_index);         
						data.branchReward3 = reader.getStr(i, branchReward3_index);         
						data.branchReward4 = reader.getStr(i, branchReward4_index);         
						data.rewardItems = reader.getStr(i, rewardItems_index);         
						data.rewardSilvers = reader.getInt(i, rewardSilvers_index, 0);         
						data.rewardGolds = reader.getInt(i, rewardGolds_index, 0);         
						data.rewardJasper = reader.getInt(i, rewardJasper_index, 0);         
						data.rewardExp = reader.getInt(i, rewardExp_index, 0);         
						data.rewardXiuwei = reader.getInt(i, rewardXiuwei_index, 0);         
						data.rewardPetExp = reader.getInt(i, rewardPetExp_index, 0);         
						data.rewardPersonal = reader.getStr(i, rewardPersonal_index);         
						data.rewardGuildPresent = reader.getInt(i, rewardGuildPresent_index, 0);         
						data.rewardCareerPresent = reader.getInt(i, rewardCareerPresent_index, 0);         
						data.rewardHonour = reader.getInt(i, rewardHonour_index, 0);         
						data.rewardGuildMoney = reader.getInt(i, rewardGuildMoney_index, 0);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Task/TaskDefine#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int groupId_index = reader.GetIndex("groupId");
                int taskChapterName_index = reader.GetIndex("taskChapterName");
                int chapterOrder_index = reader.GetIndex("chapterOrder");
                int chapterName_index = reader.GetIndex("chapterName");
                int taskName_index = reader.GetIndex("taskName");
                int acceptableDesc_index = reader.GetIndex("acceptableDesc");
                int taskDesc_index = reader.GetIndex("taskDesc");
                int acceptableTraceDesc_index = reader.GetIndex("acceptableTraceDesc");
                int acceptedTraceDesc_index = reader.GetIndex("acceptedTraceDesc");
                int completedTraceDesc_index = reader.GetIndex("completedTraceDesc");
                int showCompletedTips_index = reader.GetIndex("showCompletedTips");
                int taskType_index = reader.GetIndex("taskType");
                int loopId_index = reader.GetIndex("loopId");
                int canGiveup_index = reader.GetIndex("canGiveup");
                int isAutoPathfind_index = reader.GetIndex("isAutoPathfind");
                int isAutoOpt_index = reader.GetIndex("isAutoOpt");
                int nextTaskId_index = reader.GetIndex("nextTaskId");
                int nextTaskIds_index = reader.GetIndex("nextTaskIds");
                int acceptLevelMin_index = reader.GetIndex("acceptLevelMin");
                int acceptLevelMax_index = reader.GetIndex("acceptLevelMax");
                int acceptPersonal_index = reader.GetIndex("acceptPersonal");
                int acceptCareer_index = reader.GetIndex("acceptCareer");
                int acceptHasGuild_index = reader.GetIndex("acceptHasGuild");
                int autoAcceptType_index = reader.GetIndex("autoAcceptType");
                int beforeAcceptStoryId_index = reader.GetIndex("beforeAcceptStoryId");
                int acceptNpcId_index = reader.GetIndex("acceptNpcId");
                int acceptAreaId_index = reader.GetIndex("acceptAreaId");
                int afterAcceptStoryId_index = reader.GetIndex("afterAcceptStoryId");
                int acceptEventId_index = reader.GetIndex("acceptEventId");
                int competedItems_index = reader.GetIndex("competedItems");
                int competedCountId_index = reader.GetIndex("competedCountId");
                int competedBattleId_index = reader.GetIndex("competedBattleId");
                int competedKills_index = reader.GetIndex("competedKills");
                int competedLevelId_index = reader.GetIndex("competedLevelId");
                int competedTimeLimit_index = reader.GetIndex("competedTimeLimit");
                int competedGuildItems_index = reader.GetIndex("competedGuildItems");
                int competedEventId_index = reader.GetIndex("competedEventId");
                int isAutoAttack_index = reader.GetIndex("isAutoAttack");
                int autoCompetedType_index = reader.GetIndex("autoCompetedType");
                int submitNpcId_index = reader.GetIndex("submitNpcId");
                int submitAreaId_index = reader.GetIndex("submitAreaId");
                int submitStoryId_index = reader.GetIndex("submitStoryId");
                int submitEventId_index = reader.GetIndex("submitEventId");
                int isPlaySubmitEffect_index = reader.GetIndex("isPlaySubmitEffect");
                int submitBranchDialogId_index = reader.GetIndex("submitBranchDialogId");
                int branchId1_index = reader.GetIndex("branchId1");
                int branchId2_index = reader.GetIndex("branchId2");
                int branchId3_index = reader.GetIndex("branchId3");
                int branchId4_index = reader.GetIndex("branchId4");
                int branchReward1_index = reader.GetIndex("branchReward1");
                int branchReward2_index = reader.GetIndex("branchReward2");
                int branchReward3_index = reader.GetIndex("branchReward3");
                int branchReward4_index = reader.GetIndex("branchReward4");
                int rewardItems_index = reader.GetIndex("rewardItems");
                int rewardSilvers_index = reader.GetIndex("rewardSilvers");
                int rewardGolds_index = reader.GetIndex("rewardGolds");
                int rewardJasper_index = reader.GetIndex("rewardJasper");
                int rewardExp_index = reader.GetIndex("rewardExp");
                int rewardXiuwei_index = reader.GetIndex("rewardXiuwei");
                int rewardPetExp_index = reader.GetIndex("rewardPetExp");
                int rewardPersonal_index = reader.GetIndex("rewardPersonal");
                int rewardGuildPresent_index = reader.GetIndex("rewardGuildPresent");
                int rewardCareerPresent_index = reader.GetIndex("rewardCareerPresent");
                int rewardHonour_index = reader.GetIndex("rewardHonour");
                int rewardGuildMoney_index = reader.GetIndex("rewardGuildMoney");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskDefine data = new TaskDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.groupId = reader.getInt(i, groupId_index, 0);         
						data.taskChapterName = reader.getStr(i, taskChapterName_index);         
						data.chapterOrder = reader.getInt(i, chapterOrder_index, 0);         
						data.chapterName = reader.getStr(i, chapterName_index);         
						data.taskName = reader.getStr(i, taskName_index);         
						data.acceptableDesc = reader.getStr(i, acceptableDesc_index);         
						data.taskDesc = reader.getStr(i, taskDesc_index);         
						data.acceptableTraceDesc = reader.getStr(i, acceptableTraceDesc_index);         
						data.acceptedTraceDesc = reader.getStr(i, acceptedTraceDesc_index);         
						data.completedTraceDesc = reader.getStr(i, completedTraceDesc_index);         
						data.showCompletedTips = reader.getInt(i, showCompletedTips_index, 0);         
						data.taskType = reader.getInt(i, taskType_index, 0);         
						data.loopId = reader.getInt(i, loopId_index, 0);         
						data.canGiveup = reader.getBool(i, canGiveup_index, false);         
						data.isAutoPathfind = reader.getBool(i, isAutoPathfind_index, false);         
						data.isAutoOpt = reader.getBool(i, isAutoOpt_index, false);         
						data.nextTaskId = reader.getInt(i, nextTaskId_index, 0);         
						data.nextTaskIds = reader.getInts(i, nextTaskIds_index, ';');         
						data.acceptLevelMin = reader.getInt(i, acceptLevelMin_index, 0);         
						data.acceptLevelMax = reader.getInt(i, acceptLevelMax_index, 0);         
						data.acceptPersonal = reader.getStr(i, acceptPersonal_index);         
						data.acceptCareer = reader.getInt(i, acceptCareer_index, 0);         
						data.acceptHasGuild = reader.getBool(i, acceptHasGuild_index, false);         
						data.autoAcceptType = reader.getInt(i, autoAcceptType_index, 0);         
						data.beforeAcceptStoryId = reader.getInt(i, beforeAcceptStoryId_index, 0);         
						data.acceptNpcId = reader.getInt(i, acceptNpcId_index, 0);         
						data.acceptAreaId = reader.getStr(i, acceptAreaId_index);         
						data.afterAcceptStoryId = reader.getInt(i, afterAcceptStoryId_index, 0);         
						data.acceptEventId = reader.getInt(i, acceptEventId_index, 0);         
						data.competedItems = reader.getStr(i, competedItems_index);         
						data.competedCountId = reader.getStr(i, competedCountId_index);         
						data.competedBattleId = reader.getStr(i, competedBattleId_index);         
						data.competedKills = reader.getStr(i, competedKills_index);         
						data.competedLevelId = reader.getStr(i, competedLevelId_index);         
						data.competedTimeLimit = reader.getFloat(i, competedTimeLimit_index, 0f);         
						data.competedGuildItems = reader.getStr(i, competedGuildItems_index);         
						data.competedEventId = reader.getInt(i, competedEventId_index, 0);         
						data.isAutoAttack = reader.getBool(i, isAutoAttack_index, false);         
						data.autoCompetedType = reader.getInt(i, autoCompetedType_index, 0);         
						data.submitNpcId = reader.getInt(i, submitNpcId_index, 0);         
						data.submitAreaId = reader.getStr(i, submitAreaId_index);         
						data.submitStoryId = reader.getInt(i, submitStoryId_index, 0);         
						data.submitEventId = reader.getInt(i, submitEventId_index, 0);         
						data.isPlaySubmitEffect = reader.getBool(i, isPlaySubmitEffect_index, false);         
						data.submitBranchDialogId = reader.getInt(i, submitBranchDialogId_index, 0);         
						data.branchId1 = reader.getInt(i, branchId1_index, 0);         
						data.branchId2 = reader.getInt(i, branchId2_index, 0);         
						data.branchId3 = reader.getInt(i, branchId3_index, 0);         
						data.branchId4 = reader.getInt(i, branchId4_index, 0);         
						data.branchReward1 = reader.getStr(i, branchReward1_index);         
						data.branchReward2 = reader.getStr(i, branchReward2_index);         
						data.branchReward3 = reader.getStr(i, branchReward3_index);         
						data.branchReward4 = reader.getStr(i, branchReward4_index);         
						data.rewardItems = reader.getStr(i, rewardItems_index);         
						data.rewardSilvers = reader.getInt(i, rewardSilvers_index, 0);         
						data.rewardGolds = reader.getInt(i, rewardGolds_index, 0);         
						data.rewardJasper = reader.getInt(i, rewardJasper_index, 0);         
						data.rewardExp = reader.getInt(i, rewardExp_index, 0);         
						data.rewardXiuwei = reader.getInt(i, rewardXiuwei_index, 0);         
						data.rewardPetExp = reader.getInt(i, rewardPetExp_index, 0);         
						data.rewardPersonal = reader.getStr(i, rewardPersonal_index);         
						data.rewardGuildPresent = reader.getInt(i, rewardGuildPresent_index, 0);         
						data.rewardCareerPresent = reader.getInt(i, rewardCareerPresent_index, 0);         
						data.rewardHonour = reader.getInt(i, rewardHonour_index, 0);         
						data.rewardGuildMoney = reader.getInt(i, rewardGuildMoney_index, 0);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("TaskDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskDefine);
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



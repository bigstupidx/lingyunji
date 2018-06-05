// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillConfig 
    {
        static Dictionary<int, SkillConfig> DataList = new Dictionary<int, SkillConfig>();

        static public Dictionary<int, SkillConfig> GetAll()
        {
            return DataList;
        }

        static public SkillConfig Get(int key)
        {
            SkillConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillConfig.Get({0}) not find!", key);
            return null;
        }



        // 技能ID
        public int id { get; set; }

        // 名称
        public string name { get; set; }

        // 动作ID
        public string aniid { get; set; }

        // 技能类型
        public Type type { get; set; }

        // 是否宠物绝技
        public bool isPetStunt { get; set; }

        // 开放等级
        public int openLevel { get; set; }

        // 是否强制技能
        public bool forceSkill { get; set; }

        // 所属槽位
        public int slotid { get; set; }

        // 技能冷却时间
        public float cd { get; set; }

        // 真气消耗
        public int costZhenqi { get; set; }

        // 灵力消耗
        public int CostLingqi { get; set; }

        // 气力消耗
        public int CostQili { get; set; }

        // 施放战斗状态
        public CastingBattleState castingBattleState { get; set; }

        // 施放条件
        public int castingConditions { get; set; }

        // 施放姿态
        public int[] castingPosture { get; set; }

        // 指定目标
        public bool isNeedTarget { get; set; }

        // 射程
        public float range { get; set; }

        // 智能选取
        public bool isAutoTarget { get; set; }

        // 智能选取射程
        public float autoTargetRange { get; set; }

        // 智能选取角度
        public float autoTargetAngle { get; set; }

        // 施放是否朝向目标
        public int lookAtTarget { get; set; }

        // 切换技能
        public int changeSkill { get; set; }

        // 切换技能时间
        public float changeSkillTime { get; set; }

        // 切换类型
        public SwitchType changeSkillType { get; set; }

        // 切换限制
        public int changeSkillLimit { get; set; }

        // 施法时间
        public float castingTime { get; set; }

        // 持续施法时间
        public float castingContinueTime { get; set; }

        // 施放切换姿态
        public int toPosture { get; set; }

        // 描述
        public string des { get; set; }

        // 特殊技能目标
        public string targetSearch { get; set; }

        // 真气恢复描述
        public int zhenqiRec { get; set; }

        // 技能切换优先级
        public int switchPriority { get; set; }

        // 技能施放action
        public string[] excuteActionIds { get; set; }

        // 技能装逼描述
        public string bDes { get; set; }

        // 技能范围描述
        public string searchDes { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillConfig> allDatas = new List<SkillConfig>();

            {
                string file = "Skill/SkillConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int aniid_index = reader.GetIndex("aniid");
                int type_index = reader.GetIndex("type");
                int isPetStunt_index = reader.GetIndex("isPetStunt");
                int openLevel_index = reader.GetIndex("openLevel");
                int forceSkill_index = reader.GetIndex("forceSkill");
                int slotid_index = reader.GetIndex("slotid");
                int cd_index = reader.GetIndex("cd");
                int costZhenqi_index = reader.GetIndex("costZhenqi");
                int CostLingqi_index = reader.GetIndex("CostLingqi");
                int CostQili_index = reader.GetIndex("CostQili");
                int castingBattleState_index = reader.GetIndex("castingBattleState");
                int castingConditions_index = reader.GetIndex("castingConditions");
                int castingPosture_index = reader.GetIndex("castingPosture");
                int isNeedTarget_index = reader.GetIndex("isNeedTarget");
                int range_index = reader.GetIndex("range");
                int isAutoTarget_index = reader.GetIndex("isAutoTarget");
                int autoTargetRange_index = reader.GetIndex("autoTargetRange");
                int autoTargetAngle_index = reader.GetIndex("autoTargetAngle");
                int lookAtTarget_index = reader.GetIndex("lookAtTarget");
                int changeSkill_index = reader.GetIndex("changeSkill");
                int changeSkillTime_index = reader.GetIndex("changeSkillTime");
                int changeSkillType_index = reader.GetIndex("changeSkillType");
                int changeSkillLimit_index = reader.GetIndex("changeSkillLimit");
                int castingTime_index = reader.GetIndex("castingTime");
                int castingContinueTime_index = reader.GetIndex("castingContinueTime");
                int toPosture_index = reader.GetIndex("toPosture");
                int des_index = reader.GetIndex("des");
                int targetSearch_index = reader.GetIndex("targetSearch");
                int zhenqiRec_index = reader.TryIndex("zhenqiRec");
                int switchPriority_index = reader.TryIndex("switchPriority");
                int excuteActionIds_index = reader.TryIndex("excuteActionIds");
                int bDes_index = reader.TryIndex("bDes");
                int searchDes_index = reader.TryIndex("searchDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillConfig data = new SkillConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.aniid = reader.getStr(i, aniid_index);         
						data.type = ((Type)reader.getInt(i, type_index, 0));         
						data.isPetStunt = reader.getBool(i, isPetStunt_index, false);         
						data.openLevel = reader.getInt(i, openLevel_index, 0);         
						data.forceSkill = reader.getBool(i, forceSkill_index, false);         
						data.slotid = reader.getInt(i, slotid_index, 0);         
						data.cd = reader.getFloat(i, cd_index, 0f);         
						data.costZhenqi = reader.getInt(i, costZhenqi_index, 0);         
						data.CostLingqi = reader.getInt(i, CostLingqi_index, 0);         
						data.CostQili = reader.getInt(i, CostQili_index, 0);         
						data.castingBattleState = ((CastingBattleState)reader.getInt(i, castingBattleState_index, 0));         
						data.castingConditions = reader.getInt(i, castingConditions_index, 0);         
						data.castingPosture = reader.getInts(i, castingPosture_index, ';');         
						data.isNeedTarget = reader.getBool(i, isNeedTarget_index, false);         
						data.range = reader.getFloat(i, range_index, 0f);         
						data.isAutoTarget = reader.getBool(i, isAutoTarget_index, false);         
						data.autoTargetRange = reader.getFloat(i, autoTargetRange_index, 0f);         
						data.autoTargetAngle = reader.getFloat(i, autoTargetAngle_index, 0f);         
						data.lookAtTarget = reader.getInt(i, lookAtTarget_index, 0);         
						data.changeSkill = reader.getInt(i, changeSkill_index, 0);         
						data.changeSkillTime = reader.getFloat(i, changeSkillTime_index, 0f);         
						data.changeSkillType = ((SwitchType)reader.getInt(i, changeSkillType_index, 0));         
						data.changeSkillLimit = reader.getInt(i, changeSkillLimit_index, 0);         
						data.castingTime = reader.getFloat(i, castingTime_index, 0f);         
						data.castingContinueTime = reader.getFloat(i, castingContinueTime_index, 0f);         
						data.toPosture = reader.getInt(i, toPosture_index, 0);         
						data.des = reader.getStr(i, des_index);         
						data.targetSearch = reader.getStr(i, targetSearch_index);         
						data.zhenqiRec = reader.getInt(i, zhenqiRec_index, 0);         
						data.switchPriority = reader.getInt(i, switchPriority_index, 0);         
						data.excuteActionIds = reader.getStrs(i, excuteActionIds_index, ';');         
						data.bDes = reader.getStr(i, bDes_index);         
						data.searchDes = reader.getStr(i, searchDes_index);         
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
                string file = "Skill/SkillConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int aniid_index = reader.GetIndex("aniid");
                int type_index = reader.GetIndex("type");
                int isPetStunt_index = reader.GetIndex("isPetStunt");
                int openLevel_index = reader.GetIndex("openLevel");
                int forceSkill_index = reader.GetIndex("forceSkill");
                int slotid_index = reader.GetIndex("slotid");
                int cd_index = reader.GetIndex("cd");
                int costZhenqi_index = reader.GetIndex("costZhenqi");
                int CostLingqi_index = reader.GetIndex("CostLingqi");
                int CostQili_index = reader.GetIndex("CostQili");
                int castingBattleState_index = reader.GetIndex("castingBattleState");
                int castingConditions_index = reader.GetIndex("castingConditions");
                int castingPosture_index = reader.GetIndex("castingPosture");
                int isNeedTarget_index = reader.GetIndex("isNeedTarget");
                int range_index = reader.GetIndex("range");
                int isAutoTarget_index = reader.GetIndex("isAutoTarget");
                int autoTargetRange_index = reader.GetIndex("autoTargetRange");
                int autoTargetAngle_index = reader.GetIndex("autoTargetAngle");
                int lookAtTarget_index = reader.GetIndex("lookAtTarget");
                int changeSkill_index = reader.GetIndex("changeSkill");
                int changeSkillTime_index = reader.GetIndex("changeSkillTime");
                int changeSkillType_index = reader.GetIndex("changeSkillType");
                int changeSkillLimit_index = reader.GetIndex("changeSkillLimit");
                int castingTime_index = reader.GetIndex("castingTime");
                int castingContinueTime_index = reader.GetIndex("castingContinueTime");
                int toPosture_index = reader.GetIndex("toPosture");
                int des_index = reader.GetIndex("des");
                int targetSearch_index = reader.GetIndex("targetSearch");
                int zhenqiRec_index = reader.TryIndex("zhenqiRec");
                int switchPriority_index = reader.TryIndex("switchPriority");
                int excuteActionIds_index = reader.TryIndex("excuteActionIds");
                int bDes_index = reader.TryIndex("bDes");
                int searchDes_index = reader.TryIndex("searchDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillConfig data = new SkillConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.aniid = reader.getStr(i, aniid_index);         
						data.type = ((Type)reader.getInt(i, type_index, 0));         
						data.isPetStunt = reader.getBool(i, isPetStunt_index, false);         
						data.openLevel = reader.getInt(i, openLevel_index, 0);         
						data.forceSkill = reader.getBool(i, forceSkill_index, false);         
						data.slotid = reader.getInt(i, slotid_index, 0);         
						data.cd = reader.getFloat(i, cd_index, 0f);         
						data.costZhenqi = reader.getInt(i, costZhenqi_index, 0);         
						data.CostLingqi = reader.getInt(i, CostLingqi_index, 0);         
						data.CostQili = reader.getInt(i, CostQili_index, 0);         
						data.castingBattleState = ((CastingBattleState)reader.getInt(i, castingBattleState_index, 0));         
						data.castingConditions = reader.getInt(i, castingConditions_index, 0);         
						data.castingPosture = reader.getInts(i, castingPosture_index, ';');         
						data.isNeedTarget = reader.getBool(i, isNeedTarget_index, false);         
						data.range = reader.getFloat(i, range_index, 0f);         
						data.isAutoTarget = reader.getBool(i, isAutoTarget_index, false);         
						data.autoTargetRange = reader.getFloat(i, autoTargetRange_index, 0f);         
						data.autoTargetAngle = reader.getFloat(i, autoTargetAngle_index, 0f);         
						data.lookAtTarget = reader.getInt(i, lookAtTarget_index, 0);         
						data.changeSkill = reader.getInt(i, changeSkill_index, 0);         
						data.changeSkillTime = reader.getFloat(i, changeSkillTime_index, 0f);         
						data.changeSkillType = ((SwitchType)reader.getInt(i, changeSkillType_index, 0));         
						data.changeSkillLimit = reader.getInt(i, changeSkillLimit_index, 0);         
						data.castingTime = reader.getFloat(i, castingTime_index, 0f);         
						data.castingContinueTime = reader.getFloat(i, castingContinueTime_index, 0f);         
						data.toPosture = reader.getInt(i, toPosture_index, 0);         
						data.des = reader.getStr(i, des_index);         
						data.targetSearch = reader.getStr(i, targetSearch_index);         
						data.zhenqiRec = reader.getInt(i, zhenqiRec_index, 0);         
						data.switchPriority = reader.getInt(i, switchPriority_index, 0);         
						data.excuteActionIds = reader.getStrs(i, excuteActionIds_index, ';');         
						data.bDes = reader.getStr(i, bDes_index);         
						data.searchDes = reader.getStr(i, searchDes_index);         
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
                string file = "Skill/SkillConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int aniid_index = reader.GetIndex("aniid");
                int type_index = reader.GetIndex("type");
                int isPetStunt_index = reader.GetIndex("isPetStunt");
                int openLevel_index = reader.GetIndex("openLevel");
                int forceSkill_index = reader.GetIndex("forceSkill");
                int slotid_index = reader.GetIndex("slotid");
                int cd_index = reader.GetIndex("cd");
                int costZhenqi_index = reader.GetIndex("costZhenqi");
                int CostLingqi_index = reader.GetIndex("CostLingqi");
                int CostQili_index = reader.GetIndex("CostQili");
                int castingBattleState_index = reader.GetIndex("castingBattleState");
                int castingConditions_index = reader.GetIndex("castingConditions");
                int castingPosture_index = reader.GetIndex("castingPosture");
                int isNeedTarget_index = reader.GetIndex("isNeedTarget");
                int range_index = reader.GetIndex("range");
                int isAutoTarget_index = reader.GetIndex("isAutoTarget");
                int autoTargetRange_index = reader.GetIndex("autoTargetRange");
                int autoTargetAngle_index = reader.GetIndex("autoTargetAngle");
                int lookAtTarget_index = reader.GetIndex("lookAtTarget");
                int changeSkill_index = reader.GetIndex("changeSkill");
                int changeSkillTime_index = reader.GetIndex("changeSkillTime");
                int changeSkillType_index = reader.GetIndex("changeSkillType");
                int changeSkillLimit_index = reader.GetIndex("changeSkillLimit");
                int castingTime_index = reader.GetIndex("castingTime");
                int castingContinueTime_index = reader.GetIndex("castingContinueTime");
                int toPosture_index = reader.TryIndex("toPosture");
                int des_index = reader.GetIndex("des");
                int targetSearch_index = reader.GetIndex("targetSearch");
                int zhenqiRec_index = reader.GetIndex("zhenqiRec");
                int switchPriority_index = reader.GetIndex("switchPriority");
                int excuteActionIds_index = reader.GetIndex("excuteActionIds");
                int bDes_index = reader.GetIndex("bDes");
                int searchDes_index = reader.GetIndex("searchDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillConfig data = new SkillConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.aniid = reader.getStr(i, aniid_index);         
						data.type = ((Type)reader.getInt(i, type_index, 0));         
						data.isPetStunt = reader.getBool(i, isPetStunt_index, false);         
						data.openLevel = reader.getInt(i, openLevel_index, 0);         
						data.forceSkill = reader.getBool(i, forceSkill_index, false);         
						data.slotid = reader.getInt(i, slotid_index, 0);         
						data.cd = reader.getFloat(i, cd_index, 0f);         
						data.costZhenqi = reader.getInt(i, costZhenqi_index, 0);         
						data.CostLingqi = reader.getInt(i, CostLingqi_index, 0);         
						data.CostQili = reader.getInt(i, CostQili_index, 0);         
						data.castingBattleState = ((CastingBattleState)reader.getInt(i, castingBattleState_index, 0));         
						data.castingConditions = reader.getInt(i, castingConditions_index, 0);         
						data.castingPosture = reader.getInts(i, castingPosture_index, ';');         
						data.isNeedTarget = reader.getBool(i, isNeedTarget_index, false);         
						data.range = reader.getFloat(i, range_index, 0f);         
						data.isAutoTarget = reader.getBool(i, isAutoTarget_index, false);         
						data.autoTargetRange = reader.getFloat(i, autoTargetRange_index, 0f);         
						data.autoTargetAngle = reader.getFloat(i, autoTargetAngle_index, 0f);         
						data.lookAtTarget = reader.getInt(i, lookAtTarget_index, 0);         
						data.changeSkill = reader.getInt(i, changeSkill_index, 0);         
						data.changeSkillTime = reader.getFloat(i, changeSkillTime_index, 0f);         
						data.changeSkillType = ((SwitchType)reader.getInt(i, changeSkillType_index, 0));         
						data.changeSkillLimit = reader.getInt(i, changeSkillLimit_index, 0);         
						data.castingTime = reader.getFloat(i, castingTime_index, 0f);         
						data.castingContinueTime = reader.getFloat(i, castingContinueTime_index, 0f);         
						data.toPosture = reader.getInt(i, toPosture_index, 0);         
						data.des = reader.getStr(i, des_index);         
						data.targetSearch = reader.getStr(i, targetSearch_index);         
						data.zhenqiRec = reader.getInt(i, zhenqiRec_index, 0);         
						data.switchPriority = reader.getInt(i, switchPriority_index, 0);         
						data.excuteActionIds = reader.getStrs(i, excuteActionIds_index, ';');         
						data.bDes = reader.getStr(i, bDes_index);         
						data.searchDes = reader.getStr(i, searchDes_index);         
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
                    CsvCommon.Log.Error("SkillConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillConfig);
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



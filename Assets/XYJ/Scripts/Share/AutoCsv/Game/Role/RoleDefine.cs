// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleDefine 
    {
        static Dictionary<int, RoleDefine> DataList = new Dictionary<int, RoleDefine>();

        static public Dictionary<int, RoleDefine> GetAll()
        {
            return DataList;
        }

        static public RoleDefine Get(int key)
        {
            RoleDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleDefine.Get({0}) not find!", key);
            return null;
        }



        // 角色ID
        public int id { get; set; }

        // 名字
        public string name { get; set; }

        // 模型
        public string model { get; set; }

        // 简单AI
        public int simpleAI { get; set; }

        // 行为树
        public string btAI { get; set; }

        // 思考间隔
        public float[] thinkInterval { get; set; }

        // 视野
        public float fieldOfView { get; set; }

        // 追踪距离
        public float trackDisance { get; set; }

        // 放缩比例
        public float modelScale { get; set; }

        // 默认姿态
        public int posture { get; set; }

        // 选中圈比例
        public float selectScaleRate { get; set; }

        // 选中框比例
        public float selectTargetScaleRate { get; set; }

        // 头像
        public string headIcon { get; set; }

        // 阵营
        public BattleCamp battleCamp { get; set; }

        // 隐藏名称
        public int hideName { get; set; }

        // 循环特效
        public string loopEffect { get; set; }

        // 移动速度
        public float speed { get; set; }

        // 受击半径
        public float behitRaidus { get; set; }

        // 免疫控制
        public int isBati { get; set; }

        // 默认技能
        public int[] defaultSkills { get; set; }

        // 后端动作
        public string serverAni { get; set; }

        // 等级设置类型
        public LevelGradeSetting levelGradeSetting { get; set; }

        // 怪物等级
        public int level { get; set; }

        // 生命修正
        public float hpfix { get; set; }

        // 攻击修正
        public float attackfix { get; set; }

        // 防御修正
        public float defensefix { get; set; }

        // 力量资质
        public float liliang_zz { get; set; }

        // 智慧资质
        public float zihui_zz { get; set; }

        // 根骨资质
        public float gengu_zz { get; set; }

        // 体魄资质
        public float tipo_zz { get; set; }

        // 敏捷资质
        public float minjie_zz { get; set; }

        // 身法资质
        public float shenfa_zz { get; set; }

        // 生命资质
        public float shengming_zz { get; set; }

        // 物理攻击资质
        public float wuligongji_zz { get; set; }

        // 法术攻击资质
        public float fashugongji_zz { get; set; }

        // 物理防御资质
        public float wulifangyu_zz { get; set; }

        // 法术防御资质
        public float fashufangyu_zz { get; set; }

        // 称谓
        public string title { get; set; }

        // 屏蔽点击
        public int shieldClick { get; set; }

        // 触发转向
        public int isCanRotate { get; set; }

        // 转向距离
        public float rotateDistance { get; set; }

        // npc功能id
        public int npcFunId { get; set; }

        // 性格类型
        public int personalityType { get; set; }

        // 欣赏冒泡
        public string enjoyBubble { get; set; }

        // 厌恶冒泡
        public string hateBubble { get; set; }

        // 冒泡概率
        public float bubbleRate { get; set; }

        // 是否有任务触发
        public int isHasTask { get; set; }

        // 触发需要任务
        public int taskId { get; set; }

        // 触发事件
        public int eventId { get; set; }

        // 触发时特效
        public string taskTrigerEffect { get; set; }

        // 显示任务进行中标识
        public int showTaskTag { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleDefine> allDatas = new List<RoleDefine>();

            {
                string file = "Role/RoleDefine@FA#Role.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int model_index = reader.GetIndex("model");
                int simpleAI_index = reader.GetIndex("simpleAI");
                int btAI_index = reader.GetIndex("btAI");
                int thinkInterval_index = reader.GetIndex("thinkInterval");
                int fieldOfView_index = reader.GetIndex("fieldOfView");
                int trackDisance_index = reader.GetIndex("trackDisance");
                int modelScale_index = reader.GetIndex("modelScale");
                int posture_index = reader.GetIndex("posture");
                int selectScaleRate_index = reader.GetIndex("selectScaleRate");
                int selectTargetScaleRate_index = reader.GetIndex("selectTargetScaleRate");
                int headIcon_index = reader.GetIndex("headIcon");
                int battleCamp_index = reader.GetIndex("battleCamp");
                int hideName_index = reader.GetIndex("hideName");
                int loopEffect_index = reader.GetIndex("loopEffect");
                int speed_index = reader.GetIndex("speed");
                int behitRaidus_index = reader.GetIndex("behitRaidus");
                int isBati_index = reader.GetIndex("isBati");
                int defaultSkills_index = reader.GetIndex("defaultSkills");
                int serverAni_index = reader.GetIndex("serverAni");
                int levelGradeSetting_index = reader.GetIndex("levelGradeSetting");
                int level_index = reader.GetIndex("level");
                int hpfix_index = reader.GetIndex("hpfix");
                int attackfix_index = reader.GetIndex("attackfix");
                int defensefix_index = reader.GetIndex("defensefix");
                int liliang_zz_index = reader.GetIndex("liliang_zz");
                int zihui_zz_index = reader.GetIndex("zihui_zz");
                int gengu_zz_index = reader.GetIndex("gengu_zz");
                int tipo_zz_index = reader.GetIndex("tipo_zz");
                int minjie_zz_index = reader.GetIndex("minjie_zz");
                int shenfa_zz_index = reader.GetIndex("shenfa_zz");
                int shengming_zz_index = reader.GetIndex("shengming_zz");
                int wuligongji_zz_index = reader.GetIndex("wuligongji_zz");
                int fashugongji_zz_index = reader.GetIndex("fashugongji_zz");
                int wulifangyu_zz_index = reader.GetIndex("wulifangyu_zz");
                int fashufangyu_zz_index = reader.GetIndex("fashufangyu_zz");
                int title_index = reader.TryIndex("title");
                int shieldClick_index = reader.TryIndex("shieldClick");
                int isCanRotate_index = reader.TryIndex("isCanRotate");
                int rotateDistance_index = reader.TryIndex("rotateDistance");
                int npcFunId_index = reader.TryIndex("npcFunId");
                int personalityType_index = reader.TryIndex("personalityType");
                int enjoyBubble_index = reader.TryIndex("enjoyBubble");
                int hateBubble_index = reader.TryIndex("hateBubble");
                int bubbleRate_index = reader.TryIndex("bubbleRate");
                int isHasTask_index = reader.TryIndex("isHasTask");
                int taskId_index = reader.TryIndex("taskId");
                int eventId_index = reader.TryIndex("eventId");
                int taskTrigerEffect_index = reader.TryIndex("taskTrigerEffect");
                int showTaskTag_index = reader.TryIndex("showTaskTag");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDefine data = new RoleDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.model = reader.getStr(i, model_index);         
						data.simpleAI = reader.getInt(i, simpleAI_index, 0);         
						data.btAI = reader.getStr(i, btAI_index);         
						data.thinkInterval = reader.getFloats(i, thinkInterval_index, ';');         
						data.fieldOfView = reader.getFloat(i, fieldOfView_index, 10.0f);         
						data.trackDisance = reader.getFloat(i, trackDisance_index, 15.0f);         
						data.modelScale = reader.getFloat(i, modelScale_index, 0f);         
						data.posture = reader.getInt(i, posture_index, 0);         
						data.selectScaleRate = reader.getFloat(i, selectScaleRate_index, 0f);         
						data.selectTargetScaleRate = reader.getFloat(i, selectTargetScaleRate_index, 0f);         
						data.headIcon = reader.getStr(i, headIcon_index);         
						data.battleCamp = ((BattleCamp)reader.getInt(i, battleCamp_index, 3));         
						data.hideName = reader.getInt(i, hideName_index, 0);         
						data.loopEffect = reader.getStr(i, loopEffect_index);         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.behitRaidus = reader.getFloat(i, behitRaidus_index, 0f);         
						data.isBati = reader.getInt(i, isBati_index, 0);         
						data.defaultSkills = reader.getInts(i, defaultSkills_index, ';');         
						data.serverAni = reader.getStr(i, serverAni_index);         
						data.levelGradeSetting = ((LevelGradeSetting)reader.getInt(i, levelGradeSetting_index, 0));         
						data.level = reader.getInt(i, level_index, 0);         
						data.hpfix = reader.getFloat(i, hpfix_index, 0f);         
						data.attackfix = reader.getFloat(i, attackfix_index, 0f);         
						data.defensefix = reader.getFloat(i, defensefix_index, 0f);         
						data.liliang_zz = reader.getFloat(i, liliang_zz_index, 0f);         
						data.zihui_zz = reader.getFloat(i, zihui_zz_index, 0f);         
						data.gengu_zz = reader.getFloat(i, gengu_zz_index, 0f);         
						data.tipo_zz = reader.getFloat(i, tipo_zz_index, 0f);         
						data.minjie_zz = reader.getFloat(i, minjie_zz_index, 0f);         
						data.shenfa_zz = reader.getFloat(i, shenfa_zz_index, 0f);         
						data.shengming_zz = reader.getFloat(i, shengming_zz_index, 0f);         
						data.wuligongji_zz = reader.getFloat(i, wuligongji_zz_index, 0f);         
						data.fashugongji_zz = reader.getFloat(i, fashugongji_zz_index, 0f);         
						data.wulifangyu_zz = reader.getFloat(i, wulifangyu_zz_index, 0f);         
						data.fashufangyu_zz = reader.getFloat(i, fashufangyu_zz_index, 0f);         
						data.title = reader.getStr(i, title_index);         
						data.shieldClick = reader.getInt(i, shieldClick_index, 0);         
						data.isCanRotate = reader.getInt(i, isCanRotate_index, 0);         
						data.rotateDistance = reader.getFloat(i, rotateDistance_index, 0f);         
						data.npcFunId = reader.getInt(i, npcFunId_index, 0);         
						data.personalityType = reader.getInt(i, personalityType_index, 0);         
						data.enjoyBubble = reader.getStr(i, enjoyBubble_index);         
						data.hateBubble = reader.getStr(i, hateBubble_index);         
						data.bubbleRate = reader.getFloat(i, bubbleRate_index, 0f);         
						data.isHasTask = reader.getInt(i, isHasTask_index, 0);         
						data.taskId = reader.getInt(i, taskId_index, 0);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.taskTrigerEffect = reader.getStr(i, taskTrigerEffect_index);         
						data.showTaskTag = reader.getInt(i, showTaskTag_index, 0);         
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
                string file = "Role/RoleDefine@FA#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int model_index = reader.GetIndex("model");
                int simpleAI_index = reader.GetIndex("simpleAI");
                int btAI_index = reader.GetIndex("btAI");
                int thinkInterval_index = reader.TryIndex("thinkInterval");
                int fieldOfView_index = reader.GetIndex("fieldOfView");
                int trackDisance_index = reader.GetIndex("trackDisance");
                int modelScale_index = reader.GetIndex("modelScale");
                int posture_index = reader.GetIndex("posture");
                int selectScaleRate_index = reader.GetIndex("selectScaleRate");
                int selectTargetScaleRate_index = reader.GetIndex("selectTargetScaleRate");
                int headIcon_index = reader.GetIndex("headIcon");
                int battleCamp_index = reader.GetIndex("battleCamp");
                int hideName_index = reader.GetIndex("hideName");
                int loopEffect_index = reader.TryIndex("loopEffect");
                int speed_index = reader.GetIndex("speed");
                int behitRaidus_index = reader.TryIndex("behitRaidus");
                int isBati_index = reader.TryIndex("isBati");
                int defaultSkills_index = reader.TryIndex("defaultSkills");
                int serverAni_index = reader.TryIndex("serverAni");
                int levelGradeSetting_index = reader.TryIndex("levelGradeSetting");
                int level_index = reader.TryIndex("level");
                int hpfix_index = reader.TryIndex("hpfix");
                int attackfix_index = reader.TryIndex("attackfix");
                int defensefix_index = reader.TryIndex("defensefix");
                int liliang_zz_index = reader.TryIndex("liliang_zz");
                int zihui_zz_index = reader.TryIndex("zihui_zz");
                int gengu_zz_index = reader.TryIndex("gengu_zz");
                int tipo_zz_index = reader.TryIndex("tipo_zz");
                int minjie_zz_index = reader.TryIndex("minjie_zz");
                int shenfa_zz_index = reader.TryIndex("shenfa_zz");
                int shengming_zz_index = reader.TryIndex("shengming_zz");
                int wuligongji_zz_index = reader.TryIndex("wuligongji_zz");
                int fashugongji_zz_index = reader.TryIndex("fashugongji_zz");
                int wulifangyu_zz_index = reader.TryIndex("wulifangyu_zz");
                int fashufangyu_zz_index = reader.TryIndex("fashufangyu_zz");
                int title_index = reader.GetIndex("title");
                int shieldClick_index = reader.GetIndex("shieldClick");
                int isCanRotate_index = reader.GetIndex("isCanRotate");
                int rotateDistance_index = reader.GetIndex("rotateDistance");
                int npcFunId_index = reader.GetIndex("npcFunId");
                int personalityType_index = reader.GetIndex("personalityType");
                int enjoyBubble_index = reader.GetIndex("enjoyBubble");
                int hateBubble_index = reader.GetIndex("hateBubble");
                int bubbleRate_index = reader.GetIndex("bubbleRate");
                int isHasTask_index = reader.GetIndex("isHasTask");
                int taskId_index = reader.GetIndex("taskId");
                int eventId_index = reader.GetIndex("eventId");
                int taskTrigerEffect_index = reader.GetIndex("taskTrigerEffect");
                int showTaskTag_index = reader.GetIndex("showTaskTag");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDefine data = new RoleDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.model = reader.getStr(i, model_index);         
						data.simpleAI = reader.getInt(i, simpleAI_index, 0);         
						data.btAI = reader.getStr(i, btAI_index);         
						data.thinkInterval = reader.getFloats(i, thinkInterval_index, ';');         
						data.fieldOfView = reader.getFloat(i, fieldOfView_index, 10.0f);         
						data.trackDisance = reader.getFloat(i, trackDisance_index, 15.0f);         
						data.modelScale = reader.getFloat(i, modelScale_index, 0f);         
						data.posture = reader.getInt(i, posture_index, 0);         
						data.selectScaleRate = reader.getFloat(i, selectScaleRate_index, 0f);         
						data.selectTargetScaleRate = reader.getFloat(i, selectTargetScaleRate_index, 0f);         
						data.headIcon = reader.getStr(i, headIcon_index);         
						data.battleCamp = ((BattleCamp)reader.getInt(i, battleCamp_index, 3));         
						data.hideName = reader.getInt(i, hideName_index, 0);         
						data.loopEffect = reader.getStr(i, loopEffect_index);         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.behitRaidus = reader.getFloat(i, behitRaidus_index, 0f);         
						data.isBati = reader.getInt(i, isBati_index, 0);         
						data.defaultSkills = reader.getInts(i, defaultSkills_index, ';');         
						data.serverAni = reader.getStr(i, serverAni_index);         
						data.levelGradeSetting = ((LevelGradeSetting)reader.getInt(i, levelGradeSetting_index, 0));         
						data.level = reader.getInt(i, level_index, 0);         
						data.hpfix = reader.getFloat(i, hpfix_index, 0f);         
						data.attackfix = reader.getFloat(i, attackfix_index, 0f);         
						data.defensefix = reader.getFloat(i, defensefix_index, 0f);         
						data.liliang_zz = reader.getFloat(i, liliang_zz_index, 0f);         
						data.zihui_zz = reader.getFloat(i, zihui_zz_index, 0f);         
						data.gengu_zz = reader.getFloat(i, gengu_zz_index, 0f);         
						data.tipo_zz = reader.getFloat(i, tipo_zz_index, 0f);         
						data.minjie_zz = reader.getFloat(i, minjie_zz_index, 0f);         
						data.shenfa_zz = reader.getFloat(i, shenfa_zz_index, 0f);         
						data.shengming_zz = reader.getFloat(i, shengming_zz_index, 0f);         
						data.wuligongji_zz = reader.getFloat(i, wuligongji_zz_index, 0f);         
						data.fashugongji_zz = reader.getFloat(i, fashugongji_zz_index, 0f);         
						data.wulifangyu_zz = reader.getFloat(i, wulifangyu_zz_index, 0f);         
						data.fashufangyu_zz = reader.getFloat(i, fashufangyu_zz_index, 0f);         
						data.title = reader.getStr(i, title_index);         
						data.shieldClick = reader.getInt(i, shieldClick_index, 0);         
						data.isCanRotate = reader.getInt(i, isCanRotate_index, 0);         
						data.rotateDistance = reader.getFloat(i, rotateDistance_index, 0f);         
						data.npcFunId = reader.getInt(i, npcFunId_index, 0);         
						data.personalityType = reader.getInt(i, personalityType_index, 0);         
						data.enjoyBubble = reader.getStr(i, enjoyBubble_index);         
						data.hateBubble = reader.getStr(i, hateBubble_index);         
						data.bubbleRate = reader.getFloat(i, bubbleRate_index, 0f);         
						data.isHasTask = reader.getInt(i, isHasTask_index, 0);         
						data.taskId = reader.getInt(i, taskId_index, 0);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.taskTrigerEffect = reader.getStr(i, taskTrigerEffect_index);         
						data.showTaskTag = reader.getInt(i, showTaskTag_index, 0);         
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
                string file = "Role/RoleDefine@FA#YZ.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int model_index = reader.GetIndex("model");
                int simpleAI_index = reader.TryIndex("simpleAI");
                int btAI_index = reader.TryIndex("btAI");
                int thinkInterval_index = reader.TryIndex("thinkInterval");
                int fieldOfView_index = reader.TryIndex("fieldOfView");
                int trackDisance_index = reader.TryIndex("trackDisance");
                int modelScale_index = reader.GetIndex("modelScale");
                int posture_index = reader.GetIndex("posture");
                int selectScaleRate_index = reader.TryIndex("selectScaleRate");
                int selectTargetScaleRate_index = reader.TryIndex("selectTargetScaleRate");
                int headIcon_index = reader.TryIndex("headIcon");
                int battleCamp_index = reader.GetIndex("battleCamp");
                int hideName_index = reader.GetIndex("hideName");
                int loopEffect_index = reader.TryIndex("loopEffect");
                int speed_index = reader.GetIndex("speed");
                int behitRaidus_index = reader.TryIndex("behitRaidus");
                int isBati_index = reader.TryIndex("isBati");
                int defaultSkills_index = reader.TryIndex("defaultSkills");
                int serverAni_index = reader.TryIndex("serverAni");
                int levelGradeSetting_index = reader.TryIndex("levelGradeSetting");
                int level_index = reader.TryIndex("level");
                int hpfix_index = reader.TryIndex("hpfix");
                int attackfix_index = reader.TryIndex("attackfix");
                int defensefix_index = reader.TryIndex("defensefix");
                int liliang_zz_index = reader.TryIndex("liliang_zz");
                int zihui_zz_index = reader.TryIndex("zihui_zz");
                int gengu_zz_index = reader.TryIndex("gengu_zz");
                int tipo_zz_index = reader.TryIndex("tipo_zz");
                int minjie_zz_index = reader.TryIndex("minjie_zz");
                int shenfa_zz_index = reader.TryIndex("shenfa_zz");
                int shengming_zz_index = reader.TryIndex("shengming_zz");
                int wuligongji_zz_index = reader.TryIndex("wuligongji_zz");
                int fashugongji_zz_index = reader.TryIndex("fashugongji_zz");
                int wulifangyu_zz_index = reader.TryIndex("wulifangyu_zz");
                int fashufangyu_zz_index = reader.TryIndex("fashufangyu_zz");
                int title_index = reader.TryIndex("title");
                int shieldClick_index = reader.TryIndex("shieldClick");
                int isCanRotate_index = reader.TryIndex("isCanRotate");
                int rotateDistance_index = reader.TryIndex("rotateDistance");
                int npcFunId_index = reader.TryIndex("npcFunId");
                int personalityType_index = reader.TryIndex("personalityType");
                int enjoyBubble_index = reader.TryIndex("enjoyBubble");
                int hateBubble_index = reader.TryIndex("hateBubble");
                int bubbleRate_index = reader.TryIndex("bubbleRate");
                int isHasTask_index = reader.TryIndex("isHasTask");
                int taskId_index = reader.TryIndex("taskId");
                int eventId_index = reader.TryIndex("eventId");
                int taskTrigerEffect_index = reader.TryIndex("taskTrigerEffect");
                int showTaskTag_index = reader.TryIndex("showTaskTag");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDefine data = new RoleDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.model = reader.getStr(i, model_index);         
						data.simpleAI = reader.getInt(i, simpleAI_index, 0);         
						data.btAI = reader.getStr(i, btAI_index);         
						data.thinkInterval = reader.getFloats(i, thinkInterval_index, ';');         
						data.fieldOfView = reader.getFloat(i, fieldOfView_index, 10.0f);         
						data.trackDisance = reader.getFloat(i, trackDisance_index, 15.0f);         
						data.modelScale = reader.getFloat(i, modelScale_index, 0f);         
						data.posture = reader.getInt(i, posture_index, 0);         
						data.selectScaleRate = reader.getFloat(i, selectScaleRate_index, 0f);         
						data.selectTargetScaleRate = reader.getFloat(i, selectTargetScaleRate_index, 0f);         
						data.headIcon = reader.getStr(i, headIcon_index);         
						data.battleCamp = ((BattleCamp)reader.getInt(i, battleCamp_index, 3));         
						data.hideName = reader.getInt(i, hideName_index, 0);         
						data.loopEffect = reader.getStr(i, loopEffect_index);         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.behitRaidus = reader.getFloat(i, behitRaidus_index, 0f);         
						data.isBati = reader.getInt(i, isBati_index, 0);         
						data.defaultSkills = reader.getInts(i, defaultSkills_index, ';');         
						data.serverAni = reader.getStr(i, serverAni_index);         
						data.levelGradeSetting = ((LevelGradeSetting)reader.getInt(i, levelGradeSetting_index, 0));         
						data.level = reader.getInt(i, level_index, 0);         
						data.hpfix = reader.getFloat(i, hpfix_index, 0f);         
						data.attackfix = reader.getFloat(i, attackfix_index, 0f);         
						data.defensefix = reader.getFloat(i, defensefix_index, 0f);         
						data.liliang_zz = reader.getFloat(i, liliang_zz_index, 0f);         
						data.zihui_zz = reader.getFloat(i, zihui_zz_index, 0f);         
						data.gengu_zz = reader.getFloat(i, gengu_zz_index, 0f);         
						data.tipo_zz = reader.getFloat(i, tipo_zz_index, 0f);         
						data.minjie_zz = reader.getFloat(i, minjie_zz_index, 0f);         
						data.shenfa_zz = reader.getFloat(i, shenfa_zz_index, 0f);         
						data.shengming_zz = reader.getFloat(i, shengming_zz_index, 0f);         
						data.wuligongji_zz = reader.getFloat(i, wuligongji_zz_index, 0f);         
						data.fashugongji_zz = reader.getFloat(i, fashugongji_zz_index, 0f);         
						data.wulifangyu_zz = reader.getFloat(i, wulifangyu_zz_index, 0f);         
						data.fashufangyu_zz = reader.getFloat(i, fashufangyu_zz_index, 0f);         
						data.title = reader.getStr(i, title_index);         
						data.shieldClick = reader.getInt(i, shieldClick_index, 0);         
						data.isCanRotate = reader.getInt(i, isCanRotate_index, 0);         
						data.rotateDistance = reader.getFloat(i, rotateDistance_index, 0f);         
						data.npcFunId = reader.getInt(i, npcFunId_index, 0);         
						data.personalityType = reader.getInt(i, personalityType_index, 0);         
						data.enjoyBubble = reader.getStr(i, enjoyBubble_index);         
						data.hateBubble = reader.getStr(i, hateBubble_index);         
						data.bubbleRate = reader.getFloat(i, bubbleRate_index, 0f);         
						data.isHasTask = reader.getInt(i, isHasTask_index, 0);         
						data.taskId = reader.getInt(i, taskId_index, 0);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.taskTrigerEffect = reader.getStr(i, taskTrigerEffect_index);         
						data.showTaskTag = reader.getInt(i, showTaskTag_index, 0);         
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
                string file = "Role/RoleDefine@FA#ZZ.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int model_index = reader.GetIndex("model");
                int simpleAI_index = reader.TryIndex("simpleAI");
                int btAI_index = reader.TryIndex("btAI");
                int thinkInterval_index = reader.TryIndex("thinkInterval");
                int fieldOfView_index = reader.TryIndex("fieldOfView");
                int trackDisance_index = reader.TryIndex("trackDisance");
                int modelScale_index = reader.TryIndex("modelScale");
                int posture_index = reader.TryIndex("posture");
                int selectScaleRate_index = reader.TryIndex("selectScaleRate");
                int selectTargetScaleRate_index = reader.TryIndex("selectTargetScaleRate");
                int headIcon_index = reader.TryIndex("headIcon");
                int battleCamp_index = reader.TryIndex("battleCamp");
                int hideName_index = reader.TryIndex("hideName");
                int loopEffect_index = reader.TryIndex("loopEffect");
                int speed_index = reader.TryIndex("speed");
                int behitRaidus_index = reader.TryIndex("behitRaidus");
                int isBati_index = reader.TryIndex("isBati");
                int defaultSkills_index = reader.TryIndex("defaultSkills");
                int serverAni_index = reader.TryIndex("serverAni");
                int levelGradeSetting_index = reader.TryIndex("levelGradeSetting");
                int level_index = reader.TryIndex("level");
                int hpfix_index = reader.TryIndex("hpfix");
                int attackfix_index = reader.TryIndex("attackfix");
                int defensefix_index = reader.TryIndex("defensefix");
                int liliang_zz_index = reader.TryIndex("liliang_zz");
                int zihui_zz_index = reader.TryIndex("zihui_zz");
                int gengu_zz_index = reader.TryIndex("gengu_zz");
                int tipo_zz_index = reader.TryIndex("tipo_zz");
                int minjie_zz_index = reader.TryIndex("minjie_zz");
                int shenfa_zz_index = reader.TryIndex("shenfa_zz");
                int shengming_zz_index = reader.TryIndex("shengming_zz");
                int wuligongji_zz_index = reader.TryIndex("wuligongji_zz");
                int fashugongji_zz_index = reader.TryIndex("fashugongji_zz");
                int wulifangyu_zz_index = reader.TryIndex("wulifangyu_zz");
                int fashufangyu_zz_index = reader.TryIndex("fashufangyu_zz");
                int title_index = reader.TryIndex("title");
                int shieldClick_index = reader.TryIndex("shieldClick");
                int isCanRotate_index = reader.TryIndex("isCanRotate");
                int rotateDistance_index = reader.TryIndex("rotateDistance");
                int npcFunId_index = reader.TryIndex("npcFunId");
                int personalityType_index = reader.TryIndex("personalityType");
                int enjoyBubble_index = reader.TryIndex("enjoyBubble");
                int hateBubble_index = reader.TryIndex("hateBubble");
                int bubbleRate_index = reader.TryIndex("bubbleRate");
                int isHasTask_index = reader.TryIndex("isHasTask");
                int taskId_index = reader.TryIndex("taskId");
                int eventId_index = reader.TryIndex("eventId");
                int taskTrigerEffect_index = reader.TryIndex("taskTrigerEffect");
                int showTaskTag_index = reader.TryIndex("showTaskTag");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDefine data = new RoleDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.model = reader.getStr(i, model_index);         
						data.simpleAI = reader.getInt(i, simpleAI_index, 0);         
						data.btAI = reader.getStr(i, btAI_index);         
						data.thinkInterval = reader.getFloats(i, thinkInterval_index, ';');         
						data.fieldOfView = reader.getFloat(i, fieldOfView_index, 10.0f);         
						data.trackDisance = reader.getFloat(i, trackDisance_index, 15.0f);         
						data.modelScale = reader.getFloat(i, modelScale_index, 0f);         
						data.posture = reader.getInt(i, posture_index, 0);         
						data.selectScaleRate = reader.getFloat(i, selectScaleRate_index, 0f);         
						data.selectTargetScaleRate = reader.getFloat(i, selectTargetScaleRate_index, 0f);         
						data.headIcon = reader.getStr(i, headIcon_index);         
						data.battleCamp = ((BattleCamp)reader.getInt(i, battleCamp_index, 3));         
						data.hideName = reader.getInt(i, hideName_index, 0);         
						data.loopEffect = reader.getStr(i, loopEffect_index);         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.behitRaidus = reader.getFloat(i, behitRaidus_index, 0f);         
						data.isBati = reader.getInt(i, isBati_index, 0);         
						data.defaultSkills = reader.getInts(i, defaultSkills_index, ';');         
						data.serverAni = reader.getStr(i, serverAni_index);         
						data.levelGradeSetting = ((LevelGradeSetting)reader.getInt(i, levelGradeSetting_index, 0));         
						data.level = reader.getInt(i, level_index, 0);         
						data.hpfix = reader.getFloat(i, hpfix_index, 0f);         
						data.attackfix = reader.getFloat(i, attackfix_index, 0f);         
						data.defensefix = reader.getFloat(i, defensefix_index, 0f);         
						data.liliang_zz = reader.getFloat(i, liliang_zz_index, 0f);         
						data.zihui_zz = reader.getFloat(i, zihui_zz_index, 0f);         
						data.gengu_zz = reader.getFloat(i, gengu_zz_index, 0f);         
						data.tipo_zz = reader.getFloat(i, tipo_zz_index, 0f);         
						data.minjie_zz = reader.getFloat(i, minjie_zz_index, 0f);         
						data.shenfa_zz = reader.getFloat(i, shenfa_zz_index, 0f);         
						data.shengming_zz = reader.getFloat(i, shengming_zz_index, 0f);         
						data.wuligongji_zz = reader.getFloat(i, wuligongji_zz_index, 0f);         
						data.fashugongji_zz = reader.getFloat(i, fashugongji_zz_index, 0f);         
						data.wulifangyu_zz = reader.getFloat(i, wulifangyu_zz_index, 0f);         
						data.fashufangyu_zz = reader.getFloat(i, fashufangyu_zz_index, 0f);         
						data.title = reader.getStr(i, title_index);         
						data.shieldClick = reader.getInt(i, shieldClick_index, 0);         
						data.isCanRotate = reader.getInt(i, isCanRotate_index, 0);         
						data.rotateDistance = reader.getFloat(i, rotateDistance_index, 0f);         
						data.npcFunId = reader.getInt(i, npcFunId_index, 0);         
						data.personalityType = reader.getInt(i, personalityType_index, 0);         
						data.enjoyBubble = reader.getStr(i, enjoyBubble_index);         
						data.hateBubble = reader.getStr(i, hateBubble_index);         
						data.bubbleRate = reader.getFloat(i, bubbleRate_index, 0f);         
						data.isHasTask = reader.getInt(i, isHasTask_index, 0);         
						data.taskId = reader.getInt(i, taskId_index, 0);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.taskTrigerEffect = reader.getStr(i, taskTrigerEffect_index);         
						data.showTaskTag = reader.getInt(i, showTaskTag_index, 0);         
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
                    CsvCommon.Log.Error("RoleDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleDefine);
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



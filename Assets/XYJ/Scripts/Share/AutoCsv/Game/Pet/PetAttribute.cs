// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PetAttribute 
    {
        static List<PetAttribute> DataList = new List<PetAttribute>();
        static public List<PetAttribute> GetAll()
        {
            return DataList;
        }

        static public PetAttribute Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].identity == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("PetAttribute.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].identity == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<PetAttribute>> DataList_identity = new Dictionary<int, List<PetAttribute>>();

        static public Dictionary<int, List<PetAttribute>> GetAllGroupByidentity()
        {
            return DataList_identity;
        }

        static public List<PetAttribute> GetGroupByidentity(int key)
        {
            List<PetAttribute> value = null;
            if (DataList_identity.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PetAttribute.GetGroupByidentity({0}) not find!", key);
            return null;
        }


        // 灵兽ID
        public int identity { get; set; }

        // 名称
        public string name { get; set; }

        // 携带等级
        public int carry_level { get; set; }

        // 初始AI类型
        public int ai_type { get; set; }

        // 图标
        public string icon { get; set; }

        // 元神珠ID
        public int pet2item { get; set; }

        // 品质类型
        public int type { get; set; }

        // 是否变异
        public int variation { get; set; }

        // 五行属性
        public int fiveElements { get; set; }

        // 摄像机位置参数
        public float[] camPos { get; set; }

        // 广角大小
        public int camView { get; set; }

        // 模型旋转角度
        public int camRota { get; set; }

        // 放生奖励
        public int delReward { get; set; }

        // 勇敢性格权重
        public int brave_weight { get; set; }

        // 聪明性格权重
        public int clever_weight { get; set; }

        // 冷静性格权重
        public int chill_weight { get; set; }

        // 冲动性格权重
        public int impulse_weight { get; set; }

        // 顺从性格权重
        public int obedience_weight { get; set; }

        // 狂妄性格权重
        public int arrogant_weight { get; set; }

        // 成长率下限
        public float grow_rate_min { get; set; }

        // 成长率上限
        public float grow_rate_max { get; set; }

        // 力量资质下限
        public int grow_pow_min { get; set; }

        // 力量资质上限
        public int grow_pow_max { get; set; }

        // 智慧资质下限
        public int grow_intelligence_min { get; set; }

        // 智慧资质上限
        public int grow_intelligence_max { get; set; }

        // 根骨资质下限
        public int grow_bone_min { get; set; }

        // 根骨资质上限
        public int grow_bone_max { get; set; }

        // 体魄资质下限
        public int grow_bodies_min { get; set; }

        // 体魄资质上限
        public int grow_bodies_max { get; set; }

        // 敏捷资质下限
        public int grow_agile_min { get; set; }

        // 敏捷资质上限
        public int grow_agile_max { get; set; }

        // 身法资质下限
        public int grow_bodyposition_min { get; set; }

        // 身法资质上限
        public int grow_bodyposition_max { get; set; }

        // 初始力量
        public int init_power { get; set; }

        // 初始智慧
        public int init_intelligence { get; set; }

        // 初始根骨
        public int init_root_bone { get; set; }

        // 初始体魄
        public int init_bodies { get; set; }

        // 初始敏捷
        public int init_agile { get; set; }

        // 初始身法
        public int init_body_position { get; set; }

        // 力量加点方案
        public int power_slider_point { get; set; }

        // 智慧加点方案
        public int intelligence_slider_point { get; set; }

        // 根骨加点方案
        public int root_bone_slider_point { get; set; }

        // 体魄加点方案
        public int bodies_slider_point { get; set; }

        // 敏捷加点方案
        public int agile_slider_point { get; set; }

        // 身法加点方案
        public int body_slider_position_point { get; set; }

        // 洗炼材料ID
        public int wash_item_id { get; set; }

        // 洗炼材料数量
        public int wash_item_num { get; set; }

        // 变异几率
        public float variation_Intervall { get; set; }

        // 变异ID
        public int variation_id { get; set; }

        // 基础转化值
        public int init_transfrom { get; set; }

        // 绝技几率
        public float trickIntervall { get; set; }

        // 绝技ID
        public int trickSkillsID { get; set; }

        // 天赋技能几率
        public float talentSkill_Intervall { get; set; }

        // 天赋技能ID
        public int talentSKillsID { get; set; }

        // 必带普通技能
        public int defaultSkillId { get; set; }

        // 普通技能权重
        public int[] skillsIntervall { get; set; }

        // 普通技能ID
        public int[] skills { get; set; }

        // 战斗状态对白
        public string masterFight { get; set; }

        // 触发概率
        public float speakProb1 { get; set; }

        // 触发间隔
        public int speakinterval1 { get; set; }

        // 非战斗状态对白
        public string masterNoFight { get; set; }

        // 触发概率
        public float speakProb2 { get; set; }

        // 触发间隔
        public int speakinterval2 { get; set; }

        // 进入关卡对白
        public string masterChangeLevel { get; set; }

        // 触发概率
        public float speakProb3 { get; set; }

        // 主人生命值过低对白
        public string masterHP20P { get; set; }

        // 触发概率
        public float speakProb4 { get; set; }

        // 主人升级对白
        public string masterUpgrade { get; set; }

        // 触发概率
        public float speakProb5 { get; set; }

        // 图鉴描述
        public string des { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PetAttribute);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_identity.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PetAttribute);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PetAttribute> allDatas = new List<PetAttribute>();

            {
                string file = "Pet/PetAttribute.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int identity_index = reader.TryIndex("identity:group");
                int name_index = reader.GetIndex("name");
                int carry_level_index = reader.GetIndex("carry_level");
                int ai_type_index = reader.GetIndex("ai_type");
                int icon_index = reader.GetIndex("icon");
                int pet2item_index = reader.GetIndex("pet2item");
                int type_index = reader.GetIndex("type");
                int variation_index = reader.GetIndex("variation");
                int fiveElements_index = reader.GetIndex("fiveElements");
                int camPos_index = reader.GetIndex("camPos");
                int camView_index = reader.GetIndex("camView");
                int camRota_index = reader.GetIndex("camRota");
                int delReward_index = reader.GetIndex("delReward");
                int brave_weight_index = reader.GetIndex("brave_weight");
                int clever_weight_index = reader.GetIndex("clever_weight");
                int chill_weight_index = reader.GetIndex("chill_weight");
                int impulse_weight_index = reader.GetIndex("impulse_weight");
                int obedience_weight_index = reader.GetIndex("obedience_weight");
                int arrogant_weight_index = reader.GetIndex("arrogant_weight");
                int grow_rate_min_index = reader.GetIndex("grow_rate_min");
                int grow_rate_max_index = reader.GetIndex("grow_rate_max");
                int grow_pow_min_index = reader.GetIndex("grow_pow_min");
                int grow_pow_max_index = reader.GetIndex("grow_pow_max");
                int grow_intelligence_min_index = reader.GetIndex("grow_intelligence_min");
                int grow_intelligence_max_index = reader.GetIndex("grow_intelligence_max");
                int grow_bone_min_index = reader.GetIndex("grow_bone_min");
                int grow_bone_max_index = reader.GetIndex("grow_bone_max");
                int grow_bodies_min_index = reader.GetIndex("grow_bodies_min");
                int grow_bodies_max_index = reader.GetIndex("grow_bodies_max");
                int grow_agile_min_index = reader.GetIndex("grow_agile_min");
                int grow_agile_max_index = reader.GetIndex("grow_agile_max");
                int grow_bodyposition_min_index = reader.GetIndex("grow_bodyposition_min");
                int grow_bodyposition_max_index = reader.GetIndex("grow_bodyposition_max");
                int init_power_index = reader.GetIndex("init_power");
                int init_intelligence_index = reader.GetIndex("init_intelligence");
                int init_root_bone_index = reader.GetIndex("init_root_bone");
                int init_bodies_index = reader.GetIndex("init_bodies");
                int init_agile_index = reader.GetIndex("init_agile");
                int init_body_position_index = reader.GetIndex("init_body_position");
                int power_slider_point_index = reader.GetIndex("power_slider_point");
                int intelligence_slider_point_index = reader.GetIndex("intelligence_slider_point");
                int root_bone_slider_point_index = reader.GetIndex("root_bone_slider_point");
                int bodies_slider_point_index = reader.GetIndex("bodies_slider_point");
                int agile_slider_point_index = reader.GetIndex("agile_slider_point");
                int body_slider_position_point_index = reader.GetIndex("body_slider_position_point");
                int wash_item_id_index = reader.GetIndex("wash_item_id");
                int wash_item_num_index = reader.GetIndex("wash_item_num");
                int variation_Intervall_index = reader.GetIndex("variation_Intervall");
                int variation_id_index = reader.GetIndex("variation_id");
                int init_transfrom_index = reader.GetIndex("init_transfrom");
                int trickIntervall_index = reader.GetIndex("trickIntervall");
                int trickSkillsID_index = reader.GetIndex("trickSkillsID");
                int talentSkill_Intervall_index = reader.GetIndex("talentSkill_Intervall");
                int talentSKillsID_index = reader.GetIndex("talentSKillsID");
                int defaultSkillId_index = reader.GetIndex("defaultSkillId");
                int skillsIntervall_index = reader.GetIndex("skillsIntervall");
                int skills_index = reader.GetIndex("skills");
                int masterFight_index = reader.GetIndex("masterFight");
                int speakProb1_index = reader.GetIndex("speakProb1");
                int speakinterval1_index = reader.GetIndex("speakinterval1");
                int masterNoFight_index = reader.GetIndex("masterNoFight");
                int speakProb2_index = reader.GetIndex("speakProb2");
                int speakinterval2_index = reader.GetIndex("speakinterval2");
                int masterChangeLevel_index = reader.GetIndex("masterChangeLevel");
                int speakProb3_index = reader.GetIndex("speakProb3");
                int masterHP20P_index = reader.GetIndex("masterHP20P");
                int speakProb4_index = reader.GetIndex("speakProb4");
                int masterUpgrade_index = reader.GetIndex("masterUpgrade");
                int speakProb5_index = reader.GetIndex("speakProb5");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PetAttribute data = new PetAttribute();
						data.identity = reader.getInt(i, identity_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.carry_level = reader.getInt(i, carry_level_index, 0);         
						data.ai_type = reader.getInt(i, ai_type_index, 0);         
						data.icon = reader.getStr(i, icon_index);         
						data.pet2item = reader.getInt(i, pet2item_index, 0);         
						data.type = reader.getInt(i, type_index, 0);         
						data.variation = reader.getInt(i, variation_index, 0);         
						data.fiveElements = reader.getInt(i, fiveElements_index, 0);         
						data.camPos = reader.getFloats(i, camPos_index, ';');         
						data.camView = reader.getInt(i, camView_index, 0);         
						data.camRota = reader.getInt(i, camRota_index, 0);         
						data.delReward = reader.getInt(i, delReward_index, 0);         
						data.brave_weight = reader.getInt(i, brave_weight_index, 0);         
						data.clever_weight = reader.getInt(i, clever_weight_index, 0);         
						data.chill_weight = reader.getInt(i, chill_weight_index, 0);         
						data.impulse_weight = reader.getInt(i, impulse_weight_index, 0);         
						data.obedience_weight = reader.getInt(i, obedience_weight_index, 0);         
						data.arrogant_weight = reader.getInt(i, arrogant_weight_index, 0);         
						data.grow_rate_min = reader.getFloat(i, grow_rate_min_index, 0f);         
						data.grow_rate_max = reader.getFloat(i, grow_rate_max_index, 0f);         
						data.grow_pow_min = reader.getInt(i, grow_pow_min_index, 0);         
						data.grow_pow_max = reader.getInt(i, grow_pow_max_index, 0);         
						data.grow_intelligence_min = reader.getInt(i, grow_intelligence_min_index, 0);         
						data.grow_intelligence_max = reader.getInt(i, grow_intelligence_max_index, 0);         
						data.grow_bone_min = reader.getInt(i, grow_bone_min_index, 0);         
						data.grow_bone_max = reader.getInt(i, grow_bone_max_index, 0);         
						data.grow_bodies_min = reader.getInt(i, grow_bodies_min_index, 0);         
						data.grow_bodies_max = reader.getInt(i, grow_bodies_max_index, 0);         
						data.grow_agile_min = reader.getInt(i, grow_agile_min_index, 0);         
						data.grow_agile_max = reader.getInt(i, grow_agile_max_index, 0);         
						data.grow_bodyposition_min = reader.getInt(i, grow_bodyposition_min_index, 0);         
						data.grow_bodyposition_max = reader.getInt(i, grow_bodyposition_max_index, 0);         
						data.init_power = reader.getInt(i, init_power_index, 0);         
						data.init_intelligence = reader.getInt(i, init_intelligence_index, 0);         
						data.init_root_bone = reader.getInt(i, init_root_bone_index, 0);         
						data.init_bodies = reader.getInt(i, init_bodies_index, 0);         
						data.init_agile = reader.getInt(i, init_agile_index, 0);         
						data.init_body_position = reader.getInt(i, init_body_position_index, 0);         
						data.power_slider_point = reader.getInt(i, power_slider_point_index, 0);         
						data.intelligence_slider_point = reader.getInt(i, intelligence_slider_point_index, 0);         
						data.root_bone_slider_point = reader.getInt(i, root_bone_slider_point_index, 0);         
						data.bodies_slider_point = reader.getInt(i, bodies_slider_point_index, 0);         
						data.agile_slider_point = reader.getInt(i, agile_slider_point_index, 0);         
						data.body_slider_position_point = reader.getInt(i, body_slider_position_point_index, 0);         
						data.wash_item_id = reader.getInt(i, wash_item_id_index, 0);         
						data.wash_item_num = reader.getInt(i, wash_item_num_index, 0);         
						data.variation_Intervall = reader.getFloat(i, variation_Intervall_index, 0f);         
						data.variation_id = reader.getInt(i, variation_id_index, 0);         
						data.init_transfrom = reader.getInt(i, init_transfrom_index, 0);         
						data.trickIntervall = reader.getFloat(i, trickIntervall_index, 0f);         
						data.trickSkillsID = reader.getInt(i, trickSkillsID_index, 0);         
						data.talentSkill_Intervall = reader.getFloat(i, talentSkill_Intervall_index, 0f);         
						data.talentSKillsID = reader.getInt(i, talentSKillsID_index, 0);         
						data.defaultSkillId = reader.getInt(i, defaultSkillId_index, 0);         
						data.skillsIntervall = reader.getInts(i, skillsIntervall_index, ';');         
						data.skills = reader.getInts(i, skills_index, ';');         
						data.masterFight = reader.getStr(i, masterFight_index);         
						data.speakProb1 = reader.getFloat(i, speakProb1_index, 0f);         
						data.speakinterval1 = reader.getInt(i, speakinterval1_index, 0);         
						data.masterNoFight = reader.getStr(i, masterNoFight_index);         
						data.speakProb2 = reader.getFloat(i, speakProb2_index, 0f);         
						data.speakinterval2 = reader.getInt(i, speakinterval2_index, 0);         
						data.masterChangeLevel = reader.getStr(i, masterChangeLevel_index);         
						data.speakProb3 = reader.getFloat(i, speakProb3_index, 0f);         
						data.masterHP20P = reader.getStr(i, masterHP20P_index);         
						data.speakProb4 = reader.getFloat(i, speakProb4_index, 0f);         
						data.masterUpgrade = reader.getStr(i, masterUpgrade_index);         
						data.speakProb5 = reader.getFloat(i, speakProb5_index, 0f);         
						data.des = reader.getStr(i, des_index);         
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
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<PetAttribute> l = null;
                    if (!DataList_identity.TryGetValue(data.identity, out l))
                    {
                        l = new List<PetAttribute>();
                        DataList_identity[data.identity] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(PetAttribute);
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



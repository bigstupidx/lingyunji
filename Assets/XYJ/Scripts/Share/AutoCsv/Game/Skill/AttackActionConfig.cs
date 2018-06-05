// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class AttackActionConfig 
    {
        static Dictionary<string, AttackActionConfig> DataList = new Dictionary<string, AttackActionConfig>();

        static public Dictionary<string, AttackActionConfig> GetAll()
        {
            return DataList;
        }

        static public AttackActionConfig Get(string key)
        {
            AttackActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("AttackActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 命中状态
        public string stateName { get; set; }

        // 状态参数
        public float[] statePara { get; set; }

        // 命中action
        public string[] hitActionStr { get; set; }

        // 附加命中率
        public float mingzhongExtra { get; set; }

        // 附加暴击率
        public float baojiRateExtra { get; set; }

        // 无视防御类技能
        public bool ignoreDefenseSkill { get; set; }

        // 吸血系数
        public float vampireFix { get; set; }

        // 护体伤害
        public int hutiDamage { get; set; }

        // 伤害类型
        public bool isMagicDamage { get; set; }

        // 受击特效
        public string hitEffect { get; set; }

        // 受击方向
        public string hitEffectAngle { get; set; }

        // 伤害系数
        public float damageFix { get; set; }

        // 附加伤害系数
        public float extraDamageFix { get; set; }

        // 附加伤害
        public float extraDamage { get; set; }

        // 受击不改变朝向
        public bool stateChangeToward { get; set; }

        // 仅命中第一次执行aciton
        public string[] hitActionStrOnce { get; set; }

        // 仇恨系数
        public float haterdMul { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(AttackActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(AttackActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<AttackActionConfig> allDatas = new List<AttackActionConfig>();

            {
                string file = "Skill/AttackActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int stateName_index = reader.GetIndex("stateName");
                int statePara_index = reader.GetIndex("statePara");
                int hitActionStr_index = reader.GetIndex("hitActionStr");
                int mingzhongExtra_index = reader.GetIndex("mingzhongExtra");
                int baojiRateExtra_index = reader.GetIndex("baojiRateExtra");
                int ignoreDefenseSkill_index = reader.GetIndex("ignoreDefenseSkill");
                int vampireFix_index = reader.GetIndex("vampireFix");
                int hutiDamage_index = reader.GetIndex("hutiDamage");
                int isMagicDamage_index = reader.GetIndex("isMagicDamage");
                int hitEffect_index = reader.GetIndex("hitEffect");
                int hitEffectAngle_index = reader.GetIndex("hitEffectAngle");
                int damageFix_index = reader.GetIndex("damageFix");
                int extraDamageFix_index = reader.GetIndex("extraDamageFix");
                int extraDamage_index = reader.GetIndex("extraDamage");
                int stateChangeToward_index = reader.TryIndex("stateChangeToward");
                int hitActionStrOnce_index = reader.TryIndex("hitActionStrOnce");
                int haterdMul_index = reader.TryIndex("haterdMul");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AttackActionConfig data = new AttackActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.stateName = reader.getStr(i, stateName_index);         
						data.statePara = reader.getFloats(i, statePara_index, ';');         
						data.hitActionStr = reader.getStrs(i, hitActionStr_index, ';');         
						data.mingzhongExtra = reader.getFloat(i, mingzhongExtra_index, 0f);         
						data.baojiRateExtra = reader.getFloat(i, baojiRateExtra_index, 0f);         
						data.ignoreDefenseSkill = reader.getBool(i, ignoreDefenseSkill_index, false);         
						data.vampireFix = reader.getFloat(i, vampireFix_index, 0f);         
						data.hutiDamage = reader.getInt(i, hutiDamage_index, 0);         
						data.isMagicDamage = reader.getBool(i, isMagicDamage_index, false);         
						data.hitEffect = reader.getStr(i, hitEffect_index);         
						data.hitEffectAngle = reader.getStr(i, hitEffectAngle_index);         
						data.damageFix = reader.getFloat(i, damageFix_index, 1.0f);         
						data.extraDamageFix = reader.getFloat(i, extraDamageFix_index, 0f);         
						data.extraDamage = reader.getFloat(i, extraDamage_index, 0f);         
						data.stateChangeToward = reader.getBool(i, stateChangeToward_index, false);         
						data.hitActionStrOnce = reader.getStrs(i, hitActionStrOnce_index, ';');         
						data.haterdMul = reader.getFloat(i, haterdMul_index, 1.0f);         
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
                string file = "Skill/AttackActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int stateName_index = reader.GetIndex("stateName");
                int statePara_index = reader.GetIndex("statePara");
                int hitActionStr_index = reader.TryIndex("hitActionStr");
                int mingzhongExtra_index = reader.TryIndex("mingzhongExtra");
                int baojiRateExtra_index = reader.TryIndex("baojiRateExtra");
                int ignoreDefenseSkill_index = reader.TryIndex("ignoreDefenseSkill");
                int vampireFix_index = reader.TryIndex("vampireFix");
                int hutiDamage_index = reader.TryIndex("hutiDamage");
                int isMagicDamage_index = reader.TryIndex("isMagicDamage");
                int hitEffect_index = reader.GetIndex("hitEffect");
                int hitEffectAngle_index = reader.GetIndex("hitEffectAngle");
                int damageFix_index = reader.GetIndex("damageFix");
                int extraDamageFix_index = reader.GetIndex("extraDamageFix");
                int extraDamage_index = reader.GetIndex("extraDamage");
                int stateChangeToward_index = reader.TryIndex("stateChangeToward");
                int hitActionStrOnce_index = reader.TryIndex("hitActionStrOnce");
                int haterdMul_index = reader.TryIndex("haterdMul");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AttackActionConfig data = new AttackActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.stateName = reader.getStr(i, stateName_index);         
						data.statePara = reader.getFloats(i, statePara_index, ';');         
						data.hitActionStr = reader.getStrs(i, hitActionStr_index, ';');         
						data.mingzhongExtra = reader.getFloat(i, mingzhongExtra_index, 0f);         
						data.baojiRateExtra = reader.getFloat(i, baojiRateExtra_index, 0f);         
						data.ignoreDefenseSkill = reader.getBool(i, ignoreDefenseSkill_index, false);         
						data.vampireFix = reader.getFloat(i, vampireFix_index, 0f);         
						data.hutiDamage = reader.getInt(i, hutiDamage_index, 0);         
						data.isMagicDamage = reader.getBool(i, isMagicDamage_index, false);         
						data.hitEffect = reader.getStr(i, hitEffect_index);         
						data.hitEffectAngle = reader.getStr(i, hitEffectAngle_index);         
						data.damageFix = reader.getFloat(i, damageFix_index, 1.0f);         
						data.extraDamageFix = reader.getFloat(i, extraDamageFix_index, 0f);         
						data.extraDamage = reader.getFloat(i, extraDamage_index, 0f);         
						data.stateChangeToward = reader.getBool(i, stateChangeToward_index, false);         
						data.hitActionStrOnce = reader.getStrs(i, hitActionStrOnce_index, ';');         
						data.haterdMul = reader.getFloat(i, haterdMul_index, 1.0f);         
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
                string file = "Skill/AttackActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int stateName_index = reader.GetIndex("stateName");
                int statePara_index = reader.GetIndex("statePara");
                int hitActionStr_index = reader.GetIndex("hitActionStr");
                int mingzhongExtra_index = reader.GetIndex("mingzhongExtra");
                int baojiRateExtra_index = reader.GetIndex("baojiRateExtra");
                int ignoreDefenseSkill_index = reader.GetIndex("ignoreDefenseSkill");
                int vampireFix_index = reader.GetIndex("vampireFix");
                int hutiDamage_index = reader.GetIndex("hutiDamage");
                int isMagicDamage_index = reader.GetIndex("isMagicDamage");
                int hitEffect_index = reader.GetIndex("hitEffect");
                int hitEffectAngle_index = reader.GetIndex("hitEffectAngle");
                int damageFix_index = reader.GetIndex("damageFix");
                int extraDamageFix_index = reader.GetIndex("extraDamageFix");
                int extraDamage_index = reader.GetIndex("extraDamage");
                int stateChangeToward_index = reader.GetIndex("stateChangeToward");
                int hitActionStrOnce_index = reader.GetIndex("hitActionStrOnce");
                int haterdMul_index = reader.GetIndex("haterdMul");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AttackActionConfig data = new AttackActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.stateName = reader.getStr(i, stateName_index);         
						data.statePara = reader.getFloats(i, statePara_index, ';');         
						data.hitActionStr = reader.getStrs(i, hitActionStr_index, ';');         
						data.mingzhongExtra = reader.getFloat(i, mingzhongExtra_index, 0f);         
						data.baojiRateExtra = reader.getFloat(i, baojiRateExtra_index, 0f);         
						data.ignoreDefenseSkill = reader.getBool(i, ignoreDefenseSkill_index, false);         
						data.vampireFix = reader.getFloat(i, vampireFix_index, 0f);         
						data.hutiDamage = reader.getInt(i, hutiDamage_index, 0);         
						data.isMagicDamage = reader.getBool(i, isMagicDamage_index, false);         
						data.hitEffect = reader.getStr(i, hitEffect_index);         
						data.hitEffectAngle = reader.getStr(i, hitEffectAngle_index);         
						data.damageFix = reader.getFloat(i, damageFix_index, 1.0f);         
						data.extraDamageFix = reader.getFloat(i, extraDamageFix_index, 0f);         
						data.extraDamage = reader.getFloat(i, extraDamage_index, 0f);         
						data.stateChangeToward = reader.getBool(i, stateChangeToward_index, false);         
						data.hitActionStrOnce = reader.getStrs(i, hitActionStrOnce_index, ';');         
						data.haterdMul = reader.getFloat(i, haterdMul_index, 1.0f);         
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
                    CsvCommon.Log.Error("AttackActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(AttackActionConfig);
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



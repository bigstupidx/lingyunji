// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ConditionActionConfig 
    {
        static Dictionary<string, ConditionActionConfig> DataList = new Dictionary<string, ConditionActionConfig>();

        static public Dictionary<string, ConditionActionConfig> GetAll()
        {
            return DataList;
        }

        static public ConditionActionConfig Get(string key)
        {
            ConditionActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ConditionActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 目标距离
        public float[] targetDistance { get; set; }

        // 概率
        public float rate { get; set; }

        // 作用单位
        public EffectTarget targetType { get; set; }

        // 血量百分比
        public float[] hpMul { get; set; }

        // 携带buff
        public int buff { get; set; }

        // buff层数
        public int buffcnt { get; set; }

        // 条件成立action
        public string[] sucessActionStr { get; set; }

        // 条件不成立action
        public string[] failActopmStr { get; set; }

        // 描述
        public string[] desc { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ConditionActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ConditionActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ConditionActionConfig> allDatas = new List<ConditionActionConfig>();

            {
                string file = "Skill/ConditionActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetDistance_index = reader.GetIndex("targetDistance");
                int rate_index = reader.GetIndex("rate");
                int targetType_index = reader.GetIndex("targetType");
                int hpMul_index = reader.GetIndex("hpMul");
                int buff_index = reader.GetIndex("buff");
                int buffcnt_index = reader.GetIndex("buffcnt");
                int sucessActionStr_index = reader.GetIndex("sucessActionStr");
                int failActopmStr_index = reader.GetIndex("failActopmStr");
                int desc_index = reader.GetIndex("desc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ConditionActionConfig data = new ConditionActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetDistance = reader.getFloats(i, targetDistance_index, ';');         
						data.rate = reader.getFloat(i, rate_index, 1f);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.hpMul = reader.getFloats(i, hpMul_index, ';');         
						data.buff = reader.getInt(i, buff_index, 0);         
						data.buffcnt = reader.getInt(i, buffcnt_index, 0);         
						data.sucessActionStr = reader.getStrs(i, sucessActionStr_index, ';');         
						data.failActopmStr = reader.getStrs(i, failActopmStr_index, ';');         
						data.desc = reader.getStrs(i, desc_index, ';');         
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
                    CsvCommon.Log.Error("ConditionActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ConditionActionConfig);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillConditionConfig 
    {
        static Dictionary<int, SkillConditionConfig> DataList = new Dictionary<int, SkillConditionConfig>();

        static public Dictionary<int, SkillConditionConfig> GetAll()
        {
            return DataList;
        }

        static public SkillConditionConfig Get(int key)
        {
            SkillConditionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillConditionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public int id { get; set; }

        // 作用单位
        public EffectTarget effectTarget { get; set; }

        // 状态
        public string[] state { get; set; }

        // 技能暴击
        public int baojiid { get; set; }

        // 格挡成功
        public bool block { get; set; }

        // 描述
        public string[] des { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillConditionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillConditionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillConditionConfig> allDatas = new List<SkillConditionConfig>();

            {
                string file = "Skill/SkillConditionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int effectTarget_index = reader.GetIndex("effectTarget");
                int state_index = reader.GetIndex("state");
                int baojiid_index = reader.GetIndex("baojiid");
                int block_index = reader.GetIndex("block");
                int des_index = reader.TryIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillConditionConfig data = new SkillConditionConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.effectTarget = ((EffectTarget)reader.getInt(i, effectTarget_index, 0));         
						data.state = reader.getStrs(i, state_index, ';');         
						data.baojiid = reader.getInt(i, baojiid_index, 0);         
						data.block = reader.getBool(i, block_index, false);         
						data.des = reader.getStrs(i, des_index, ';');         
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
                string file = "Skill/SkillConditionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int effectTarget_index = reader.GetIndex("effectTarget");
                int state_index = reader.GetIndex("state");
                int baojiid_index = reader.GetIndex("baojiid");
                int block_index = reader.GetIndex("block");
                int des_index = reader.TryIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillConditionConfig data = new SkillConditionConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.effectTarget = ((EffectTarget)reader.getInt(i, effectTarget_index, 0));         
						data.state = reader.getStrs(i, state_index, ';');         
						data.baojiid = reader.getInt(i, baojiid_index, 0);         
						data.block = reader.getBool(i, block_index, false);         
						data.des = reader.getStrs(i, des_index, ';');         
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
                string file = "Skill/SkillConditionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int effectTarget_index = reader.GetIndex("effectTarget");
                int state_index = reader.GetIndex("state");
                int baojiid_index = reader.GetIndex("baojiid");
                int block_index = reader.GetIndex("block");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillConditionConfig data = new SkillConditionConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.effectTarget = ((EffectTarget)reader.getInt(i, effectTarget_index, 0));         
						data.state = reader.getStrs(i, state_index, ';');         
						data.baojiid = reader.getInt(i, baojiid_index, 0);         
						data.block = reader.getBool(i, block_index, false);         
						data.des = reader.getStrs(i, des_index, ';');         
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
                    CsvCommon.Log.Error("SkillConditionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillConditionConfig);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class AddBuffActionConfig 
    {
        static Dictionary<string, AddBuffActionConfig> DataList = new Dictionary<string, AddBuffActionConfig>();

        static public Dictionary<string, AddBuffActionConfig> GetAll()
        {
            return DataList;
        }

        static public AddBuffActionConfig Get(string key)
        {
            AddBuffActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("AddBuffActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 作用单位
        public EffectTarget targetType { get; set; }

        // 概率
        public int rate { get; set; }

        // buff编号
        public int[] buffid { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(AddBuffActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(AddBuffActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<AddBuffActionConfig> allDatas = new List<AddBuffActionConfig>();

            {
                string file = "Skill/AddBuffActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int rate_index = reader.GetIndex("rate");
                int buffid_index = reader.GetIndex("buffid");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AddBuffActionConfig data = new AddBuffActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.rate = reader.getInt(i, rate_index, 0);         
						data.buffid = reader.getInts(i, buffid_index, ';');         
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
                string file = "Skill/AddBuffActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int rate_index = reader.GetIndex("rate");
                int buffid_index = reader.GetIndex("buffid");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AddBuffActionConfig data = new AddBuffActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.rate = reader.getInt(i, rate_index, 0);         
						data.buffid = reader.getInts(i, buffid_index, ';');         
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
                string file = "Skill/AddBuffActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int rate_index = reader.GetIndex("rate");
                int buffid_index = reader.GetIndex("buffid");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AddBuffActionConfig data = new AddBuffActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.rate = reader.getInt(i, rate_index, 0);         
						data.buffid = reader.getInts(i, buffid_index, ';');         
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
                    CsvCommon.Log.Error("AddBuffActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(AddBuffActionConfig);
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



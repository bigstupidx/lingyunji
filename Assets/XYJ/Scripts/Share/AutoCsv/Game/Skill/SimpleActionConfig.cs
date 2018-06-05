// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SimpleActionConfig 
    {
        static Dictionary<string, SimpleActionConfig> DataList = new Dictionary<string, SimpleActionConfig>();

        static public Dictionary<string, SimpleActionConfig> GetAll()
        {
            return DataList;
        }

        static public SimpleActionConfig Get(string key)
        {
            SimpleActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SimpleActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 作用单位
        public EffectTarget targetType { get; set; }

        // 类型
        public string actionType { get; set; }

        // 类型参数
        public string[] typeParas { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SimpleActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SimpleActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SimpleActionConfig> allDatas = new List<SimpleActionConfig>();

            {
                string file = "Skill/SimpleActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int actionType_index = reader.GetIndex("actionType");
                int typeParas_index = reader.GetIndex("typeParas");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SimpleActionConfig data = new SimpleActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.actionType = reader.getStr(i, actionType_index);         
						data.typeParas = reader.getStrs(i, typeParas_index, ';');         
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
                string file = "Skill/SimpleActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int actionType_index = reader.GetIndex("actionType");
                int typeParas_index = reader.GetIndex("typeParas");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SimpleActionConfig data = new SimpleActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.actionType = reader.getStr(i, actionType_index);         
						data.typeParas = reader.getStrs(i, typeParas_index, ';');         
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
                    CsvCommon.Log.Error("SimpleActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SimpleActionConfig);
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



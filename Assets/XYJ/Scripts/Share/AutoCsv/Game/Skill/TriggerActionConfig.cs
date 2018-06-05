// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TriggerActionConfig 
    {
        static Dictionary<string, TriggerActionConfig> DataList = new Dictionary<string, TriggerActionConfig>();

        static public Dictionary<string, TriggerActionConfig> GetAll()
        {
            return DataList;
        }

        static public TriggerActionConfig Get(string key)
        {
            TriggerActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TriggerActionConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public string id { get; set; }

        // 造成伤害触发
        public bool attackTrigger { get; set; }

        // 受到伤害触发
        public bool behitTrigger { get; set; }

        // 触发几率
        public float triggerRate { get; set; }

        // 触发action
        public string[] actionStrs { get; set; }

        // 触发冷却
        public float cd { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TriggerActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TriggerActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TriggerActionConfig> allDatas = new List<TriggerActionConfig>();

            {
                string file = "Skill/TriggerActionConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int attackTrigger_index = reader.GetIndex("attackTrigger");
                int behitTrigger_index = reader.GetIndex("behitTrigger");
                int triggerRate_index = reader.GetIndex("triggerRate");
                int actionStrs_index = reader.GetIndex("actionStrs");
                int cd_index = reader.GetIndex("cd");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TriggerActionConfig data = new TriggerActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.attackTrigger = reader.getBool(i, attackTrigger_index, false);         
						data.behitTrigger = reader.getBool(i, behitTrigger_index, false);         
						data.triggerRate = reader.getFloat(i, triggerRate_index, 0f);         
						data.actionStrs = reader.getStrs(i, actionStrs_index, ';');         
						data.cd = reader.getFloat(i, cd_index, 0f);         
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
                    CsvCommon.Log.Error("TriggerActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TriggerActionConfig);
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



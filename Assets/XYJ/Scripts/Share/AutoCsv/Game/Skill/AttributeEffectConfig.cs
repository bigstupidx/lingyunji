// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class AttributeEffectConfig 
    {
        static Dictionary<string, AttributeEffectConfig> DataList = new Dictionary<string, AttributeEffectConfig>();

        static public Dictionary<string, AttributeEffectConfig> GetAll()
        {
            return DataList;
        }

        static public AttributeEffectConfig Get(string key)
        {
            AttributeEffectConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("AttributeEffectConfig.Get({0}) not find!", key);
            return null;
        }



        // 属性效果ID
        public string id { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(AttributeEffectConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(AttributeEffectConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<AttributeEffectConfig> allDatas = new List<AttributeEffectConfig>();

            {
                string file = "Skill/AttributeEffectConfig@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AttributeEffectConfig data = new AttributeEffectConfig();
						data.id = reader.getStr(i, id_index);         
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
                    CsvCommon.Log.Error("AttributeEffectConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(AttributeEffectConfig);
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



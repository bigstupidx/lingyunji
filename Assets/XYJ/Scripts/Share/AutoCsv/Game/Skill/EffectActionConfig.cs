// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EffectActionConfig 
    {
        static Dictionary<string, EffectActionConfig> DataList = new Dictionary<string, EffectActionConfig>();

        static public Dictionary<string, EffectActionConfig> GetAll()
        {
            return DataList;
        }

        static public EffectActionConfig Get(string key)
        {
            EffectActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("EffectActionConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public string id { get; set; }

        // 作用单位
        public EffectTarget targetType { get; set; }

        // 特效id
        public string fxid { get; set; }

        // 消失类型
        public DestroyType destroyType { get; set; }

        // 持续帧数
        public int lastFrame { get; set; }

        // 预警search
        public string warningSearchId { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EffectActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EffectActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EffectActionConfig> allDatas = new List<EffectActionConfig>();

            {
                string file = "Skill/EffectActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int fxid_index = reader.GetIndex("fxid");
                int destroyType_index = reader.GetIndex("destroyType");
                int lastFrame_index = reader.GetIndex("lastFrame");
                int warningSearchId_index = reader.GetIndex("warningSearchId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EffectActionConfig data = new EffectActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.fxid = reader.getStr(i, fxid_index);         
						data.destroyType = ((DestroyType)reader.getInt(i, destroyType_index, 0));         
						data.lastFrame = reader.getInt(i, lastFrame_index, 0);         
						data.warningSearchId = reader.getStr(i, warningSearchId_index);         
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
                string file = "Skill/EffectActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int fxid_index = reader.GetIndex("fxid");
                int destroyType_index = reader.GetIndex("destroyType");
                int lastFrame_index = reader.GetIndex("lastFrame");
                int warningSearchId_index = reader.GetIndex("warningSearchId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EffectActionConfig data = new EffectActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.fxid = reader.getStr(i, fxid_index);         
						data.destroyType = ((DestroyType)reader.getInt(i, destroyType_index, 0));         
						data.lastFrame = reader.getInt(i, lastFrame_index, 0);         
						data.warningSearchId = reader.getStr(i, warningSearchId_index);         
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
                string file = "Skill/EffectActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetType_index = reader.GetIndex("targetType");
                int fxid_index = reader.GetIndex("fxid");
                int destroyType_index = reader.GetIndex("destroyType");
                int lastFrame_index = reader.GetIndex("lastFrame");
                int warningSearchId_index = reader.GetIndex("warningSearchId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EffectActionConfig data = new EffectActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetType = ((EffectTarget)reader.getInt(i, targetType_index, 0));         
						data.fxid = reader.getStr(i, fxid_index);         
						data.destroyType = ((DestroyType)reader.getInt(i, destroyType_index, 0));         
						data.lastFrame = reader.getInt(i, lastFrame_index, 0);         
						data.warningSearchId = reader.getStr(i, warningSearchId_index);         
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
                    CsvCommon.Log.Error("EffectActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(EffectActionConfig);
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



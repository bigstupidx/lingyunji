// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SummonActionConfig 
    {
        static Dictionary<string, SummonActionConfig> DataList = new Dictionary<string, SummonActionConfig>();

        static public Dictionary<string, SummonActionConfig> GetAll()
        {
            return DataList;
        }

        static public SummonActionConfig Get(string key)
        {
            SummonActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SummonActionConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public string id { get; set; }

        // 作用位置
        public PosType posType { get; set; }

        // 召唤物出生点集合
        public float[] bornPos { get; set; }

        // 召唤物id
        public int[] objIds { get; set; }

        // 召唤物个数
        public int objCnt { get; set; }

        // 召唤物存活时间
        public float lifeTime { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SummonActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SummonActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SummonActionConfig> allDatas = new List<SummonActionConfig>();

            {
                string file = "Skill/SummonActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int posType_index = reader.GetIndex("posType");
                int bornPos_index = reader.GetIndex("bornPos");
                int objIds_index = reader.GetIndex("objIds");
                int objCnt_index = reader.GetIndex("objCnt");
                int lifeTime_index = reader.GetIndex("lifeTime");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SummonActionConfig data = new SummonActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.posType = ((PosType)reader.getInt(i, posType_index, 0));         
						data.bornPos = reader.getFloats(i, bornPos_index, ';');         
						data.objIds = reader.getInts(i, objIds_index, ';');         
						data.objCnt = reader.getInt(i, objCnt_index, 0);         
						data.lifeTime = reader.getFloat(i, lifeTime_index, 0f);         
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
                    CsvCommon.Log.Error("SummonActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SummonActionConfig);
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



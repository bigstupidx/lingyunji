// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class NpcSpawn 
    {
        static Dictionary<int, NpcSpawn> DataList = new Dictionary<int, NpcSpawn>();

        static public Dictionary<int, NpcSpawn> GetAll()
        {
            return DataList;
        }

        static public NpcSpawn Get(int key)
        {
            NpcSpawn value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("NpcSpawn.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 场景位置
        public LevelPos levelPos { get; set; }

        // 怪物ID
        public int[] monsterId { get; set; }

        // 位置选取方式
        public SelectPosType selectPosType { get; set; }

        // 怪物选取方式
        public SelectNpcType selectNpcType { get; set; }

        // 怪物数量下限
        public int floorLimit { get; set; }

        // 怪物数量上限
        public int upperLimit { get; set; }

        // 删除时间
        public float deleteTime { get; set; }

        // 刷新周期
        public float periodTime { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(NpcSpawn);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(NpcSpawn);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<NpcSpawn> allDatas = new List<NpcSpawn>();

            {
                string file = "Level/NpcSpawn.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int levelPos_index = reader.GetIndex("levelPos");
                int monsterId_index = reader.GetIndex("monsterId");
                int selectPosType_index = reader.GetIndex("selectPosType");
                int selectNpcType_index = reader.GetIndex("selectNpcType");
                int floorLimit_index = reader.GetIndex("floorLimit");
                int upperLimit_index = reader.GetIndex("upperLimit");
                int deleteTime_index = reader.GetIndex("deleteTime");
                int periodTime_index = reader.GetIndex("periodTime");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        NpcSpawn data = new NpcSpawn();
						data.id = reader.getInt(i, id_index, 0);         
						data.levelPos = LevelPos.InitConfig(reader.getStr(i, levelPos_index));         
						data.monsterId = reader.getInts(i, monsterId_index, ';');         
						data.selectPosType = ((SelectPosType)reader.getInt(i, selectPosType_index, 0));         
						data.selectNpcType = ((SelectNpcType)reader.getInt(i, selectNpcType_index, 0));         
						data.floorLimit = reader.getInt(i, floorLimit_index, 0);         
						data.upperLimit = reader.getInt(i, upperLimit_index, 0);         
						data.deleteTime = reader.getFloat(i, deleteTime_index, 0f);         
						data.periodTime = reader.getFloat(i, periodTime_index, 0f);         
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
                    CsvCommon.Log.Error("NpcSpawn.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(NpcSpawn);
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



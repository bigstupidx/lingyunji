// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RefinePropertyTable 
    {
        static List<RefinePropertyTable> DataList = new List<RefinePropertyTable>();
        static public List<RefinePropertyTable> GetAll()
        {
            return DataList;
        }

        static public RefinePropertyTable Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].type == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("RefinePropertyTable.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].type == key)
                    return i;
            }
            return -1;
        }



        // 炼化类型
        public int type { get; set; }

        // 炼化名称
        public string name { get; set; }

        // 消耗材料
        public int materialCostId { get; set; }

        // 材料数量
        public int materialCostCount { get; set; }

        // 消耗银贝
        public int copperCostCount { get; set; }

        // 属性数量权重
        public ProbabilityTable entryRate { get; set; }

        // 属性品质权重
        public ProbabilityTable qualityWeight { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RefinePropertyTable);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RefinePropertyTable);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RefinePropertyTable> allDatas = new List<RefinePropertyTable>();

            {
                string file = "Item/RefinePropertyTable.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int type_index = reader.GetIndex("type");
                int name_index = reader.GetIndex("name");
                int materialCostId_index = reader.GetIndex("materialCostId");
                int materialCostCount_index = reader.GetIndex("materialCostCount");
                int copperCostCount_index = reader.GetIndex("copperCostCount");
                int entryRate_index = reader.GetIndex("entryRate");
                int qualityWeight_index = reader.GetIndex("qualityWeight");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RefinePropertyTable data = new RefinePropertyTable();
						data.type = reader.getInt(i, type_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.materialCostId = reader.getInt(i, materialCostId_index, 0);         
						data.materialCostCount = reader.getInt(i, materialCostCount_index, 0);         
						data.copperCostCount = reader.getInt(i, copperCostCount_index, 0);         
						data.entryRate = ProbabilityTable.InitConfig(reader.getStr(i, entryRate_index));         
						data.qualityWeight = ProbabilityTable.InitConfig(reader.getStr(i, qualityWeight_index));         
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
            
            DataList = allDatas;

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RefinePropertyTable);
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



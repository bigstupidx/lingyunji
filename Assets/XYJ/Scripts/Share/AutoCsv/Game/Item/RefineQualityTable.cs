// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RefineQualityTable 
    {
        static List<RefineQualityTable> DataList = new List<RefineQualityTable>();
        static public List<RefineQualityTable> GetAll()
        {
            return DataList;
        }

        static public RefineQualityTable Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].qualityId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("RefineQualityTable.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].qualityId == key)
                    return i;
            }
            return -1;
        }



        // 品质ID
        public int qualityId { get; set; }

        // 品质颜色
        public string qualityName { get; set; }

        // 属性等级下限
        public int propMinLevel { get; set; }

        // 属性等级上限
        public int propMaxLevel { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RefineQualityTable);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RefineQualityTable);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RefineQualityTable> allDatas = new List<RefineQualityTable>();

            {
                string file = "Item/RefineQualityTable.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int qualityId_index = reader.GetIndex("qualityId");
                int qualityName_index = reader.GetIndex("qualityName");
                int propMinLevel_index = reader.GetIndex("propMinLevel");
                int propMaxLevel_index = reader.GetIndex("propMaxLevel");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RefineQualityTable data = new RefineQualityTable();
						data.qualityId = reader.getInt(i, qualityId_index, 0);         
						data.qualityName = reader.getStr(i, qualityName_index);         
						data.propMinLevel = reader.getInt(i, propMinLevel_index, 0);         
						data.propMaxLevel = reader.getInt(i, propMaxLevel_index, 0);         
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
                    var curType = typeof(RefineQualityTable);
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



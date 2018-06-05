// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class AwakeInforceTab 
    {
        static List<AwakeInforceTab> DataList = new List<AwakeInforceTab>();
        static public List<AwakeInforceTab> GetAll()
        {
            return DataList;
        }

        static public AwakeInforceTab Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("AwakeInforceTab.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return i;
            }
            return -1;
        }



        // 强化等级
        public int level { get; set; }

        // 类型
        public string types { get; set; }

        // 消耗材料
        public int materialCostId { get; set; }

        // 消耗数量
        public int materialCostCount { get; set; }

        // 增加属性百分比
        public float increasePercent { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(AwakeInforceTab);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(AwakeInforceTab);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<AwakeInforceTab> allDatas = new List<AwakeInforceTab>();

            {
                string file = "Item/AwakeInforceTab.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int level_index = reader.GetIndex("level");
                int types_index = reader.GetIndex("types");
                int materialCostId_index = reader.GetIndex("materialCostId");
                int materialCostCount_index = reader.GetIndex("materialCostCount");
                int increasePercent_index = reader.GetIndex("increasePercent");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AwakeInforceTab data = new AwakeInforceTab();
						data.level = reader.getInt(i, level_index, 0);         
						data.types = reader.getStr(i, types_index);         
						data.materialCostId = reader.getInt(i, materialCostId_index, 0);         
						data.materialCostCount = reader.getInt(i, materialCostCount_index, 0);         
						data.increasePercent = reader.getFloat(i, increasePercent_index, 0f);         
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
                    var curType = typeof(AwakeInforceTab);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ItemCompound 
    {
        static Dictionary<int, ItemCompound> DataList = new Dictionary<int, ItemCompound>();

        static public Dictionary<int, ItemCompound> GetAll()
        {
            return DataList;
        }

        static public ItemCompound Get(int key)
        {
            ItemCompound value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ItemCompound.Get({0}) not find!", key);
            return null;
        }



        // 合成ID
        public int id { get; set; }

        // 合成类型
        public ItemCompositeType type { get; set; }

        // 目标物品ID
        public int targetId { get; set; }

        // 材料1ID
        public int materialId_1 { get; set; }

        // 材料1数量
        public int count_1 { get; set; }

        // 材料2ID
        public int materialId_2 { get; set; }

        // 材料2数量
        public int count_2 { get; set; }

        // 材料3ID
        public int materialId_3 { get; set; }

        // 材料3数量
        public int count_3 { get; set; }

        // 材料4ID
        public int materialId_4 { get; set; }

        // 材料4数量
        public int count_4 { get; set; }

        // 消耗银贝
        public int needSilver { get; set; }

        // 成功率
        public int random { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ItemCompound);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ItemCompound);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ItemCompound> allDatas = new List<ItemCompound>();

            {
                string file = "Item/ItemCompound.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int type_index = reader.GetIndex("type");
                int targetId_index = reader.GetIndex("targetId");
                int materialId_1_index = reader.GetIndex("materialId_1");
                int count_1_index = reader.GetIndex("count_1");
                int materialId_2_index = reader.GetIndex("materialId_2");
                int count_2_index = reader.GetIndex("count_2");
                int materialId_3_index = reader.GetIndex("materialId_3");
                int count_3_index = reader.GetIndex("count_3");
                int materialId_4_index = reader.GetIndex("materialId_4");
                int count_4_index = reader.GetIndex("count_4");
                int needSilver_index = reader.GetIndex("needSilver");
                int random_index = reader.GetIndex("random");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ItemCompound data = new ItemCompound();
						data.id = reader.getInt(i, id_index, 0);         
						data.type = ((ItemCompositeType)reader.getInt(i, type_index, 0));         
						data.targetId = reader.getInt(i, targetId_index, 0);         
						data.materialId_1 = reader.getInt(i, materialId_1_index, 0);         
						data.count_1 = reader.getInt(i, count_1_index, 0);         
						data.materialId_2 = reader.getInt(i, materialId_2_index, 0);         
						data.count_2 = reader.getInt(i, count_2_index, 0);         
						data.materialId_3 = reader.getInt(i, materialId_3_index, 0);         
						data.count_3 = reader.getInt(i, count_3_index, 0);         
						data.materialId_4 = reader.getInt(i, materialId_4_index, 0);         
						data.count_4 = reader.getInt(i, count_4_index, 0);         
						data.needSilver = reader.getInt(i, needSilver_index, 0);         
						data.random = reader.getInt(i, random_index, 0);         
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
                    CsvCommon.Log.Error("ItemCompound.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ItemCompound);
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



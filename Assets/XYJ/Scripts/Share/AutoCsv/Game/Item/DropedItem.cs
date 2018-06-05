// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class DropedItem 
    {
        static List<DropedItem> DataList = new List<DropedItem>();
        static public List<DropedItem> GetAll()
        {
            return DataList;
        }

        static public DropedItem Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].groupId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("DropedItem.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].groupId == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<DropedItem>> DataList_groupId = new Dictionary<int, List<DropedItem>>();

        static public Dictionary<int, List<DropedItem>> GetAllGroupBygroupId()
        {
            return DataList_groupId;
        }

        static public List<DropedItem> GetGroupBygroupId(int key)
        {
            List<DropedItem> value = null;
            if (DataList_groupId.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("DropedItem.GetGroupBygroupId({0}) not find!", key);
            return null;
        }


        // id
        public int groupId { get; set; }

        // 道具id
        public int itemId { get; set; }

        // 名字
        public string name { get; set; }

        // 等级下限
        public int minLevel { get; set; }

        // 等级上限
        public int maxLevel { get; set; }

        // 权重
        public int weight { get; set; }

        // 数量
        public int num { get; set; }

        // 随机数量
        public RandomCount random { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(DropedItem);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_groupId.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(DropedItem);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<DropedItem> allDatas = new List<DropedItem>();

            {
                string file = "Item/DropedItem.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int groupId_index = reader.TryIndex("groupId:group");
                int itemId_index = reader.GetIndex("itemId");
                int name_index = reader.GetIndex("name");
                int minLevel_index = reader.GetIndex("minLevel");
                int maxLevel_index = reader.GetIndex("maxLevel");
                int weight_index = reader.GetIndex("weight");
                int num_index = reader.GetIndex("num");
                int random_index = reader.GetIndex("random");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        DropedItem data = new DropedItem();
						data.groupId = reader.getInt(i, groupId_index, 0);         
						data.itemId = reader.getInt(i, itemId_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.minLevel = reader.getInt(i, minLevel_index, 0);         
						data.maxLevel = reader.getInt(i, maxLevel_index, 0);         
						data.weight = reader.getInt(i, weight_index, 0);         
						data.num = reader.getInt(i, num_index, 0);         
						data.random = RandomCount.InitConfig(reader.getStr(i, random_index));         
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

            foreach (var data in allDatas)
            {
                {
                    List<DropedItem> l = null;
                    if (!DataList_groupId.TryGetValue(data.groupId, out l))
                    {
                        l = new List<DropedItem>();
                        DataList_groupId[data.groupId] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(DropedItem);
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



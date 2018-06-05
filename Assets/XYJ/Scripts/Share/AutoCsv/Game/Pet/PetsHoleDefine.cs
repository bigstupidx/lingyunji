// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PetsHoleDefine 
    {
        static List<PetsHoleDefine> DataList = new List<PetsHoleDefine>();
        static public List<PetsHoleDefine> GetAll()
        {
            return DataList;
        }

        static public PetsHoleDefine Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].identity == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("PetsHoleDefine.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].identity == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<PetsHoleDefine>> DataList_identity = new Dictionary<int, List<PetsHoleDefine>>();

        static public Dictionary<int, List<PetsHoleDefine>> GetAllGroupByidentity()
        {
            return DataList_identity;
        }

        static public List<PetsHoleDefine> GetGroupByidentity(int key)
        {
            List<PetsHoleDefine> value = null;
            if (DataList_identity.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PetsHoleDefine.GetGroupByidentity({0}) not find!", key);
            return null;
        }


        // 栏位顺序
        public int identity { get; set; }

        // 消耗道具
        public int itemID { get; set; }

        // 消耗数量
        public int Count { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PetsHoleDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_identity.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PetsHoleDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PetsHoleDefine> allDatas = new List<PetsHoleDefine>();

            {
                string file = "Pet/PetsHoleDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int identity_index = reader.TryIndex("identity:group");
                int itemID_index = reader.GetIndex("itemID");
                int Count_index = reader.GetIndex("Count");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PetsHoleDefine data = new PetsHoleDefine();
						data.identity = reader.getInt(i, identity_index, 0);         
						data.itemID = reader.getInt(i, itemID_index, 0);         
						data.Count = reader.getInt(i, Count_index, 0);         
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
                    List<PetsHoleDefine> l = null;
                    if (!DataList_identity.TryGetValue(data.identity, out l))
                    {
                        l = new List<PetsHoleDefine>();
                        DataList_identity[data.identity] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(PetsHoleDefine);
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



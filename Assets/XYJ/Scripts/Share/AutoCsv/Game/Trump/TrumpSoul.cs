// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TrumpSoul 
    {
        static List<TrumpSoul> DataList = new List<TrumpSoul>();
        static public List<TrumpSoul> GetAll()
        {
            return DataList;
        }

        static public TrumpSoul Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].trumpid == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("TrumpSoul.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].trumpid == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<TrumpSoul>> DataList_trumpid = new Dictionary<int, List<TrumpSoul>>();

        static public Dictionary<int, List<TrumpSoul>> GetAllGroupBytrumpid()
        {
            return DataList_trumpid;
        }

        static public List<TrumpSoul> GetGroupBytrumpid(int key)
        {
            List<TrumpSoul> value = null;
            if (DataList_trumpid.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TrumpSoul.GetGroupBytrumpid({0}) not find!", key);
            return null;
        }


        // 法宝ID
        public int trumpid { get; set; }

        // 境界等级
        public int tastelv { get; set; }

        // 消耗银贝
        public int cost { get; set; }

        // 需要潜修等级
        public int soullv { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TrumpSoul);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_trumpid.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TrumpSoul);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TrumpSoul> allDatas = new List<TrumpSoul>();

            {
                string file = "Trump/TrumpSoul.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int trumpid_index = reader.TryIndex("trumpid:group");
                int tastelv_index = reader.GetIndex("tastelv");
                int cost_index = reader.GetIndex("cost");
                int soullv_index = reader.GetIndex("soullv");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TrumpSoul data = new TrumpSoul();
						data.trumpid = reader.getInt(i, trumpid_index, 0);         
						data.tastelv = reader.getInt(i, tastelv_index, 0);         
						data.cost = reader.getInt(i, cost_index, 0);         
						data.soullv = reader.getInt(i, soullv_index, 0);         
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
                    List<TrumpSoul> l = null;
                    if (!DataList_trumpid.TryGetValue(data.trumpid, out l))
                    {
                        l = new List<TrumpSoul>();
                        DataList_trumpid[data.trumpid] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(TrumpSoul);
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



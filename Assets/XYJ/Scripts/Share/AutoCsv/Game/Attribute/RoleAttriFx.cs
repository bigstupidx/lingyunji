// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleAttriFx 
    {
        static List<RoleAttriFx> DataList = new List<RoleAttriFx>();
        static public List<RoleAttriFx> GetAll()
        {
            return DataList;
        }

        static public RoleAttriFx Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("RoleAttriFx.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<RoleAttriFx>> DataList_key = new Dictionary<int, List<RoleAttriFx>>();

        static public Dictionary<int, List<RoleAttriFx>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<RoleAttriFx> GetGroupBykey(int key)
        {
            List<RoleAttriFx> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleAttriFx.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 职业id
        public int key { get; set; }

        // 防御参数A
        public int DefenseA { get; set; }

        // 防御参数B
        public int DefenseB { get; set; }

        // 等级参数A
        public float LvA { get; set; }

        // 等级参数B
        public float LvB { get; set; }

        // 等级参数C
        public float LvC { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleAttriFx);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleAttriFx);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleAttriFx> allDatas = new List<RoleAttriFx>();

            {
                string file = "Attribute/RoleAttriFx.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int DefenseA_index = reader.GetIndex("DefenseA");
                int DefenseB_index = reader.GetIndex("DefenseB");
                int LvA_index = reader.GetIndex("LvA");
                int LvB_index = reader.GetIndex("LvB");
                int LvC_index = reader.GetIndex("LvC");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleAttriFx data = new RoleAttriFx();
						data.key = reader.getInt(i, key_index, 0);         
						data.DefenseA = reader.getInt(i, DefenseA_index, 0);         
						data.DefenseB = reader.getInt(i, DefenseB_index, 0);         
						data.LvA = reader.getFloat(i, LvA_index, 0f);         
						data.LvB = reader.getFloat(i, LvB_index, 0f);         
						data.LvC = reader.getFloat(i, LvC_index, 0f);         
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
                    List<RoleAttriFx> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<RoleAttriFx>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleAttriFx);
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



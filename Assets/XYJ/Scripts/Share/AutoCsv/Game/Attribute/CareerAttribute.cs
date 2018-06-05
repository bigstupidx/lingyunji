// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class CareerAttribute 
    {
        static List<CareerAttribute> DataList = new List<CareerAttribute>();
        static public List<CareerAttribute> GetAll()
        {
            return DataList;
        }

        static public CareerAttribute Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("CareerAttribute.Get({0}) not find!", key);
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

        static Dictionary<int, List<CareerAttribute>> DataList_key = new Dictionary<int, List<CareerAttribute>>();

        static public Dictionary<int, List<CareerAttribute>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<CareerAttribute> GetGroupBykey(int key)
        {
            List<CareerAttribute> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("CareerAttribute.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 职业
        public int key { get; set; }

        // 等级
        public int level { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(CareerAttribute);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(CareerAttribute);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<CareerAttribute> allDatas = new List<CareerAttribute>();

            {
                string file = "Attribute/CareerAttribute@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int level_index = reader.GetIndex("level");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        CareerAttribute data = new CareerAttribute();
						data.key = reader.getInt(i, key_index, 0);         
						data.level = reader.getInt(i, level_index, 0);         
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
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<CareerAttribute> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<CareerAttribute>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(CareerAttribute);
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



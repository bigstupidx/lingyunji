// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class MapConfig 
    {
        static Dictionary<int, MapConfig> DataList = new Dictionary<int, MapConfig>();

        static public Dictionary<int, MapConfig> GetAll()
        {
            return DataList;
        }

        static public MapConfig Get(int key)
        {
            MapConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("MapConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 资源id
        public string resId { get; set; }

        // 名称资源id
        public string nameId { get; set; }

        // 地图名称
        public string name { get; set; }

        // 坐标
        public string pos { get; set; }

        // npc列表
        public string npc { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(MapConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(MapConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<MapConfig> allDatas = new List<MapConfig>();

            {
                string file = "Map/MapConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int resId_index = reader.GetIndex("resId");
                int nameId_index = reader.GetIndex("nameId");
                int name_index = reader.GetIndex("name");
                int pos_index = reader.GetIndex("pos");
                int npc_index = reader.GetIndex("npc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        MapConfig data = new MapConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.resId = reader.getStr(i, resId_index);         
						data.nameId = reader.getStr(i, nameId_index);         
						data.name = reader.getStr(i, name_index);         
						data.pos = reader.getStr(i, pos_index);         
						data.npc = reader.getStr(i, npc_index);         
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
                    CsvCommon.Log.Error("MapConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(MapConfig);
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



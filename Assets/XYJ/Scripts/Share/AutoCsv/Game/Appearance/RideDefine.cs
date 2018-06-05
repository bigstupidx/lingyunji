// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RideDefine 
    {
        static List<RideDefine> DataList = new List<RideDefine>();
        static public List<RideDefine> GetAll()
        {
            return DataList;
        }

        static public RideDefine Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("RideDefine.Get({0}) not find!", key);
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

        static Dictionary<int, List<RideDefine>> DataList_key = new Dictionary<int, List<RideDefine>>();

        static public Dictionary<int, List<RideDefine>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<RideDefine> GetGroupBykey(int key)
        {
            List<RideDefine> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RideDefine.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // id
        public int key { get; set; }

        // 名称
        public string name { get; set; }

        // 图标
        public string icon { get; set; }

        // 描述
        public string des { get; set; }

        // 模型预制体
        public string mod { get; set; }

        // 镜头
        public string camera { get; set; }

        // 视野
        public string fov { get; set; }

        // 换色贴图
        public string tex { get; set; }

        // 坐骑界面色块
        public string hsv { get; set; }

        // 移动速度
        public int speed { get; set; }

        // 解锁道具id
        public string item { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RideDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RideDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RideDefine> allDatas = new List<RideDefine>();

            {
                string file = "Appearance/RideDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int des_index = reader.GetIndex("des");
                int mod_index = reader.GetIndex("mod");
                int camera_index = reader.GetIndex("camera");
                int fov_index = reader.GetIndex("fov");
                int tex_index = reader.GetIndex("tex");
                int hsv_index = reader.GetIndex("hsv");
                int speed_index = reader.GetIndex("speed");
                int item_index = reader.GetIndex("item");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RideDefine data = new RideDefine();
						data.key = reader.getInt(i, key_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.des = reader.getStr(i, des_index);         
						data.mod = reader.getStr(i, mod_index);         
						data.camera = reader.getStr(i, camera_index);         
						data.fov = reader.getStr(i, fov_index);         
						data.tex = reader.getStr(i, tex_index);         
						data.hsv = reader.getStr(i, hsv_index);         
						data.speed = reader.getInt(i, speed_index, 0);         
						data.item = reader.getStr(i, item_index);         
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
                    List<RideDefine> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<RideDefine>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(RideDefine);
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



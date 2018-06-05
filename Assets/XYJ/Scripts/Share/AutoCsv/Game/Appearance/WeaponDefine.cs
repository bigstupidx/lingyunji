// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class WeaponDefine 
    {
        static List<WeaponDefine> DataList = new List<WeaponDefine>();
        static public List<WeaponDefine> GetAll()
        {
            return DataList;
        }

        static public WeaponDefine Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("WeaponDefine.Get({0}) not find!", key);
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

        static Dictionary<int, List<WeaponDefine>> DataList_key = new Dictionary<int, List<WeaponDefine>>();

        static public Dictionary<int, List<WeaponDefine>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<WeaponDefine> GetGroupBykey(int key)
        {
            List<WeaponDefine> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("WeaponDefine.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // ID
        public int key { get; set; }

        // 名称
        public string name { get; set; }

        // 图标
        public string icon { get; set; }

        // 职业
        public int job { get; set; }

        // 模型预制体
        public string mod { get; set; }

        // 武饰效果等阶
        public int effectLevel { get; set; }

        // 武饰效果界面色块
        public string hsv { get; set; }

        // 武饰特效
        public string effect { get; set; }

        // 特效解锁强化等级
        public int unlockLevel { get; set; }

        // 外观解锁道具
        public string item { get; set; }

        // 描述
        public string des { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(WeaponDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(WeaponDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<WeaponDefine> allDatas = new List<WeaponDefine>();

            {
                string file = "Appearance/WeaponDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int job_index = reader.GetIndex("job");
                int mod_index = reader.GetIndex("mod");
                int effectLevel_index = reader.GetIndex("effectLevel");
                int hsv_index = reader.GetIndex("hsv");
                int effect_index = reader.GetIndex("effect");
                int unlockLevel_index = reader.GetIndex("unlockLevel");
                int item_index = reader.GetIndex("item");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        WeaponDefine data = new WeaponDefine();
						data.key = reader.getInt(i, key_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.job = reader.getInt(i, job_index, 0);         
						data.mod = reader.getStr(i, mod_index);         
						data.effectLevel = reader.getInt(i, effectLevel_index, 0);         
						data.hsv = reader.getStr(i, hsv_index);         
						data.effect = reader.getStr(i, effect_index);         
						data.unlockLevel = reader.getInt(i, unlockLevel_index, 0);         
						data.item = reader.getStr(i, item_index);         
						data.des = reader.getStr(i, des_index);         
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
                    List<WeaponDefine> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<WeaponDefine>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(WeaponDefine);
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



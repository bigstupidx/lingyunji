// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleDisguiseDefault 
    {
        static Dictionary<int, RoleDisguiseDefault> DataList = new Dictionary<int, RoleDisguiseDefault>();

        static public Dictionary<int, RoleDisguiseDefault> GetAll()
        {
            return DataList;
        }

        static public RoleDisguiseDefault Get(int key)
        {
            RoleDisguiseDefault value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleDisguiseDefault.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 职业
        public int career { get; set; }

        // 性别
        public int sex { get; set; }

        // 基准类型
        public int faceType { get; set; }

        // 对应编辑文件
        public string prefabName { get; set; }

        // 基准脸图标
        public string iconName { get; set; }

        // 化妆配置
        public string skinConfig { get; set; }

        // 捏脸配置
        public string shapeConfig { get; set; }

        // 初始服饰
        public int clothId { get; set; }

        // 初始武器
        public int weaponId { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleDisguiseDefault);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleDisguiseDefault);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleDisguiseDefault> allDatas = new List<RoleDisguiseDefault>();

            {
                string file = "Appearance/RoleDisguiseDefault.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int career_index = reader.GetIndex("career");
                int sex_index = reader.GetIndex("sex");
                int faceType_index = reader.GetIndex("faceType");
                int prefabName_index = reader.GetIndex("prefabName");
                int iconName_index = reader.GetIndex("iconName");
                int skinConfig_index = reader.GetIndex("skinConfig");
                int shapeConfig_index = reader.GetIndex("shapeConfig");
                int clothId_index = reader.GetIndex("clothId");
                int weaponId_index = reader.GetIndex("weaponId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDisguiseDefault data = new RoleDisguiseDefault();
						data.id = reader.getInt(i, id_index, 0);         
						data.career = reader.getInt(i, career_index, 0);         
						data.sex = reader.getInt(i, sex_index, 0);         
						data.faceType = reader.getInt(i, faceType_index, 0);         
						data.prefabName = reader.getStr(i, prefabName_index);         
						data.iconName = reader.getStr(i, iconName_index);         
						data.skinConfig = reader.getStr(i, skinConfig_index);         
						data.shapeConfig = reader.getStr(i, shapeConfig_index);         
						data.clothId = reader.getInt(i, clothId_index, 0);         
						data.weaponId = reader.getInt(i, weaponId_index, 0);         
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
                    CsvCommon.Log.Error("RoleDisguiseDefault.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleDisguiseDefault);
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



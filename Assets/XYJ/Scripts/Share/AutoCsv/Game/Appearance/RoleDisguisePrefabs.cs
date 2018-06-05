// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleDisguisePrefabs 
    {
        static Dictionary<int, RoleDisguisePrefabs> DataList = new Dictionary<int, RoleDisguisePrefabs>();

        static public Dictionary<int, RoleDisguisePrefabs> GetAll()
        {
            return DataList;
        }

        static public RoleDisguisePrefabs Get(int key)
        {
            RoleDisguisePrefabs value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleDisguisePrefabs.Get({0}) not find!", key);
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

        // 部位
        public int partType { get; set; }

        // 资源编号
        public int assetId { get; set; }

        // 对应编辑文件
        public string prefabName { get; set; }

        // 图标
        public string iconName { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleDisguisePrefabs);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleDisguisePrefabs);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleDisguisePrefabs> allDatas = new List<RoleDisguisePrefabs>();

            {
                string file = "Appearance/RoleDisguisePrefabs.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int career_index = reader.GetIndex("career");
                int sex_index = reader.GetIndex("sex");
                int faceType_index = reader.GetIndex("faceType");
                int partType_index = reader.GetIndex("partType");
                int assetId_index = reader.GetIndex("assetId");
                int prefabName_index = reader.GetIndex("prefabName");
                int iconName_index = reader.GetIndex("iconName");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDisguisePrefabs data = new RoleDisguisePrefabs();
						data.id = reader.getInt(i, id_index, 0);         
						data.career = reader.getInt(i, career_index, 0);         
						data.sex = reader.getInt(i, sex_index, 0);         
						data.faceType = reader.getInt(i, faceType_index, 0);         
						data.partType = reader.getInt(i, partType_index, 0);         
						data.assetId = reader.getInt(i, assetId_index, 0);         
						data.prefabName = reader.getStr(i, prefabName_index);         
						data.iconName = reader.getStr(i, iconName_index);         
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
                    CsvCommon.Log.Error("RoleDisguisePrefabs.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleDisguisePrefabs);
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



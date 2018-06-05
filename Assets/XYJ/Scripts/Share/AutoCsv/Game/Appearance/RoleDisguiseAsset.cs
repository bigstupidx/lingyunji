// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleDisguiseAsset 
    {
        static Dictionary<int, RoleDisguiseAsset> DataList = new Dictionary<int, RoleDisguiseAsset>();

        static public Dictionary<int, RoleDisguiseAsset> GetAll()
        {
            return DataList;
        }

        static public RoleDisguiseAsset Get(int key)
        {
            RoleDisguiseAsset value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleDisguiseAsset.Get({0}) not find!", key);
            return null;
        }



        // 资源id
        public int id { get; set; }

        // 预制体
        public string modelName { get; set; }

        // 色相
        public string HSV { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleDisguiseAsset);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleDisguiseAsset);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleDisguiseAsset> allDatas = new List<RoleDisguiseAsset>();

            {
                string file = "Appearance/RoleDisguiseAsset.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int modelName_index = reader.GetIndex("modelName");
                int HSV_index = reader.GetIndex("HSV");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleDisguiseAsset data = new RoleDisguiseAsset();
						data.id = reader.getInt(i, id_index, 0);         
						data.modelName = reader.getStr(i, modelName_index);         
						data.HSV = reader.getStr(i, HSV_index);         
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
                    CsvCommon.Log.Error("RoleDisguiseAsset.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleDisguiseAsset);
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



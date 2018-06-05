// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleRandomName 
    {
        static Dictionary<int, RoleRandomName> DataList = new Dictionary<int, RoleRandomName>();

        static public Dictionary<int, RoleRandomName> GetAll()
        {
            return DataList;
        }

        static public RoleRandomName Get(int key)
        {
            RoleRandomName value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleRandomName.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public int id { get; set; }

        // 姓
        public string firstName { get; set; }

        // 男名字
        public string maleName { get; set; }

        // 女名字
        public string femaleName { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleRandomName);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleRandomName);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleRandomName> allDatas = new List<RoleRandomName>();

            {
                string file = "Role/RoleRandomName.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int firstName_index = reader.GetIndex("firstName");
                int maleName_index = reader.GetIndex("maleName");
                int femaleName_index = reader.GetIndex("femaleName");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleRandomName data = new RoleRandomName();
						data.id = reader.getInt(i, id_index, 0);         
						data.firstName = reader.getStr(i, firstName_index);         
						data.maleName = reader.getStr(i, maleName_index);         
						data.femaleName = reader.getStr(i, femaleName_index);         
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
                    CsvCommon.Log.Error("RoleRandomName.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleRandomName);
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



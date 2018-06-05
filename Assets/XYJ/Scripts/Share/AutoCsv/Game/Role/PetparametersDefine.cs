// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PetparametersDefine 
    {
        static Dictionary<int, PetparametersDefine> DataList = new Dictionary<int, PetparametersDefine>();

        static public Dictionary<int, PetparametersDefine> GetAll()
        {
            return DataList;
        }

        static public PetparametersDefine Get(int key)
        {
            PetparametersDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PetparametersDefine.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<PetparametersDefine>> DataList_id = new Dictionary<int, List<PetparametersDefine>>();

        static public Dictionary<int, List<PetparametersDefine>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<PetparametersDefine> GetGroupByid(int key)
        {
            List<PetparametersDefine> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PetparametersDefine.GetGroupByid({0}) not find!", key);
            return null;
        }


        // 宠物ID
        public int id { get; set; }

        // 攻击间隔
        public float attackInterval { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PetparametersDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PetparametersDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PetparametersDefine> allDatas = new List<PetparametersDefine>();

            {
                string file = "Role/PetparametersDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int attackInterval_index = reader.GetIndex("attackInterval");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PetparametersDefine data = new PetparametersDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.attackInterval = reader.getFloat(i, attackInterval_index, 0f);         
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
                    CsvCommon.Log.Error("PetparametersDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<PetparametersDefine> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<PetparametersDefine>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(PetparametersDefine);
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



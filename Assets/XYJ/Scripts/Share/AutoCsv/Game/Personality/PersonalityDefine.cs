// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PersonalityDefine 
    {
        static Dictionary<int, PersonalityDefine> DataList = new Dictionary<int, PersonalityDefine>();

        static public Dictionary<int, PersonalityDefine> GetAll()
        {
            return DataList;
        }

        static public PersonalityDefine Get(int key)
        {
            PersonalityDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PersonalityDefine.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<PersonalityDefine>> DataList_id = new Dictionary<int, List<PersonalityDefine>>();

        static public Dictionary<int, List<PersonalityDefine>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<PersonalityDefine> GetGroupByid(int key)
        {
            List<PersonalityDefine> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PersonalityDefine.GetGroupByid({0}) not find!", key);
            return null;
        }


        // ID
        public int id { get; set; }

        // 类型描述
        public string name { get; set; }

        // 1级下限
        public int lowerLimit_1 { get; set; }

        // 1级上限
        public int upperLimit_1 { get; set; }

        // 描述1
        public string desc_1 { get; set; }

        // 1级心法ID
        public int  cittaId_1 { get; set; }

        // 2级下限
        public int lowerLimit_2 { get; set; }

        // 2级上限
        public int upperLimit_2 { get; set; }

        // 描述2
        public string desc_2 { get; set; }

        // 2级心法ID
        public int  cittaId_2 { get; set; }

        // 3级下限
        public int lowerLimit_3 { get; set; }

        // 3级上限
        public int upperLimit_3 { get; set; }

        // 描述3
        public string desc_3 { get; set; }

        // 3级心法ID
        public int  cittaId_3 { get; set; }

        // 4级下限
        public int lowerLimit_4 { get; set; }

        // 4级上限
        public int upperLimit_4 { get; set; }

        // 描述4
        public string desc_4 { get; set; }

        // 4级心法ID
        public int  cittaId_4 { get; set; }

        // 5级下限
        public int lowerLimit_5 { get; set; }

        // 5级上限
        public int upperLimit_5 { get; set; }

        // 描述5
        public string desc_5 { get; set; }

        // 5级心法ID
        public int  cittaId_5 { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PersonalityDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PersonalityDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PersonalityDefine> allDatas = new List<PersonalityDefine>();

            {
                string file = "Personality/PersonalityDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int name_index = reader.GetIndex("name");
                int lowerLimit_1_index = reader.GetIndex("lowerLimit_1");
                int upperLimit_1_index = reader.GetIndex("upperLimit_1");
                int desc_1_index = reader.GetIndex("desc_1");
                int  cittaId_1_index = reader.GetIndex(" cittaId_1");
                int lowerLimit_2_index = reader.GetIndex("lowerLimit_2");
                int upperLimit_2_index = reader.GetIndex("upperLimit_2");
                int desc_2_index = reader.GetIndex("desc_2");
                int  cittaId_2_index = reader.GetIndex(" cittaId_2");
                int lowerLimit_3_index = reader.GetIndex("lowerLimit_3");
                int upperLimit_3_index = reader.GetIndex("upperLimit_3");
                int desc_3_index = reader.GetIndex("desc_3");
                int  cittaId_3_index = reader.GetIndex(" cittaId_3");
                int lowerLimit_4_index = reader.GetIndex("lowerLimit_4");
                int upperLimit_4_index = reader.GetIndex("upperLimit_4");
                int desc_4_index = reader.GetIndex("desc_4");
                int  cittaId_4_index = reader.GetIndex(" cittaId_4");
                int lowerLimit_5_index = reader.GetIndex("lowerLimit_5");
                int upperLimit_5_index = reader.GetIndex("upperLimit_5");
                int desc_5_index = reader.GetIndex("desc_5");
                int  cittaId_5_index = reader.GetIndex(" cittaId_5");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PersonalityDefine data = new PersonalityDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.lowerLimit_1 = reader.getInt(i, lowerLimit_1_index, 0);         
						data.upperLimit_1 = reader.getInt(i, upperLimit_1_index, 0);         
						data.desc_1 = reader.getStr(i, desc_1_index);         
						data. cittaId_1 = reader.getInt(i,  cittaId_1_index, 0);         
						data.lowerLimit_2 = reader.getInt(i, lowerLimit_2_index, 0);         
						data.upperLimit_2 = reader.getInt(i, upperLimit_2_index, 0);         
						data.desc_2 = reader.getStr(i, desc_2_index);         
						data. cittaId_2 = reader.getInt(i,  cittaId_2_index, 0);         
						data.lowerLimit_3 = reader.getInt(i, lowerLimit_3_index, 0);         
						data.upperLimit_3 = reader.getInt(i, upperLimit_3_index, 0);         
						data.desc_3 = reader.getStr(i, desc_3_index);         
						data. cittaId_3 = reader.getInt(i,  cittaId_3_index, 0);         
						data.lowerLimit_4 = reader.getInt(i, lowerLimit_4_index, 0);         
						data.upperLimit_4 = reader.getInt(i, upperLimit_4_index, 0);         
						data.desc_4 = reader.getStr(i, desc_4_index);         
						data. cittaId_4 = reader.getInt(i,  cittaId_4_index, 0);         
						data.lowerLimit_5 = reader.getInt(i, lowerLimit_5_index, 0);         
						data.upperLimit_5 = reader.getInt(i, upperLimit_5_index, 0);         
						data.desc_5 = reader.getStr(i, desc_5_index);         
						data. cittaId_5 = reader.getInt(i,  cittaId_5_index, 0);         
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
                    CsvCommon.Log.Error("PersonalityDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<PersonalityDefine> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<PersonalityDefine>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(PersonalityDefine);
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



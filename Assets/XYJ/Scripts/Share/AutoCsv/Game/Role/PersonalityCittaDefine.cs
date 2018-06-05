// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PersonalityCittaDefine 
    {
        static Dictionary<int, PersonalityCittaDefine> DataList = new Dictionary<int, PersonalityCittaDefine>();

        static public Dictionary<int, PersonalityCittaDefine> GetAll()
        {
            return DataList;
        }

        static public PersonalityCittaDefine Get(int key)
        {
            PersonalityCittaDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PersonalityCittaDefine.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<PersonalityCittaDefine>> DataList_id = new Dictionary<int, List<PersonalityCittaDefine>>();

        static public Dictionary<int, List<PersonalityCittaDefine>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<PersonalityCittaDefine> GetGroupByid(int key)
        {
            List<PersonalityCittaDefine> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PersonalityCittaDefine.GetGroupByid({0}) not find!", key);
            return null;
        }


        // 心法ID
        public int id { get; set; }

        // 心法名称
        public string name { get; set; }

        // 图标
        public string icon { get; set; }

        // 心法类型
        public int type { get; set; }

        // 心法阶段
        public int stage { get; set; }

        // 解锁等级
        public int unlockingLevel { get; set; }

        // 被动技能ID
        public int passiveSkill { get; set; }

        // 心法描述
        public string desc { get; set; }

        // 属性描述
        public string attriDesc { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PersonalityCittaDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PersonalityCittaDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PersonalityCittaDefine> allDatas = new List<PersonalityCittaDefine>();

            {
                string file = "Role/PersonalityCittaDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int type_index = reader.GetIndex("type");
                int stage_index = reader.GetIndex("stage");
                int unlockingLevel_index = reader.GetIndex("unlockingLevel");
                int passiveSkill_index = reader.GetIndex("passiveSkill");
                int desc_index = reader.GetIndex("desc");
                int attriDesc_index = reader.GetIndex("attriDesc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PersonalityCittaDefine data = new PersonalityCittaDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.type = reader.getInt(i, type_index, 0);         
						data.stage = reader.getInt(i, stage_index, 0);         
						data.unlockingLevel = reader.getInt(i, unlockingLevel_index, 0);         
						data.passiveSkill = reader.getInt(i, passiveSkill_index, 0);         
						data.desc = reader.getStr(i, desc_index);         
						data.attriDesc = reader.getStr(i, attriDesc_index);         
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
                    CsvCommon.Log.Error("PersonalityCittaDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<PersonalityCittaDefine> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<PersonalityCittaDefine>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(PersonalityCittaDefine);
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



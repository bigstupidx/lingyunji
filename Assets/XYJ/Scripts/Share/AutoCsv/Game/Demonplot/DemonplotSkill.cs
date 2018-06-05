// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class DemonplotSkill 
    {
        static Dictionary<DemonplotSkillType, DemonplotSkill> DataList = new Dictionary<DemonplotSkillType, DemonplotSkill>();

        static public Dictionary<DemonplotSkillType, DemonplotSkill> GetAll()
        {
            return DataList;
        }

        static public DemonplotSkill Get(DemonplotSkillType key)
        {
            DemonplotSkill value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("DemonplotSkill.Get({0}) not find!", key);
            return null;
        }



        // 技能类型
        public DemonplotSkillType id { get; set; }

        // 名称
        public string name { get; set; }

        // 制作描述
        public string matchinname { get; set; }

        // 技能图标
        public string icon { get; set; }

        // 技能描述
        public string des { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(DemonplotSkill);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(DemonplotSkill);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<DemonplotSkill> allDatas = new List<DemonplotSkill>();

            {
                string file = "Demonplot/DemonplotSkill.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int matchinname_index = reader.GetIndex("matchinname");
                int icon_index = reader.GetIndex("icon");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        DemonplotSkill data = new DemonplotSkill();
						data.id = ((DemonplotSkillType)reader.getInt(i, id_index, 0));         
						data.name = reader.getStr(i, name_index);         
						data.matchinname = reader.getStr(i, matchinname_index);         
						data.icon = reader.getStr(i, icon_index);         
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
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("DemonplotSkill.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(DemonplotSkill);
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



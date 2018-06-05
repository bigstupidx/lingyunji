// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class DemonplotSkillExp 
    {
        static Dictionary<int, DemonplotSkillExp> DataList = new Dictionary<int, DemonplotSkillExp>();

        static public Dictionary<int, DemonplotSkillExp> GetAll()
        {
            return DataList;
        }

        static public DemonplotSkillExp Get(int key)
        {
            DemonplotSkillExp value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("DemonplotSkillExp.Get({0}) not find!", key);
            return null;
        }



        // 技能等级
        public int id { get; set; }

        // 升级熟练度
        public int exp { get; set; }

        // 贡献
        public int familyvalue { get; set; }

        // 修为
        public int xiuwei { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(DemonplotSkillExp);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(DemonplotSkillExp);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<DemonplotSkillExp> allDatas = new List<DemonplotSkillExp>();

            {
                string file = "Demonplot/DemonplotSkillExp.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int exp_index = reader.GetIndex("exp");
                int familyvalue_index = reader.GetIndex("familyvalue");
                int xiuwei_index = reader.GetIndex("xiuwei");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        DemonplotSkillExp data = new DemonplotSkillExp();
						data.id = reader.getInt(i, id_index, 0);         
						data.exp = reader.getInt(i, exp_index, 0);         
						data.familyvalue = reader.getInt(i, familyvalue_index, 0);         
						data.xiuwei = reader.getInt(i, xiuwei_index, 0);         
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
                    CsvCommon.Log.Error("DemonplotSkillExp.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(DemonplotSkillExp);
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



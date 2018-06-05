// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TrumpSkill 
    {
        static Dictionary<int, TrumpSkill> DataList = new Dictionary<int, TrumpSkill>();

        static public Dictionary<int, TrumpSkill> GetAll()
        {
            return DataList;
        }

        static public TrumpSkill Get(int key)
        {
            TrumpSkill value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TrumpSkill.Get({0}) not find!", key);
            return null;
        }



        // 技能ID
        public int id { get; set; }

        // 下一级技能ID
        public int nextid { get; set; }

        // 技能类型
        public TrumpSkillType type { get; set; }

        // 法宝ID
        public int trumpid { get; set; }

        // 技能等级
        public int lv { get; set; }

        // 需要法宝境界
        public int needtastelv { get; set; }

        // 消耗材料ID
        public int itemid { get; set; }

        // 材料数量
        public int itemcount { get; set; }

        // 描述
        public string des { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TrumpSkill);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TrumpSkill);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TrumpSkill> allDatas = new List<TrumpSkill>();

            {
                string file = "Trump/TrumpSkill.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int nextid_index = reader.GetIndex("nextid");
                int type_index = reader.GetIndex("type");
                int trumpid_index = reader.GetIndex("trumpid");
                int lv_index = reader.GetIndex("lv");
                int needtastelv_index = reader.GetIndex("needtastelv");
                int itemid_index = reader.GetIndex("itemid");
                int itemcount_index = reader.GetIndex("itemcount");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TrumpSkill data = new TrumpSkill();
						data.id = reader.getInt(i, id_index, 0);         
						data.nextid = reader.getInt(i, nextid_index, 0);         
						data.type = ((TrumpSkillType)reader.getInt(i, type_index, 0));         
						data.trumpid = reader.getInt(i, trumpid_index, 0);         
						data.lv = reader.getInt(i, lv_index, 0);         
						data.needtastelv = reader.getInt(i, needtastelv_index, 0);         
						data.itemid = reader.getInt(i, itemid_index, 0);         
						data.itemcount = reader.getInt(i, itemcount_index, 0);         
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
                    CsvCommon.Log.Error("TrumpSkill.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TrumpSkill);
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



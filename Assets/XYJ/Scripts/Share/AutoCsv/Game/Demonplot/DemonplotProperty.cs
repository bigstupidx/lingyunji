// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class DemonplotProperty 
    {
        static Dictionary<int, DemonplotProperty> DataList = new Dictionary<int, DemonplotProperty>();

        static public Dictionary<int, DemonplotProperty> GetAll()
        {
            return DataList;
        }

        static public DemonplotProperty Get(int key)
        {
            DemonplotProperty value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("DemonplotProperty.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<DemonplotType, List<DemonplotProperty>> DataList_type = new Dictionary<DemonplotType, List<DemonplotProperty>>();

        static public Dictionary<DemonplotType, List<DemonplotProperty>> GetAllGroupBytype()
        {
            return DataList_type;
        }

        static public List<DemonplotProperty> GetGroupBytype(DemonplotType key)
        {
            List<DemonplotProperty> value = null;
            if (DataList_type.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("DemonplotProperty.GetGroupBytype({0}) not find!", key);
            return null;
        }


        // 配方ID
        public int id { get; set; }

        // 类型
        public DemonplotType type { get; set; }

        // 技能类型
        public DemonplotSkillType skilltype { get; set; }

        // 产出道具
        public ProduceData produce { get; set; }

        // 产出副道具
        public ProduceData byproduce { get; set; }

        // 产出副道具概率
        public int byproducerate { get; set; }

        // 采集需要道具
        public int useitemid { get; set; }

        // 消耗耐久
        public int durable { get; set; }

        // 产地ID
        public int senceid { get; set; }

        // 产地
        public string producemap { get; set; }

        // 产地名称
        public string producemapname { get; set; }

        // 制作材料
        public MatchinData matchinitems { get; set; }

        // 特殊配方
        public int specail { get; set; }

        // 所需等级
        public int needlv { get; set; }

        // 消耗活力
        public int needenergy { get; set; }

        // 增加熟练度
        public int exp { get; set; }

        // 吟唱动作ID
        public int singid { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(DemonplotProperty);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_type.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(DemonplotProperty);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<DemonplotProperty> allDatas = new List<DemonplotProperty>();

            {
                string file = "Demonplot/DemonplotProperty.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int type_index = reader.TryIndex("type:group");
                int skilltype_index = reader.GetIndex("skilltype");
                int produce_index = reader.GetIndex("produce");
                int byproduce_index = reader.GetIndex("byproduce");
                int byproducerate_index = reader.GetIndex("byproducerate");
                int useitemid_index = reader.GetIndex("useitemid");
                int durable_index = reader.GetIndex("durable");
                int senceid_index = reader.GetIndex("senceid");
                int producemap_index = reader.GetIndex("producemap");
                int producemapname_index = reader.GetIndex("producemapname");
                int matchinitems_index = reader.GetIndex("matchinitems");
                int specail_index = reader.GetIndex("specail");
                int needlv_index = reader.GetIndex("needlv");
                int needenergy_index = reader.GetIndex("needenergy");
                int exp_index = reader.GetIndex("exp");
                int singid_index = reader.GetIndex("singid");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        DemonplotProperty data = new DemonplotProperty();
						data.id = reader.getInt(i, id_index, 0);         
						data.type = ((DemonplotType)reader.getInt(i, type_index, 0));         
						data.skilltype = ((DemonplotSkillType)reader.getInt(i, skilltype_index, 0));         
						data.produce = ProduceData.InitConfig(reader.getStr(i, produce_index));         
						data.byproduce = ProduceData.InitConfig(reader.getStr(i, byproduce_index));         
						data.byproducerate = reader.getInt(i, byproducerate_index, 0);         
						data.useitemid = reader.getInt(i, useitemid_index, 0);         
						data.durable = reader.getInt(i, durable_index, 0);         
						data.senceid = reader.getInt(i, senceid_index, 0);         
						data.producemap = reader.getStr(i, producemap_index);         
						data.producemapname = reader.getStr(i, producemapname_index);         
						data.matchinitems = MatchinData.InitConfig(reader.getStr(i, matchinitems_index));         
						data.specail = reader.getInt(i, specail_index, 0);         
						data.needlv = reader.getInt(i, needlv_index, 0);         
						data.needenergy = reader.getInt(i, needenergy_index, 0);         
						data.exp = reader.getInt(i, exp_index, 0);         
						data.singid = reader.getInt(i, singid_index, 0);         
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
                    CsvCommon.Log.Error("DemonplotProperty.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<DemonplotProperty> l = null;
                    if (!DataList_type.TryGetValue(data.type, out l))
                    {
                        l = new List<DemonplotProperty>();
                        DataList_type[data.type] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(DemonplotProperty);
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



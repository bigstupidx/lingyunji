// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ExchangeStoreData 
    {
        static List<ExchangeStoreData> DataList = new List<ExchangeStoreData>();
        static public List<ExchangeStoreData> GetAll()
        {
            return DataList;
        }

        static public ExchangeStoreData Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].itemid == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ExchangeStoreData.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].itemid == key)
                    return i;
            }
            return -1;
        }



        // 物品id
        public int itemid { get; set; }

        // 商店ID
        public int id { get; set; }

        // 物品名称
        public string itemname { get; set; }

        // 职业限制
        public JobMask joblimit { get; set; }

        // 购买所需的货币1
        public int currency1id { get; set; }

        // 货币1数量
        public int currency1num { get; set; }

        // 购买所需的货币2
        public int currency2id { get; set; }

        // 货币2数量
        public int currency2num { get; set; }

        // 兑换所需材料
        public int materialid { get; set; }

        // 所需材料数量
        public int materialnum { get; set; }

        // 购买等级
        public int level { get; set; }

        // 每日购买次数限制
        public int daylimit { get; set; }

        // 每周购买次数限制
        public int weeklimit { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ExchangeStoreData);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ExchangeStoreData);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ExchangeStoreData> allDatas = new List<ExchangeStoreData>();

            {
                string file = "ExchangeStore/ExchangeStoreData.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int itemid_index = reader.GetIndex("itemid");
                int id_index = reader.GetIndex("id");
                int itemname_index = reader.GetIndex("itemname");
                int joblimit_index = reader.GetIndex("joblimit");
                int currency1id_index = reader.GetIndex("currency1id");
                int currency1num_index = reader.GetIndex("currency1num");
                int currency2id_index = reader.GetIndex("currency2id");
                int currency2num_index = reader.GetIndex("currency2num");
                int materialid_index = reader.GetIndex("materialid");
                int materialnum_index = reader.GetIndex("materialnum");
                int level_index = reader.GetIndex("level");
                int daylimit_index = reader.GetIndex("daylimit");
                int weeklimit_index = reader.GetIndex("weeklimit");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ExchangeStoreData data = new ExchangeStoreData();
						data.itemid = reader.getInt(i, itemid_index, 0);         
						data.id = reader.getInt(i, id_index, 0);         
						data.itemname = reader.getStr(i, itemname_index);         
						data.joblimit = JobMask.InitConfig(reader.getStr(i, joblimit_index));         
						data.currency1id = reader.getInt(i, currency1id_index, 0);         
						data.currency1num = reader.getInt(i, currency1num_index, 0);         
						data.currency2id = reader.getInt(i, currency2id_index, 0);         
						data.currency2num = reader.getInt(i, currency2num_index, 0);         
						data.materialid = reader.getInt(i, materialid_index, 0);         
						data.materialnum = reader.getInt(i, materialnum_index, 0);         
						data.level = reader.getInt(i, level_index, 0);         
						data.daylimit = reader.getInt(i, daylimit_index, 0);         
						data.weeklimit = reader.getInt(i, weeklimit_index, 0);         
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
            
            DataList = allDatas;

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ExchangeStoreData);
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



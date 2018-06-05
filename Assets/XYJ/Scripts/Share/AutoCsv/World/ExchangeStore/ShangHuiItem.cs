// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ShangHuiItem 
    {
        static List<ShangHuiItem> DataList = new List<ShangHuiItem>();
        static public List<ShangHuiItem> GetAll()
        {
            return DataList;
        }

        static public ShangHuiItem Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].itemid == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ShangHuiItem.Get({0}) not find!", key);
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



        // 物品ID
        public int itemid { get; set; }

        // 商会ID
        public int id { get; set; }

        // 商品名称
        public string itemname { get; set; }

        // 售价-碧玉
        public float priceBiyu { get; set; }

        // 售价-银贝
        public float priceSilver { get; set; }

        // 售价-金贝
        public float priceGold { get; set; }

        // 每日库存
        public int daystock { get; set; }

        // 单人限购
        public int buylimit { get; set; }

        // 标准系数
        public int normalcoefficient { get; set; }

        // 涨价系数
        public int upcoefficient { get; set; }

        // 涨价限度
        public float uplimit { get; set; }

        // 跌价限度
        public float minlimit { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ShangHuiItem);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ShangHuiItem);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ShangHuiItem> allDatas = new List<ShangHuiItem>();

            {
                string file = "ExchangeStore/ShangHuiItem.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int itemid_index = reader.GetIndex("itemid");
                int id_index = reader.GetIndex("id");
                int itemname_index = reader.GetIndex("itemname");
                int priceBiyu_index = reader.GetIndex("priceBiyu");
                int priceSilver_index = reader.GetIndex("priceSilver");
                int priceGold_index = reader.GetIndex("priceGold");
                int daystock_index = reader.GetIndex("daystock");
                int buylimit_index = reader.GetIndex("buylimit");
                int normalcoefficient_index = reader.GetIndex("normalcoefficient");
                int upcoefficient_index = reader.GetIndex("upcoefficient");
                int uplimit_index = reader.GetIndex("uplimit");
                int minlimit_index = reader.GetIndex("minlimit");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ShangHuiItem data = new ShangHuiItem();
						data.itemid = reader.getInt(i, itemid_index, 0);         
						data.id = reader.getInt(i, id_index, 0);         
						data.itemname = reader.getStr(i, itemname_index);         
						data.priceBiyu = reader.getFloat(i, priceBiyu_index, 0f);         
						data.priceSilver = reader.getFloat(i, priceSilver_index, 0f);         
						data.priceGold = reader.getFloat(i, priceGold_index, 0f);         
						data.daystock = reader.getInt(i, daystock_index, 0);         
						data.buylimit = reader.getInt(i, buylimit_index, 0);         
						data.normalcoefficient = reader.getInt(i, normalcoefficient_index, 0);         
						data.upcoefficient = reader.getInt(i, upcoefficient_index, 0);         
						data.uplimit = reader.getFloat(i, uplimit_index, 0f);         
						data.minlimit = reader.getFloat(i, minlimit_index, 0f);         
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
                    var curType = typeof(ShangHuiItem);
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



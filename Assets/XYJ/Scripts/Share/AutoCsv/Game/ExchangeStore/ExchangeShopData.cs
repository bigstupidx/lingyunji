// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ExchangeShopData 
    {
        static Dictionary<int, ExchangeShopData> DataList = new Dictionary<int, ExchangeShopData>();

        static public Dictionary<int, ExchangeShopData> GetAll()
        {
            return DataList;
        }

        static public ExchangeShopData Get(int key)
        {
            ExchangeShopData value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ExchangeShopData.Get({0}) not find!", key);
            return null;
        }



        // 商店ID
        public int id { get; set; }

        // 商店图标
        public string icon { get; set; }

        // 父类型名
        public string parentname { get; set; }

        // 子类型名
        public string childname { get; set; }

        // 商店类型
        public int type { get; set; }

        // 商店说明ID
        public int info { get; set; }

        // 备注
        public string other { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ExchangeShopData);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ExchangeShopData);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ExchangeShopData> allDatas = new List<ExchangeShopData>();

            {
                string file = "ExchangeStore/ExchangeShopData.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int icon_index = reader.GetIndex("icon");
                int parentname_index = reader.GetIndex("parentname");
                int childname_index = reader.GetIndex("childname");
                int type_index = reader.GetIndex("type");
                int info_index = reader.GetIndex("info");
                int other_index = reader.GetIndex("other");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ExchangeShopData data = new ExchangeShopData();
						data.id = reader.getInt(i, id_index, 0);         
						data.icon = reader.getStr(i, icon_index);         
						data.parentname = reader.getStr(i, parentname_index);         
						data.childname = reader.getStr(i, childname_index);         
						data.type = reader.getInt(i, type_index, 0);         
						data.info = reader.getInt(i, info_index, 0);         
						data.other = reader.getStr(i, other_index);         
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
                    CsvCommon.Log.Error("ExchangeShopData.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ExchangeShopData);
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



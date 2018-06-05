// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class FashionDefine 
    {
        static Dictionary<int, FashionDefine> DataList = new Dictionary<int, FashionDefine>();

        static public Dictionary<int, FashionDefine> GetAll()
        {
            return DataList;
        }

        static public FashionDefine Get(int key)
        {
            FashionDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("FashionDefine.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 名称
        public string name { get; set; }

        // 部位
        public int part { get; set; }

        // 类型显示
        public int fashionType { get; set; }

        // 图标
        public string icon { get; set; }

        // 职业
        public int job { get; set; }

        // 性别
        public int sex { get; set; }

        // 模型预制体名
        public string mod { get; set; }

        // 描述
        public string des { get; set; }

        // 默认色块
        public string hsv { get; set; }

        // 解锁道具
        public string item { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(FashionDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(FashionDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<FashionDefine> allDatas = new List<FashionDefine>();

            {
                string file = "Appearance/FashionDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int part_index = reader.GetIndex("part");
                int fashionType_index = reader.GetIndex("fashionType");
                int icon_index = reader.GetIndex("icon");
                int job_index = reader.GetIndex("job");
                int sex_index = reader.GetIndex("sex");
                int mod_index = reader.GetIndex("mod");
                int des_index = reader.GetIndex("des");
                int hsv_index = reader.GetIndex("hsv");
                int item_index = reader.GetIndex("item");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        FashionDefine data = new FashionDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.part = reader.getInt(i, part_index, 0);         
						data.fashionType = reader.getInt(i, fashionType_index, 0);         
						data.icon = reader.getStr(i, icon_index);         
						data.job = reader.getInt(i, job_index, 0);         
						data.sex = reader.getInt(i, sex_index, 0);         
						data.mod = reader.getStr(i, mod_index);         
						data.des = reader.getStr(i, des_index);         
						data.hsv = reader.getStr(i, hsv_index);         
						data.item = reader.getStr(i, item_index);         
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
                    CsvCommon.Log.Error("FashionDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(FashionDefine);
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



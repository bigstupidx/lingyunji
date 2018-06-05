// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EffectNameTable 
    {
        static List<EffectNameTable> DataList = new List<EffectNameTable>();
        static public List<EffectNameTable> GetAll()
        {
            return DataList;
        }

        static public EffectNameTable Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].name == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("EffectNameTable.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].name == key)
                    return i;
            }
            return -1;
        }



        // 特效名称
        public string name { get; set; }

        // 特效ID
        public int id { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EffectNameTable);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EffectNameTable);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EffectNameTable> allDatas = new List<EffectNameTable>();

            {
                string file = "Item/EffectNameTable.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int name_index = reader.GetIndex("name");
                int id_index = reader.GetIndex("id");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EffectNameTable data = new EffectNameTable();
						data.name = reader.getStr(i, name_index);         
						data.id = reader.getInt(i, id_index, 0);         
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
                    var curType = typeof(EffectNameTable);
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



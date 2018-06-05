// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class CustomPrototypeLibrary 
    {
        static Dictionary<int, CustomPrototypeLibrary> DataList = new Dictionary<int, CustomPrototypeLibrary>();

        static public Dictionary<int, CustomPrototypeLibrary> GetAll()
        {
            return DataList;
        }

        static public CustomPrototypeLibrary Get(int key)
        {
            CustomPrototypeLibrary value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("CustomPrototypeLibrary.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 名称
        public string sEquipName { get; set; }

        // 备注
        public string  Notes { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(CustomPrototypeLibrary);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(CustomPrototypeLibrary);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<CustomPrototypeLibrary> allDatas = new List<CustomPrototypeLibrary>();

            {
                string file = "Item/CustomPrototypeLibrary@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int sEquipName_index = reader.GetIndex("sEquipName");
                int  Notes_index = reader.GetIndex(" Notes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        CustomPrototypeLibrary data = new CustomPrototypeLibrary();
						data.id = reader.getInt(i, id_index, 0);         
						data.sEquipName = reader.getStr(i, sEquipName_index);         
						data. Notes = reader.getStr(i,  Notes_index);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("CustomPrototypeLibrary.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(CustomPrototypeLibrary);
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



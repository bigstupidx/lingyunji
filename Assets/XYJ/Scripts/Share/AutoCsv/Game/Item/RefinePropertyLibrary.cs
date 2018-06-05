// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RefinePropertyLibrary 
    {
        static List<RefinePropertyLibrary> DataList = new List<RefinePropertyLibrary>();
        static public List<RefinePropertyLibrary> GetAll()
        {
            return DataList;
        }

        static public RefinePropertyLibrary Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].refineId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("RefinePropertyLibrary.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].refineId == key)
                    return i;
            }
            return -1;
        }



        // 炼化id
        public int refineId { get; set; }

        // 装备部位
        public string equipmentParts { get; set; }

        // 炼化类型
        public int refineType { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RefinePropertyLibrary);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RefinePropertyLibrary);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RefinePropertyLibrary> allDatas = new List<RefinePropertyLibrary>();

            {
                string file = "Item/RefinePropertyLibrary@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int refineId_index = reader.GetIndex("refineId");
                int equipmentParts_index = reader.GetIndex("equipmentParts");
                int refineType_index = reader.GetIndex("refineType");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RefinePropertyLibrary data = new RefinePropertyLibrary();
						data.refineId = reader.getInt(i, refineId_index, 0);         
						data.equipmentParts = reader.getStr(i, equipmentParts_index);         
						data.refineType = reader.getInt(i, refineType_index, 0);         
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
            
            DataList = allDatas;

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RefinePropertyLibrary);
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



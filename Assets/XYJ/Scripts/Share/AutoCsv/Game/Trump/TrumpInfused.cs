// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TrumpInfused 
    {
        static List<TrumpInfused> DataList = new List<TrumpInfused>();
        static public List<TrumpInfused> GetAll()
        {
            return DataList;
        }

        static public TrumpInfused Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].infusedid == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("TrumpInfused.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].infusedid == key)
                    return i;
            }
            return -1;
        }



        // 注灵ID
        public int infusedid { get; set; }

        // 注灵类型
        public TrumpsInfusedType type { get; set; }

        // 法宝ID
        public int trumpid { get; set; }

        // 境界等级
        public int tastelv { get; set; }

        // 需要角色等级
        public int playlv { get; set; }

        // 注灵材料
        public InfuseMaterialData material { get; set; }

        // 位置参数
        public int posid { get; set; }

        // 前置注灵ID
        public int bindid { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TrumpInfused);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TrumpInfused);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TrumpInfused> allDatas = new List<TrumpInfused>();

            {
                string file = "Trump/TrumpInfused@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int infusedid_index = reader.GetIndex("infusedid");
                int type_index = reader.GetIndex("type");
                int trumpid_index = reader.GetIndex("trumpid");
                int tastelv_index = reader.GetIndex("tastelv");
                int playlv_index = reader.GetIndex("playlv");
                int material_index = reader.GetIndex("material");
                int posid_index = reader.GetIndex("posid");
                int bindid_index = reader.GetIndex("bindid");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TrumpInfused data = new TrumpInfused();
						data.infusedid = reader.getInt(i, infusedid_index, 0);         
						data.type = ((TrumpsInfusedType)reader.getInt(i, type_index, 0));         
						data.trumpid = reader.getInt(i, trumpid_index, 0);         
						data.tastelv = reader.getInt(i, tastelv_index, 0);         
						data.playlv = reader.getInt(i, playlv_index, 0);         
						data.material = InfuseMaterialData.InitConfig(reader.getStr(i, material_index));         
						data.posid = reader.getInt(i, posid_index, 0);         
						data.bindid = reader.getInt(i, bindid_index, 0);         
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
                    var curType = typeof(TrumpInfused);
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



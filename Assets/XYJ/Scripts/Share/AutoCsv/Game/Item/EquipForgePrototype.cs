// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EquipForgePrototype 
    {
        static List<EquipForgePrototype> DataList = new List<EquipForgePrototype>();
        static public List<EquipForgePrototype> GetAll()
        {
            return DataList;
        }

        static public EquipForgePrototype Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].equipId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("EquipForgePrototype.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].equipId == key)
                    return i;
            }
            return -1;
        }



        // 目标装备ID
        public int equipId { get; set; }

        // 目标装备类型
        public int type { get; set; }

        // 打造材料1
        public int materialId1 { get; set; }

        // 材料1数量
        public int materialNum1 { get; set; }

        // 打造材料2
        public int materialId2 { get; set; }

        // 材料2数量
        public int materialNum2 { get; set; }

        // 打造材料3
        public int materialId3 { get; set; }

        // 材料3数量
        public int materialNum3 { get; set; }

        // 材料装备
        public int materialEquip { get; set; }

        // 消耗银贝
        public int costMoney { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EquipForgePrototype);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EquipForgePrototype);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EquipForgePrototype> allDatas = new List<EquipForgePrototype>();

            {
                string file = "Item/EquipForgePrototype.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int equipId_index = reader.GetIndex("equipId");
                int type_index = reader.GetIndex("type");
                int materialId1_index = reader.GetIndex("materialId1");
                int materialNum1_index = reader.GetIndex("materialNum1");
                int materialId2_index = reader.GetIndex("materialId2");
                int materialNum2_index = reader.GetIndex("materialNum2");
                int materialId3_index = reader.GetIndex("materialId3");
                int materialNum3_index = reader.GetIndex("materialNum3");
                int materialEquip_index = reader.GetIndex("materialEquip");
                int costMoney_index = reader.GetIndex("costMoney");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EquipForgePrototype data = new EquipForgePrototype();
						data.equipId = reader.getInt(i, equipId_index, 0);         
						data.type = reader.getInt(i, type_index, 0);         
						data.materialId1 = reader.getInt(i, materialId1_index, 0);         
						data.materialNum1 = reader.getInt(i, materialNum1_index, 0);         
						data.materialId2 = reader.getInt(i, materialId2_index, 0);         
						data.materialNum2 = reader.getInt(i, materialNum2_index, 0);         
						data.materialId3 = reader.getInt(i, materialId3_index, 0);         
						data.materialNum3 = reader.getInt(i, materialNum3_index, 0);         
						data.materialEquip = reader.getInt(i, materialEquip_index, 0);         
						data.costMoney = reader.getInt(i, costMoney_index, 0);         
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
                    var curType = typeof(EquipForgePrototype);
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



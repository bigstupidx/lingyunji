// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PetProperty 
    {
        static List<PetProperty> DataList = new List<PetProperty>();
        static public List<PetProperty> GetAll()
        {
            return DataList;
        }

        static public PetProperty Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("PetProperty.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<PetProperty>> DataList_level = new Dictionary<int, List<PetProperty>>();

        static public Dictionary<int, List<PetProperty>> GetAllGroupBylevel()
        {
            return DataList_level;
        }

        static public List<PetProperty> GetGroupBylevel(int key)
        {
            List<PetProperty> value = null;
            if (DataList_level.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PetProperty.GetGroupBylevel({0}) not find!", key);
            return null;
        }


        // 等级
        public int level { get; set; }

        // 潜能点
        public int potential_point { get; set; }

        // 基础力量
        public float base_power { get; set; }

        // 基础智慧
        public float base_intelligence { get; set; }

        // 基础根骨
        public float base_root_bone { get; set; }

        // 基础体魄
        public float base_bodies { get; set; }

        // 基础敏捷
        public float base_agile { get; set; }

        // 基础身法
        public float base_body_position { get; set; }

        // 基础生命值
        public int base_life { get; set; }

        // 基础物理攻击
        public int base_physical_attack { get; set; }

        // 基础法术攻击
        public int base_magic_attack { get; set; }

        // 基础治疗强度
        public int base_treatment { get; set; }

        // 基础物理防御
        public int base_physical_defense { get; set; }

        // 基础法术防御
        public int base_magic_defense { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PetProperty);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_level.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PetProperty);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PetProperty> allDatas = new List<PetProperty>();

            {
                string file = "Pet/PetProperty.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int level_index = reader.TryIndex("level:group");
                int potential_point_index = reader.GetIndex("potential_point");
                int base_power_index = reader.GetIndex("base_power");
                int base_intelligence_index = reader.GetIndex("base_intelligence");
                int base_root_bone_index = reader.GetIndex("base_root_bone");
                int base_bodies_index = reader.GetIndex("base_bodies");
                int base_agile_index = reader.GetIndex("base_agile");
                int base_body_position_index = reader.GetIndex("base_body_position");
                int base_life_index = reader.GetIndex("base_life");
                int base_physical_attack_index = reader.GetIndex("base_physical_attack");
                int base_magic_attack_index = reader.GetIndex("base_magic_attack");
                int base_treatment_index = reader.GetIndex("base_treatment");
                int base_physical_defense_index = reader.GetIndex("base_physical_defense");
                int base_magic_defense_index = reader.GetIndex("base_magic_defense");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PetProperty data = new PetProperty();
						data.level = reader.getInt(i, level_index, 0);         
						data.potential_point = reader.getInt(i, potential_point_index, 0);         
						data.base_power = reader.getFloat(i, base_power_index, 0f);         
						data.base_intelligence = reader.getFloat(i, base_intelligence_index, 0f);         
						data.base_root_bone = reader.getFloat(i, base_root_bone_index, 0f);         
						data.base_bodies = reader.getFloat(i, base_bodies_index, 0f);         
						data.base_agile = reader.getFloat(i, base_agile_index, 0f);         
						data.base_body_position = reader.getFloat(i, base_body_position_index, 0f);         
						data.base_life = reader.getInt(i, base_life_index, 0);         
						data.base_physical_attack = reader.getInt(i, base_physical_attack_index, 0);         
						data.base_magic_attack = reader.getInt(i, base_magic_attack_index, 0);         
						data.base_treatment = reader.getInt(i, base_treatment_index, 0);         
						data.base_physical_defense = reader.getInt(i, base_physical_defense_index, 0);         
						data.base_magic_defense = reader.getInt(i, base_magic_defense_index, 0);         
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

            foreach (var data in allDatas)
            {
                {
                    List<PetProperty> l = null;
                    if (!DataList_level.TryGetValue(data.level, out l))
                    {
                        l = new List<PetProperty>();
                        DataList_level[data.level] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(PetProperty);
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



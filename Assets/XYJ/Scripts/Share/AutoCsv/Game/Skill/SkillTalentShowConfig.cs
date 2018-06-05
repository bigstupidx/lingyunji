// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillTalentShowConfig 
    {
        static List<SkillTalentShowConfig> DataList = new List<SkillTalentShowConfig>();
        static public List<SkillTalentShowConfig> GetAll()
        {
            return DataList;
        }

        static public SkillTalentShowConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("SkillTalentShowConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<SkillTalentShowConfig>> DataList_key = new Dictionary<int, List<SkillTalentShowConfig>>();

        static public Dictionary<int, List<SkillTalentShowConfig>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<SkillTalentShowConfig> GetGroupBykey(int key)
        {
            List<SkillTalentShowConfig> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillTalentShowConfig.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 职业
        public int key { get; set; }

        // 技能孔id
        public int skillPointId { get; set; }

        // 默认技能天赋系（技能同一名称不同等级为同一系）
        public int defaultSkillSeries { get; set; }

        // 关联技能孔id
        public int releSkillPointId { get; set; }

        // UI显示列
        public int colu { get; set; }

        // UI显示行
        public int row { get; set; }

        // PVP
        public int PVP { get; set; }

        // PVE
        public int PVE { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillTalentShowConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillTalentShowConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillTalentShowConfig> allDatas = new List<SkillTalentShowConfig>();

            {
                string file = "Skill/SkillTalentShowConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int skillPointId_index = reader.GetIndex("skillPointId");
                int defaultSkillSeries_index = reader.GetIndex("defaultSkillSeries");
                int releSkillPointId_index = reader.GetIndex("releSkillPointId");
                int colu_index = reader.GetIndex("colu");
                int row_index = reader.GetIndex("row");
                int PVP_index = reader.GetIndex("PVP");
                int PVE_index = reader.GetIndex("PVE");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillTalentShowConfig data = new SkillTalentShowConfig();
						data.key = reader.getInt(i, key_index, 0);         
						data.skillPointId = reader.getInt(i, skillPointId_index, 0);         
						data.defaultSkillSeries = reader.getInt(i, defaultSkillSeries_index, 0);         
						data.releSkillPointId = reader.getInt(i, releSkillPointId_index, 0);         
						data.colu = reader.getInt(i, colu_index, 0);         
						data.row = reader.getInt(i, row_index, 0);         
						data.PVP = reader.getInt(i, PVP_index, 0);         
						data.PVE = reader.getInt(i, PVE_index, 0);         
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
                    List<SkillTalentShowConfig> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<SkillTalentShowConfig>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillTalentShowConfig);
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



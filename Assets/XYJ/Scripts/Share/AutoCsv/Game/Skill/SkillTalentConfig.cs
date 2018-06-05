// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillTalentConfig 
    {
        static List<SkillTalentConfig> DataList = new List<SkillTalentConfig>();
        static public List<SkillTalentConfig> GetAll()
        {
            return DataList;
        }

        static public SkillTalentConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("SkillTalentConfig.Get({0}) not find!", key);
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

        static Dictionary<int, List<SkillTalentConfig>> DataList_key = new Dictionary<int, List<SkillTalentConfig>>();

        static public Dictionary<int, List<SkillTalentConfig>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<SkillTalentConfig> GetGroupBykey(int key)
        {
            List<SkillTalentConfig> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillTalentConfig.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 技能孔id
        public int key { get; set; }

        // 技能天赋系（技能同一名称不同等级为同一系）
        public int skillSeries { get; set; }

        // 技能id
        public int skillId { get; set; }

        // 等级描述
        public string gradeDes { get; set; }

        // 初级
        public int primary { get; set; }

        // 进阶技能id
        public int advanceSkillId { get; set; }

        // 秘籍id
        public int bookId { get; set; }

        // 效果类型
        public int effectType { get; set; }

        // 标签
        public string tag { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillTalentConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillTalentConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillTalentConfig> allDatas = new List<SkillTalentConfig>();

            {
                string file = "Skill/SkillTalentConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int skillSeries_index = reader.GetIndex("skillSeries");
                int skillId_index = reader.GetIndex("skillId");
                int gradeDes_index = reader.GetIndex("gradeDes");
                int primary_index = reader.GetIndex("primary");
                int advanceSkillId_index = reader.GetIndex("advanceSkillId");
                int bookId_index = reader.GetIndex("bookId");
                int effectType_index = reader.GetIndex("effectType");
                int tag_index = reader.GetIndex("tag");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillTalentConfig data = new SkillTalentConfig();
						data.key = reader.getInt(i, key_index, 0);         
						data.skillSeries = reader.getInt(i, skillSeries_index, 0);         
						data.skillId = reader.getInt(i, skillId_index, 0);         
						data.gradeDes = reader.getStr(i, gradeDes_index);         
						data.primary = reader.getInt(i, primary_index, 0);         
						data.advanceSkillId = reader.getInt(i, advanceSkillId_index, 0);         
						data.bookId = reader.getInt(i, bookId_index, 0);         
						data.effectType = reader.getInt(i, effectType_index, 0);         
						data.tag = reader.getStr(i, tag_index);         
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
                    List<SkillTalentConfig> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<SkillTalentConfig>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillTalentConfig);
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



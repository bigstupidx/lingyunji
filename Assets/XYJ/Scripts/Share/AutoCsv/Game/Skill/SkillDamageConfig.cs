// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillDamageConfig 
    {
        static List<SkillDamageConfig> DataList = new List<SkillDamageConfig>();
        static public List<SkillDamageConfig> GetAll()
        {
            return DataList;
        }

        static public SkillDamageConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("SkillDamageConfig.Get({0}) not find!", key);
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



        // 技能等级
        public int level { get; set; }

        // 标准伤害值
        public int damage { get; set; }

        // 标准治疗值
        public int cure { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillDamageConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillDamageConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillDamageConfig> allDatas = new List<SkillDamageConfig>();

            {
                string file = "Skill/SkillDamageConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int level_index = reader.GetIndex("level");
                int damage_index = reader.GetIndex("damage");
                int cure_index = reader.GetIndex("cure");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillDamageConfig data = new SkillDamageConfig();
						data.level = reader.getInt(i, level_index, 0);         
						data.damage = reader.getInt(i, damage_index, 0);         
						data.cure = reader.getInt(i, cure_index, 0);         
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
                    var curType = typeof(SkillDamageConfig);
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



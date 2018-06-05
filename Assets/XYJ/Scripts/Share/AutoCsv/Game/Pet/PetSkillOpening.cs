// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PetSkillOpening 
    {
        static List<PetSkillOpening> DataList = new List<PetSkillOpening>();
        static public List<PetSkillOpening> GetAll()
        {
            return DataList;
        }

        static public PetSkillOpening Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].num_skills == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("PetSkillOpening.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].num_skills == key)
                    return i;
            }
            return -1;
        }



        // 已习得技能
        public int num_skills { get; set; }

        // 开启栏位几率
        public float probability { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PetSkillOpening);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PetSkillOpening);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PetSkillOpening> allDatas = new List<PetSkillOpening>();

            {
                string file = "Pet/PetSkillOpening.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int num_skills_index = reader.GetIndex("num_skills");
                int probability_index = reader.GetIndex("probability");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PetSkillOpening data = new PetSkillOpening();
						data.num_skills = reader.getInt(i, num_skills_index, 0);         
						data.probability = reader.getFloat(i, probability_index, 0f);         
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
                    var curType = typeof(PetSkillOpening);
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



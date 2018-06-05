// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EffectConfig 
    {
        static Dictionary<string, EffectConfig> DataList = new Dictionary<string, EffectConfig>();

        static public Dictionary<string, EffectConfig> GetAll()
        {
            return DataList;
        }

        static public EffectConfig Get(string key)
        {
            EffectConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("EffectConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 文件名称
        public string m_effectName { get; set; }

        // 挂接点
        public string m_boneName { get; set; }

        // 跟随位置
        public bool m_followPos { get; set; }

        // 跟随旋转
        public bool m_followRot { get; set; }

        // 创建角度归零
        public bool m_rotCreateZero { get; set; }

        // 根据体型缩放
        public float m_scaleByBody { get; set; }

        // 材质
        public string material { get; set; }

        // 材质叠加
        public int matAddType { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EffectConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EffectConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EffectConfig> allDatas = new List<EffectConfig>();

            {
                string file = "Skill/EffectConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int m_effectName_index = reader.GetIndex("m_effectName");
                int m_boneName_index = reader.GetIndex("m_boneName");
                int m_followPos_index = reader.GetIndex("m_followPos");
                int m_followRot_index = reader.GetIndex("m_followRot");
                int m_rotCreateZero_index = reader.GetIndex("m_rotCreateZero");
                int m_scaleByBody_index = reader.GetIndex("m_scaleByBody");
                int material_index = reader.GetIndex("material");
                int matAddType_index = reader.GetIndex("matAddType");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EffectConfig data = new EffectConfig();
						data.id = reader.getStr(i, id_index);         
						data.m_effectName = reader.getStr(i, m_effectName_index);         
						data.m_boneName = reader.getStr(i, m_boneName_index);         
						data.m_followPos = reader.getBool(i, m_followPos_index, false);         
						data.m_followRot = reader.getBool(i, m_followRot_index, false);         
						data.m_rotCreateZero = reader.getBool(i, m_rotCreateZero_index, false);         
						data.m_scaleByBody = reader.getFloat(i, m_scaleByBody_index, 0f);         
						data.material = reader.getStr(i, material_index);         
						data.matAddType = reader.getInt(i, matAddType_index, 0);         
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
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("EffectConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(EffectConfig);
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



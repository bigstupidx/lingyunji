// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillEffectDesConfig 
    {
        static Dictionary<int, SkillEffectDesConfig> DataList = new Dictionary<int, SkillEffectDesConfig>();

        static public Dictionary<int, SkillEffectDesConfig> GetAll()
        {
            return DataList;
        }

        static public SkillEffectDesConfig Get(int key)
        {
            SkillEffectDesConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillEffectDesConfig.Get({0}) not find!", key);
            return null;
        }



        // 技能id
        public int id { get; set; }

        // 首要描述
        public string firstDes { get; set; }

        // block事件描述
        public string blockDes { get; set; }

        // block成功描述
        public string blockSucDes { get; set; }

        // move事件描述
        public string moveDes { get; set; }

        // omnislash事件描述
        public string omnislashDes { get; set; }

        // 击中状态描述
        public string stateDes { get; set; }

        // 伤害描述
        public string[] damageDes { get; set; }

        // buff描述
        public string[] buffDes { get; set; }

        // 技能快慢参数描述
        public string aniSpeedDes { get; set; }

        // 附加暴击率描述
        public string baojiRateDes { get; set; }

        // 无视防御描述
        public string ignoreDefDes { get; set; }

        // 条件判断效果描述
        public string[] conditionDes { get; set; }

        // 吸血描述
        public string xixueDes { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillEffectDesConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillEffectDesConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillEffectDesConfig> allDatas = new List<SkillEffectDesConfig>();

            {
                string file = "Skill/SkillEffectDesConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int firstDes_index = reader.GetIndex("firstDes");
                int blockDes_index = reader.GetIndex("blockDes");
                int blockSucDes_index = reader.GetIndex("blockSucDes");
                int moveDes_index = reader.GetIndex("moveDes");
                int omnislashDes_index = reader.GetIndex("omnislashDes");
                int stateDes_index = reader.GetIndex("stateDes");
                int damageDes_index = reader.GetIndex("damageDes");
                int buffDes_index = reader.GetIndex("buffDes");
                int aniSpeedDes_index = reader.GetIndex("aniSpeedDes");
                int baojiRateDes_index = reader.GetIndex("baojiRateDes");
                int ignoreDefDes_index = reader.GetIndex("ignoreDefDes");
                int conditionDes_index = reader.GetIndex("conditionDes");
                int xixueDes_index = reader.GetIndex("xixueDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillEffectDesConfig data = new SkillEffectDesConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.firstDes = reader.getStr(i, firstDes_index);         
						data.blockDes = reader.getStr(i, blockDes_index);         
						data.blockSucDes = reader.getStr(i, blockSucDes_index);         
						data.moveDes = reader.getStr(i, moveDes_index);         
						data.omnislashDes = reader.getStr(i, omnislashDes_index);         
						data.stateDes = reader.getStr(i, stateDes_index);         
						data.damageDes = reader.getStrs(i, damageDes_index, ';');         
						data.buffDes = reader.getStrs(i, buffDes_index, ';');         
						data.aniSpeedDes = reader.getStr(i, aniSpeedDes_index);         
						data.baojiRateDes = reader.getStr(i, baojiRateDes_index);         
						data.ignoreDefDes = reader.getStr(i, ignoreDefDes_index);         
						data.conditionDes = reader.getStrs(i, conditionDes_index, ';');         
						data.xixueDes = reader.getStr(i, xixueDes_index);         
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
                    CsvCommon.Log.Error("SkillEffectDesConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillEffectDesConfig);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PostureLogicConfig 
    {
        static Dictionary<int, PostureLogicConfig> DataList = new Dictionary<int, PostureLogicConfig>();

        static public Dictionary<int, PostureLogicConfig> GetAll()
        {
            return DataList;
        }

        static public PostureLogicConfig Get(int key)
        {
            PostureLogicConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PostureLogicConfig.Get({0}) not find!", key);
            return null;
        }



        // 姿态id
        public int id { get; set; }

        // 附加buff
        public int buffid { get; set; }

        // 进入特效
        public string enterEffect { get; set; }

        // 特效
        public string effect { get; set; }

        // 消失特效
        public string destroyEffect { get; set; }

        // 指定战斗状态
        public bool isInBattle { get; set; }

        // 强制战斗状态
        public bool forceInBattle { get; set; }

        // 受击结束姿态
        public bool behitEnd { get; set; }

        // 真气耗尽结束姿态
        public bool mpOutEnd { get; set; }

        // 结束切换姿态
        public int endToPosture { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PostureLogicConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PostureLogicConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PostureLogicConfig> allDatas = new List<PostureLogicConfig>();

            {
                string file = "Role/PostureLogicConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int buffid_index = reader.GetIndex("buffid");
                int enterEffect_index = reader.GetIndex("enterEffect");
                int effect_index = reader.GetIndex("effect");
                int destroyEffect_index = reader.GetIndex("destroyEffect");
                int isInBattle_index = reader.GetIndex("isInBattle");
                int forceInBattle_index = reader.GetIndex("forceInBattle");
                int behitEnd_index = reader.GetIndex("behitEnd");
                int mpOutEnd_index = reader.GetIndex("mpOutEnd");
                int endToPosture_index = reader.GetIndex("endToPosture");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PostureLogicConfig data = new PostureLogicConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.buffid = reader.getInt(i, buffid_index, 0);         
						data.enterEffect = reader.getStr(i, enterEffect_index);         
						data.effect = reader.getStr(i, effect_index);         
						data.destroyEffect = reader.getStr(i, destroyEffect_index);         
						data.isInBattle = reader.getBool(i, isInBattle_index, false);         
						data.forceInBattle = reader.getBool(i, forceInBattle_index, false);         
						data.behitEnd = reader.getBool(i, behitEnd_index, false);         
						data.mpOutEnd = reader.getBool(i, mpOutEnd_index, false);         
						data.endToPosture = reader.getInt(i, endToPosture_index, 0);         
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
                    CsvCommon.Log.Error("PostureLogicConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(PostureLogicConfig);
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



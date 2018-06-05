// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PostureConfig 
    {
        static Dictionary<int, PostureConfig> DataList = new Dictionary<int, PostureConfig>();

        static public Dictionary<int, PostureConfig> GetAll()
        {
            return DataList;
        }

        static public PostureConfig Get(int key)
        {
            PostureConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PostureConfig.Get({0}) not find!", key);
            return null;
        }



        // 姿态id
        public int id { get; set; }

        // UI显示名字
        public string name { get; set; }

        // 出生动作
        public string bornAni { get; set; }

        // 进入战斗动作
        public string enterBattle { get; set; }

        // 非战斗待机
        public string normalIdle { get; set; }

        // 休闲待机
        public string relaxIdle { get; set; }

        // 战斗待机
        public string battleIdle { get; set; }

        // 普通切战斗待机
        public string normalToBattleIdle { get; set; }

        // 战斗切普通待机
        public string battleToNormalIdle { get; set; }

        // 战斗待机切换姿态
        public string battleChangePosture { get; set; }

        // 普通跑步
        public string normalRun { get; set; }

        // 普通跑步停止
        public string normalRunStop { get; set; }

        // 战斗跑步
        public string battleRun { get; set; }

        // 战斗跑步停止
        public string battleRunStop { get; set; }

        // 神行
        public string fastRun { get; set; }

        // 神行停止
        public string fastRunStop { get; set; }

        // 普通跑步切换神行
        public string normalRunToFastRun { get; set; }

        // 战斗跑步切换神行
        public string battleRunToFastRun { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PostureConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PostureConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PostureConfig> allDatas = new List<PostureConfig>();

            {
                string file = "Role/PostureConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int bornAni_index = reader.GetIndex("bornAni");
                int enterBattle_index = reader.GetIndex("enterBattle");
                int normalIdle_index = reader.GetIndex("normalIdle");
                int relaxIdle_index = reader.GetIndex("relaxIdle");
                int battleIdle_index = reader.GetIndex("battleIdle");
                int normalToBattleIdle_index = reader.GetIndex("normalToBattleIdle");
                int battleToNormalIdle_index = reader.GetIndex("battleToNormalIdle");
                int battleChangePosture_index = reader.GetIndex("battleChangePosture");
                int normalRun_index = reader.GetIndex("normalRun");
                int normalRunStop_index = reader.GetIndex("normalRunStop");
                int battleRun_index = reader.GetIndex("battleRun");
                int battleRunStop_index = reader.GetIndex("battleRunStop");
                int fastRun_index = reader.GetIndex("fastRun");
                int fastRunStop_index = reader.GetIndex("fastRunStop");
                int normalRunToFastRun_index = reader.GetIndex("normalRunToFastRun");
                int battleRunToFastRun_index = reader.GetIndex("battleRunToFastRun");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PostureConfig data = new PostureConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.bornAni = reader.getStr(i, bornAni_index);         
						data.enterBattle = reader.getStr(i, enterBattle_index);         
						data.normalIdle = reader.getStr(i, normalIdle_index);         
						data.relaxIdle = reader.getStr(i, relaxIdle_index);         
						data.battleIdle = reader.getStr(i, battleIdle_index);         
						data.normalToBattleIdle = reader.getStr(i, normalToBattleIdle_index);         
						data.battleToNormalIdle = reader.getStr(i, battleToNormalIdle_index);         
						data.battleChangePosture = reader.getStr(i, battleChangePosture_index);         
						data.normalRun = reader.getStr(i, normalRun_index);         
						data.normalRunStop = reader.getStr(i, normalRunStop_index);         
						data.battleRun = reader.getStr(i, battleRun_index);         
						data.battleRunStop = reader.getStr(i, battleRunStop_index);         
						data.fastRun = reader.getStr(i, fastRun_index);         
						data.fastRunStop = reader.getStr(i, fastRunStop_index);         
						data.normalRunToFastRun = reader.getStr(i, normalRunToFastRun_index);         
						data.battleRunToFastRun = reader.getStr(i, battleRunToFastRun_index);         
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
                    CsvCommon.Log.Error("PostureConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(PostureConfig);
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



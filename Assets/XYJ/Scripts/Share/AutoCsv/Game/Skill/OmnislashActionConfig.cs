// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class OmnislashActionConfig 
    {
        static Dictionary<string, OmnislashActionConfig> DataList = new Dictionary<string, OmnislashActionConfig>();

        static public Dictionary<string, OmnislashActionConfig> GetAll()
        {
            return DataList;
        }

        static public OmnislashActionConfig Get(string key)
        {
            OmnislashActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("OmnislashActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 执行action
        public string[] actionsName { get; set; }

        // action次数
        public int actionMaxCnt { get; set; }

        // action间隔
        public float actionInterval { get; set; }

        // 单目标action次数上限
        public int objectActionMaxCnt { get; set; }

        // 特效
        public string effect { get; set; }

        // 距离
        public float distance { get; set; }

        // 角度
        public float angle { get; set; }

        // 镜头是否跟随
        public bool camerafollow { get; set; }

        // 移动方式
        public MoveType moveType { get; set; }

        // 描述
        public string des { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(OmnislashActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(OmnislashActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<OmnislashActionConfig> allDatas = new List<OmnislashActionConfig>();

            {
                string file = "Skill/OmnislashActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int actionsName_index = reader.GetIndex("actionsName");
                int actionMaxCnt_index = reader.GetIndex("actionMaxCnt");
                int actionInterval_index = reader.GetIndex("actionInterval");
                int objectActionMaxCnt_index = reader.GetIndex("objectActionMaxCnt");
                int effect_index = reader.GetIndex("effect");
                int distance_index = reader.GetIndex("distance");
                int angle_index = reader.GetIndex("angle");
                int camerafollow_index = reader.GetIndex("camerafollow");
                int moveType_index = reader.GetIndex("moveType");
                int des_index = reader.TryIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        OmnislashActionConfig data = new OmnislashActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.actionsName = reader.getStrs(i, actionsName_index, ';');         
						data.actionMaxCnt = reader.getInt(i, actionMaxCnt_index, 0);         
						data.actionInterval = reader.getFloat(i, actionInterval_index, 0f);         
						data.objectActionMaxCnt = reader.getInt(i, objectActionMaxCnt_index, 0);         
						data.effect = reader.getStr(i, effect_index);         
						data.distance = reader.getFloat(i, distance_index, 0f);         
						data.angle = reader.getFloat(i, angle_index, 0f);         
						data.camerafollow = reader.getBool(i, camerafollow_index, false);         
						data.moveType = ((MoveType)reader.getInt(i, moveType_index, 0));         
						data.des = reader.getStr(i, des_index);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Skill/OmnislashActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int actionsName_index = reader.GetIndex("actionsName");
                int actionMaxCnt_index = reader.GetIndex("actionMaxCnt");
                int actionInterval_index = reader.GetIndex("actionInterval");
                int objectActionMaxCnt_index = reader.GetIndex("objectActionMaxCnt");
                int effect_index = reader.GetIndex("effect");
                int distance_index = reader.GetIndex("distance");
                int angle_index = reader.GetIndex("angle");
                int camerafollow_index = reader.GetIndex("camerafollow");
                int moveType_index = reader.GetIndex("moveType");
                int des_index = reader.TryIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        OmnislashActionConfig data = new OmnislashActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.actionsName = reader.getStrs(i, actionsName_index, ';');         
						data.actionMaxCnt = reader.getInt(i, actionMaxCnt_index, 0);         
						data.actionInterval = reader.getFloat(i, actionInterval_index, 0f);         
						data.objectActionMaxCnt = reader.getInt(i, objectActionMaxCnt_index, 0);         
						data.effect = reader.getStr(i, effect_index);         
						data.distance = reader.getFloat(i, distance_index, 0f);         
						data.angle = reader.getFloat(i, angle_index, 0f);         
						data.camerafollow = reader.getBool(i, camerafollow_index, false);         
						data.moveType = ((MoveType)reader.getInt(i, moveType_index, 0));         
						data.des = reader.getStr(i, des_index);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Skill/OmnislashActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int actionsName_index = reader.GetIndex("actionsName");
                int actionMaxCnt_index = reader.GetIndex("actionMaxCnt");
                int actionInterval_index = reader.GetIndex("actionInterval");
                int objectActionMaxCnt_index = reader.GetIndex("objectActionMaxCnt");
                int effect_index = reader.GetIndex("effect");
                int distance_index = reader.GetIndex("distance");
                int angle_index = reader.GetIndex("angle");
                int camerafollow_index = reader.GetIndex("camerafollow");
                int moveType_index = reader.GetIndex("moveType");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        OmnislashActionConfig data = new OmnislashActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.actionsName = reader.getStrs(i, actionsName_index, ';');         
						data.actionMaxCnt = reader.getInt(i, actionMaxCnt_index, 0);         
						data.actionInterval = reader.getFloat(i, actionInterval_index, 0f);         
						data.objectActionMaxCnt = reader.getInt(i, objectActionMaxCnt_index, 0);         
						data.effect = reader.getStr(i, effect_index);         
						data.distance = reader.getFloat(i, distance_index, 0f);         
						data.angle = reader.getFloat(i, angle_index, 0f);         
						data.camerafollow = reader.getBool(i, camerafollow_index, false);         
						data.moveType = ((MoveType)reader.getInt(i, moveType_index, 0));         
						data.des = reader.getStr(i, des_index);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("OmnislashActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(OmnislashActionConfig);
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



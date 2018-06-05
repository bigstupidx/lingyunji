// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class MoveActionConfig 
    {
        static Dictionary<string, MoveActionConfig> DataList = new Dictionary<string, MoveActionConfig>();

        static public Dictionary<string, MoveActionConfig> GetAll()
        {
            return DataList;
        }

        static public MoveActionConfig Get(string key)
        {
            MoveActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("MoveActionConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public string id { get; set; }

        // 坐标原点
        public PosType posType { get; set; }

        // 坐标偏移
        public float[] posOff { get; set; }

        // 移动帧数
        public int moveFrame { get; set; }

        // 路径目标碰撞
        public float posColliderRadiu { get; set; }

        // 移动速度
        public float moveSpeed { get; set; }

        // 是否传送
        public bool teleport { get; set; }

        // 传送后是否朝向目标点
        public bool lookAtPos { get; set; }

        // 是否复位镜头
        public bool resetCamera { get; set; }

        // 描述
        public string des { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(MoveActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(MoveActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<MoveActionConfig> allDatas = new List<MoveActionConfig>();

            {
                string file = "Skill/MoveActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int posType_index = reader.GetIndex("posType");
                int posOff_index = reader.GetIndex("posOff");
                int moveFrame_index = reader.GetIndex("moveFrame");
                int posColliderRadiu_index = reader.GetIndex("posColliderRadiu");
                int moveSpeed_index = reader.GetIndex("moveSpeed");
                int teleport_index = reader.TryIndex("teleport");
                int lookAtPos_index = reader.TryIndex("lookAtPos");
                int resetCamera_index = reader.TryIndex("resetCamera");
                int des_index = reader.TryIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        MoveActionConfig data = new MoveActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.posType = ((PosType)reader.getInt(i, posType_index, 0));         
						data.posOff = reader.getFloats(i, posOff_index, ';');         
						data.moveFrame = reader.getInt(i, moveFrame_index, 0);         
						data.posColliderRadiu = reader.getFloat(i, posColliderRadiu_index, 0f);         
						data.moveSpeed = reader.getFloat(i, moveSpeed_index, 0f);         
						data.teleport = reader.getBool(i, teleport_index, false);         
						data.lookAtPos = reader.getBool(i, lookAtPos_index, false);         
						data.resetCamera = reader.getBool(i, resetCamera_index, false);         
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
                string file = "Skill/MoveActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int posType_index = reader.GetIndex("posType");
                int posOff_index = reader.GetIndex("posOff");
                int moveFrame_index = reader.GetIndex("moveFrame");
                int posColliderRadiu_index = reader.GetIndex("posColliderRadiu");
                int moveSpeed_index = reader.GetIndex("moveSpeed");
                int teleport_index = reader.TryIndex("teleport");
                int lookAtPos_index = reader.TryIndex("lookAtPos");
                int resetCamera_index = reader.TryIndex("resetCamera");
                int des_index = reader.TryIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        MoveActionConfig data = new MoveActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.posType = ((PosType)reader.getInt(i, posType_index, 0));         
						data.posOff = reader.getFloats(i, posOff_index, ';');         
						data.moveFrame = reader.getInt(i, moveFrame_index, 0);         
						data.posColliderRadiu = reader.getFloat(i, posColliderRadiu_index, 0f);         
						data.moveSpeed = reader.getFloat(i, moveSpeed_index, 0f);         
						data.teleport = reader.getBool(i, teleport_index, false);         
						data.lookAtPos = reader.getBool(i, lookAtPos_index, false);         
						data.resetCamera = reader.getBool(i, resetCamera_index, false);         
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
                string file = "Skill/MoveActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int posType_index = reader.GetIndex("posType");
                int posOff_index = reader.GetIndex("posOff");
                int moveFrame_index = reader.GetIndex("moveFrame");
                int posColliderRadiu_index = reader.GetIndex("posColliderRadiu");
                int moveSpeed_index = reader.GetIndex("moveSpeed");
                int teleport_index = reader.GetIndex("teleport");
                int lookAtPos_index = reader.GetIndex("lookAtPos");
                int resetCamera_index = reader.GetIndex("resetCamera");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        MoveActionConfig data = new MoveActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.posType = ((PosType)reader.getInt(i, posType_index, 0));         
						data.posOff = reader.getFloats(i, posOff_index, ';');         
						data.moveFrame = reader.getInt(i, moveFrame_index, 0);         
						data.posColliderRadiu = reader.getFloat(i, posColliderRadiu_index, 0f);         
						data.moveSpeed = reader.getFloat(i, moveSpeed_index, 0f);         
						data.teleport = reader.getBool(i, teleport_index, false);         
						data.lookAtPos = reader.getBool(i, lookAtPos_index, false);         
						data.resetCamera = reader.getBool(i, resetCamera_index, false);         
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
                    CsvCommon.Log.Error("MoveActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(MoveActionConfig);
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



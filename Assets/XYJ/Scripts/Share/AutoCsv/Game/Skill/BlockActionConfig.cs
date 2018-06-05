// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class BlockActionConfig 
    {
        static Dictionary<string, BlockActionConfig> DataList = new Dictionary<string, BlockActionConfig>();

        static public Dictionary<string, BlockActionConfig> GetAll()
        {
            return DataList;
        }

        static public BlockActionConfig Get(string key)
        {
            BlockActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("BlockActionConfig.Get({0}) not find!", key);
            return null;
        }



        // id
        public string id { get; set; }

        // 持续时间
        public float time { get; set; }

        // 减伤比例
        public float damageMul { get; set; }

        // 格挡动画
        public string[] animations { get; set; }

        // 格挡成功事件
        public string[] actions { get; set; }

        // 格挡成功后技能
        public int skillid { get; set; }

        // 描述
        public string des { get; set; }

        // 格挡成功描述
        public string SucDes { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(BlockActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(BlockActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<BlockActionConfig> allDatas = new List<BlockActionConfig>();

            {
                string file = "Skill/BlockActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int time_index = reader.GetIndex("time");
                int damageMul_index = reader.GetIndex("damageMul");
                int animations_index = reader.GetIndex("animations");
                int actions_index = reader.GetIndex("actions");
                int skillid_index = reader.GetIndex("skillid");
                int des_index = reader.TryIndex("des");
                int SucDes_index = reader.TryIndex("SucDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BlockActionConfig data = new BlockActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.time = reader.getFloat(i, time_index, 0f);         
						data.damageMul = reader.getFloat(i, damageMul_index, 0f);         
						data.animations = reader.getStrs(i, animations_index, ';');         
						data.actions = reader.getStrs(i, actions_index, ';');         
						data.skillid = reader.getInt(i, skillid_index, 0);         
						data.des = reader.getStr(i, des_index);         
						data.SucDes = reader.getStr(i, SucDes_index);         
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
                string file = "Skill/BlockActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int time_index = reader.GetIndex("time");
                int damageMul_index = reader.GetIndex("damageMul");
                int animations_index = reader.GetIndex("animations");
                int actions_index = reader.GetIndex("actions");
                int skillid_index = reader.GetIndex("skillid");
                int des_index = reader.TryIndex("des");
                int SucDes_index = reader.TryIndex("SucDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BlockActionConfig data = new BlockActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.time = reader.getFloat(i, time_index, 0f);         
						data.damageMul = reader.getFloat(i, damageMul_index, 0f);         
						data.animations = reader.getStrs(i, animations_index, ';');         
						data.actions = reader.getStrs(i, actions_index, ';');         
						data.skillid = reader.getInt(i, skillid_index, 0);         
						data.des = reader.getStr(i, des_index);         
						data.SucDes = reader.getStr(i, SucDes_index);         
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
                string file = "Skill/BlockActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int time_index = reader.GetIndex("time");
                int damageMul_index = reader.GetIndex("damageMul");
                int animations_index = reader.GetIndex("animations");
                int actions_index = reader.GetIndex("actions");
                int skillid_index = reader.TryIndex("skillid");
                int des_index = reader.GetIndex("des");
                int SucDes_index = reader.GetIndex("SucDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BlockActionConfig data = new BlockActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.time = reader.getFloat(i, time_index, 0f);         
						data.damageMul = reader.getFloat(i, damageMul_index, 0f);         
						data.animations = reader.getStrs(i, animations_index, ';');         
						data.actions = reader.getStrs(i, actions_index, ';');         
						data.skillid = reader.getInt(i, skillid_index, 0);         
						data.des = reader.getStr(i, des_index);         
						data.SucDes = reader.getStr(i, SucDes_index);         
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
                    CsvCommon.Log.Error("BlockActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(BlockActionConfig);
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



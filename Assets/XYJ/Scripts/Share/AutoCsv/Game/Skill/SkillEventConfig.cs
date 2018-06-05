// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillEventConfig 
    {
        static List<SkillEventConfig> DataList = new List<SkillEventConfig>();
        static public List<SkillEventConfig> GetAll()
        {
            return DataList;
        }

        static public SkillEventConfig Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("SkillEventConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<string, List<SkillEventConfig>> DataList_key = new Dictionary<string, List<SkillEventConfig>>();

        static public Dictionary<string, List<SkillEventConfig>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<SkillEventConfig> GetGroupBykey(string key)
        {
            List<SkillEventConfig> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillEventConfig.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 动作事件ID
        public string key { get; set; }

        // 执行帧
        public int beginFrame { get; set; }

        // 执行帧类型
        public FrameType frameType { get; set; }

        // action集合
        public string[] actions { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillEventConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillEventConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillEventConfig> allDatas = new List<SkillEventConfig>();

            {
                string file = "Skill/SkillEventConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int frameType_index = reader.GetIndex("frameType");
                int actions_index = reader.GetIndex("actions");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillEventConfig data = new SkillEventConfig();
						data.key = reader.getStr(i, key_index);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.frameType = ((FrameType)reader.getInt(i, frameType_index, 0));         
						data.actions = reader.getStrs(i, actions_index, ';');         
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
                string file = "Skill/SkillEventConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int frameType_index = reader.GetIndex("frameType");
                int actions_index = reader.GetIndex("actions");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillEventConfig data = new SkillEventConfig();
						data.key = reader.getStr(i, key_index);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.frameType = ((FrameType)reader.getInt(i, frameType_index, 0));         
						data.actions = reader.getStrs(i, actions_index, ';');         
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
                string file = "Skill/SkillEventConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int frameType_index = reader.GetIndex("frameType");
                int actions_index = reader.GetIndex("actions");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillEventConfig data = new SkillEventConfig();
						data.key = reader.getStr(i, key_index);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.frameType = ((FrameType)reader.getInt(i, frameType_index, 0));         
						data.actions = reader.getStrs(i, actions_index, ';');         
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
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<SkillEventConfig> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<SkillEventConfig>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillEventConfig);
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



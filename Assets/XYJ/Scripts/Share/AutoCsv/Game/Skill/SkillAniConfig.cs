// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class SkillAniConfig 
    {
        static List<SkillAniConfig> DataList = new List<SkillAniConfig>();
        static public List<SkillAniConfig> GetAll()
        {
            return DataList;
        }

        static public SkillAniConfig Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("SkillAniConfig.Get({0}) not find!", key);
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

        static Dictionary<string, List<SkillAniConfig>> DataList_key = new Dictionary<string, List<SkillAniConfig>>();

        static public Dictionary<string, List<SkillAniConfig>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<SkillAniConfig> GetGroupBykey(string key)
        {
            List<SkillAniConfig> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("SkillAniConfig.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 动作ID
        public string key { get; set; }

        // 动作名称
        public string aniName { get; set; }

        // 开始帧
        public int beginFrame { get; set; }

        // 结束帧
        public int endFrame { get; set; }

        // 动作事件
        public string eventid { get; set; }

        // 动作类型
        public AniType type { get; set; }

        // 特殊action
        public string aniAction { get; set; }

        // 动画速度
        public float aniSpeedMul { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(SkillAniConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(SkillAniConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<SkillAniConfig> allDatas = new List<SkillAniConfig>();

            {
                string file = "Skill/SkillAniConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int aniName_index = reader.GetIndex("aniName");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int endFrame_index = reader.GetIndex("endFrame");
                int eventid_index = reader.GetIndex("eventid");
                int type_index = reader.GetIndex("type");
                int aniAction_index = reader.TryIndex("aniAction");
                int aniSpeedMul_index = reader.TryIndex("aniSpeedMul");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillAniConfig data = new SkillAniConfig();
						data.key = reader.getStr(i, key_index);         
						data.aniName = reader.getStr(i, aniName_index);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.endFrame = reader.getInt(i, endFrame_index, 0);         
						data.eventid = reader.getStr(i, eventid_index);         
						data.type = ((AniType)reader.getInt(i, type_index, 0));         
						data.aniAction = reader.getStr(i, aniAction_index);         
						data.aniSpeedMul = reader.getFloat(i, aniSpeedMul_index, 0f);         
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
                string file = "Skill/SkillAniConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int aniName_index = reader.GetIndex("aniName");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int endFrame_index = reader.GetIndex("endFrame");
                int eventid_index = reader.GetIndex("eventid");
                int type_index = reader.GetIndex("type");
                int aniAction_index = reader.GetIndex("aniAction");
                int aniSpeedMul_index = reader.TryIndex("aniSpeedMul");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillAniConfig data = new SkillAniConfig();
						data.key = reader.getStr(i, key_index);         
						data.aniName = reader.getStr(i, aniName_index);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.endFrame = reader.getInt(i, endFrame_index, 0);         
						data.eventid = reader.getStr(i, eventid_index);         
						data.type = ((AniType)reader.getInt(i, type_index, 0));         
						data.aniAction = reader.getStr(i, aniAction_index);         
						data.aniSpeedMul = reader.getFloat(i, aniSpeedMul_index, 0f);         
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
                string file = "Skill/SkillAniConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int aniName_index = reader.GetIndex("aniName");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int endFrame_index = reader.GetIndex("endFrame");
                int eventid_index = reader.GetIndex("eventid");
                int type_index = reader.GetIndex("type");
                int aniAction_index = reader.TryIndex("aniAction");
                int aniSpeedMul_index = reader.GetIndex("aniSpeedMul");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        SkillAniConfig data = new SkillAniConfig();
						data.key = reader.getStr(i, key_index);         
						data.aniName = reader.getStr(i, aniName_index);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.endFrame = reader.getInt(i, endFrame_index, 0);         
						data.eventid = reader.getStr(i, eventid_index);         
						data.type = ((AniType)reader.getInt(i, type_index, 0));         
						data.aniAction = reader.getStr(i, aniAction_index);         
						data.aniSpeedMul = reader.getFloat(i, aniSpeedMul_index, 0f);         
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
                    List<SkillAniConfig> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<SkillAniConfig>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(SkillAniConfig);
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



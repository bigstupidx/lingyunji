// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class MonsterAttributeFix 
    {
        static List<MonsterAttributeFix> DataList = new List<MonsterAttributeFix>();
        static public List<MonsterAttributeFix> GetAll()
        {
            return DataList;
        }

        static public MonsterAttributeFix Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("MonsterAttributeFix.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].level == key)
                    return i;
            }
            return -1;
        }



        // 等级
        public int level { get; set; }

        // 生命修正
        public float hpfix { get; set; }

        // 攻击修正
        public float attackfix { get; set; }

        // 防御修正
        public float defensefix { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(MonsterAttributeFix);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(MonsterAttributeFix);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<MonsterAttributeFix> allDatas = new List<MonsterAttributeFix>();

            {
                string file = "Attribute/MonsterAttributeFix@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int level_index = reader.GetIndex("level");
                int hpfix_index = reader.GetIndex("hpfix");
                int attackfix_index = reader.GetIndex("attackfix");
                int defensefix_index = reader.GetIndex("defensefix");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        MonsterAttributeFix data = new MonsterAttributeFix();
						data.level = reader.getInt(i, level_index, 0);         
						data.hpfix = reader.getFloat(i, hpfix_index, 0f);         
						data.attackfix = reader.getFloat(i, attackfix_index, 0f);         
						data.defensefix = reader.getFloat(i, defensefix_index, 0f);         
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

            {
                MethodInfo method = null;
                {
                    var curType = typeof(MonsterAttributeFix);
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



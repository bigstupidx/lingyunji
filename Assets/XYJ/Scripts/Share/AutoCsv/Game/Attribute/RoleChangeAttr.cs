// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleChangeAttr 
    {
        static List<RoleChangeAttr> DataList = new List<RoleChangeAttr>();
        static public List<RoleChangeAttr> GetAll()
        {
            return DataList;
        }

        static public RoleChangeAttr Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("RoleChangeAttr.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<RoleChangeAttr>> DataList_key = new Dictionary<int, List<RoleChangeAttr>>();

        static public Dictionary<int, List<RoleChangeAttr>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<RoleChangeAttr> GetGroupBykey(int key)
        {
            List<RoleChangeAttr> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleChangeAttr.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 职业id
        public int key { get; set; }

        // 属性名称
        public string name { get; set; }

        // 物理攻击
        public float physicalAttack { get; set; }

        // 法术攻击
        public float magicAttack { get; set; }

        // 治疗强度
        public float cureRate { get; set; }

        // 生命值
        public float iHp { get; set; }

        // 物理防御
        public float physicalDefense { get; set; }

        // 法术防御
        public float magicDefense { get; set; }

        // 暴击等级
        public float critLevel { get; set; }

        // 命中等级
        public float hitLevel { get; set; }

        // 暴击防御等级
        public float critDefenseLevel { get; set; }

        // 回避等级
        public float avoidLevel { get; set; }

        // 招架等级
        public float parryLevel { get; set; }

        // 是否主属性
        public int mainAttri { get; set; }

        // 主属性描述
        public string mainAttriDesc { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleChangeAttr);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleChangeAttr);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleChangeAttr> allDatas = new List<RoleChangeAttr>();

            {
                string file = "Attribute/RoleChangeAttr@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int name_index = reader.GetIndex("name");
                int physicalAttack_index = reader.GetIndex("physicalAttack");
                int magicAttack_index = reader.GetIndex("magicAttack");
                int cureRate_index = reader.GetIndex("cureRate");
                int iHp_index = reader.GetIndex("iHp");
                int physicalDefense_index = reader.GetIndex("physicalDefense");
                int magicDefense_index = reader.GetIndex("magicDefense");
                int critLevel_index = reader.GetIndex("critLevel");
                int hitLevel_index = reader.GetIndex("hitLevel");
                int critDefenseLevel_index = reader.GetIndex("critDefenseLevel");
                int avoidLevel_index = reader.GetIndex("avoidLevel");
                int parryLevel_index = reader.GetIndex("parryLevel");
                int mainAttri_index = reader.GetIndex("mainAttri");
                int mainAttriDesc_index = reader.GetIndex("mainAttriDesc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleChangeAttr data = new RoleChangeAttr();
						data.key = reader.getInt(i, key_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.physicalAttack = reader.getFloat(i, physicalAttack_index, 0f);         
						data.magicAttack = reader.getFloat(i, magicAttack_index, 0f);         
						data.cureRate = reader.getFloat(i, cureRate_index, 0f);         
						data.iHp = reader.getFloat(i, iHp_index, 0f);         
						data.physicalDefense = reader.getFloat(i, physicalDefense_index, 0f);         
						data.magicDefense = reader.getFloat(i, magicDefense_index, 0f);         
						data.critLevel = reader.getFloat(i, critLevel_index, 0f);         
						data.hitLevel = reader.getFloat(i, hitLevel_index, 0f);         
						data.critDefenseLevel = reader.getFloat(i, critDefenseLevel_index, 0f);         
						data.avoidLevel = reader.getFloat(i, avoidLevel_index, 0f);         
						data.parryLevel = reader.getFloat(i, parryLevel_index, 0f);         
						data.mainAttri = reader.getInt(i, mainAttri_index, 0);         
						data.mainAttriDesc = reader.getStr(i, mainAttriDesc_index);         
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
                    List<RoleChangeAttr> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<RoleChangeAttr>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleChangeAttr);
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



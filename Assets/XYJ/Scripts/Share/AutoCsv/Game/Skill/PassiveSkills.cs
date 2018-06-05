// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PassiveSkills 
    {
        static Dictionary<int, PassiveSkills> DataList = new Dictionary<int, PassiveSkills>();

        static public Dictionary<int, PassiveSkills> GetAll()
        {
            return DataList;
        }

        static public PassiveSkills Get(int key)
        {
            PassiveSkills value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("PassiveSkills.Get({0}) not find!", key);
            return null;
        }



        // 技能ID
        public int id { get; set; }

        // 技能名称
        public string name { get; set; }

        // 技能类型
        public int type { get; set; }

        // 技能图标
        public string icon { get; set; }

        // 对应高级技能ID
        public int advanceSkillId { get; set; }

        // buff效果
        public int buffId { get; set; }

        // 属性效果ID
        public int attrId { get; set; }

        // 生效条件
        public int takeEffect { get; set; }

        // 触发行为
        public string triggerAction { get; set; }

        // 技能描述
        public string des { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(PassiveSkills);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(PassiveSkills);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<PassiveSkills> allDatas = new List<PassiveSkills>();

            {
                string file = "Skill/PassiveSkills@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int type_index = reader.GetIndex("type");
                int icon_index = reader.GetIndex("icon");
                int advanceSkillId_index = reader.GetIndex("advanceSkillId");
                int buffId_index = reader.GetIndex("buffId");
                int attrId_index = reader.GetIndex("attrId");
                int takeEffect_index = reader.GetIndex("takeEffect");
                int triggerAction_index = reader.GetIndex("triggerAction");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        PassiveSkills data = new PassiveSkills();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.type = reader.getInt(i, type_index, 0);         
						data.icon = reader.getStr(i, icon_index);         
						data.advanceSkillId = reader.getInt(i, advanceSkillId_index, 0);         
						data.buffId = reader.getInt(i, buffId_index, 0);         
						data.attrId = reader.getInt(i, attrId_index, 0);         
						data.takeEffect = reader.getInt(i, takeEffect_index, 0);         
						data.triggerAction = reader.getStr(i, triggerAction_index);         
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
                    CsvCommon.Log.Error("PassiveSkills.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(PassiveSkills);
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



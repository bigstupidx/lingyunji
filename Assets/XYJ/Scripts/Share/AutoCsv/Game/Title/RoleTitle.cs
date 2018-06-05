// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleTitle 
    {
        static Dictionary<int, RoleTitle> DataList = new Dictionary<int, RoleTitle>();

        static public Dictionary<int, RoleTitle> GetAll()
        {
            return DataList;
        }

        static public RoleTitle Get(int key)
        {
            RoleTitle value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleTitle.Get({0}) not find!", key);
            return null;
        }



        // 称号ID
        public int id { get; set; }

        // 称号名称
        public string name { get; set; }

        // 称号品质
        public ItemQuality quality { get; set; }

        // 特殊颜色
        public string specialEqulity { get; set; }

        // 特殊描边
        public string specialStroke { get; set; }

        // 头顶称号颜色
        public string topEquilty { get; set; }

        // 头顶称号描边
        public string topStoke { get; set; }

        // 称号分类
        public int type { get; set; }

        // 称号类型
        public TitleTimeLimit timeLimt { get; set; }

        // 持续时间
        public int duration { get; set; }

        // 等级
        public int level { get; set; }

        // 战力
        public int combatValue { get; set; }

        // 属性描述
        public string attrDesc { get; set; }

        // 获得方式描述
        public string waysToObtainDesc { get; set; }

        // 称号描述
        public string titleDesc { get; set; }

        // 激活后穿戴
        public bool autoWear { get; set; }

        // 是否显示
        public bool isShow { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleTitle);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleTitle);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleTitle> allDatas = new List<RoleTitle>();

            {
                string file = "Title/RoleTitle@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int quality_index = reader.GetIndex("quality");
                int specialEqulity_index = reader.GetIndex("specialEqulity");
                int specialStroke_index = reader.GetIndex("specialStroke");
                int topEquilty_index = reader.GetIndex("topEquilty");
                int topStoke_index = reader.GetIndex("topStoke");
                int type_index = reader.GetIndex("type");
                int timeLimt_index = reader.GetIndex("timeLimt");
                int duration_index = reader.GetIndex("duration");
                int level_index = reader.GetIndex("level");
                int combatValue_index = reader.GetIndex("combatValue");
                int attrDesc_index = reader.GetIndex("attrDesc");
                int waysToObtainDesc_index = reader.GetIndex("waysToObtainDesc");
                int titleDesc_index = reader.GetIndex("titleDesc");
                int autoWear_index = reader.GetIndex("autoWear");
                int isShow_index = reader.GetIndex("isShow");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleTitle data = new RoleTitle();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.quality = ((ItemQuality)reader.getInt(i, quality_index, 0));         
						data.specialEqulity = reader.getStr(i, specialEqulity_index);         
						data.specialStroke = reader.getStr(i, specialStroke_index);         
						data.topEquilty = reader.getStr(i, topEquilty_index);         
						data.topStoke = reader.getStr(i, topStoke_index);         
						data.type = reader.getInt(i, type_index, 0);         
						data.timeLimt = ((TitleTimeLimit)reader.getInt(i, timeLimt_index, 0));         
						data.duration = reader.getInt(i, duration_index, 0);         
						data.level = reader.getInt(i, level_index, 0);         
						data.combatValue = reader.getInt(i, combatValue_index, 0);         
						data.attrDesc = reader.getStr(i, attrDesc_index);         
						data.waysToObtainDesc = reader.getStr(i, waysToObtainDesc_index);         
						data.titleDesc = reader.getStr(i, titleDesc_index);         
						data.autoWear = reader.getBool(i, autoWear_index, false);         
						data.isShow = reader.getBool(i, isShow_index, false);         
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
                    CsvCommon.Log.Error("RoleTitle.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleTitle);
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



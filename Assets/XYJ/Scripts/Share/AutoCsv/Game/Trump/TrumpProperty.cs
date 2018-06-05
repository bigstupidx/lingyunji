// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TrumpProperty 
    {
        static Dictionary<int, TrumpProperty> DataList = new Dictionary<int, TrumpProperty>();

        static public Dictionary<int, TrumpProperty> GetAll()
        {
            return DataList;
        }

        static public TrumpProperty Get(int key)
        {
            TrumpProperty value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TrumpProperty.Get({0}) not find!", key);
            return null;
        }



        // 法宝ID
        public int id { get; set; }

        // 法宝名称
        public string name { get; set; }

        // 法宝品质
        public ItemQuality quality { get; set; }

        // 法宝模型
        public string modelname { get; set; }

        // 摄像机广角
        public int camView { get; set; }

        // 摄像机位置参数
        public float[] campos { get; set; }

        // 图标
        public string icon { get; set; }

        // 主动技能
        public int activeskill { get; set; }

        // 被动技能
        public int passiveskill { get; set; }

        // 连携ID组
        public int[] joinings { get; set; }

        // 法宝描述
        public string des { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TrumpProperty);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TrumpProperty);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TrumpProperty> allDatas = new List<TrumpProperty>();

            {
                string file = "Trump/TrumpProperty@FA.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int quality_index = reader.GetIndex("quality");
                int modelname_index = reader.GetIndex("modelname");
                int camView_index = reader.GetIndex("camView");
                int campos_index = reader.GetIndex("campos");
                int icon_index = reader.GetIndex("icon");
                int activeskill_index = reader.GetIndex("activeskill");
                int passiveskill_index = reader.GetIndex("passiveskill");
                int joinings_index = reader.GetIndex("joinings");
                int des_index = reader.GetIndex("des");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TrumpProperty data = new TrumpProperty();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.quality = ((ItemQuality)reader.getInt(i, quality_index, 0));         
						data.modelname = reader.getStr(i, modelname_index);         
						data.camView = reader.getInt(i, camView_index, 0);         
						data.campos = reader.getFloats(i, campos_index, ';');         
						data.icon = reader.getStr(i, icon_index);         
						data.activeskill = reader.getInt(i, activeskill_index, 0);         
						data.passiveskill = reader.getInt(i, passiveskill_index, 0);         
						data.joinings = reader.getInts(i, joinings_index, ';');         
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
                    CsvCommon.Log.Error("TrumpProperty.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TrumpProperty);
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



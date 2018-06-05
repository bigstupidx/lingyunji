// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class GameClanFlagColor 
    {
        static Dictionary<int, GameClanFlagColor> DataList = new Dictionary<int, GameClanFlagColor>();

        static public Dictionary<int, GameClanFlagColor> GetAll()
        {
            return DataList;
        }

        static public GameClanFlagColor Get(int key)
        {
            GameClanFlagColor value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("GameClanFlagColor.Get({0}) not find!", key);
            return null;
        }



        // 颜色id
        public int id { get; set; }

        // 字体颜色值
        public string mainColor { get; set; }

        // 描边颜色值
        public string outlineColor { get; set; }

        // 颜色代号
        public string colorName { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(GameClanFlagColor);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(GameClanFlagColor);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<GameClanFlagColor> allDatas = new List<GameClanFlagColor>();

            {
                string file = "Clan/GameClanFlagColor.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int mainColor_index = reader.GetIndex("mainColor");
                int outlineColor_index = reader.GetIndex("outlineColor");
                int colorName_index = reader.GetIndex("colorName");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        GameClanFlagColor data = new GameClanFlagColor();
						data.id = reader.getInt(i, id_index, 0);         
						data.mainColor = reader.getStr(i, mainColor_index);         
						data.outlineColor = reader.getStr(i, outlineColor_index);         
						data.colorName = reader.getStr(i, colorName_index);         
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
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("GameClanFlagColor.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(GameClanFlagColor);
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



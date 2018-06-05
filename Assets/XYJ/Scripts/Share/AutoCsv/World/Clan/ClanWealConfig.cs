// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanWealConfig 
    {
        static Dictionary<int, ClanWealConfig> DataList = new Dictionary<int, ClanWealConfig>();

        static public Dictionary<int, ClanWealConfig> GetAll()
        {
            return DataList;
        }

        static public ClanWealConfig Get(int key)
        {
            ClanWealConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ClanWealConfig.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 活动名称
        public string name { get; set; }

        // 活动图标
        public string icon { get; set; }

        // 活动描述
        public string dec { get; set; }

        // 开放等级
        public int openLv { get; set; }

        // 未开放提示
        public string openTips { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanWealConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanWealConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanWealConfig> allDatas = new List<ClanWealConfig>();

            {
                string file = "Clan/ClanWealConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int dec_index = reader.GetIndex("dec");
                int openLv_index = reader.GetIndex("openLv");
                int openTips_index = reader.GetIndex("openTips");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanWealConfig data = new ClanWealConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.dec = reader.getStr(i, dec_index);         
						data.openLv = reader.getInt(i, openLv_index, 0);         
						data.openTips = reader.getStr(i, openTips_index);         
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
                    CsvCommon.Log.Error("ClanWealConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanWealConfig);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanOiranConfig 
    {
        static Dictionary<int, ClanOiranConfig> DataList = new Dictionary<int, ClanOiranConfig>();

        static public Dictionary<int, ClanOiranConfig> GetAll()
        {
            return DataList;
        }

        static public ClanOiranConfig Get(int key)
        {
            ClanOiranConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ClanOiranConfig.Get({0}) not find!", key);
            return null;
        }



        // 职务ID
        public int id { get; set; }

        // 职务
        public string post { get; set; }

        // 报名奖励
        public int applyRw { get; set; }

        // 投票奖励
        public int voteRw { get; set; }

        // 当选奖励
        public int successRw { get; set; }

        // 上架奖励
        public int putAwayRw { get; set; }

        // 投票积分
        public int voteIntegral { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanOiranConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanOiranConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanOiranConfig> allDatas = new List<ClanOiranConfig>();

            {
                string file = "Clan/ClanOiranConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int post_index = reader.GetIndex("post");
                int applyRw_index = reader.GetIndex("applyRw");
                int voteRw_index = reader.GetIndex("voteRw");
                int successRw_index = reader.GetIndex("successRw");
                int putAwayRw_index = reader.GetIndex("putAwayRw");
                int voteIntegral_index = reader.GetIndex("voteIntegral");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanOiranConfig data = new ClanOiranConfig();
						data.id = reader.getInt(i, id_index, 0);         
						data.post = reader.getStr(i, post_index);         
						data.applyRw = reader.getInt(i, applyRw_index, 0);         
						data.voteRw = reader.getInt(i, voteRw_index, 0);         
						data.successRw = reader.getInt(i, successRw_index, 0);         
						data.putAwayRw = reader.getInt(i, putAwayRw_index, 0);         
						data.voteIntegral = reader.getInt(i, voteIntegral_index, 0);         
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
                    CsvCommon.Log.Error("ClanOiranConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanOiranConfig);
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



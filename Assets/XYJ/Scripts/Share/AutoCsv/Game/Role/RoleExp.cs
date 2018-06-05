// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleExp 
    {
        static Dictionary<int, RoleExp> DataList = new Dictionary<int, RoleExp>();

        static public Dictionary<int, RoleExp> GetAll()
        {
            return DataList;
        }

        static public RoleExp Get(int key)
        {
            RoleExp value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleExp.Get({0}) not find!", key);
            return null;
        }



        // 等级
        public int id { get; set; }

        // 角色经验
        public int player_exp { get; set; }

        // 灵兽经验
        public int pet_exp { get; set; }

        // 摇钱树基础奖励
        public int baseRewards { get; set; }

        // 摇钱树成长值价值
        public int growthNum { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleExp);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleExp);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleExp> allDatas = new List<RoleExp>();

            {
                string file = "Role/RoleExp.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:groud");
                int player_exp_index = reader.GetIndex("player_exp");
                int pet_exp_index = reader.GetIndex("pet_exp");
                int baseRewards_index = reader.GetIndex("baseRewards");
                int growthNum_index = reader.GetIndex("growthNum");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleExp data = new RoleExp();
						data.id = reader.getInt(i, id_index, 0);         
						data.player_exp = reader.getInt(i, player_exp_index, 0);         
						data.pet_exp = reader.getInt(i, pet_exp_index, 0);         
						data.baseRewards = reader.getInt(i, baseRewards_index, 0);         
						data.growthNum = reader.getInt(i, growthNum_index, 0);         
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
                    CsvCommon.Log.Error("RoleExp.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleExp);
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



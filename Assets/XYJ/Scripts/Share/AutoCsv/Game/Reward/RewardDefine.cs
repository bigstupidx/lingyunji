// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RewardDefine 
    {
        static Dictionary<int, RewardDefine> DataList = new Dictionary<int, RewardDefine>();

        static public Dictionary<int, RewardDefine> GetAll()
        {
            return DataList;
        }

        static public RewardDefine Get(int key)
        {
            RewardDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RewardDefine.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 奖励物品
        public JobItems item { get; set; }

        // 奖励银贝
        public Expression silverShell { get; set; }

        // 奖励金贝
        public Expression goldShell { get; set; }

        // 奖励碧玉
        public Expression jasperJade { get; set; }

        // 奖励经验
        public Expression exp { get; set; }

        // 奖励修为
        public Expression xiuwei { get; set; }

        // 奖励宠物经验
        public Expression pet_exp { get; set; }

        // 奖励性格值
        public Expression mettleValue { get; set; }

        // 奖励氏族贡献
        public Expression guildValue { get; set; }

        // 奖励门派贡献
        public Expression partyValue { get; set; }

        // 奖励氏族资金
        public Expression guildMoney { get; set; }

        // 奖励荣誉
        public Expression honorValue { get; set; }

        // 掉落包ID
        public int[] dropids { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RewardDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RewardDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RewardDefine> allDatas = new List<RewardDefine>();

            {
                string file = "Reward/RewardDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int item_index = reader.GetIndex("item");
                int silverShell_index = reader.GetIndex("silverShell");
                int goldShell_index = reader.GetIndex("goldShell");
                int jasperJade_index = reader.GetIndex("jasperJade");
                int exp_index = reader.GetIndex("exp");
                int xiuwei_index = reader.GetIndex("xiuwei");
                int pet_exp_index = reader.GetIndex("pet_exp");
                int mettleValue_index = reader.GetIndex("mettleValue");
                int guildValue_index = reader.GetIndex("guildValue");
                int partyValue_index = reader.GetIndex("partyValue");
                int guildMoney_index = reader.GetIndex("guildMoney");
                int honorValue_index = reader.GetIndex("honorValue");
                int dropids_index = reader.GetIndex("dropids");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RewardDefine data = new RewardDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.item = JobItems.InitConfig(reader.getStr(i, item_index));         
						data.silverShell = Expression.InitConfig(reader.getStr(i, silverShell_index));         
						data.goldShell = Expression.InitConfig(reader.getStr(i, goldShell_index));         
						data.jasperJade = Expression.InitConfig(reader.getStr(i, jasperJade_index));         
						data.exp = Expression.InitConfig(reader.getStr(i, exp_index));         
						data.xiuwei = Expression.InitConfig(reader.getStr(i, xiuwei_index));         
						data.pet_exp = Expression.InitConfig(reader.getStr(i, pet_exp_index));         
						data.mettleValue = Expression.InitConfig(reader.getStr(i, mettleValue_index));         
						data.guildValue = Expression.InitConfig(reader.getStr(i, guildValue_index));         
						data.partyValue = Expression.InitConfig(reader.getStr(i, partyValue_index));         
						data.guildMoney = Expression.InitConfig(reader.getStr(i, guildMoney_index));         
						data.honorValue = Expression.InitConfig(reader.getStr(i, honorValue_index));         
						data.dropids = reader.getInts(i, dropids_index, ';');         
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
                    CsvCommon.Log.Error("RewardDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RewardDefine);
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



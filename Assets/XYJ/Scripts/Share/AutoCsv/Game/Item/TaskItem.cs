// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskItem : ItemBase
    {
        static Dictionary<int, TaskItem> DataList = new Dictionary<int, TaskItem>();

        static public Dictionary<int, TaskItem> GetAll()
        {
            return DataList;
        }

        static public TaskItem Get(int key)
        {
            TaskItem value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskItem.Get({0}) not find!", key);
            return null;
        }



        // 是否可使用
        public bool isCanUse { get; set; }

        // 获得后自动使用
        public bool isAutoUse { get; set; }

        // 购买价格
        public int buyPrice { get; set; }

        // 触发事件
        public int eventId { get; set; }

        // 转化值下限
        public int bound { get; set; }

        // 转化值上限
        public int upper { get; set; }

        // 吟唱时间
        public int singTimer { get; set; }

        // 奖励ID
        public int rewardId { get; set; }

        // 获得宠物技能
        public int petSkill { get; set; }

        // 激活宠物
        public int actPet { get; set; }

        // 激活法宝
        public int actWeapon { get; set; }

        // 获得状态
        public int status { get; set; }

        // 获得法宝强化经验
        public int weaponExp { get; set; }

        // 宝图-点集
        public int map { get; set; }

        // 用途
        public string use { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskItem);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskItem);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskItem> allDatas = new List<TaskItem>();

            {
                string file = "Item/Item-TaskItem.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int type_index = reader.GetIndex("type");
                int sonType_index = reader.GetIndex("sonType");
                int quality_index = reader.GetIndex("quality");
                int job_index = reader.GetIndex("job");
                int isCanUse_index = reader.GetIndex("isCanUse");
                int isBind_index = reader.GetIndex("isBind");
                int isAutoUse_index = reader.GetIndex("isAutoUse");
                int useLevel_index = reader.GetIndex("useLevel");
                int buyPrice_index = reader.GetIndex("buyPrice");
                int stackNum_index = reader.GetIndex("stackNum");
                int eventId_index = reader.GetIndex("eventId");
                int bound_index = reader.GetIndex("bound");
                int upper_index = reader.GetIndex("upper");
                int singTimer_index = reader.GetIndex("singTimer");
                int rewardId_index = reader.GetIndex("rewardId");
                int petSkill_index = reader.GetIndex("petSkill");
                int actPet_index = reader.GetIndex("actPet");
                int actWeapon_index = reader.GetIndex("actWeapon");
                int status_index = reader.GetIndex("status");
                int weaponExp_index = reader.GetIndex("weaponExp");
                int map_index = reader.GetIndex("map");
                int icon_index = reader.GetIndex("icon");
                int use_index = reader.GetIndex("use");
                int  desc_index = reader.GetIndex(" desc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskItem data = new TaskItem();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.type = ((ItemType)reader.getInt(i, type_index, 0));         
						data.sonType = reader.getInt(i, sonType_index, 0);         
						data.quality = ((ItemQuality)reader.getInt(i, quality_index, 0));         
						data.job = JobMask.InitConfig(reader.getStr(i, job_index));         
						data.isCanUse = reader.getBool(i, isCanUse_index, false);         
						data.isBind = reader.getBool(i, isBind_index, false);         
						data.isAutoUse = reader.getBool(i, isAutoUse_index, false);         
						data.useLevel = reader.getInt(i, useLevel_index, 0);         
						data.buyPrice = reader.getInt(i, buyPrice_index, 0);         
						data.stackNum = reader.getInt(i, stackNum_index, 0);         
						data.eventId = reader.getInt(i, eventId_index, 0);         
						data.bound = reader.getInt(i, bound_index, 0);         
						data.upper = reader.getInt(i, upper_index, 0);         
						data.singTimer = reader.getInt(i, singTimer_index, 0);         
						data.rewardId = reader.getInt(i, rewardId_index, 0);         
						data.petSkill = reader.getInt(i, petSkill_index, 0);         
						data.actPet = reader.getInt(i, actPet_index, 0);         
						data.actWeapon = reader.getInt(i, actWeapon_index, 0);         
						data.status = reader.getInt(i, status_index, 0);         
						data.weaponExp = reader.getInt(i, weaponExp_index, 0);         
						data.map = reader.getInt(i, map_index, 0);         
						data.icon = reader.getStr(i, icon_index);         
						data.use = reader.getStr(i, use_index);         
						data. desc = reader.getStr(i,  desc_index);         
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
                    CsvCommon.Log.Error("TaskItem.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskItem);
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



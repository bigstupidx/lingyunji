// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskEventDefine 
    {
        static Dictionary<int, TaskEventDefine> DataList = new Dictionary<int, TaskEventDefine>();

        static public Dictionary<int, TaskEventDefine> GetAll()
        {
            return DataList;
        }

        static public TaskEventDefine Get(int key)
        {
            TaskEventDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskEventDefine.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 回收物品
        public string recycleItems { get; set; }

        // 对白
        public int playDialogId { get; set; }

        // 玩法确认对话
        public int dialogGroupId { get; set; }

        // 冒泡
        public string playBubbleText { get; set; }

        // 播放动作
        public string playAniName { get; set; }

        // 播放特效
        public string playEffectName { get; set; }

        // 播放客户端剧本
        public string playStoryId { get; set; }

        // 播放动画
        public int playCGId { get; set; }

        // 展示模型
        public int showModel { get; set; }

        // 获得物品
        public string getItems { get; set; }

        // 奖励ID
        public int getRewardId { get; set; }

        // 获得任务计数
        public string getTaskCount { get; set; }

        // 获得任务
        public int getTaskId { get; set; }

        // 提交任务
        public int submitTaskId { get; set; }

        // 触发区域
        public string areaId { get; set; }

        // 触发npc
        public int roleId { get; set; }

        // 触发道具
        public int itemId { get; set; }

        // 触发任务
        public int taskId { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskEventDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskEventDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskEventDefine> allDatas = new List<TaskEventDefine>();

            {
                string file = "Task/TaskEventDefine#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int recycleItems_index = reader.GetIndex("recycleItems");
                int playDialogId_index = reader.GetIndex("playDialogId");
                int dialogGroupId_index = reader.GetIndex("dialogGroupId");
                int playBubbleText_index = reader.GetIndex("playBubbleText");
                int playAniName_index = reader.GetIndex("playAniName");
                int playEffectName_index = reader.GetIndex("playEffectName");
                int playStoryId_index = reader.GetIndex("playStoryId");
                int playCGId_index = reader.GetIndex("playCGId");
                int showModel_index = reader.GetIndex("showModel");
                int getItems_index = reader.GetIndex("getItems");
                int getRewardId_index = reader.GetIndex("getRewardId");
                int getTaskCount_index = reader.GetIndex("getTaskCount");
                int getTaskId_index = reader.GetIndex("getTaskId");
                int submitTaskId_index = reader.GetIndex("submitTaskId");
                int areaId_index = reader.GetIndex("areaId");
                int roleId_index = reader.GetIndex("roleId");
                int itemId_index = reader.GetIndex("itemId");
                int taskId_index = reader.GetIndex("taskId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskEventDefine data = new TaskEventDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.recycleItems = reader.getStr(i, recycleItems_index);         
						data.playDialogId = reader.getInt(i, playDialogId_index, 0);         
						data.dialogGroupId = reader.getInt(i, dialogGroupId_index, 0);         
						data.playBubbleText = reader.getStr(i, playBubbleText_index);         
						data.playAniName = reader.getStr(i, playAniName_index);         
						data.playEffectName = reader.getStr(i, playEffectName_index);         
						data.playStoryId = reader.getStr(i, playStoryId_index);         
						data.playCGId = reader.getInt(i, playCGId_index, 0);         
						data.showModel = reader.getInt(i, showModel_index, 0);         
						data.getItems = reader.getStr(i, getItems_index);         
						data.getRewardId = reader.getInt(i, getRewardId_index, 0);         
						data.getTaskCount = reader.getStr(i, getTaskCount_index);         
						data.getTaskId = reader.getInt(i, getTaskId_index, 0);         
						data.submitTaskId = reader.getInt(i, submitTaskId_index, 0);         
						data.areaId = reader.getStr(i, areaId_index);         
						data.roleId = reader.getInt(i, roleId_index, 0);         
						data.itemId = reader.getInt(i, itemId_index, 0);         
						data.taskId = reader.getInt(i, taskId_index, 0);         
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
                    CsvCommon.Log.Error("TaskEventDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskEventDefine);
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



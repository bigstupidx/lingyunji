// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ActivityDefine 
    {
        static Dictionary<int, ActivityDefine> DataList = new Dictionary<int, ActivityDefine>();

        static public Dictionary<int, ActivityDefine> GetAll()
        {
            return DataList;
        }

        static public ActivityDefine Get(int key)
        {
            ActivityDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ActivityDefine.Get({0}) not find!", key);
            return null;
        }



        // 活动ID
        public int id { get; set; }

        // 不开放
        public int notOpen { get; set; }

        // 活动名称
        public string name { get; set; }

        // 活动图标
        public string icon { get; set; }

        // 周日期
        public int[] openDayOfWeek { get; set; }

        // 通知开放周期
        public string openDateDesc { get; set; }

        // 人数限制
        public int pepoleLimit { get; set; }

        // 等级需求
        public int requireLv { get; set; }

        // 活动归类
        public int classify { get; set; }

        // 时间表ID
        public int timeId { get; set; }

        // 答题ID
        public int knowledgeId { get; set; }

        // 环任务ID
        public int taskId { get; set; }

        // 刷怪ID
        public int[] monsterId { get; set; }

        // 关联npciID
        public int npcId { get; set; }

        // 跳转场景
        public int sceneId { get; set; }

        // 打开界面
        public string panelName { get; set; }

        // 前尘往事
        public int oldThing { get; set; }

        // 是否关卡活动
        public bool copyId { get; set; }

        // 活动有效次数
        public float maxNum { get; set; }

        // 活动类型
        public int activityType { get; set; }

        // 标记
        public int mark { get; set; }

        // 活跃度上限
        public float maxActiveness { get; set; }

        // 单次活跃度
        public float activeness { get; set; }

        // 活动开始前的系统滚动公告ID
        public int announceId { get; set; }

        // 奖励展示
        public int[] prize { get; set; }

        // 活动描述
        public string desc { get; set; }

        // 活动等级解锁描述
        public string unlockDesc { get; set; }

        // 活动开始前2分钟确认提示描述
        public string openDesc { get; set; }

        // 活动默认推送
        public int isPush { get; set; }

        // 活动次数满后是否显示参加按钮
        public int isShowEnter { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ActivityDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ActivityDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ActivityDefine> allDatas = new List<ActivityDefine>();

            {
                string file = "Level/ActivityDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int notOpen_index = reader.GetIndex("notOpen");
                int name_index = reader.GetIndex("name");
                int icon_index = reader.GetIndex("icon");
                int openDayOfWeek_index = reader.GetIndex("openDayOfWeek");
                int openDateDesc_index = reader.GetIndex("openDateDesc");
                int pepoleLimit_index = reader.GetIndex("pepoleLimit");
                int requireLv_index = reader.GetIndex("requireLv");
                int classify_index = reader.GetIndex("classify");
                int timeId_index = reader.GetIndex("timeId");
                int knowledgeId_index = reader.GetIndex("knowledgeId");
                int taskId_index = reader.GetIndex("taskId");
                int monsterId_index = reader.GetIndex("monsterId");
                int npcId_index = reader.GetIndex("npcId");
                int sceneId_index = reader.GetIndex("sceneId");
                int panelName_index = reader.GetIndex("panelName");
                int oldThing_index = reader.GetIndex("oldThing");
                int copyId_index = reader.GetIndex("copyId");
                int maxNum_index = reader.GetIndex("maxNum");
                int activityType_index = reader.GetIndex("activityType");
                int mark_index = reader.GetIndex("mark");
                int maxActiveness_index = reader.GetIndex("maxActiveness");
                int activeness_index = reader.GetIndex("activeness");
                int announceId_index = reader.GetIndex("announceId");
                int prize_index = reader.GetIndex("prize");
                int desc_index = reader.GetIndex("desc");
                int unlockDesc_index = reader.GetIndex("unlockDesc");
                int openDesc_index = reader.GetIndex("openDesc");
                int isPush_index = reader.GetIndex("isPush");
                int isShowEnter_index = reader.GetIndex("isShowEnter");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ActivityDefine data = new ActivityDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.notOpen = reader.getInt(i, notOpen_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.openDayOfWeek = reader.getInts(i, openDayOfWeek_index, ';');         
						data.openDateDesc = reader.getStr(i, openDateDesc_index);         
						data.pepoleLimit = reader.getInt(i, pepoleLimit_index, 0);         
						data.requireLv = reader.getInt(i, requireLv_index, 0);         
						data.classify = reader.getInt(i, classify_index, 0);         
						data.timeId = reader.getInt(i, timeId_index, 0);         
						data.knowledgeId = reader.getInt(i, knowledgeId_index, 0);         
						data.taskId = reader.getInt(i, taskId_index, 0);         
						data.monsterId = reader.getInts(i, monsterId_index, ';');         
						data.npcId = reader.getInt(i, npcId_index, 0);         
						data.sceneId = reader.getInt(i, sceneId_index, 0);         
						data.panelName = reader.getStr(i, panelName_index);         
						data.oldThing = reader.getInt(i, oldThing_index, 0);         
						data.copyId = reader.getBool(i, copyId_index, false);         
						data.maxNum = reader.getFloat(i, maxNum_index, 0f);         
						data.activityType = reader.getInt(i, activityType_index, 0);         
						data.mark = reader.getInt(i, mark_index, 0);         
						data.maxActiveness = reader.getFloat(i, maxActiveness_index, 0f);         
						data.activeness = reader.getFloat(i, activeness_index, 0f);         
						data.announceId = reader.getInt(i, announceId_index, 0);         
						data.prize = reader.getInts(i, prize_index, ';');         
						data.desc = reader.getStr(i, desc_index);         
						data.unlockDesc = reader.getStr(i, unlockDesc_index);         
						data.openDesc = reader.getStr(i, openDesc_index);         
						data.isPush = reader.getInt(i, isPush_index, 0);         
						data.isShowEnter = reader.getInt(i, isShowEnter_index, 0);         
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
                    CsvCommon.Log.Error("ActivityDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ActivityDefine);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TaskDialogDefine 
    {
        static List<TaskDialogDefine> DataList = new List<TaskDialogDefine>();
        static public List<TaskDialogDefine> GetAll()
        {
            return DataList;
        }

        static public TaskDialogDefine Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].groupId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("TaskDialogDefine.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].groupId == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<TaskDialogDefine>> DataList_groupId = new Dictionary<int, List<TaskDialogDefine>>();

        static public Dictionary<int, List<TaskDialogDefine>> GetAllGroupBygroupId()
        {
            return DataList_groupId;
        }

        static public List<TaskDialogDefine> GetGroupBygroupId(int key)
        {
            List<TaskDialogDefine> value = null;
            if (DataList_groupId.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TaskDialogDefine.GetGroupBygroupId({0}) not find!", key);
            return null;
        }


        // ID
        public int groupId { get; set; }

        // 对白编号
        public int numIdx { get; set; }

        // 说话npc
        public int roleId { get; set; }

        // 头像
        public string roleIcon { get; set; }

        // 名字
        public string roleName { get; set; }

        // 对白表现类型
        public int baseType { get; set; }

        // 是否屏蔽其他操作
        public bool shieldOpt { get; set; }

        // 对白表现
        public string aniName { get; set; }

        // 对白内容
        public string content { get; set; }

        // 自动播放时长
        public float autoPlayTime { get; set; }

        // 结束标记
        public bool isGroupEnd { get; set; }

        // 调用镜头
        public int camType { get; set; }

        // 选项界面样式
        public int uiType { get; set; }

        // 选项1
        public string optName1 { get; set; }

        // 选项1描述
        public string optDesc1 { get; set; }

        // 选项1后续对白
        public int nextNumIdx1 { get; set; }

        // 选项1关闭对话
        public bool optIsClose1 { get; set; }

        // 选项1确认
        public int optResult1 { get; set; }

        // 选项2
        public string optName2 { get; set; }

        // 选项2描述
        public string optDesc2 { get; set; }

        // 选项2后续对白
        public int nextNumIdx2 { get; set; }

        // 选项2关闭对话
        public bool optIsClose2 { get; set; }

        // 选项2确认
        public int optResult2 { get; set; }

        // 选项3
        public string optName3 { get; set; }

        // 选项3后续对白
        public int nextNumIdx3 { get; set; }

        // 选项3关闭对话
        public bool optIsClose3 { get; set; }

        // 选项3确认
        public int optResult3 { get; set; }

        // 选项4
        public string optName4 { get; set; }

        // 选项4后续对白
        public int nextNumIdx4 { get; set; }

        // 选项4关闭对话
        public bool optIsClose4 { get; set; }

        // 选项4确认
        public int optResult4 { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TaskDialogDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_groupId.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TaskDialogDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TaskDialogDefine> allDatas = new List<TaskDialogDefine>();

            {
                string file = "Task/TaskDialogDefine#PY.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int groupId_index = reader.TryIndex("groupId:group");
                int numIdx_index = reader.GetIndex("numIdx");
                int roleId_index = reader.GetIndex("roleId");
                int roleIcon_index = reader.GetIndex("roleIcon");
                int roleName_index = reader.GetIndex("roleName");
                int baseType_index = reader.GetIndex("baseType");
                int shieldOpt_index = reader.GetIndex("shieldOpt");
                int aniName_index = reader.GetIndex("aniName");
                int content_index = reader.GetIndex("content");
                int autoPlayTime_index = reader.GetIndex("autoPlayTime");
                int isGroupEnd_index = reader.GetIndex("isGroupEnd");
                int camType_index = reader.GetIndex("camType");
                int uiType_index = reader.GetIndex("uiType");
                int optName1_index = reader.GetIndex("optName1");
                int optDesc1_index = reader.GetIndex("optDesc1");
                int nextNumIdx1_index = reader.GetIndex("nextNumIdx1");
                int optIsClose1_index = reader.GetIndex("optIsClose1");
                int optResult1_index = reader.GetIndex("optResult1");
                int optName2_index = reader.GetIndex("optName2");
                int optDesc2_index = reader.GetIndex("optDesc2");
                int nextNumIdx2_index = reader.GetIndex("nextNumIdx2");
                int optIsClose2_index = reader.GetIndex("optIsClose2");
                int optResult2_index = reader.GetIndex("optResult2");
                int optName3_index = reader.GetIndex("optName3");
                int nextNumIdx3_index = reader.GetIndex("nextNumIdx3");
                int optIsClose3_index = reader.GetIndex("optIsClose3");
                int optResult3_index = reader.GetIndex("optResult3");
                int optName4_index = reader.GetIndex("optName4");
                int nextNumIdx4_index = reader.GetIndex("nextNumIdx4");
                int optIsClose4_index = reader.GetIndex("optIsClose4");
                int optResult4_index = reader.GetIndex("optResult4");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskDialogDefine data = new TaskDialogDefine();
						data.groupId = reader.getInt(i, groupId_index, 0);         
						data.numIdx = reader.getInt(i, numIdx_index, 0);         
						data.roleId = reader.getInt(i, roleId_index, 0);         
						data.roleIcon = reader.getStr(i, roleIcon_index);         
						data.roleName = reader.getStr(i, roleName_index);         
						data.baseType = reader.getInt(i, baseType_index, 0);         
						data.shieldOpt = reader.getBool(i, shieldOpt_index, true);         
						data.aniName = reader.getStr(i, aniName_index);         
						data.content = reader.getStr(i, content_index);         
						data.autoPlayTime = reader.getFloat(i, autoPlayTime_index, 0f);         
						data.isGroupEnd = reader.getBool(i, isGroupEnd_index, false);         
						data.camType = reader.getInt(i, camType_index, 0);         
						data.uiType = reader.getInt(i, uiType_index, 0);         
						data.optName1 = reader.getStr(i, optName1_index);         
						data.optDesc1 = reader.getStr(i, optDesc1_index);         
						data.nextNumIdx1 = reader.getInt(i, nextNumIdx1_index, 0);         
						data.optIsClose1 = reader.getBool(i, optIsClose1_index, false);         
						data.optResult1 = reader.getInt(i, optResult1_index, 0);         
						data.optName2 = reader.getStr(i, optName2_index);         
						data.optDesc2 = reader.getStr(i, optDesc2_index);         
						data.nextNumIdx2 = reader.getInt(i, nextNumIdx2_index, 0);         
						data.optIsClose2 = reader.getBool(i, optIsClose2_index, false);         
						data.optResult2 = reader.getInt(i, optResult2_index, 0);         
						data.optName3 = reader.getStr(i, optName3_index);         
						data.nextNumIdx3 = reader.getInt(i, nextNumIdx3_index, 0);         
						data.optIsClose3 = reader.getBool(i, optIsClose3_index, false);         
						data.optResult3 = reader.getInt(i, optResult3_index, 0);         
						data.optName4 = reader.getStr(i, optName4_index);         
						data.nextNumIdx4 = reader.getInt(i, nextNumIdx4_index, 0);         
						data.optIsClose4 = reader.getBool(i, optIsClose4_index, false);         
						data.optResult4 = reader.getInt(i, optResult4_index, 0);         
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
            {
                string file = "Task/TaskDialogDefine#Task.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int groupId_index = reader.TryIndex("groupId:group");
                int numIdx_index = reader.GetIndex("numIdx");
                int roleId_index = reader.GetIndex("roleId");
                int roleIcon_index = reader.GetIndex("roleIcon");
                int roleName_index = reader.GetIndex("roleName");
                int baseType_index = reader.GetIndex("baseType");
                int shieldOpt_index = reader.GetIndex("shieldOpt");
                int aniName_index = reader.GetIndex("aniName");
                int content_index = reader.GetIndex("content");
                int autoPlayTime_index = reader.GetIndex("autoPlayTime");
                int isGroupEnd_index = reader.GetIndex("isGroupEnd");
                int camType_index = reader.GetIndex("camType");
                int uiType_index = reader.GetIndex("uiType");
                int optName1_index = reader.GetIndex("optName1");
                int optDesc1_index = reader.GetIndex("optDesc1");
                int nextNumIdx1_index = reader.GetIndex("nextNumIdx1");
                int optIsClose1_index = reader.GetIndex("optIsClose1");
                int optResult1_index = reader.GetIndex("optResult1");
                int optName2_index = reader.GetIndex("optName2");
                int optDesc2_index = reader.GetIndex("optDesc2");
                int nextNumIdx2_index = reader.GetIndex("nextNumIdx2");
                int optIsClose2_index = reader.GetIndex("optIsClose2");
                int optResult2_index = reader.GetIndex("optResult2");
                int optName3_index = reader.GetIndex("optName3");
                int nextNumIdx3_index = reader.GetIndex("nextNumIdx3");
                int optIsClose3_index = reader.GetIndex("optIsClose3");
                int optResult3_index = reader.GetIndex("optResult3");
                int optName4_index = reader.GetIndex("optName4");
                int nextNumIdx4_index = reader.GetIndex("nextNumIdx4");
                int optIsClose4_index = reader.GetIndex("optIsClose4");
                int optResult4_index = reader.GetIndex("optResult4");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TaskDialogDefine data = new TaskDialogDefine();
						data.groupId = reader.getInt(i, groupId_index, 0);         
						data.numIdx = reader.getInt(i, numIdx_index, 0);         
						data.roleId = reader.getInt(i, roleId_index, 0);         
						data.roleIcon = reader.getStr(i, roleIcon_index);         
						data.roleName = reader.getStr(i, roleName_index);         
						data.baseType = reader.getInt(i, baseType_index, 0);         
						data.shieldOpt = reader.getBool(i, shieldOpt_index, true);         
						data.aniName = reader.getStr(i, aniName_index);         
						data.content = reader.getStr(i, content_index);         
						data.autoPlayTime = reader.getFloat(i, autoPlayTime_index, 0f);         
						data.isGroupEnd = reader.getBool(i, isGroupEnd_index, false);         
						data.camType = reader.getInt(i, camType_index, 0);         
						data.uiType = reader.getInt(i, uiType_index, 0);         
						data.optName1 = reader.getStr(i, optName1_index);         
						data.optDesc1 = reader.getStr(i, optDesc1_index);         
						data.nextNumIdx1 = reader.getInt(i, nextNumIdx1_index, 0);         
						data.optIsClose1 = reader.getBool(i, optIsClose1_index, false);         
						data.optResult1 = reader.getInt(i, optResult1_index, 0);         
						data.optName2 = reader.getStr(i, optName2_index);         
						data.optDesc2 = reader.getStr(i, optDesc2_index);         
						data.nextNumIdx2 = reader.getInt(i, nextNumIdx2_index, 0);         
						data.optIsClose2 = reader.getBool(i, optIsClose2_index, false);         
						data.optResult2 = reader.getInt(i, optResult2_index, 0);         
						data.optName3 = reader.getStr(i, optName3_index);         
						data.nextNumIdx3 = reader.getInt(i, nextNumIdx3_index, 0);         
						data.optIsClose3 = reader.getBool(i, optIsClose3_index, false);         
						data.optResult3 = reader.getInt(i, optResult3_index, 0);         
						data.optName4 = reader.getStr(i, optName4_index);         
						data.nextNumIdx4 = reader.getInt(i, nextNumIdx4_index, 0);         
						data.optIsClose4 = reader.getBool(i, optIsClose4_index, false);         
						data.optResult4 = reader.getInt(i, optResult4_index, 0);         
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
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<TaskDialogDefine> l = null;
                    if (!DataList_groupId.TryGetValue(data.groupId, out l))
                    {
                        l = new List<TaskDialogDefine>();
                        DataList_groupId[data.groupId] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(TaskDialogDefine);
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



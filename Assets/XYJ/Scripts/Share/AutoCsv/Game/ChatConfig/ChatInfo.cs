// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ChatInfo 
    {
        static Dictionary<int, ChatInfo> DataList = new Dictionary<int, ChatInfo>();

        static public Dictionary<int, ChatInfo> GetAll()
        {
            return DataList;
        }

        static public ChatInfo Get(int key)
        {
            ChatInfo value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatInfo.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<ChatInfo>> DataList_id = new Dictionary<int, List<ChatInfo>>();

        static public Dictionary<int, List<ChatInfo>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<ChatInfo> GetGroupByid(int key)
        {
            List<ChatInfo> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatInfo.GetGroupByid({0}) not find!", key);
            return null;
        }
        static Dictionary<string, List<ChatInfo>> DataList_name = new Dictionary<string, List<ChatInfo>>();

        static public Dictionary<string, List<ChatInfo>> GetAllGroupByname()
        {
            return DataList_name;
        }

        static public List<ChatInfo> GetGroupByname(string key)
        {
            List<ChatInfo> value = null;
            if (DataList_name.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatInfo.GetGroupByname({0}) not find!", key);
            return null;
        }


        // ID
        public int id { get; set; }

        // name
        public string name { get; set; }

        // 备注
        public string desc { get; set; }

        // 系统频道
        public bool showInSystemChannel { get; set; }

        // 队伍频道
        public bool showInTeamChannel { get; set; }

        // 组队频道
        public bool teamSay { get; set; }

        // 世界频道
        public bool showInWorldChannel { get; set; }

        // 帮派频道
        public bool showInGuildChannel { get; set; }

        // 战斗频道
        public bool n_showInBattleChannel { get; set; }

        // 跑马灯
        public bool showInHorseLantern { get; set; }

        // 系统消息
        public bool showInFriendSystemMessage { get; set; }

        // 飘字提示
        public bool n_showTip { get; set; }

        // 离线通知
        public bool offlinenotice { get; set; }

        // 是否群发
        public bool needBroadCast { get; set; }

        // 是否弹窗提示
        public bool needPopUpBox { get; set; }

        // 字段数量
        public int propertyCount { get; set; }

        // 信息描述
        public string messageDes { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ChatInfo);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            DataList_name.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ChatInfo);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ChatInfo> allDatas = new List<ChatInfo>();

            {
                string file = "ChatConfig/ChatInfo.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int name_index = reader.TryIndex("name:group");
                int desc_index = reader.GetIndex("desc");
                int showInSystemChannel_index = reader.GetIndex("showInSystemChannel");
                int showInTeamChannel_index = reader.GetIndex("showInTeamChannel");
                int teamSay_index = reader.GetIndex("teamSay");
                int showInWorldChannel_index = reader.GetIndex("showInWorldChannel");
                int showInGuildChannel_index = reader.GetIndex("showInGuildChannel");
                int n_showInBattleChannel_index = reader.GetIndex("n_showInBattleChannel");
                int showInHorseLantern_index = reader.GetIndex("showInHorseLantern");
                int showInFriendSystemMessage_index = reader.GetIndex("showInFriendSystemMessage");
                int n_showTip_index = reader.GetIndex("n_showTip");
                int offlinenotice_index = reader.GetIndex("offlinenotice");
                int needBroadCast_index = reader.GetIndex("needBroadCast");
                int needPopUpBox_index = reader.GetIndex("needPopUpBox");
                int propertyCount_index = reader.GetIndex("propertyCount");
                int messageDes_index = reader.GetIndex("messageDes");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ChatInfo data = new ChatInfo();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.desc = reader.getStr(i, desc_index);         
						data.showInSystemChannel = reader.getBool(i, showInSystemChannel_index, false);         
						data.showInTeamChannel = reader.getBool(i, showInTeamChannel_index, false);         
						data.teamSay = reader.getBool(i, teamSay_index, false);         
						data.showInWorldChannel = reader.getBool(i, showInWorldChannel_index, false);         
						data.showInGuildChannel = reader.getBool(i, showInGuildChannel_index, false);         
						data.n_showInBattleChannel = reader.getBool(i, n_showInBattleChannel_index, false);         
						data.showInHorseLantern = reader.getBool(i, showInHorseLantern_index, false);         
						data.showInFriendSystemMessage = reader.getBool(i, showInFriendSystemMessage_index, false);         
						data.n_showTip = reader.getBool(i, n_showTip_index, false);         
						data.offlinenotice = reader.getBool(i, offlinenotice_index, false);         
						data.needBroadCast = reader.getBool(i, needBroadCast_index, false);         
						data.needPopUpBox = reader.getBool(i, needPopUpBox_index, false);         
						data.propertyCount = reader.getInt(i, propertyCount_index, 0);         
						data.messageDes = reader.getStr(i, messageDes_index);         
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
                    CsvCommon.Log.Error("ChatInfo.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<ChatInfo> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<ChatInfo>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
                {
                    List<ChatInfo> l = null;
                    if (!DataList_name.TryGetValue(data.name, out l))
                    {
                        l = new List<ChatInfo>();
                        DataList_name[data.name] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(ChatInfo);
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



// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ChatChannel 
    {
        static Dictionary<int, ChatChannel> DataList = new Dictionary<int, ChatChannel>();

        static public Dictionary<int, ChatChannel> GetAll()
        {
            return DataList;
        }

        static public ChatChannel Get(int key)
        {
            ChatChannel value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatChannel.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<ChatChannel>> DataList_id = new Dictionary<int, List<ChatChannel>>();

        static public Dictionary<int, List<ChatChannel>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<ChatChannel> GetGroupByid(int key)
        {
            List<ChatChannel> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatChannel.GetGroupByid({0}) not find!", key);
            return null;
        }


        // ID
        public int id { get; set; }

        // 频道名称
        public string channelName { get; set; }

        // 聊天频道简称
        public string channelNickName { get; set; }

        // 银贝消耗
        public int silver { get; set; }

        // 金贝消耗
        public int Gold { get; set; }

        // 仙玉消耗
        public int Fairy { get; set; }

        // 碧玉消耗
        public int Jasper { get; set; }

        // 物品消耗
        public string items { get; set; }

        // 冷却时间
        public int time { get; set; }

        // 等级限制
        public int level { get; set; }

        // 字符图片发送开关
        public bool picOpen { get; set; }

        // 语音发送开关
        public bool voiceOpen { get; set; }

        // 消息最大数量
        public int max { get; set; }

        // 信息保存条数
        public int saveNum { get; set; }

        // 聊天频道名称颜色
        public string color { get; set; }

        // 是否显示冒泡
        public bool pop { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ChatChannel);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ChatChannel);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ChatChannel> allDatas = new List<ChatChannel>();

            {
                string file = "ChatConfig/ChatChannel.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int channelName_index = reader.GetIndex("channelName");
                int channelNickName_index = reader.GetIndex("channelNickName");
                int silver_index = reader.GetIndex("silver");
                int Gold_index = reader.GetIndex("Gold");
                int Fairy_index = reader.GetIndex("Fairy");
                int Jasper_index = reader.GetIndex("Jasper");
                int items_index = reader.GetIndex("items");
                int time_index = reader.GetIndex("time");
                int level_index = reader.GetIndex("level");
                int picOpen_index = reader.GetIndex("picOpen");
                int voiceOpen_index = reader.GetIndex("voiceOpen");
                int max_index = reader.GetIndex("max");
                int saveNum_index = reader.GetIndex("saveNum");
                int color_index = reader.GetIndex("color");
                int pop_index = reader.GetIndex("pop");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ChatChannel data = new ChatChannel();
						data.id = reader.getInt(i, id_index, 0);         
						data.channelName = reader.getStr(i, channelName_index);         
						data.channelNickName = reader.getStr(i, channelNickName_index);         
						data.silver = reader.getInt(i, silver_index, 0);         
						data.Gold = reader.getInt(i, Gold_index, 0);         
						data.Fairy = reader.getInt(i, Fairy_index, 0);         
						data.Jasper = reader.getInt(i, Jasper_index, 0);         
						data.items = reader.getStr(i, items_index);         
						data.time = reader.getInt(i, time_index, 0);         
						data.level = reader.getInt(i, level_index, 0);         
						data.picOpen = reader.getBool(i, picOpen_index, false);         
						data.voiceOpen = reader.getBool(i, voiceOpen_index, false);         
						data.max = reader.getInt(i, max_index, 0);         
						data.saveNum = reader.getInt(i, saveNum_index, 0);         
						data.color = reader.getStr(i, color_index);         
						data.pop = reader.getBool(i, pop_index, false);         
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
                    CsvCommon.Log.Error("ChatChannel.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<ChatChannel> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<ChatChannel>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(ChatChannel);
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



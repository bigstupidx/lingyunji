// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ChatInputSimple 
    {
        static Dictionary<int, ChatInputSimple> DataList = new Dictionary<int, ChatInputSimple>();

        static public Dictionary<int, ChatInputSimple> GetAll()
        {
            return DataList;
        }

        static public ChatInputSimple Get(int key)
        {
            ChatInputSimple value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatInputSimple.Get({0}) not find!", key);
            return null;
        }

        static Dictionary<int, List<ChatInputSimple>> DataList_id = new Dictionary<int, List<ChatInputSimple>>();

        static public Dictionary<int, List<ChatInputSimple>> GetAllGroupByid()
        {
            return DataList_id;
        }

        static public List<ChatInputSimple> GetGroupByid(int key)
        {
            List<ChatInputSimple> value = null;
            if (DataList_id.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ChatInputSimple.GetGroupByid({0}) not find!", key);
            return null;
        }


        // ID
        public int id { get; set; }

        // 快捷用语
        public string input { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ChatInputSimple);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_id.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ChatInputSimple);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ChatInputSimple> allDatas = new List<ChatInputSimple>();

            {
                string file = "ChatConfig/ChatInputSimple.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.TryIndex("id:group");
                int input_index = reader.GetIndex("input");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ChatInputSimple data = new ChatInputSimple();
						data.id = reader.getInt(i, id_index, 0);         
						data.input = reader.getStr(i, input_index);         
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
                    CsvCommon.Log.Error("ChatInputSimple.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            foreach (var data in allDatas)
            {
                {
                    List<ChatInputSimple> l = null;
                    if (!DataList_id.TryGetValue(data.id, out l))
                    {
                        l = new List<ChatInputSimple>();
                        DataList_id[data.id] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(ChatInputSimple);
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



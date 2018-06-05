// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class BubbleContents 
    {
        static List<BubbleContents> DataList = new List<BubbleContents>();
        static public List<BubbleContents> GetAll()
        {
            return DataList;
        }

        static public BubbleContents Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("BubbleContents.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<string, List<BubbleContents>> DataList_key = new Dictionary<string, List<BubbleContents>>();

        static public Dictionary<string, List<BubbleContents>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<BubbleContents> GetGroupBykey(string key)
        {
            List<BubbleContents> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("BubbleContents.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 冒泡ID
        public string key { get; set; }

        // 冒泡内容
        public string content { get; set; }

        // 冒泡时长
        public float timeLen { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(BubbleContents);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(BubbleContents);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<BubbleContents> allDatas = new List<BubbleContents>();

            {
                string file = "StoryConfig/BubbleContents.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int content_index = reader.GetIndex("content");
                int timeLen_index = reader.GetIndex("timeLen");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BubbleContents data = new BubbleContents();
						data.key = reader.getStr(i, key_index);         
						data.content = reader.getStr(i, content_index);         
						data.timeLen = reader.getFloat(i, timeLen_index, 0f);         
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
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<BubbleContents> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<BubbleContents>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(BubbleContents);
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



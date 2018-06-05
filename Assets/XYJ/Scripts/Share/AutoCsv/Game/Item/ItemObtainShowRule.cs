// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ItemObtainShowRule 
    {
        static List<ItemObtainShowRule> DataList = new List<ItemObtainShowRule>();
        static public List<ItemObtainShowRule> GetAll()
        {
            return DataList;
        }

        static public ItemObtainShowRule Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ItemObtainShowRule.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<int, List<ItemObtainShowRule>> DataList_key = new Dictionary<int, List<ItemObtainShowRule>>();

        static public Dictionary<int, List<ItemObtainShowRule>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<ItemObtainShowRule> GetGroupBykey(int key)
        {
            List<ItemObtainShowRule> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ItemObtainShowRule.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 界面情况
        public int key { get; set; }

        // 物品类型
        public int itemType { get; set; }

        // 物品ID
        public int itemId { get; set; }

        // 飘字
        public int waveWord { get; set; }

        // 飘字优先级
        public int waveWordPriority { get; set; }

        // 飘字颜色
        public int colorState { get; set; }

        // 系统提示条
        public int systemHint { get; set; }

        // 系统提示条文字模板
        public string systemHintDesc { get; set; }

        // 动画
        public int Ani { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ItemObtainShowRule);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ItemObtainShowRule);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ItemObtainShowRule> allDatas = new List<ItemObtainShowRule>();

            {
                string file = "Item/ItemObtainShowRule.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int itemType_index = reader.GetIndex("itemType");
                int itemId_index = reader.GetIndex("itemId");
                int waveWord_index = reader.GetIndex("waveWord");
                int waveWordPriority_index = reader.GetIndex("waveWordPriority");
                int colorState_index = reader.GetIndex("colorState");
                int systemHint_index = reader.GetIndex("systemHint");
                int systemHintDesc_index = reader.GetIndex("systemHintDesc");
                int Ani_index = reader.GetIndex("Ani");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ItemObtainShowRule data = new ItemObtainShowRule();
						data.key = reader.getInt(i, key_index, 0);         
						data.itemType = reader.getInt(i, itemType_index, 0);         
						data.itemId = reader.getInt(i, itemId_index, 0);         
						data.waveWord = reader.getInt(i, waveWord_index, 0);         
						data.waveWordPriority = reader.getInt(i, waveWordPriority_index, 0);         
						data.colorState = reader.getInt(i, colorState_index, 0);         
						data.systemHint = reader.getInt(i, systemHint_index, 0);         
						data.systemHintDesc = reader.getStr(i, systemHintDesc_index);         
						data.Ani = reader.getInt(i, Ani_index, 0);         
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
                    List<ItemObtainShowRule> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<ItemObtainShowRule>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(ItemObtainShowRule);
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



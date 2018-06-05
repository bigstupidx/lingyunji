
// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ItemBaseAll
    {
        static Dictionary<int, ItemBase> DataList = new Dictionary<int, ItemBase>();
        static public Dictionary<int, ItemBase> GetAll()
        {
            return DataList;
        }

        public static void Init()
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ItemBaseAll);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }
            
            DataList.Clear();

            foreach (var itor in EquipPrototype.GetAll())
                DataList.Add(itor.Key, itor.Value);
            foreach (var itor in Item.GetAll())
                DataList.Add(itor.Key, itor.Value);
            foreach (var itor in TaskItem.GetAll())
                DataList.Add(itor.Key, itor.Value);

            {
                var type = typeof(ItemBaseAll);
                var method = type.GetMethod("OnLoadEnd", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }
        }

        static public ItemBase Get(int key)
        {
            ItemBase value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ItemBaseAll.Get({0}) not find!", key);
            return null;
        }
    }
}


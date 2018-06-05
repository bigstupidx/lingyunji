using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class PostureLogicConfig
    {
        //如果没有会取0
        static public PostureLogicConfig GetKey(int key)
        {
            PostureLogicConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;

            return DataList[0];
        }
    }

    public partial class PostureConfig
    {
        public PostureLogicConfig logic { get; private set; }

        static void OnLoadEnd()
        {
            CsvLoadAdapter.AddCallAfterAllLoad(OnLoadAllCsv);
        }
        
        static void OnLoadAllCsv()
        {
            foreach (var p in DataList.Values)
            {
                p.logic = PostureLogicConfig.GetKey(p.id);
            }
        }

        //如果没有会取0
        static public PostureConfig GetKey(int key)
        {
            PostureConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            else
                XYJLogger.LogError("找不到姿态 id=" + key);
            return DataList[0];
        }
    }
}
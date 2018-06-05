#if MEMORY_CHECK
using System;
using System.Collections.Generic;

public class MemoryDiffer
{
    public class Data
    {
        public MemoryInfo.TypeData before; // 之前的数据
        public MemoryInfo.TypeData current; // 当前的数据

        public MemoryInfo.TypeData data
        {
            get { return current != null ? current : before; }
        }

        public string fullName
        {
            get { return data.fullName; }
        }

        public long total
        {
            get { return data.total; }
        }

        public long last_time
        {
            get { return data.last_time; }
        }

        public long historyTotal
        {
            get { return data.historyTotal; }
        }

        public Type type
        {
            get { return data.type; }
        }

        public int subCount
        {
            get
            {
                if (before == null)
                    return current.total;
                if (current == null)
                    return -before.total;

                return current.total - before.total;
            }
        }

        public int subHistoryCount
        {
            get
            {
                if (before == null)
                    return current.historyTotal;
                if (current == null)
                    return -before.historyTotal;

                return current.historyTotal - before.historyTotal;
            }
        }

        public override string ToString()
        {
            if (before == null)
                return current.ToString();

            // 当前增加或减少的数量
            return string.Format("subCount:{0} subHistoryCount:{1}", subCount, subHistoryCount);
        }
    }

    public long add_total = 0; // 总的增加数理

    Dictionary<string, Data> AllList = new Dictionary<string, Data>();
    List<Data> mTypeDatas = new List<Data>();

    public List<Data> List { get { return mTypeDatas; } }

    // 两份数据的差异
    public static MemoryDiffer Differ(MemoryInfo before, MemoryInfo current)
    {
        MemoryDiffer differ = new MemoryDiffer();
        var before_dic = before.Dic;
        var current_dic = current.Dic;

        foreach (var itor in before_dic)
        {
            MemoryInfo.TypeData td = null;
            Data data = new Data();
            data.before = itor.Value;
            if (current_dic.TryGetValue(itor.Key, out td))
            {
                data.current = td;
            }
            else
            {
                // 当前没有，现在有
            }

            // 当前与之前的差异
            differ.AllList.Add(itor.Key, data);
            differ.mTypeDatas.Add(data);
        }

        foreach (var itor in current_dic)
        {
            MemoryInfo.TypeData td = null;
            if (before_dic.TryGetValue(itor.Key, out td))
                continue;

            Data data = new Data();
            data.before = null;
            data.current = itor.Value;

            // 当前与之前的差异
            differ.AllList.Add(itor.Key, data);
            differ.mTypeDatas.Add(data);
        }

        return differ;
    }
}

#endif
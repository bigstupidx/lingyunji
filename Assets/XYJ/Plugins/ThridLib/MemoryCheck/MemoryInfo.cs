#if MEMORY_CHECK
using System;
using System.Collections.Generic;

public class MemoryInfo
{
    public class TypeData
    {
        public Type type;
        public string fullName;
        public int total = 0; // 当前总个数
        public int historyTotal = 0; // 历史最高个数
        public long last_time = 0;

        public void AddRef(long last_time)
        {
            ++total;
            historyTotal = Math.Max(total, historyTotal);
            this.last_time = last_time;
        }

        public void SubRef()
        {
            --total;
        }

        public override string ToString()
        {
            return string.Format("currentTotal:{0} MaxTotal:{1}", total, historyTotal);
        }

        public void Write(Network.BitStream stream)
        {
            stream.Write(fullName);
            stream.Write(total);
            stream.Write(historyTotal);
            stream.Write(last_time);
        }

        public void Read(Network.BitStream stream)
        {
            fullName = stream.ReadString();
            type = TypeFind.Get(fullName);

            total = stream.ReadInt32();
            historyTotal = stream.ReadInt32();
            last_time = stream.ReadInt64();
        }
    }

    public long total = 0;
    public long historyTotal = 0;

    Dictionary<string, TypeData> AllList = new Dictionary<string, TypeData>();
    List<TypeData> mTypeDatas = new List<TypeData>();

    public bool isDirty = false;

    public long last_timer = 0;

    public Dictionary<string, TypeData> Dic
    {
        get { return AllList; }
    }

    public List<TypeData> List
    {
        get { return mTypeDatas; }
    }

    TypeData GetOrCreate(Type type)
    {
        TypeData td = null;
        if (!AllList.TryGetValue(type.FullName, out td))
        {
            td = new TypeData() { type = type, fullName = type.FullName };
            AllList.Add(type.FullName, td);
            mTypeDatas.Add(td);
        }

        return td;
    }

    public void Add(object obj)
    {
        isDirty = true;
        Type type = obj.GetType();

        var td = GetOrCreate(type);
        last_timer = System.DateTime.Now.Ticks;
        td.AddRef(last_timer);

        ++total;
        historyTotal = Math.Max(total, historyTotal);
    }

    public void Sub(object obj)
    {
        isDirty = true;
        Type type = obj.GetType();
        var td = GetOrCreate(type);
        td.SubRef();

        --total;
    }

    public void Write(Network.BitStream stream)
    {
        stream.Write(last_timer);
        stream.Write(total);
        stream.Write(historyTotal);

        stream.Write(AllList.Count);
        foreach (var itor in AllList)
        {
            itor.Value.Write(stream);
        }
    }

    public void Read(Network.BitStream stream)
    {
        last_timer = stream.ReadInt64();
        total = stream.ReadInt64();
        historyTotal = stream.ReadInt64();

        AllList.Clear();
        List.Clear();
        int count = stream.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            TypeData td = new TypeData();
            try
            {
                td.Read(stream);
            }
            catch (Exception ex)
            {
                Debuger.LogException(ex);
                continue;
            }

            AllList.Add(td.fullName, td);
            List.Add(td);
        }
    }

    public void CopyTo(MemoryInfo info)
    {
        info.last_timer = last_timer;
        info.total = total;
        info.historyTotal = historyTotal;

        info.AllList.Clear();
        info.List.Clear();
        foreach (var itor in AllList)
        {
            info.AllList.Add(itor.Key, itor.Value);
            info.List.Add(itor.Value);
        }
    }

    public override string ToString()
    {
        DateTime dt = new DateTime(last_timer, DateTimeKind.Local);
        return string.Format("time:{0} {1}/{2}", To(dt), total, historyTotal);
    }

    static string To(DateTime dt)
    {
        return string.Format("{0}:{1} {2}.{3}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
    }
}

#endif
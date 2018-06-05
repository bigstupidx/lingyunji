#if ASSET_DEBUG
using System;
using System.Collections.Generic;

namespace PackTool
{
    public class TimeTrack
    {
        TimeTrack()
        {

        }

        static public TimeTrack Create()
        {
            return new TimeTrack();
        }

        long totaltime = 0;

        public JSONObject ToJson()
        {
            JSONObject json = new JSONObject();
            json.put("totaltime", totaltime);
            lock (RecordList)
            {
                foreach (KeyValuePair<string, Data> itor in RecordList)
                    json.put(itor.Key, itor.Value.ToJson());
            }

            return json;
        }

        public void InitByJson(JSONObject json)
        {
            totaltime = json.getLong("totaltime");
            lock (RecordList)
            {
                RecordList.Clear();
                foreach (string key in JSONObject.getNames(json))
                {
                    if (key == "totaltime")
                        continue;

                    Data d = new Data();
                    d.InitByJson(json.getJSONObject(key));
                    RecordList.Add(key, d);
                }
            }
        }

        public object Begin(string key)
        {
            return BeginKey(key);
        }

        public float Totaltime { get { return totaltime * 0.0000001f; } }

        public void End(object p)
        {
            EndKey(p as Data.TimePart);
        }

        public static Func<string, int> get_file_size;

        static int GetFileSize(string file)
        {
            if (get_file_size == null)
            {
                return GetDefaultFileSize(file);
            }

            try
            {
                return get_file_size(file);
            }
            catch (System.Exception e)
            {
                get_file_size = null;
                return GetDefaultFileSize(file);
            }
        }

        static int GetDefaultFileSize(string file)
        {
            string f = ResourcesPath.LocalDataPath + file;
            System.IO.FileInfo info = new System.IO.FileInfo(f);
            if (info.Exists)
                return (int)info.Length;

            return 0;
        }

        Data.TimePart BeginKey(string key)
        {
            Data data = null;
            lock (RecordList)
            {
                data = RecordList[key];
            }

            data.file_size = GetFileSize(key);

            data.key = key;
            Data.TimePart tp = new Data.TimePart();
            tp.begintime = DateTime.Now.Ticks;
            tp.begin_frame = XTools.TimerMgrObj.frameCount;
            data.times.Add(tp);

            return tp;
        }

        void EndKey(Data.TimePart tp)
        {
            tp.endtime = DateTime.Now.Ticks;
            tp.end_frame = XTools.TimerMgrObj.frameCount;

            totaltime += (tp.endtime - tp.begintime);
        }

        public delegate object Exe_Return();

        public int count { get { return RecordList.Count; } }

        public class Data
        {
            public Data()
            {

            }

            public string key; // key
            public class TimePart
            {
                public long begintime;
                public long endtime;

                public long begin_frame = 0;
                public long end_frame = 0;

                public float usetime 
                { 
                    get 
                    {
                        if (endtime != 0)
                            return (endtime - begintime) * 0.0000001f;
                        return 0;
                    }
                }

                public string usetime_text
                {
                    get
                    {
                        DateTime dt = new DateTime();
                        dt = dt.AddTicks(begintime);

                        return string.Format("{0}:{1}:{2}", dt.Hour, dt.Minute, dt.Second);
                    }
                }

                public JSONObject ToJson()
                {
                    JSONObject json = new JSONObject();
                    json.put("begintime", begintime);
                    json.put("endtime", endtime);
                    json.put("begin_frame", begin_frame);
                    json.put("end_frame", end_frame);
                    return json;
                }

                public void InitByJson(JSONObject json)
                {
                    begintime = json.getLong("begintime");
                    endtime = json.getLong("endtime");
                    begin_frame = json.getLong("begin_frame");
                    end_frame = json.getLong("end_frame");
                }
            }

            public List<TimePart> times = new List<TimePart>(); // 用时时间

            public long lasttime
            {
                get
                {
                    long t = 0;
                    foreach (TimePart d in times)
                    {
                        if (d.endtime > t)
                            t = d.endtime;
                    }

                    return t;
                }
            }

            public long starttime
            {
                get
                {
                    long t = long.MaxValue;
                    foreach (TimePart d in times)
                    {
                        if (d.begintime < t)
                            t = d.begintime;
                    }

                    return t;
                }
            }

            public long begin_frame
            {
                get
                {
                    long t = long.MaxValue;
                    foreach (TimePart d in times)
                    {
                        if (d.begin_frame < t)
                            t = d.begin_frame;
                    }

                    return t;
                }
            }

            public long end_frame
            {
                get
                {
                    long t = long.MinValue;
                    foreach (TimePart d in times)
                    {
                        if (d.end_frame > t)
                            t = d.end_frame;
                    }

                    return t;
                }
            }

            public string lasttime_text
            {
                get
                {
                    DateTime dt = new DateTime();
                    dt = dt.AddTicks(lasttime);

                    return string.Format("{0}:{1}:{2}", dt.Hour, dt.Minute, dt.Second);
                }
            }

            public string starttime_text
            {
                get
                {
                    DateTime dt = new DateTime();
                    dt = dt.AddTicks(starttime);

                    return string.Format("{0}:{1}:{2}", dt.Hour, dt.Minute, dt.Second);
                }
            }

            // 文件大小
            public long file_size;

            public JSONObject ToJson()
            {
                JSONObject json = new JSONObject();
                json.put("key", key);

                JSONArray array = new JSONArray();
                foreach (TimePart t in times)
                    array.put(t.ToJson());

                json.put("times", array);

                return json;
            }

            public void InitByJson(JSONObject json)
            {
                key = json.getString("key");

                JSONArray array = json.getJSONArray("times");
                times.Clear();
                for (int i = 0; i < array.Count(); ++i)
                {
                    TimePart tp = new TimePart();
                    tp.InitByJson(array.getJSONObject(i));
                    times.Add(tp);
                }

                file_size = GetFileSize(key);
            }

            public float total
            {
                get
                {
                    float t = 0f;
                    for (int i = 0; i < times.Count; ++i)
                        t += times[i].usetime;

                    return t;
                }
            }
        }

        // 记录列表
        XTools.SortedMap<string, Data> RecordList = new XTools.SortedMap<string, Data>();

        public void Reset()
        {
            RecordList.Clear();
            totaltime = 0;
        }

        public XTools.SortedMap<string, Data> GetAll()
        {
            return RecordList;
        }

        public object Execution(string key, Exe_Return action)
        {
            object o = null;
            Data.TimePart dt = BeginKey(key);
            o = action();
            EndKey(dt);

            return o;
        }

        public void Execution(string key, System.Action action)
        {
            Data.TimePart dt = BeginKey(key);
            action();
            EndKey(dt);
        }
    }
}
#endif

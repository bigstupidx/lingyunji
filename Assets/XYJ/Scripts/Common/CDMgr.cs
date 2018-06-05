namespace xys
{
    using wProtobuf;
    using System.Collections.Generic;

    public class CDEventData
    {
        public CDType type;
        public short id;

        public bool isEnd;
        public float total;
        public float elapse;
    }

    public class CDMgr
    {
        class CD
        {
            public float total; // 总时长
            public long end; // 结束时间戳

            public bool Update(CDType type, short id, long current)
            {
                CDEventData data = new CDEventData();
                data.type = type;
                data.id = id;

                if (current > end)
                {
                    // CD结束了
                    data.isEnd = true;
                    data.total = total;
                    data.elapse = total;
                }
                else
                {
                    data.isEnd = false;
                    data.total = total;
                    data.elapse = total - (end - current) * TickToSecond;
                }

                App.my.eventSet.FireEvent(type, data);
                if (id == 0)
                {

                }
                else
                {
                    App.my.eventSet.FireEvent(type, id, data);
                }
                return data.isEnd;
            }

            public CDEventData GetData(CDType type, short id, long current)
            {
                CDEventData data = new CDEventData();
                data.type = type;
                data.id = id;

                if (current > end)
                {
                    // CD结束了
                    data.isEnd = true;
                    data.total = total;
                    data.elapse = total;
                }
                else
                {
                    data.isEnd = false;
                    data.total = total;
                    data.elapse = total - (end - current) * TickToSecond;
                }

                return data;
            }
        }

        // CD计时
        Dictionary<int, CD> cdTimes = new Dictionary<int, CD>();

        public bool isEnd(CDType type)
        {
            return isEnd(type, 0);
        }

        public bool isEnd(CDType type, short id)
        {
            int key = xys.CDUtils.Combine(type, id);
            CD cd = null;
            if (!cdTimes.TryGetValue(key, out cd))
                return true;

            if (App.my.srvTimer.GetTime.GetCurrentTime() >= cd.end)
                return true;

            return false;
        }

        public bool isSkillEnd(short id) { return isEnd(xys.CDType.Skill, id); }

        public bool isItemEnd(short id) { return isEnd(xys.CDType.Item, id); }

        public void Deserialize(IReadStream input)
        {
            if (input == null)
                return;

            int count = input.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                int id = input.ReadSFixed32();
                CD cd = new CD();
                cd.end = input.ReadInt64();
                cd.total = input.ReadFloat();
                cdTimes.Add(id, cd); // 还没有到期
            }

            if (cdTimes.Count != 0)
                RegUpdate();
        }

        static readonly long SecondToTick = (new System.TimeSpan(0, 0, 1)).Ticks;
        static readonly float TickToSecond = 1.0f / (new System.TimeSpan(0, 0, 1)).Ticks;

        public void OnStartCD(NetProto.CDData data)
        {
            CD cd = null;
            if (!cdTimes.TryGetValue(data.cdType, out cd))
            {
                cd = new CD();
                cdTimes.Add(data.cdType, cd);
            }

            cd.total = data.total * 0.001f;
            cd.end = App.my.srvTimer.GetTime.GetCurrentTime() + (long)(SecondToTick * cd.total);

            RegUpdate();
        }

        void RegUpdate()
        {
            if (update_frame == null)
            {
                if (cdTimes.Count != 0)
                    update_frame = App.my.frameMgr.AddLateUpdate(Update, null);
            }
        }

        XTools.TimerFrame.Frame update_frame;

        bool Update(object p)
        {
            long current = App.my.srvTimer.GetTime.GetCurrentTime(); // 当前时间
            List<int> ends = new List<int>();
            foreach (var itor in cdTimes)
            {
                CDType type;
                short id;
                CDUtils.Combine(itor.Key, out type, out id);
                if (itor.Value.Update(type, id, current))
                    ends.Add(itor.Key);
            }

            for (int i = 0; i < ends.Count; ++i)
                cdTimes.Remove(ends[i]);

            if (cdTimes.Count == 0)
            {
                update_frame = null;
                return false;
            }

            return true;
        }

        public CDEventData GetData(CDType type)
        {
            return GetData(type, 0);
        }

        public CDEventData GetData(CDType type, short id)
        {
            int key = CDUtils.Combine(type, id);
            CD cd = null;
            if (!cdTimes.TryGetValue(key, out cd))
                return null;

            return cdTimes[key].GetData(type, id, App.my.srvTimer.GetTime.GetCurrentTime());
        }
    }
}

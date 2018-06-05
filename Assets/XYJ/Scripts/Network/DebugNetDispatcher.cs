#if COM_DEBUG
using System;
using System.Collections.Generic;

namespace xys
{
    public class DebugNetDispatcher : IDispatcher
    {
        class Data
        {
            public MSG.Dis dis;
            public float delay;
        }

        LinkedList<Data> mMessages = new LinkedList<Data>();
        Random random = new Random();

        bool isOpen = false;
        int minValue = 1000;
        int maxValue = 2000;

        public void OpenDelay(float minValue, float maxValue)
        {
            isOpen = true;
            this.minValue = (int)(minValue * 10000);
            this.maxValue = (int)(maxValue * 10000);
        }

        public void StopDelay()
        {
            isOpen = false;
        }

        float delay
        {
            get
            {
                return isOpen ? (random.Next(minValue, maxValue) * 0.0001f) : 0f;
            }
        }

        float realtimeSinceStartup;

        public void Message<T>(Action<T> action, T obj)
        {
            Data data = new Data();
            data.dis = MSG.Factory.CreateMessage(action, obj);
            data.delay = delay;

            lock (mMessages)
            {
                mMessages.AddLast(data);
            }
        }

        public void Message<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Data data = new Data();
            data.dis = MSG.Factory.CreateMessage(action, t1, t2);
            data.delay = delay;

            lock (mMessages)
            {
                mMessages.AddLast(data);
            }
        }

        public void Message(Action action)
        {
            Data data = new Data();
            data.dis = MSG.Factory.CreateMessage(action);
            data.delay = delay;

            lock (mMessages)
            {
                mMessages.AddLast(data);
            }
        }

        List<MSG.Dis> mTempMessages = new List<MSG.Dis>();

        public void Update()
        {
            float realtimeSinceStartup = UnityEngine.Time.realtimeSinceStartup;
            float delay = realtimeSinceStartup - this.realtimeSinceStartup;
            this.realtimeSinceStartup = realtimeSinceStartup;

            if (mMessages.Count == 0)
                return;

            lock (mMessages)
            {
                if (isOpen)
                {
                    foreach (var node in mMessages)
                        node.delay -= delay;

                    var itor = mMessages.First;
                    while (itor != null)
                    {
                        itor.Value.delay -= delay;
                        if (itor.Value.delay <= 0f)
                        {
                            mTempMessages.Add(itor.Value.dis);
                            mMessages.RemoveFirst();
                            itor = mMessages.First;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var d in mMessages)
                        mTempMessages.Add(d.dis);
                    mMessages.Clear();
                }
            }

            for (int i = 0; i < mTempMessages.Count; ++i)
            {
                mTempMessages[i].OnMessage();
            }

            mTempMessages.Clear();
        }
    }
}
#endif
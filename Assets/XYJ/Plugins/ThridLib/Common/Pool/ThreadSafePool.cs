using System;
using System.Collections.Generic;

namespace Pool
{
    public class ThreadSafe<T> where T : new()
    {
        public ThreadSafe(Func<T> f, int mt)
        {
            create = f;
            maxTotal = mt;
        }

        protected int maxTotal;

        protected List<T> bufs = new List<T>();

        protected Func<T> create;

        public int count
        {
            get { return bufs.Count; }
        }
        
        public T Get()
        {
            T t;
            lock (bufs)
            {
                int count = bufs.Count;
                if (count == 0)
                {
                    t = create();
                }
                else
                {
                    t = bufs[count - 1];
                    bufs.RemoveAt(count - 1);
                }
            }

            return t;
        }

        public void Free(T t)
        {
            lock (bufs)
            {
                if (bufs.Count >= maxTotal)
                    return;

                bufs.Add(t);
            }
        }
    }
}
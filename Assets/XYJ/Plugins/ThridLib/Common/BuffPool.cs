using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class Buff<T>
#if COM_DEBUG
#if !NOPOOL
        : AllBuff.Buf
#endif
#endif
        where T : new()
    {
#if !NOPOOL
        static Buff<T> _instance = null;

#if COM_DEBUG
        // 创建的总次数
        public long create_total { get; protected set; }

        // 使用的总次数
        public long used_total { get; protected set; }

        // 得到当前所有对象池的数据
        public void GetBufInfo(System.Text.StringBuilder sb)
        {
            sb.AppendLine(string.Format("type:Buff<{0}> total:{1} current:{2} used:{3}", GetTypeName(typeof(T)), _instance.create_total, _instance.bufs.Count, used_total));
        }

        static string GetTypeName(System.Type type)
        {
            return Utility.GetTypeName(type);
        }
#endif
        List<T> bufs;
#endif
        Buff(int num)
        {
#if !NOPOOL
#if COM_DEBUG
            create_total = num;
            AllBuff.Add(this);
#endif
            bufs = new List<T>(num);
            for (int i = 0; i < num; ++i)
                bufs.Add(new T());
#endif
        }

        public static T Get()
        {
#if !NOPOOL
            if (_instance == null)
            {
                _instance = new Buff<T>(8);
            }

            return _instance.get();
#else
            return new T();
#endif
        }

#if !NOPOOL
        protected T get()
        {
            if (bufs.Count == 0)
            {
#if COM_DEBUG
                ++create_total;
#endif
                return new T();
            }

            T t = bufs[bufs.Count - 1];
            bufs.RemoveAt(bufs.Count - 1);

#if COM_DEBUG
            ++used_total;
#endif
            return t;
        }
#endif

        public static void Free(T t)
        {
#if !NOPOOL
            if (_instance != null)
                _instance.free(t);
#endif
        }

#if NOPOOL

#else
        protected void free(T t)
        {
#if UNITY_EDITOR && COM_DEBUG
            if(bufs.Contains(t))
            {
                Debuger.LogError("对象池重复释放对象:" + typeof(T));
                return;
            }
#endif
            bufs.Add(t);
        }

        public static void Release()
        {
            if (_instance != null)
                _instance.release();
        }

        protected void release()
        {
            bufs.Clear();
        }

        public static void Reset(int num)
        {
            if (_instance == null)
            {
                _instance = new Buff<T>(num);
            }
            else
            {
                _instance.reset(num);
            }
        }

        protected void reset(int num)
        {
            if (bufs.Count >= num)
            {
                bufs.RemoveRange(num, bufs.Count - num);
                return;
            }

            bufs.Capacity = num;
            for (int i = num - bufs.Count; i >= 0; --i)
                bufs.Add(new T());
        }
#endif
    }
}
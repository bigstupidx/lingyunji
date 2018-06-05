#if MEMORY_CHECK
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace System.Collections.Generic
{
    public class SortedDictionaryM<TKey, TValue>
    {
        public SortedDictionaryM()
        {
            mList = new SortedDictionary<TKey, object>();
            Count = 0;
        }

        public SortedDictionaryM(IComparer<TKey> comparer)
        {
            mList = new SortedDictionary<TKey, object>(comparer);
            Count = 0;
        }

        SortedDictionary<TKey, object> mList = null;

        public int Count { get; protected set; }

        //public TValue this[TKey key]
        //{
        //    get
        //    {
        //        object obj = null;
        //        if (mList.TryGetValue(key, out obj))
        //        {
        //            if (obj is List<TValue>)
        //                return ((List<TValue>)obj)[0];

        //            return (TValue)obj;
        //        }

        //        throw new KeyNotFoundException(string.Format("Key:{0}", key.ToString()));
        //    }

        //    set
        //    {
        //        object obj = null;
        //        if (mList.TryGetValue(key, out obj))
        //        {
        //            if (obj is List<TValue>)
        //            {
        //                ((List<TValue>)obj)[0] = value;
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            ++Count;
        //        }

        //        mList[key] = value;
        //    }
        //}

        public void Add(TKey key, TValue value)
        {
            object current;
            if (mList.TryGetValue(key, out current))
            {
                if (current is List<TValue>)
                {
                    ((List<TValue>)current).Add(value);
                }
                else
                {
                    List<TValue> nl = new List<TValue>();
                    nl.Add((TValue)current);
                    nl.Add(value);

                    mList[key] = nl;
                }
            }
            else
            {
                mList[key] = value;
            }

            ++Count;
        }

        public int Remove(TKey key)
        {
            object obj;
            if (mList.TryGetValue(key, out obj))
            {
                mList.Remove(key);
                if (obj is TValue)
                {
                    --Count;
                    return 1;
                }
                else
                {
                    List<TValue> objs = (List<TValue>)obj;
                    Count -= objs.Count;
                    return objs.Count;
                }
            }

            return 0;
        }

        public bool Remove(TKey key, TValue value)
        {
            object obj = null;
            if (mList.TryGetValue(key, out obj))
            {
                if (obj is List<TValue>)
                {
                    List<TValue> objs = (List<TValue>)obj;
                    bool b = objs.Remove(value);
                    if (objs.Count == 0)
                    {
                        mList.Remove(key);
                    }

                    --Count;
                    return b;
                }
                else if (obj.GetHashCode() == value.GetHashCode())
                {
                    --Count;
                    return mList.Remove(key);
                }
            }

            return false;
        }

        public SortedDictionaryM<TKey, TValue>.Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        // 摘要: 
        //     枚举 System.Collections.Generic.SortedDictionary<TKey,TValue> 的元素。
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable
        {
            public Enumerator(SortedDictionaryM<TKey, TValue> dic)
            {
                Dic = dic;
                enumerator = Dic.mList.GetEnumerator();
                current_index = -2;
                CurrentItor = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            }

            SortedDictionaryM<TKey, TValue> Dic;
            SortedDictionary<TKey, object>.Enumerator enumerator;
            KeyValuePair<TKey, TValue> CurrentItor;

            int current_index;

            public void Reset() 
            {
                enumerator = Dic.mList.GetEnumerator();
                current_index = -2;
                CurrentItor = new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            }

            // 摘要: 
            //     获取枚举数当前位置的元素。
            //
            // 返回结果: 
            //     System.Collections.Generic.SortedDictionary<TKey,TValue> 中位于该枚举数当前位置的元素。
            public KeyValuePair<TKey, TValue> Current { get { return CurrentItor; } }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            // 摘要: 
            //     释放由 System.Collections.Generic.SortedDictionary<TKey,TValue>.Enumerator 使用的所有资源。
            public void Dispose()
            {
                enumerator.Dispose();
            }

            //
            // 摘要: 
            //     使枚举数前进到 System.Collections.Generic.SortedDictionary<TKey,TValue> 的下一个元素。
            //
            // 返回结果: 
            //     如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。
            //
            // 异常: 
            //   System.InvalidOperationException:
            //     在创建了枚举数后集合被修改了。
            public bool MoveNext()
            {
                do
                {
                    if (current_index == -2)
                    {
                        // 第一次调用
                        bool value = enumerator.MoveNext();
                        if (value == false)
                            return false;

                        // 
                        current_index = -1;
                    }

                    KeyValuePair<TKey, object> obj = enumerator.Current;
                    if (obj.Value is List<TValue>)
                    {
                        ++current_index;
                        List<TValue> lv = (List<TValue>)obj.Value;
                        if (current_index < lv.Count)
                        {
                            CurrentItor = new KeyValuePair<TKey, TValue>(obj.Key, lv[current_index]);
                            return true;
                        }
                        else
                        {
                            current_index = -2;
                        }
                    }
                    else
                    {
                        CurrentItor = new KeyValuePair<TKey, TValue>(obj.Key, (TValue)obj.Value);
                        current_index = -2;
                        return true;
                    }
                }
                while (true);
            }
        }
    }
}
#endif
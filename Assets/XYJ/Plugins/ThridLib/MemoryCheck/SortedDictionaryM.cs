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

        // ժҪ: 
        //     ö�� System.Collections.Generic.SortedDictionary<TKey,TValue> ��Ԫ�ء�
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

            // ժҪ: 
            //     ��ȡö������ǰλ�õ�Ԫ�ء�
            //
            // ���ؽ��: 
            //     System.Collections.Generic.SortedDictionary<TKey,TValue> ��λ�ڸ�ö������ǰλ�õ�Ԫ�ء�
            public KeyValuePair<TKey, TValue> Current { get { return CurrentItor; } }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            // ժҪ: 
            //     �ͷ��� System.Collections.Generic.SortedDictionary<TKey,TValue>.Enumerator ʹ�õ�������Դ��
            public void Dispose()
            {
                enumerator.Dispose();
            }

            //
            // ժҪ: 
            //     ʹö����ǰ���� System.Collections.Generic.SortedDictionary<TKey,TValue> ����һ��Ԫ�ء�
            //
            // ���ؽ��: 
            //     ���ö�����ɹ����ƽ�����һ��Ԫ�أ���Ϊ true�����ö����Խ�����ϵĽ�β����Ϊ false��
            //
            // �쳣: 
            //   System.InvalidOperationException:
            //     �ڴ�����ö�����󼯺ϱ��޸��ˡ�
            public bool MoveNext()
            {
                do
                {
                    if (current_index == -2)
                    {
                        // ��һ�ε���
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
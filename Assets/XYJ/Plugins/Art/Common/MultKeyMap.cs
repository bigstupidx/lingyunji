using System;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    // 多key映射
    public class MultKeyMap<V> : Dictionary<Type, IDictionary> where V : new()
	{
		public void Add<T>(T key, V value)
		{
            Map<T, V> dic = Get<T>(true);
            dic.Add(key, value);
		}

        public Map<T, V> Get<T>(bool iscreate)
        {
            Map<T, V> dic = null;
            {
                IDictionary id = null;
                if (!TryGetValue(typeof(T), out id))
                {
                    if (iscreate == false)
                        return null;

                    dic = new Map<T, V>();
                    Add(typeof(T), dic);
                }
                else
                {
                    dic = id as Map<T, V>;
                }
            }

            return dic;
        }

        public bool TryGetValue<T>(out Map<T, V> dic)
        {
            dic = Get<T>(false);
            if (dic == null)
                return false;

            return true;
        }

        public void RemoveAll()
        {
            Clear();
        }
	}
}

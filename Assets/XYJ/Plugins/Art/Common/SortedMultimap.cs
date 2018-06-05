using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class SortedMultimap<K, V> : SortedDictionary<K, List<V>> 
	{
        public SortedMultimap()
        {

        }

        public SortedMultimap(IComparer<K> comparer)
            : base(comparer)
        {

        }

		public void Add(K key, V value)
		{
			List<V> list;
			if (TryGetValue(key, out list) == true)
			{
				list.Add(value);
			}
			else
			{
				list = new List<V>();
				list.Add(value);
                Add(key, list);
			}
		}

        public bool Remove(K key, V v)
        {
            List<V> vl = null;
            bool has = false;
            if (TryGetValue(key, out vl))
            {
                has = vl.Remove(v);
                if (vl.Count == 0)
                    Remove(key);
            }

            return has;
        }
	}
}

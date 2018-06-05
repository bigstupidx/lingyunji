using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class Multimap<K, V> : Dictionary<K, List<V>>
    {
        public Dictionary<K, List<V>> dictionary { get { return this; } }

		public int Add(K key, V value)
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
                dictionary[key] = list;
			}

            return list.Count;
		}
	}
}

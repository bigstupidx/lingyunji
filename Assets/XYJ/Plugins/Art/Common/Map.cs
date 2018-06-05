using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class Map<TKey, TValue> : Dictionary<TKey, TValue> where TValue : new ()
    {
        public new TValue this[TKey key]
        {
            get
            {
                TValue value = default(TValue);
                if (TryGetValue(key, out value))
                    return value;

                value = new TValue();
                Add(key, value);
                return value;
            }

            set
            {
                if (ContainsKey(key))
                    Remove(key);

                Add(key, value);
            }
        }
	}

    public class SortedMap<TKey, TValue> : SortedDictionary<TKey, TValue> where TValue : new()
    {
        public SortedMap()
            : base()
        {

        }

        public SortedMap(IComparer<TKey> comparer)
            : base(comparer)
        {

        }

        public SortedMap(IDictionary<TKey, TValue> dic)
            : base(dic)
        {

        }

        public SortedMap(IDictionary<TKey, TValue> dic, IComparer<TKey> comparer)
            : base(dic, comparer)
        {

        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue value = default(TValue);
                if (TryGetValue(key, out value))
                    return value;

                value = new TValue();
                Add(key, value);
                return value;
            }

            set
            {
                if (ContainsKey(key))
                    Remove(key);

                Add(key, value);
            }
        }
    }
}

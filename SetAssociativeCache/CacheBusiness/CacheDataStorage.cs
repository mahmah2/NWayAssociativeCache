using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public class CacheDataStorage<TKey, TValue> : Dictionary<int, CacheEntryList<TKey, TValue>> 
        where TKey : IComparable<TKey> where TValue : IComparable<TValue>
    {
        private int _capacity;

        public CacheDataStorage(int n) : base(n)
        {
            _capacity = n;
        }
    }
}

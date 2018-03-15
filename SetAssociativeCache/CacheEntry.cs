using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public class CacheEntry<TKey, TValue>
    {
        public CacheEntry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            LastReadTime = DateTime.Now;
            ReadCount = 0;
        }


        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public int ReadCount { get; set; }
        public DateTime LastReadTime { get; set; }


        public TValue ReadValueAndUpdateStat()
        {
            ReadCount++;
            LastReadTime = DateTime.Now;
            return Value;
        }
    }
}

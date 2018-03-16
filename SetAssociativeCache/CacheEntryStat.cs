using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public class CacheEntryStat<TKey, TValue>
    {
        public CacheEntryStat(CacheEntry<TKey, TValue> cacheEntry)
        {
            Key = cacheEntry.Key;
            LastReadTime = cacheEntry.LastReadTime;
            ReadCount = cacheEntry.ReadCount;
        }
        public TKey Key { get; set; }
        public int ReadCount { get; set; }
        public DateTime LastReadTime { get; set; }
    }
}

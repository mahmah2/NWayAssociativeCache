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
            LastReadTick = cacheEntry.LastReadTick;
            ReadCount = cacheEntry.ReadCount;
        }
        public TKey Key { get; private set; }
        public int ReadCount { get; private set; }
        public long LastReadTick { get; private set; }
    }
}

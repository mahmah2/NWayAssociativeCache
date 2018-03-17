using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public class CacheEntryStat<TKey>
    {
        public CacheEntryStat(TKey key, long lastReadTick, int readCount)
        {
            Key = key;
            LastReadTick = lastReadTick;
            ReadCount = readCount;
        }
        public TKey Key { get; private set; }
        public int ReadCount { get; private set; }
        public long LastReadTick { get; private set; }
    }
}

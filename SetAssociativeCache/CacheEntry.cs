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
            LastReadTick = TimeHelper.NanoTime();
            ReadCount = 0;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
        public int ReadCount { get; set; }
        //public DateTime LastReadTime { get; set; }
        public long LastReadTick { get; set; }

        public TValue ReadValueAndUpdateStat()
        {
            ReadCount++;
            LastReadTick = TimeHelper.NanoTime();
            return Value;
        }
        public void UpdateValue(TValue value)
        {
            Value = value;
            LastReadTick = TimeHelper.NanoTime();
            ReadCount = 0;
        }
    }
}

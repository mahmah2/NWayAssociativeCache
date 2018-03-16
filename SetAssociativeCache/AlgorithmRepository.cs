using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public static class AlgorithmRepository<TKey, TValue>
    {
        public static TKey LRUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var earliestTime = list.Min(t => t.LastReadTime);

            var entry = list.FirstOrDefault(p => p.LastReadTime == earliestTime);

            return entry.Key;
        }

        public static TKey LFUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var leastFrequentlyUsed = list.Min(t => t.ReadCount);

            var entry = list.FirstOrDefault(p => p.ReadCount == leastFrequentlyUsed);

            return entry.Key;
        }
    }
}

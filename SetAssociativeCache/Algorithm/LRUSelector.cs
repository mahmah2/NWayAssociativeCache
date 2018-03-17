using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public class LRUSelector<TKey> : IEntrySelector<TKey>
    {
        public TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey>> list)
        {
            var earliestTime = list.Min(t => t.LastReadTick);

            var entry = list.FirstOrDefault(p => p.LastReadTick == earliestTime);

            return entry.Key;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public class LFUSelector<TKey> : IEntrySelector<TKey>
    {
        public TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey>> list)
        {
            var leastFrequentlyUsed = list.Min(t => t.ReadCount);

            var entry = list.FirstOrDefault(p => p.ReadCount == leastFrequentlyUsed);

            return entry.Key;
        }
    }
}

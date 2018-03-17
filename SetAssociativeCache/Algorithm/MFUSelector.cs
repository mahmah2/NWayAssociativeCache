using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public class MFUSelector<TKey> : IEntrySelector<TKey>
    {
        public TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey>> list)
        {
            var mostFrequentlyUsed = list.Max(t => t.ReadCount);

            var entry = list.FirstOrDefault(p => p.ReadCount == mostFrequentlyUsed);

            return entry.Key;
        }
    }
}

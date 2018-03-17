using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public class MRUSelector<TKey> : IEntrySelector<TKey>
    {
        public TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey>> list)
        {
            var latestTime = list.Max(t => t.LastReadTick);

            var entry = list.FirstOrDefault(p => p.LastReadTick == latestTime);

            return entry.Key;
        }
    }
}

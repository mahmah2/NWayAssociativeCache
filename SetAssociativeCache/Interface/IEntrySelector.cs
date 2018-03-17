using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public interface IEntrySelector<TKey, TValue>
    {
        TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey, TValue>> list);
    }
}

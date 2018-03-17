using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public interface IEntrySelector<TKey>
    {
        TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey>> list);
    }
}

using SetAssociativeCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheTester
{

   public class FirstEntrySelector<TKey, TValue> : IEntrySelector<TKey, TValue>
    {
        public TKey SelectEntryKey(IEnumerable<CacheEntryStat<TKey, TValue>> list)
        {
            return list.ToList()[0].Key;
        }
    }
}

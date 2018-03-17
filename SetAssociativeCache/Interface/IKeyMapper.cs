using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public interface IKeyMapper<TKey>
    {
        int MapKeyToIndex(TKey key, int targetLength);
    }
}

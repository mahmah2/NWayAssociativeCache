using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public class IntKeyMapper : IKeyMapper<int>
    {
        public int MapKeyToIndex(int key, int targetLength)
        {
            return key % targetLength;
        }
    }
}

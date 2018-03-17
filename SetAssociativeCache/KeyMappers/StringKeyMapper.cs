using System;
using System.Collections.Generic;
using System.Text;

namespace SetAssociativeCache
{
    public class StringKeyMapper : IKeyMapper<string>
    {
        public int MapKeyToIndex(string key, int targetLength)
        {
            return key.Length % targetLength; //Map string to its set by their string length
        }
    }
}

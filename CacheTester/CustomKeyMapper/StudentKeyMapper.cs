using SetAssociativeCache;
using System;
using System.Collections.Generic;
using System.Text;

namespace CacheTester
{
    public class StudentKeyMapper : IKeyMapper<string>
    {
        public int MapKeyToIndex(string key, int targetLength)
        {
            return key.Length % targetLength; //Map Student to its set by their Name length
        }
    }
}

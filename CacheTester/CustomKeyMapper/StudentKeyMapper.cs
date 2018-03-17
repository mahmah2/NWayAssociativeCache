using SetAssociativeCache;
using System;
using System.Collections.Generic;
using System.Text;

namespace CacheTester
{
    public class StudentKeyMapper : IKeyMapper<Student>
    {
        public int MapKeyToIndex(Student key, int targetLength)
        {
            return key.Name.Length % targetLength; //Map Student to its set by their Name length
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public static class AlgorithmRepository<TKey, TValue>
    {
        public static Dictionary<AlgorithmTypeEnum, IEntrySelector<TKey>> Definitions
            = new Dictionary<AlgorithmTypeEnum, IEntrySelector<TKey>>
            {
                {AlgorithmTypeEnum.LRU ,  new LRUSelector<TKey>()},
                {AlgorithmTypeEnum.MRU ,  new MRUSelector<TKey>()},
                {AlgorithmTypeEnum.LFU ,  new LFUSelector<TKey>()},
                {AlgorithmTypeEnum.MFU ,  new MFUSelector<TKey>()},
            };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public static class AlgorithmRepository<TKey, TValue>
    {
        public static Dictionary<AlgorithmTypeEnum, IEntrySelector<TKey,TValue>> Definitions
            = new Dictionary<AlgorithmTypeEnum, IEntrySelector<TKey, TValue>>
            {
                {AlgorithmTypeEnum.LRU ,  new LRUSelector<TKey,TValue>()},
                {AlgorithmTypeEnum.MRU ,  new MRUSelector<TKey,TValue>()},
                {AlgorithmTypeEnum.LFU ,  new LFUSelector<TKey,TValue>()},
                {AlgorithmTypeEnum.MFU ,  new MFUSelector<TKey,TValue>()},
            };
    }
}

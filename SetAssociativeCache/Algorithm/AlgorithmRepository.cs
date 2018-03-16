using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public static class AlgorithmRepository<TKey, TValue>
    {
        public static Dictionary<AlgorithmTypeEnum, SelectKeyToDeleteFunc<TKey, TValue>> AlgorithmMethods
            = new Dictionary<AlgorithmTypeEnum, SelectKeyToDeleteFunc<TKey, TValue>>()
            {
                {AlgorithmTypeEnum.LRU ,  LRUSelector},
                {AlgorithmTypeEnum.MRU ,  MRUSelector},
                {AlgorithmTypeEnum.LFU ,  LFUSelector},
                {AlgorithmTypeEnum.MFU ,  MFUSelector},
            };
        
        /// <summary>
        /// Implementation of Least Recently Used
        /// </summary>
        /// <param name="list">list of each entry statistics</param>
        /// <returns></returns>
        public static TKey LRUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var earliestTime = list.Min(t => t.LastReadTick);

            var entry = list.FirstOrDefault(p => p.LastReadTick == earliestTime);

            return entry.Key;
        }

        /// <summary>
        /// Implementation of Most Recently Used
        /// </summary>
        /// <param name="list">list of each entry statistics</param>
        /// <returns></returns>
        public static TKey MRUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var latestTime = list.Max(t => t.LastReadTick);

            var entry = list.FirstOrDefault(p => p.LastReadTick == latestTime);

            return entry.Key;
        }

        /// <summary>
        /// Implementation of Least Frequently Used
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TKey LFUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var leastFrequentlyUsed = list.Min(t => t.ReadCount);

            var entry = list.FirstOrDefault(p => p.ReadCount == leastFrequentlyUsed);

            return entry.Key;
        }

        /// <summary>
        /// Implementation of Most Frequently Used
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TKey MFUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var mostFrequentlyUsed = list.Max(t => t.ReadCount);

            var entry = list.FirstOrDefault(p => p.ReadCount == mostFrequentlyUsed);

            return entry.Key;
        }

    }
}

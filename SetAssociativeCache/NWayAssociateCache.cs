using System;
using System.Collections.Generic;
using System.Linq;

namespace SetAssociativeCache
{
    public delegate int Comparer<T>(T v1, T v2);
    public delegate int GetKeySetIndex<T>(T v);


    public class NWayAssociateCache<TKey, TValue>
    {

        //public NWayAssociateCache(int n, int memoryLength, int blockSizeBytes, int cacheLengthBytes,
        public NWayAssociateCache(int nWays, int setCapacity,
            Comparer<TKey> keyComparer, 
            Comparer<TValue> valueComparer, 
            GetKeySetIndex<TKey> getKeySetIndex,
            SelectKeyToDeleteFunc<TKey, TValue> selectDeleteIndexFunc = null)
        {
            if (nWays < 1)
                throw new Exception($"{nameof(nWays)} should be greater than zero.");

            if (setCapacity < 1)
                throw new Exception($"{nameof(setCapacity)} should be greater than zero.");

            _n = nWays;
            _setCapacity = setCapacity;
            _keyComparer = keyComparer;
            _valueComparer = valueComparer;
            _getKeySetIndex = getKeySetIndex;

            //enforce a default LRU algorithm
            if (selectDeleteIndexFunc == null)
                selectDeleteIndexFunc = LRUSelector;
            _selectKeyToDeleteFunc = selectDeleteIndexFunc;

            _cache = new CacheDataStorage<TKey, TValue>(nWays);
            for (int setIndex = 0; setIndex < nWays; setIndex++)
            {
                _cache[setIndex] = new CacheEntryList<TKey, TValue>(setCapacity, selectDeleteIndexFunc, keyComparer);
            }
        }

        public event EventHandler OnMiss;
        public event EventHandler OnHit;

        private TKey LRUSelector(List<CacheEntry<TKey, TValue>> list)
        {
            var earliestTime =list.Min(t => t.LastReadTime);

            var entry = list.FirstOrDefault(p => p.LastReadTime == earliestTime);

            return entry.Key;
        }

        private CacheDataStorage<TKey, TValue> _cache;

        private Comparer<TKey> _keyComparer;

        private Comparer<TValue> _valueComparer;

        private GetKeySetIndex<TKey> _getKeySetIndex;

        private SelectKeyToDeleteFunc<TKey, TValue> _selectKeyToDeleteFunc;

        private int _n;
        private int _setCapacity;

        private int GetKeyIndex(TKey key)
        {
            var index = _getKeySetIndex(key);
            if (index < 0 || index >= _setCapacity || _setCapacity < 1)
            {
                throw new Exception($"Set index out of range, Set length = {_setCapacity}, Requested index = {index}");
            }
            return index;
        }

        public void SetValue(TKey key, TValue value)
        {
            var index = GetKeyIndex(key);

            _cache[index].SetValue(key, value);
        }

        public bool ReadValue(TKey key, out TValue value)
        {
            value = default(TValue);

            var index = GetKeyIndex(key);

            if (_cache[index].ContainsKey(key))
            {
                value = _cache[index].ReadValue(key);

                return true;
            }
            else
                OnMiss?.Invoke(null, EventArgs.Empty);

            return false;
        }

    }
}

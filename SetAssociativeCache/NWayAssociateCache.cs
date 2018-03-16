using System;
using System.Collections.Generic;
using System.Linq;

namespace SetAssociativeCache
{
    public delegate int Comparer<T>(T v1, T v2);
    public delegate int GetKeySetIndex<T>(T v);

    public class NWayAssociateCache<TKey, TValue>
    {

        /// <summary>
        /// Implements a NWay Cache which can be seen as N different cache sets which the index of a given data
        /// is defined by a user function(getKeySetIndex)
        /// </summary>
        /// <param name="nWays">Number of different cache sets</param>
        /// <param name="setCapacity">Capacity of each cache set</param>
        /// <param name="keyComparer">Function that compares two keys and returns 0 if they are equal</param>
        /// <param name="valueComparer">Function that compares two values and returns 0 if they are equal</param>
        /// <param name="getKeySetIndex">Function that gets a key and returns a hash number out of its value which is greater equal to 0 and less than N</param>
        /// <param name="selectDeleteIndexFunc">This function receives a list of statistics in a set of cach entries and choses one to be deleted</param>
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

            //enforce a default LRU algorithm if nothing was selected
            if (selectDeleteIndexFunc == null)
                selectDeleteIndexFunc = AlgorithmRepository<TKey, TValue>.LRUSelector;

            _selectKeyToDeleteFunc = selectDeleteIndexFunc;

            _cache = new CacheDataStorage<TKey, TValue>(nWays);
            for (int setIndex = 0; setIndex < nWays; setIndex++)
            {
                _cache[setIndex] = new CacheEntryList<TKey, TValue>(setCapacity, selectDeleteIndexFunc, keyComparer, valueComparer);
                _cache[setIndex].OnMiss += NWayAssociateCache_OnEntryListMiss;
                _cache[setIndex].OnHit += NWayAssociateCache_OnEntryListHit;
            }
        }

        private void NWayAssociateCache_OnEntryListHit(object sender, EventArgs e)
        {
            OnHit?.Invoke(sender, e);
        }

        private void NWayAssociateCache_OnEntryListMiss(object sender, EventArgs e)
        {
            OnMiss?.Invoke(sender, e);
        }

        public event EventHandler OnMiss;
        public event EventHandler OnHit;

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
            if (index < 0 || index >= _n || _setCapacity < 1)
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
                OnMiss?.Invoke(this, EventArgs.Empty);

            return false;
        }

        public override string ToString()
        {
            var s = $"Dumping of {_n} Way cache with {_setCapacity} rows in each set: \r\n";
            for (int i = 0; i < _cache.Count; i++)
            {
                   s += $"CacheSet {i} \r\n{_cache[i].ToString("\t")}\r\n"; 
            }
            return s;
        }

    }
}

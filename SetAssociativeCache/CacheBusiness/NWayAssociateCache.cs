using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetAssociativeCache
{
    public delegate int Comparer<T>(T v1, T v2);
    public delegate int GetKeySetIndex<T>(T v);

    public class NWayAssociateCache<TKey, TValue> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
    {

        /// <summary>
        /// Implements a NWay Cache which can be seen as N different cache sets which the index of a given data
        /// is defined by a user function(getKeySetIndex)
        /// </summary>
        /// <param name="nWays">Number of different cache sets</param>
        /// <param name="setCapacity">Capacity of each cache set</param>
        /// <param name="keyMapper">And object that implements IKeyMapper to map a key to its corresponding index</param>
        public NWayAssociateCache(int nWays, int setCapacity, IKeyMapper<TKey> keyMapper)
        {
            if (nWays < 1)
                throw new Exception($"{nameof(nWays)} should be greater than zero.");

            if (setCapacity < 1)
                throw new Exception($"{nameof(setCapacity)} should be greater than zero.");

            _numbertOfSets = nWays;
            _setCapacity = setCapacity;
            _keyMapper = keyMapper;

            _keyToDeletSelector = AlgorithmRepository<TKey, TValue>.Definitions[AlgorithmTypeEnum.LRU];

            _cache = new CacheDataStorage<TKey, TValue>(nWays);
            for (int setIndex = 0; setIndex < nWays; setIndex++)
            {
                _cache[setIndex] = new CacheEntryList<TKey, TValue>(setCapacity, _keyToDeletSelector);
                _cache[setIndex].OnMiss += NWayAssociateCache_OnEntryListMiss;
                _cache[setIndex].OnHit += NWayAssociateCache_OnEntryListHit;
            }
        }

        /// <summary>
        /// Sets the algorithm used to select next key to be deleted
        /// </summary>
        /// <param name="algorithmType">AlgorithTypeEnum has already four standard algorithms and a custom one</param>
        /// <param name="customDeleteKeySelector">If first algorithm is custom then this one should be provided by
        /// a class that implements IEntrySelector interface</param>
        /// <returns></returns>
        public bool SetRemoveAlgorithm(AlgorithmTypeEnum algorithmType,
            IEntrySelector<TKey> customDeleteKeySelector = null)
        {
            if (algorithmType == AlgorithmTypeEnum.Custom)
            {
                _keyToDeletSelector = customDeleteKeySelector ?? throw new ArgumentNullException(nameof(customDeleteKeySelector));
            }
            else
            {
                _keyToDeletSelector = AlgorithmRepository<TKey, TValue>.Definitions[algorithmType];
            }

            //Apply the change to underlying cache sets
            for (int setIndex = 0; setIndex < _numbertOfSets; setIndex++)
            {
                _cache[setIndex].SetDeleteKeySelector(_keyToDeletSelector);
            }

            return true;
        }

        /// <summary>
        /// Triggers when a miss occurs in the cache
        /// </summary>
        public event EventHandler OnMiss;
        /// <summary>
        /// Triggers when a hit occurs in the cache
        /// </summary>
        public event EventHandler OnHit;

        /// <summary>
        /// Sets a key value in the cache
        /// </summary>
        /// <param name="key">Key calue</param>
        /// <param name="value">Value to be set</param>
        public void SetValue(TKey key, TValue value)
        {
            var index = GetKeyIndex(key);

            _cache[index].SetValue(key, value);
        }

        public async Task SetValueAsync(TKey key, TValue value)
        {
            await Task.Run(() =>
            {
                SetValue(key, value);
            });
        }


        /// <summary>
        /// Reads a key value in the cache
        /// </summary>
        /// <param name="key">Key value to be read</param>
        /// <param name="value">Value of the key that would be read</param>
        /// <returns></returns>
        public bool ReadValue(TKey key, out TValue value)
        {
            value = default(TValue);

            var index = GetKeyIndex(key);

            if (_cache[index].ReadValue(key, out value))
            {
                OnHit?.Invoke(this, EventArgs.Empty);

                return true;
            }
            else
            {
                OnMiss?.Invoke(this, EventArgs.Empty);

                return false;
            }
        }

        public async Task<OperationResult<TValue>> ReadValueAsync(TKey key)
        {
            bool result = false;
            TValue par = default(TValue);

            await Task.Run(()=> {
                result =  ReadValue(key, out par);
            });

            return new OperationResult<TValue>() {
                ReturnedValue = par,
                Successful = result
            };
        }





        /// <summary>
        /// Checks if a given key exists in cache or not
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            var index = GetKeyIndex(key);

            return _cache[index].ContainsKey(key);
        }

        /// <summary>
        /// Showd the current Sets, Keys and Values of the cache
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var s = $"Dumping of {_numbertOfSets} Way cache with {_setCapacity} rows in each set: \r\n";
            for (int i = 0; i < _cache.Count; i++)
            {
                s += $"CacheSet {i} \r\n{_cache[i].ToString("\t")}\r\n";
            }
            return s;
        }

        private int _numbertOfSets;
        public int NumbertOfSets { get { return _numbertOfSets; } }

        private int _setCapacity;
        public int SetCapacity { get { return _setCapacity; } }

        private CacheDataStorage<TKey, TValue> _cache;

        private IKeyMapper<TKey> _keyMapper;

        private IEntrySelector<TKey> _keyToDeletSelector;

        private int GetKeyIndex(TKey key)
        {
            var index = _keyMapper.MapKeyToIndex(key, _numbertOfSets);
            if (index < 0 || index >= _numbertOfSets)
            {
                throw new Exception($"Set index out of range, Set length = {_setCapacity}, Requested index = {index}");
            }
            return index;
        }

        private void NWayAssociateCache_OnEntryListHit(object sender, EventArgs e)
        {
            OnHit?.Invoke(sender, e);
        }

        private void NWayAssociateCache_OnEntryListMiss(object sender, EventArgs e)
        {
            OnMiss?.Invoke(sender, e);
        }


    }

    public class OperationResult<TValue>
    {
        public TValue ReturnedValue { get; set; }
        public bool Successful { get; set; }
    }
}
